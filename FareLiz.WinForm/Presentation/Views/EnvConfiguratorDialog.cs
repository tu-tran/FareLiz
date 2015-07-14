namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Config;
    using SkyDean.FareLiz.WinForm.Properties;

    /// <summary>The env configurator dialog.</summary>
    internal partial class EnvConfiguratorDialog : SmartForm
    {
        /// <summary>The _execution param.</summary>
        private readonly ExecutionParam _executionParam;

        /// <summary>The _instance data.</summary>
        private readonly Dictionary<Type, IPlugin> _instanceData = new Dictionary<Type, IPlugin>();

        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>The _type resolver.</summary>
        private readonly TypeResolver _typeResolver;

        /// <summary>The _env.</summary>
        private MonitorEnvironment _env;

        /// <summary>The _initialized.</summary>
        private volatile bool _initialized;

        /// <summary>Initializes a new instance of the <see cref="EnvConfiguratorDialog" /> class.</summary>
        protected EnvConfiguratorDialog()
        {
            this.InitializeComponent();

            this.btnInfoArchiveManager.Tag = this.btnResetArchiveManager.Tag = this.btnConfigArchiveManager.Tag = this.cbArchiveManager;
            this.btnInfoFareProvider.Tag = this.btnResetFareProvider.Tag = this.btnConfigFareProvider.Tag = this.cbFareDataProvider;
            this.btnInfoFareDatabase.Tag = this.btnResetFareDatabase.Tag = this.btnConfigFareDatabase.Tag = this.cbFareDatabase;
            this.btnInfoDbSyncer.Tag = this.btnResetDbSyncer.Tag = this.btnConfigDbSyncer.Tag = this.cbDbSyncer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvConfiguratorDialog"/> class.
        /// </summary>
        /// <param name="environment">
        /// The environment.
        /// </param>
        /// <param name="executionParam">
        /// The execution param.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal EnvConfiguratorDialog(MonitorEnvironment environment, ExecutionParam executionParam)
            : this()
        {
            if (environment == null)
            {
                throw new ArgumentException("Environment cannot be null");
            }

            if (environment.Logger == null)
            {
                throw new ArgumentException("Environment logger cannot be null");
            }

            if (environment.ConfigStore == null)
            {
                throw new ArgumentException("Environment config store cannot be null");
            }

            if (executionParam.ConfigHandler == null)
            {
                throw new ArgumentException("Execution param config store cannot be null");
            }

            this._logger = environment.Logger;
            this._typeResolver = new TypeResolver(this._logger);
            this._executionParam = executionParam.ReflectionDeepClone(this._logger);

            this.Height -= this.pnlTop.Height;
            this._env = environment.ReflectionDeepClone(this._logger);
#if DEBUG
            this._env.Id = Guid.NewGuid().ToString();
#endif

            this.InitializeData();
        }

        /// <summary>Gets the result environment.</summary>
        internal MonitorEnvironment ResultEnvironment
        {
            get
            {
                return this._env;
            }
        }

        /// <summary>Gets the result param.</summary>
        internal ExecutionParam ResultParam
        {
            get
            {
                return this._executionParam;
            }
        }

        /// <summary>The initialize data.</summary>
        private void InitializeData()
        {
            this._initialized = false;
            this._instanceData.Clear();
            if (this._env.ArchiveManager != null)
            {
                this._instanceData.Add(this._env.ArchiveManager.GetType(), this._env.ArchiveManager);
            }

            if (this._env.FareDataProvider != null)
            {
                this._instanceData.Add(this._env.FareDataProvider.GetType(), this._env.FareDataProvider);
            }

            if (this._env.FareDatabase != null)
            {
                this._instanceData.Add(this._env.FareDatabase.GetType(), this._env.FareDatabase);
                var syncDb = this._env.FareDatabase as ISyncableDatabase;
                if (syncDb != null && syncDb.DataSynchronizer != null)
                {
                    this._instanceData.Add(syncDb.DataSynchronizer.GetType(), syncDb.DataSynchronizer);
                }
            }

            var resolver = this._env.PluginResolver;
            this.Bind(this.cbArchiveManager, resolver.GetArchiveManagerTypes(), this._env.ArchiveManager);
            this.Bind(this.cbFareDataProvider, resolver.GetFareDataProviderTypes(), this._env.FareDataProvider);
            this.Bind(this.cbFareDatabase, resolver.GetFareDatabaseTypes(), this._env.FareDatabase);
            this.BindCurrencies(this._env);
            this._initialized = true;
            this.ApplyChanges();
            this.ValidateEnvironment();

            this.txtDeparture.SelectedAirport = this._executionParam.Departure;
            this.txtDestination.SelectedAirport = this._executionParam.Destination;
        }

        /// <summary>
        /// The fill syncer combo box.
        /// </summary>
        /// <param name="targetDbType">
        /// The target db type.
        /// </param>
        private void FillSyncerComboBox(Type targetDbType)
        {
            if (typeof(ISyncableDatabase).IsAssignableFrom(targetDbType))
            {
                var selectedSyncer = this.GetActiveDbSyncer();
                this.Bind(this.cbDbSyncer, this._env.PluginResolver.GetDbSyncerTypes(targetDbType), selectedSyncer);
            }
        }

        /// <summary>The get active db syncer.</summary>
        /// <returns>The <see cref="object" />.</returns>
        private object GetActiveDbSyncer()
        {
            if (this._env.FareDatabase != null)
            {
                var syncDb = this._env.FareDatabase as ISyncableDatabase;
                if (syncDb != null)
                {
                    return syncDb.DataSynchronizer;
                }
            }

            return null;
        }

        /// <summary>
        /// The bind.
        /// </summary>
        /// <param name="cb">
        /// The cb.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="selectedInstance">
        /// The selected instance.
        /// </param>
        private void Bind(ComboBox cb, IList<Type> data, object selectedInstance)
        {
            if (data == null || data.Count < 1)
            {
                cb.DataSource = null;
            }
            else
            {
                var ds = new PluginInfoHolder[data.Count];
                for (var i = 0; i < data.Count; i++)
                {
                    ds[i] = new PluginInfoHolder(data[i]);
                }

                cb.DisplayMember = "Name";
                cb.ValueMember = "Type";
                cb.DataSource = ds;
            }

            if (selectedInstance == null)
            {
                if (data != null && data.Count > 0)
                {
                    cb.SelectedIndex = 0;
                }
            }
            else
            {
                cb.SelectedValue = selectedInstance.GetType();
            }
        }

        /// <summary>
        /// The bind currencies.
        /// </summary>
        /// <param name="env">
        /// The env.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        private void BindCurrencies(MonitorEnvironment env)
        {
            var provider = env.CurrencyProvider;
            if (provider == null)
            {
                throw new ArgumentException("Missing Currency Provider");
            }

            var allCurrencies = provider.GetCurrencies();
            if (provider.Configuration == null)
            {
                provider.Configuration = provider.DefaultConfig;
            }

            var index = 0;
            var sel = provider.AllowedCurrencies;
            this.lstCurrency.Items.Clear(); // Clear the list of items before repopulating the default currency list
            foreach (var item in allCurrencies)
            {
                this.lstCurrency.Items.Add(item);
                var isSelected = sel != null && sel.Contains(item.Key);
                this.lstCurrency.SetItemChecked(index, isSelected);
                index++;
            }
        }

        /// <summary>The apply changes.</summary>
        private void ApplyChanges()
        {
            if (!this._initialized)
            {
                return;
            }

            var selType = this.cbArchiveManager.SelectedValue as Type;
            if (selType != null)
            {
                if (this._instanceData.ContainsKey(selType))
                {
                    this.ResultEnvironment.ArchiveManager = this._instanceData[selType] as IArchiveManager;
                }
                else
                {
                    this.ResultEnvironment.ArchiveManager = this._typeResolver.CreateInstance<IArchiveManager>(selType);
                    this.ResultEnvironment.ArchiveManager.Configuration = this.ResultEnvironment.ArchiveManager.DefaultConfig;
                }
            }

            selType = this.cbFareDatabase.SelectedValue as Type;
            if (selType != null)
            {
                if (this._instanceData.ContainsKey(selType))
                {
                    this.ResultEnvironment.FareDatabase = this._instanceData[selType] as IFareDatabase;
                }
                else
                {
                    this.ResultEnvironment.FareDatabase = this._typeResolver.CreateInstance<IFareDatabase>(selType);
                    this.ResultEnvironment.FareDatabase.Configuration = this.ResultEnvironment.FareDatabase.DefaultConfig;
                }
            }

            selType = this.cbFareDataProvider.SelectedValue as Type;
            if (selType != null)
            {
                if (this._instanceData.ContainsKey(selType))
                {
                    this.ResultEnvironment.FareDataProvider = this._instanceData[selType] as IFareDataProvider;
                }
                else
                {
                    this.ResultEnvironment.FareDataProvider = this._typeResolver.CreateInstance<IFareDataProvider>(selType);
                    this.ResultEnvironment.FareDataProvider.Configuration = this.ResultEnvironment.FareDataProvider.DefaultConfig;
                }
            }

            var syncDb = this.ResultEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                selType = this.cbDbSyncer.SelectedValue as Type;
                if (selType != null)
                {
                    IDataSyncer syncer;
                    if (this._instanceData.ContainsKey(selType))
                    {
                        syncer = this._instanceData[selType] as IDataSyncer;
                    }
                    else
                    {
                        syncer = this._typeResolver.CreateInstance<IDataSyncer>(selType);
                    }

                    syncDb.DataSynchronizer = syncer;
                    syncDb.PackageSynchronizer = syncer as IPackageSyncer<TravelRoute>;
                }
            }

            var currencyProvider = this.ResultEnvironment.CurrencyProvider;
            if (currencyProvider != null)
            {
                var selCurrencies = new List<string>();
                foreach (var item in this.lstCurrency.CheckedItems)
                {
                    var cur = (KeyValuePair<string, CurrencyInfo>)item;
                    selCurrencies.Add(cur.Key);
                }

                currencyProvider.AllowedCurrencies = selCurrencies;
            }
        }

        /// <summary>The update buttons.</summary>
        private void UpdateButtons()
        {
            this.btnImportToDatabase.Enabled =
                this.btnExportDatabase.Enabled = this.ResultEnvironment.FareDatabase != null && this.ResultEnvironment.ArchiveManager != null;
            this.btnRepairDatabase.Enabled = this.btnResetDatabase.Enabled = this.btnDbStat.Enabled = this.ResultEnvironment.FareDatabase != null;
            this.grpDatabaseSync.Enabled = this.ResultEnvironment.FareDatabase != null && this.cbDbSyncer.SelectedValue != null
                                           && this.imgSyncerStatus.Tag != null && (bool)this.imgSyncerStatus.Tag;
        }

        /// <summary>The validate environment.</summary>
        private void ValidateEnvironment()
        {
            this.Validate(this.ResultEnvironment.ArchiveManager, this.imgArchiveStatus);
            this.Validate(this.ResultEnvironment.FareDatabase, this.imgDatabaseStatus);
            this.Validate(this.ResultEnvironment.FareDataProvider, this.imgHandlerStatus);
            var syncDb = this.ResultEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                this.Validate(syncDb.DataSynchronizer, this.imgSyncerStatus);
            }

            this.UpdateButtons();
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="targetPictureBox">
        /// The target picture box.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool Validate(IPlugin target, PictureBox targetPictureBox)
        {
            var status = "Ok";
            var isValid = false;

            if (target == null)
            {
                status = "You have not selected any item!";
            }

            if (target != null)
            {
                isValid = true;
                if (target.Configuration != null)
                {
                    var valResult = target.Configuration.Validate();
                    if (!(isValid = valResult.Succeeded))
                    {
                        status = valResult.ErrorMessage;
                    }
                }
            }

            targetPictureBox.Image = isValid ? Resources.Success : Resources.Warning;
            targetPictureBox.Cursor = isValid ? Cursors.Default : Cursors.Hand;
            targetPictureBox.Tag = isValid;
            this.toolTip.SetToolTip(targetPictureBox, status);

            return isValid;
        }

        /// <summary>
        /// The btn info_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnInfo_Click(object sender, EventArgs e)
        {
            var cb = ((Control)sender).Tag as ComboBox;
            if (cb != null)
            {
                var selItem = cb.SelectedValue as Type;
                if (selItem != null)
                {
                    var detail = selItem.GetPluginDetail();
                    MessageBox.Show(this, detail.Value, detail.Key, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        /// <summary>
        /// The btn reset_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            var cb = ((Control)sender).Tag as ComboBox;
            if (cb != null)
            {
                var selItem = cb.SelectedValue as Type;
                if (selItem != null)
                {
                    IPlugin instance = null;
                    if (this._instanceData.ContainsKey(selItem))
                    {
                        instance = this._instanceData[selItem];
                    }
                    else
                    {
                        instance = this._typeResolver.CreateInstance<IPlugin>(selItem);
                        this._instanceData.Add(selItem, instance);
                    }

                    var defaultConf = instance.DefaultConfig;

                    if (defaultConf == null)
                    {
                        MessageBox.Show(
                            this, 
                            "This type does not need to be configured", 
                            "No configuration", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Asterisk);
                    }
                    else if (MessageBox.Show(
                        this, 
                        "Are you sure you want to reset the default settings for this plugin?", 
                        "Reset Settings", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        instance.Configuration = defaultConf;
                        this.ApplyChanges();
                        this.ValidateEnvironment();
                    }
                }
            }
        }

        /// <summary>
        /// The btn config_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnConfig_Click(object sender, EventArgs e)
        {
            var cb = ((Control)sender).Tag as ComboBox;
            if (cb != null)
            {
                var selItem = cb.SelectedValue as Type;
                if (selItem != null)
                {
                    IPlugin instance = null;
                    if (this._instanceData.ContainsKey(selItem))
                    {
                        instance = this._instanceData[selItem];
                    }
                    else
                    {
                        instance = this._typeResolver.CreateInstance<IPlugin>(selItem);
                        instance.Logger = this._logger;
                        this._instanceData.Add(selItem, instance);
                    }

                    if (instance.Configuration == null)
                    {
                        instance.Configuration = instance.DefaultConfig;
                    }

                    var config = instance.Configuration;
                    if (instance.Configuration == null)
                    {
                        MessageBox.Show(
                            this, 
                            "This type does not need to be configured", 
                            "No configuration", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Asterisk);
                    }
                    else
                    {
                        if (instance.CustomConfigBuilder != null)
                        {
                            instance.Configuration = instance.CustomConfigBuilder.Configure(instance);
                        }
                        else
                        {
                            instance.Configuration = new DefaultConfigBuilder(this._logger).Configure(instance);
                        }
                    }
                }
            }

            this.ApplyChanges();
            this.ValidateEnvironment();
        }

        /// <summary>
        /// The cb fare database_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void cbFareDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null)
            {
                var selItem = cb.SelectedValue as Type;
                if (selItem != null && typeof(ISyncableDatabase).IsAssignableFrom(selItem))
                {
                    this.FillSyncerComboBox(selItem);
                }
            }
        }

        /// <summary>
        /// The btn save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.txtDeparture.SelectedAirportCode == this.txtDestination.SelectedAirportCode)
            {
                MessageBox.Show(
                    this, 
                    "Default departure and destination airport cannot be the same!" + Environment.NewLine + "You selected: "
                    + this.txtDeparture.SelectedAirport, 
                    "Invalid setting", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
                this.DialogResult = DialogResult.None;
                return;
            }

            this.ApplyChanges();
            this._executionParam.Departure = this.txtDeparture.SelectedAirport;
            this._executionParam.Destination = this.txtDestination.SelectedAirport;
            this._executionParam.ConfigHandler.SaveData(this._executionParam);

            this.ResultEnvironment.ConfigStore.SaveEnv(this.ResultEnvironment);
        }

        /// <summary>
        /// The btn backup sync db_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnBackupSyncDb_Click(object sender, EventArgs e)
        {
            var syncDb = this.GetSyncDatabase();
            if (syncDb != null)
            {
                AppContext.ExecuteTask(
                    null, 
                    "Backup database", 
                    "Please wait...", 
                    this.GetType() + "-BackupDb", 
                    ProgressStyle.Marquee, 
                    this._logger, 
                    callback => { syncDb.Synchronize(SyncOperation.Upload, callback); });
            }
        }

        /// <summary>
        /// The btn restore sync db_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRestoreSyncDb_Click(object sender, EventArgs e)
        {
            var syncDb = this.GetSyncDatabase();
            if (syncDb != null)
            {
                AppContext.ExecuteTask(
                    this, 
                    "Restoring database", 
                    "Please wait...", 
                    this.GetType().Name + "-ImportDb", 
                    ProgressStyle.Continuous, 
                    this._logger, 
                    callback => syncDb.Synchronize(SyncOperation.Download, callback));
            }
        }

        /// <summary>The get sync database.</summary>
        /// <returns>The <see cref="ISyncableDatabase" />.</returns>
        private ISyncableDatabase GetSyncDatabase()
        {
            this.ApplyChanges();
            ISyncableDatabase result = null;
            var syncDb = this.ResultEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb == null)
            {
                MessageBox.Show(
                    this, 
                    "The selected database is not syncable", 
                    "Unsupported Operation", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (syncDb.DataSynchronizer == null)
                {
                    MessageBox.Show(
                        this, 
                        "There is no compatible database synchronizer selected for the fare database", 
                        "Unsupported Operation", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    result = syncDb;
                }
            }

            return result;
        }

        /// <summary>
        /// The export database.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        private void ExportDatabase(DataFormat format)
        {
            this.ApplyChanges();
            using (var selectPath = new FolderBrowserDialog())
            {
                selectPath.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                if (selectPath.ShowDialog(this) == DialogResult.OK)
                {
                    var exportPath = selectPath.SelectedPath;
                    AppContext.ExecuteTask(
                        this, 
                        "Exporting database", 
                        "Please wait...", 
                        this.GetType().Name + "-ExportDb", 
                        ProgressStyle.Continuous, 
                        this._logger, 
                        callback => { this.DoExportDatabase(exportPath, format, format == DataFormat.Binary ? 500 : 30, callback); });
                }
            }
        }

        /// <summary>
        /// The do export database.
        /// </summary>
        /// <param name="exportPath">
        /// The export path.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="batchSize">
        /// The batch size.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        private void DoExportDatabase(string exportPath, DataFormat format, int batchSize, IProgressCallback callback)
        {
            var routes = this.ResultEnvironment.FareDatabase.GetRoutes(true, false, false, false, callback);
            var journeyBatch = new List<Journey>();
            var routesBatch = new List<TravelRoute>();
            var jCount = routes.Sum(r => r.Journeys.Count);
            var stackCount = 0;

            callback.Begin(0, jCount);
            callback.Title = string.Format("Processing {0} journeys...", jCount);
            var routesCount = routes.Count;
            var lastRoutesIdx = routesCount - 1;

            for (var i = 0; i < routesCount; i++)
            {
                if (callback.IsAborting)
                {
                    return;
                }

                var route = routes[i];
                callback.Text = string.Format("{0} - {1}", route.Departure, route.Destination);
                var journeys = route.Journeys;
                var journeysCount = journeys.Count;
                var lastJourneyIdx = journeysCount - 1;

                if (journeysCount > 0)
                {
                    for (var j = 0; j < journeysCount; j++)
                    {
                        var journey = journeys[j];
                        journeyBatch.Add(journey);
                        if (journeyBatch.Count >= batchSize || j == lastJourneyIdx)
                        {
                            // Batch is full or this is the last item
                            var expRoute = new TravelRoute(route); // Gather journeys into 1 exported route
                            expRoute.AddJourney(journeyBatch, false);
                            this.ResultEnvironment.FareDatabase.LoadData(journeyBatch, true, callback);
                            routesBatch.Add(expRoute);
                            stackCount += journeyBatch.Count;
                            journeyBatch.Clear();
                        }

                        if (stackCount >= batchSize || i == lastRoutesIdx)
                        {
                            if (routesBatch.Count > 0)
                            {
                                this.ResultEnvironment.ArchiveManager.ExportData(routesBatch, exportPath, format, callback);
                                routesBatch.Clear();
                            }

                            callback.Increment(stackCount);
                            stackCount = 0;
                        }
                    }
                }

                callback.Increment(1);
            }
        }

        /// <summary>
        /// The btn import to database_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnImportToDatabase_Click(object sender, EventArgs e)
        {
            this.ApplyChanges();
            using (var selectPath = new FolderBrowserDialog())
            {
                selectPath.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                if (selectPath.ShowDialog(this) == DialogResult.OK)
                {
                    var importPath = selectPath.SelectedPath;
                    var dataOptions = new DataOptions { ArchiveDataFiles = false, ShowProgressDialog = true };
                    AppContext.ExecuteTask(
                        null, 
                        "Import data package", 
                        "Please wait...", 
                        this.GetType() + "-ImportData", 
                        ProgressStyle.Marquee, 
                        this._logger, 
                        callback => { this.ResultEnvironment.ArchiveManager.ImportData(importPath, dataOptions, callback); });
                }
            }
        }

        /// <summary>
        /// The btn repair database_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRepairDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                this.ApplyChanges();
                AppContext.ExecuteTask(
                    null, 
                    "Repair database", 
                    "Please wait...", 
                    this.GetType() + "-RepairDb", 
                    ProgressStyle.Marquee, 
                    this._logger, 
                    callback =>
                        {
                            this.ResultEnvironment.FareDatabase.RepairDatabase(callback);
                            callback.Inform(this, "Database was successfully repaired!", "Database Maintenance", NotificationType.Info);
                        });
            }
            catch (Exception ex)
            {
                var err = "Failed to repair database: " + ex.Message;
                this._logger.Error(err + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(this, err, "Database Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The btn reset database_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnResetDatabase_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, @"Are you sure you want to empty the whole database?
(ALL DATA WILL BE LOST)", "Database Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    this.ApplyChanges();
                    AppContext.ExecuteTask(
                        null, 
                        "Reset database", 
                        "Please wait...", 
                        this.GetType() + "-ResetDb", 
                        ProgressStyle.Marquee, 
                        this._logger, 
                        callback =>
                            {
                                this.ResultEnvironment.FareDatabase.Reset(callback);
                                callback.Inform(this, "Database was successfully reset!", "Database Maintenance", NotificationType.Info);
                            });
                }
                catch (Exception ex)
                {
                    var err = "Failed to reset database: " + ex.Message;
                    this._logger.Error(err + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(this, err, "Database Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// The btn db stat_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDbStat_Click(object sender, EventArgs e)
        {
            try
            {
                this.ApplyChanges();
                AppContext.ExecuteTask(
                    null, 
                    "Database statistics", 
                    "Please wait...", 
                    this.GetType() + "-DbStatistics", 
                    ProgressStyle.Marquee, 
                    this._logger, 
                    callback =>
                        {
                            callback.Inform(this, this.ResultEnvironment.FareDatabase.GetStatistics(), "Database Statistics", NotificationType.Info);
                        });
            }
            catch (Exception ex)
            {
                var err = "Failed to gather statistics for database: " + ex.Message;
                this._logger.Error(err + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(this, err, "Database Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The export db xml tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void exportDbXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExportDatabase(DataFormat.XML);
        }

        /// <summary>
        /// The export db binary tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void exportDbBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ExportDatabase(DataFormat.Binary);
        }

        /// <summary>
        /// The btn export database_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnExportDatabase_Click(object sender, EventArgs e)
        {
            this.btnExportDatabase.ShowContextMenuStrip();
        }

        /// <summary>
        /// The btn select all currency_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnSelectAllCurrency_Click(object sender, EventArgs e)
        {
            this.CheckAllCurrencies(true);
        }

        /// <summary>
        /// The btn select none currency_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnSelectNoneCurrency_Click(object sender, EventArgs e)
        {
            this.CheckAllCurrencies(false);
        }

        /// <summary>
        /// The check all currencies.
        /// </summary>
        /// <param name="newState">
        /// The new state.
        /// </param>
        private void CheckAllCurrencies(bool newState)
        {
            for (var i = 0; i < this.lstCurrency.Items.Count; i++)
            {
                this.lstCurrency.SetItemChecked(i, newState);
            }
        }

        /// <summary>
        /// The config changed_ event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void configChanged_EventHandler(object sender, EventArgs e)
        {
            if (!this._initialized)
            {
                return;
            }

            this.ApplyChanges();
            this.ValidateEnvironment();
        }

        /// <summary>
        /// The btn default currency_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDefaultCurrency_Click(object sender, EventArgs e)
        {
            if (this.ResultEnvironment.CurrencyProvider == null)
            {
                MessageBox.Show(
                    this, 
                    "There is no currency provider available", 
                    "Invalid Operation", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                this.ResultEnvironment.CurrencyProvider.Configuration = this.ResultEnvironment.CurrencyProvider.DefaultConfig;
                this.BindCurrencies(this.ResultEnvironment);
            }
        }

        /// <summary>
        /// The btn import config_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnImportConfig_Click(object sender, EventArgs e)
        {
            using (var selectFileDlg = new OpenFileDialog())
            {
                selectFileDlg.Title = "Import " + AppUtil.ProductName + " Configuration";
                selectFileDlg.Filter = AppUtil.ProductName + " Configuration File (*.bin)|*.bin";
                var result = selectFileDlg.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    MonitorEnvironment newEnv = null;
                    try
                    {
                        newEnv = this._env.ConfigStore.LoadEnv(selectFileDlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.ToString());
                    }

                    if (newEnv == null)
                    {
                        MessageBox.Show(
                            this, 
                            "The selected configuration file is corrupted or does not have a valid file format", 
                            "Invalid configuration file", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        newEnv.ConfigStore = this._env.ConfigStore;
                        this._env = newEnv;
                        this.InitializeData();
                    }
                }
            }
        }

        /// <summary>
        /// The btn export config_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnExportConfig_Click(object sender, EventArgs e)
        {
            using (var selectFileDlg = new SaveFileDialog())
            {
                selectFileDlg.Title = "Export " + AppUtil.ProductName + " Configuration";
                selectFileDlg.Filter = AppUtil.ProductName + " Configuration File (*.bin)|*.bin";
                var result = selectFileDlg.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    try
                    {
                        this.ApplyChanges();
                        this._env.ConfigStore.SaveEnv(this._env, selectFileDlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            this, 
                            "Failed to export configuration file:" + Environment.NewLine + ex.Message, 
                            "Configuration export failed", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

    /// <summary>The plugin info holder.</summary>
    public class PluginInfoHolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInfoHolder"/> class.
        /// </summary>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public PluginInfoHolder(Type targetType)
        {
            if (targetType == null)
            {
                throw new ArgumentException("Type cannot be null");
            }

            this.Type = targetType;
        }

        /// <summary>Gets the name.</summary>
        public string Name
        {
            get
            {
                return this.Type.GetPluginDetail().Key;
            }
        }

        /// <summary>Gets the type.</summary>
        public Type Type { get; private set; }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}