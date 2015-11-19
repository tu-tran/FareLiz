namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Text;

    using log4net;

    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Working environment for monitoring fare data</summary>
    [Serializable]
    public class MonitorEnvironment
    {
#if DEBUG

        /// <summary>The id.</summary>
        [UniqueData]
        public string Id = Guid.NewGuid().ToString();
#endif

        /// <summary>Initializes a new instance of the <see cref="MonitorEnvironment" /> class.</summary>
        protected MonitorEnvironment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorEnvironment"/> class.
        /// </summary>
        /// <param name="configStore">
        /// The config store.
        /// </param>
        /// <param name="pluginResolver">
        /// The plugin resolver.
        /// </param>
        /// <param name="servicesManager">
        /// The services manager.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MonitorEnvironment(IConfigStore configStore, IPluginResolver pluginResolver, IServiceManager servicesManager, ILogger logger)
            : this(configStore, pluginResolver, null, null, null, null, servicesManager, logger)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorEnvironment"/> class.
        /// </summary>
        /// <param name="configStore">
        /// The config store.
        /// </param>
        /// <param name="pluginResolver">
        /// The plugin resolver.
        /// </param>
        /// <param name="fareDataProvider">
        /// The fare data provider.
        /// </param>
        /// <param name="fareDatabase">
        /// The fare database.
        /// </param>
        /// <param name="archiveManager">
        /// The archive manager.
        /// </param>
        /// <param name="currencyProvider">
        /// The currency provider.
        /// </param>
        /// <param name="servicesManager">
        /// The services manager.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public MonitorEnvironment(
            IConfigStore configStore, 
            IPluginResolver pluginResolver, 
            IFareDataProvider fareDataProvider, 
            IFareDatabase fareDatabase, 
            IArchiveManager archiveManager, 
            ICurrencyProvider currencyProvider, 
            IServiceManager servicesManager, 
            ILogger logger)
        {
            this.Logger = logger;
            this.ConfigStore = configStore;
            this.PluginResolver = pluginResolver;
            this.FareDataProvider = fareDataProvider;
            if (this.FareDataProvider != null)
            {
                this.FareDataProvider.CurrencyProvider = currencyProvider;
            }

            this.FareDatabase = fareDatabase;
            this.ArchiveManager = archiveManager;
            this.CurrencyProvider = currencyProvider;
            this.BackgroundServices = servicesManager;
        }

        /// <summary>This object is responsible for storing the environment configurations</summary>
        public IConfigStore ConfigStore { get; set; }

        /// <summary>This object is responsible for loading plugins</summary>
        public IPluginResolver PluginResolver { get; set; }

        /// <summary>This object is responsible for handling the fare data</summary>
        public IFareDataProvider FareDataProvider { get; set; }

        /// <summary>This object is responsible for fare database storage</summary>
        public IFareDatabase FareDatabase { get; set; }

        /// <summary>This object is responsible for archiving fare data</summary>
        public IArchiveManager ArchiveManager { get; set; }

        /// <summary>Currency conversion and handling</summary>
        public ICurrencyProvider CurrencyProvider { get; set; }

        /// <summary>Background services for running background tasks</summary>
        public IServiceManager BackgroundServices { get; set; }

        /// <summary>Gets or sets the logger.</summary>
        public ILogger Logger { get; set; }

        /// <summary>Initialize the working environment and all configured helper objects</summary>
        /// <returns>The <see cref="ValidateResult" />.</returns>
        public ValidateResult Initialize()
        {
            var sb = new StringBuilder();
            ValidateResult valResult;

            if (this.FareDatabase != null)
            {
                this.FareDatabase.Logger = this.Logger;
                this.FareDatabase.Initialize();
                if (this.FareDatabase.Configuration != null)
                {
                    if (!(valResult = this.FareDatabase.Configuration.Validate()).Succeeded)
                    {
                        sb.AppendLine("Fare Database: " + valResult.ErrorMessage);
                    }
                }
            }

            if (this.FareDataProvider != null)
            {
                this.FareDataProvider.Logger = this.Logger;
                this.FareDataProvider.Initialize();
                if (this.FareDataProvider.Configuration != null)
                {
                    if (!(valResult = this.FareDataProvider.Configuration.Validate()).Succeeded)
                    {
                        sb.AppendLine("Fare Data Handler: " + valResult.ErrorMessage);
                    }
                }

                this.FareDataProvider.CurrencyProvider = this.CurrencyProvider;
            }

            if (this.ArchiveManager != null)
            {
                this.ArchiveManager.Logger = this.Logger;
                this.ArchiveManager.Initialize();
                if (this.ArchiveManager.Configuration != null)
                {
                    if (!(valResult = this.ArchiveManager.Configuration.Validate()).Succeeded)
                    {
                        sb.AppendLine("Archive Manager: " + valResult.ErrorMessage);
                    }
                }

                this.ArchiveManager.FareDatabase = this.FareDatabase;
                this.ArchiveManager.FareDataProvider = this.FareDataProvider;
            }

            var syncDb = this.FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                var syncer = syncDb.DataSynchronizer;
                if (syncer != null)
                {
                    syncer.Logger = this.Logger;
                    syncer.Initialize();
                    if (syncer.Configuration != null)
                    {
                        if (!(valResult = syncer.Configuration.Validate()).Succeeded)
                        {
                            sb.AppendLine("Data Synchronizer: " + valResult.ErrorMessage);
                        }
                    }

                    syncer.SyncTargetObject = syncDb;
                }
            }

            if (this.CurrencyProvider != null)
            {
                if (this.CurrencyProvider.Configuration != null)
                {
                    if (!(valResult = this.CurrencyProvider.Configuration.Validate()).Succeeded)
                    {
                        sb.AppendLine("Currency Provider: " + valResult.ErrorMessage);
                    }
                }

                if (this.BackgroundServices != null && this.BackgroundServices.Get(this.CurrencyProvider.GetType()) == null)
                {
                    this.BackgroundServices.Add(this.CurrencyProvider, false);
                }
            }

            var allServices = this.BackgroundServices.GetAll();
            foreach (var svc in allServices)
            {
                this.Logger.Info("Starting service " + svc);
                svc.Logger = this.Logger;
                svc.Initialize();
                svc.Start();
            }

            string errors = sb.ToString();
            var result = new ValidateResult(errors.Length == 0, errors);
            return result;
        }

        /// <summary>Stop all background services</summary>
        public void Close()
        {
            if (this.BackgroundServices != null)
            {
                this.BackgroundServices.Clear();
            }
        }
    }
}