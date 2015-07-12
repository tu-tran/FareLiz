using log4net;
using SkyDean.FareLiz.Core.Config;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Text;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Working environment for monitoring fare data
    /// </summary>
    [Serializable]
    public class MonitorEnvironment
    {
        /// <summary>
        /// This object is responsible for storing the environment configurations
        /// </summary>
        public IConfigStore ConfigStore { get; set; }

        /// <summary>
        /// This object is responsible for loading plugins
        /// </summary>
        public IPluginResolver PluginResolver { get; set; }

        /// <summary>
        /// This object is responsible for handling the fare data
        /// </summary>
        public IFareDataProvider FareDataProvider { get; set; }

        /// <summary>
        /// This object is responsible for fare database storage
        /// </summary>
        public IFareDatabase FareDatabase { get; set; }

        /// <summary>
        /// This object is responsible for archiving fare data
        /// </summary>
        public IArchiveManager ArchiveManager { get; set; }

        /// <summary>
        /// Currency conversion and handling
        /// </summary>
        public ICurrencyProvider CurrencyProvider { get; set; }

        /// <summary>
        /// Background services for running background tasks
        /// </summary>
        public IServiceManager BackgroundServices { get; set; }

#if DEBUG
        [UniqueData]
        public string Id = Guid.NewGuid().ToString();
#endif

        public ILog Logger { get; set; }

        protected MonitorEnvironment() { }

        public MonitorEnvironment(IConfigStore configStore, IPluginResolver pluginResolver, IServiceManager servicesManager, ILog logger)
            : this(configStore, pluginResolver, null, null, null, null, servicesManager, logger)
        { }

        public MonitorEnvironment(IConfigStore configStore, IPluginResolver pluginResolver, IFareDataProvider fareDataProvider, IFareDatabase fareDatabase,
            IArchiveManager archiveManager, ICurrencyProvider currencyProvider, IServiceManager servicesManager, ILog logger)
        {
            Logger = logger;
            ConfigStore = configStore;
            PluginResolver = pluginResolver;
            FareDataProvider = fareDataProvider;
            if (FareDataProvider != null)
                FareDataProvider.CurrencyProvider = currencyProvider;
            FareDatabase = fareDatabase;
            ArchiveManager = archiveManager;
            CurrencyProvider = currencyProvider;
            BackgroundServices = servicesManager;
        }

        /// <summary>
        /// Initialize the working environment and all configured helper objects
        /// </summary>
        public ValidateResult Initialize()
        {
            var sb = new StringBuilder();
            ValidateResult valResult;

            if (FareDatabase != null)
            {
                FareDatabase.Logger = Logger;
                FareDatabase.Initialize();
                if (FareDatabase.Configuration != null)
                    if (!(valResult = FareDatabase.Configuration.Validate()).Succeeded)
                        sb.AppendLine("Fare Database: " + valResult.ErrorMessage);
            }
            if (FareDataProvider != null)
            {
                FareDataProvider.Logger = Logger;
                FareDataProvider.Initialize();
                if (FareDataProvider.Configuration != null)
                    if (!(valResult = FareDataProvider.Configuration.Validate()).Succeeded)
                        sb.AppendLine("Fare Data Handler: " + valResult.ErrorMessage);
                FareDataProvider.CurrencyProvider = CurrencyProvider;
            }
            if (ArchiveManager != null)
            {
                ArchiveManager.Logger = Logger;
                ArchiveManager.Initialize();
                if (ArchiveManager.Configuration != null)
                    if (!(valResult = ArchiveManager.Configuration.Validate()).Succeeded)
                        sb.AppendLine("Archive Manager: " + valResult.ErrorMessage);
                ArchiveManager.FareDatabase = FareDatabase;
                ArchiveManager.FareDataProvider = FareDataProvider;
            }
            var syncDb = FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                var syncer = syncDb.DataSynchronizer;
                if (syncer != null)
                {
                    syncer.Logger = Logger;
                    syncer.Initialize();
                    if (syncer.Configuration != null)
                        if (!(valResult = syncer.Configuration.Validate()).Succeeded)
                            sb.AppendLine("Data Synchronizer: " + valResult.ErrorMessage);
                    syncer.SyncTargetObject = syncDb;
                }
            }
            if (CurrencyProvider != null)
            {
                if (CurrencyProvider.Configuration != null)
                    if (!(valResult = CurrencyProvider.Configuration.Validate()).Succeeded)
                        sb.AppendLine("Currency Provider: " + valResult.ErrorMessage);

                if (BackgroundServices != null && BackgroundServices.Get(CurrencyProvider.GetType()) == null)
                    BackgroundServices.Add(CurrencyProvider, false);
            }

            var allServices = BackgroundServices.GetAll();
            foreach (var svc in allServices)
            {
                Logger.Info("Starting service " + svc);
                svc.Logger = Logger;
                svc.Initialize();
                svc.Start();
            }

            string errors = sb.ToString();
            var result = new ValidateResult(errors.Length == 0, errors);
            return result;
        }

        /// <summary>
        /// Stop all background services
        /// </summary>
        public void Close()
        {
            if (BackgroundServices != null)
                BackgroundServices.Clear();
        }
    }
}
