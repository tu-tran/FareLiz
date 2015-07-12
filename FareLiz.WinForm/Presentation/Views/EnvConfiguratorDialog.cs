using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Presentation;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.WinForm.Components.Dialog;
using SkyDean.FareLiz.WinForm.Config;
using SkyDean.FareLiz.WinForm.Properties;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    internal partial class EnvConfiguratorDialog : SmartForm
    {
        private readonly Dictionary<Type, IPlugin> _instanceData = new Dictionary<Type, IPlugin>();
        private volatile bool _initialized = false;
        private readonly ILog _logger;
        private readonly TypeResolver _typeResolver;

        private MonitorEnvironment _env;
        internal MonitorEnvironment ResultEnvironment { get { return _env; } }

        private readonly ExecutionParam _executionParam;
        internal ExecutionParam ResultParam { get { return _executionParam; } }

        protected EnvConfiguratorDialog()
        {
            InitializeComponent();

            btnInfoArchiveManager.Tag = btnResetArchiveManager.Tag = btnConfigArchiveManager.Tag = cbArchiveManager;
            btnInfoFareProvider.Tag = btnResetFareProvider.Tag = btnConfigFareProvider.Tag = cbFareDataProvider;
            btnInfoFareDatabase.Tag = btnResetFareDatabase.Tag = btnConfigFareDatabase.Tag = cbFareDatabase;
            btnInfoDbSyncer.Tag = btnResetDbSyncer.Tag = btnConfigDbSyncer.Tag = cbDbSyncer;
        }

        internal EnvConfiguratorDialog(MonitorEnvironment environment, ExecutionParam executionParam)
            : this()
        {
            if (environment == null)
                throw new ArgumentException("Environment cannot be null");
            if (environment.Logger == null)
                throw new ArgumentException("Environment logger cannot be null");
            if (environment.ConfigStore == null)
                throw new ArgumentException("Environment config store cannot be null");
            if (executionParam.ConfigHandler == null)
                throw new ArgumentException("Execution param config store cannot be null");

            _logger = environment.Logger;
            _typeResolver = new TypeResolver(_logger);
            _executionParam = executionParam.ReflectionDeepClone(_logger);

            Height -= pnlTop.Height;
            _env = environment.ReflectionDeepClone(_logger);
#if DEBUG
            _env.Id = Guid.NewGuid().ToString();
#endif

            InitializeData();
        }

        private void InitializeData()
        {
            _initialized = false;
            _instanceData.Clear();
            if (_env.ArchiveManager != null) _instanceData.Add(_env.ArchiveManager.GetType(), _env.ArchiveManager);
            if (_env.FareDataProvider != null) _instanceData.Add(_env.FareDataProvider.GetType(), _env.FareDataProvider);
            if (_env.FareDatabase != null)
            {
                _instanceData.Add(_env.FareDatabase.GetType(), _env.FareDatabase);
                var syncDb = _env.FareDatabase as ISyncableDatabase;
                if (syncDb != null && syncDb.DataSynchronizer != null)
                {
                    _instanceData.Add(syncDb.DataSynchronizer.GetType(), syncDb.DataSynchronizer);
                }
            }

            var resolver = _env.PluginResolver;
            Bind(cbArchiveManager, resolver.GetArchiveManagerTypes(), _env.ArchiveManager);
            Bind(cbFareDataProvider, resolver.GetFareDataProviderTypes(), _env.FareDataProvider);
            Bind(cbFareDatabase, resolver.GetFareDatabaseTypes(), _env.FareDatabase);
            BindCurrencies(_env);
            _initialized = true;
            ApplyChanges();
            ValidateEnvironment();

            txtDeparture.SelectedAirport = _executionParam.Departure;
            txtDestination.SelectedAirport = _executionParam.Destination;
        }

        private void FillSyncerComboBox(Type targetDbType)
        {
            if (typeof(ISyncableDatabase).IsAssignableFrom(targetDbType))
            {
                var selectedSyncer = GetActiveDbSyncer();
                Bind(cbDbSyncer, _env.PluginResolver.GetDbSyncerTypes(targetDbType), selectedSyncer);
            }
        }

        private object GetActiveDbSyncer()
        {
            if (_env.FareDatabase != null)
            {
                var syncDb = _env.FareDatabase as ISyncableDatabase;
                if (syncDb != null)
                    return syncDb.DataSynchronizer;
            }

            return null;
        }

        private void Bind(ComboBox cb, IList<Type> data, object selectedInstance)
        {
            if (data == null || data.Count < 1)
                cb.DataSource = null;
            else
            {
                var ds = new PluginInfoHolder[data.Count];
                for (int i = 0; i < data.Count; i++)
                    ds[i] = new PluginInfoHolder(data[i]);

                cb.DisplayMember = "Name";
                cb.ValueMember = "Type";
                cb.DataSource = ds;
            }

            if (selectedInstance == null)
            {
                if (data != null && data.Count > 0)
                    cb.SelectedIndex = 0;
            }
            else
            {
                cb.SelectedValue = selectedInstance.GetType();
            }
        }

        private void BindCurrencies(MonitorEnvironment env)
        {
            ICurrencyProvider provider = env.CurrencyProvider;
            if (provider == null)
                throw new ArgumentException("Missing Currency Provider");

            var allCurrencies = provider.GetCurrencies();
            if (provider.Configuration == null)
                provider.Configuration = provider.DefaultConfig;

            int index = 0;
            var sel = provider.AllowedCurrencies;
            lstCurrency.Items.Clear();  // Clear the list of items before repopulating the default currency list
            foreach (var item in allCurrencies)
            {
                lstCurrency.Items.Add(item);
                bool isSelected = (sel != null && sel.Contains(item.Key));
                lstCurrency.SetItemChecked(index, isSelected);
                index++;
            }
        }

        private void ApplyChanges()
        {
            if (!_initialized)
                return;

            var selType = cbArchiveManager.SelectedValue as Type;
            if (selType != null)
            {
                if (_instanceData.ContainsKey(selType))
                    ResultEnvironment.ArchiveManager = _instanceData[selType] as IArchiveManager;
                else
                {
                    ResultEnvironment.ArchiveManager = _typeResolver.CreateInstance<IArchiveManager>(selType);
                    ResultEnvironment.ArchiveManager.Configuration = ResultEnvironment.ArchiveManager.DefaultConfig;
                }
            }

            selType = cbFareDatabase.SelectedValue as Type;
            if (selType != null)
            {
                if (_instanceData.ContainsKey(selType))
                    ResultEnvironment.FareDatabase = _instanceData[selType] as IFareDatabase;
                else
                {
                    ResultEnvironment.FareDatabase = _typeResolver.CreateInstance<IFareDatabase>(selType);
                    ResultEnvironment.FareDatabase.Configuration = ResultEnvironment.FareDatabase.DefaultConfig;
                }
            }

            selType = cbFareDataProvider.SelectedValue as Type;
            if (selType != null)
            {
                if (_instanceData.ContainsKey(selType))
                    ResultEnvironment.FareDataProvider = _instanceData[selType] as IFareDataProvider;
                else
                {

                    ResultEnvironment.FareDataProvider = _typeResolver.CreateInstance<IFareDataProvider>(selType) as IFareDataProvider;
                    ResultEnvironment.FareDataProvider.Configuration = ResultEnvironment.FareDataProvider.DefaultConfig;
                }
            }

            var syncDb = ResultEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                selType = cbDbSyncer.SelectedValue as Type;
                if (selType != null)
                {
                    IDataSyncer syncer;
                    if (_instanceData.ContainsKey(selType))
                        syncer = _instanceData[selType] as IDataSyncer;
                    else
                        syncer = _typeResolver.CreateInstance<IDataSyncer>(selType);

                    syncDb.DataSynchronizer = syncer;
                    syncDb.PackageSynchronizer = syncer as IPackageSyncer<TravelRoute>;
                }
            }

            var currencyProvider = ResultEnvironment.CurrencyProvider;
            if (currencyProvider != null)
            {
                var selCurrencies = new List<string>();
                foreach (var item in lstCurrency.CheckedItems)
                {
                    var cur = (KeyValuePair<string, CurrencyInfo>)item;
                    selCurrencies.Add(cur.Key);
                }
                currencyProvider.AllowedCurrencies = selCurrencies;
            }
        }

        private void UpdateButtons()
        {
            btnImportToDatabase.Enabled = btnExportDatabase.Enabled = (ResultEnvironment.FareDatabase != null && ResultEnvironment.ArchiveManager != null);
            btnRepairDatabase.Enabled = btnResetDatabase.Enabled = btnDbStat.Enabled = ResultEnvironment.FareDatabase != null;
            grpDatabaseSync.Enabled = (ResultEnvironment.FareDatabase != null && cbDbSyncer.SelectedValue != null
                && imgSyncerStatus.Tag != null && (bool)imgSyncerStatus.Tag);
        }

        private void ValidateEnvironment()
        {
            Validate(ResultEnvironment.ArchiveManager, imgArchiveStatus);
            Validate(ResultEnvironment.FareDatabase, imgDatabaseStatus);
            Validate(ResultEnvironment.FareDataProvider, imgHandlerStatus);
            ISyncableDatabase syncDb = ResultEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                Validate(syncDb.DataSynchronizer, imgSyncerStatus);
            }

            UpdateButtons();
        }

        private bool Validate(IPlugin target, PictureBox targetPictureBox)
        {
            string status = "Ok";
            bool isValid = false;

            if (target == null)
                status = "You have not selected any item!";
            if (target != null)
            {
                isValid = true;
                if (target.Configuration != null)
                {
                    var valResult = target.Configuration.Validate();
                    if (!(isValid = valResult.Succeeded))
                        status = valResult.ErrorMessage;
                }
            }

            targetPictureBox.Image = (isValid ? Resources.Success : Resources.Warning);
            targetPictureBox.Cursor = (isValid ? Cursors.Default : Cursors.Hand);
            targetPictureBox.Tag = isValid;
            toolTip.SetToolTip(targetPictureBox, status);

            return isValid;
        }

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            var cb = ((Control)sender).Tag as ComboBox;
            if (cb != null)
            {
                var selItem = cb.SelectedValue as Type;
                if (selItem != null)
                {
                    IPlugin instance = null;
                    if (_instanceData.ContainsKey(selItem))
                        instance = _instanceData[selItem];
                    else
                    {
                        instance = _typeResolver.CreateInstance<IPlugin>(selItem);
                        _instanceData.Add(selItem, instance);
                    }

                    var defaultConf = instance.DefaultConfig;

                    if (defaultConf == null)
                        MessageBox.Show(this, "This type does not need to be configured", "No configuration", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    else if (MessageBox.Show(this, "Are you sure you want to reset the default settings for this plugin?", "Reset Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        instance.Configuration = defaultConf;
                        ApplyChanges();
                        ValidateEnvironment();
                    }
                }
            }
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            var cb = ((Control)sender).Tag as ComboBox;
            if (cb != null)
            {
                var selItem = cb.SelectedValue as Type;
                if (selItem != null)
                {
                    IPlugin instance = null;
                    if (_instanceData.ContainsKey(selItem))
                        instance = _instanceData[selItem];
                    else
                    {
                        instance = _typeResolver.CreateInstance<IPlugin>(selItem);
                        instance.Logger = _logger;
                        _instanceData.Add(selItem, instance);
                    }

                    if (instance.Configuration == null)
                        instance.Configuration = instance.DefaultConfig;

                    IConfig config = instance.Configuration;
                    if (instance.Configuration == null)
                        MessageBox.Show(this, "This type does not need to be configured", "No configuration", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    else
                    {
                        if (instance.CustomConfigBuilder != null)
                            instance.Configuration = instance.CustomConfigBuilder.Configure(instance);
                        else
                        {
                            instance.Configuration = (new DefaultConfigBuilder(_logger).Configure(instance));
                        }
                    }
                }
            }

            ApplyChanges();
            ValidateEnvironment();
        }

        private void cbFareDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            if (cb != null)
            {
                var selItem = cb.SelectedValue as Type;
                if (selItem != null && typeof(ISyncableDatabase).IsAssignableFrom(selItem))
                {
                    FillSyncerComboBox(selItem);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDeparture.SelectedAirportCode == txtDestination.SelectedAirportCode)
            {
                MessageBox.Show(this, "Default departure and destination airport cannot be the same!" + Environment.NewLine + 
                    "You selected: " + txtDeparture.SelectedAirport, "Invalid setting",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                DialogResult = DialogResult.None;
                return;
            }

            ApplyChanges();
            _executionParam.Departure = txtDeparture.SelectedAirport;
            _executionParam.Destination = txtDestination.SelectedAirport;
            _executionParam.ConfigHandler.SaveData(_executionParam);

            ResultEnvironment.ConfigStore.SaveEnv(ResultEnvironment);
        }

        private void btnBackupSyncDb_Click(object sender, EventArgs e)
        {
            var syncDb = GetSyncDatabase();
            if (syncDb != null)
                syncDb.Synchronize(SyncOperation.Upload, AppContext.ProgressCallback);
        }

        private void btnRestoreSyncDb_Click(object sender, EventArgs e)
        {
            var syncDb = GetSyncDatabase();
            if (syncDb != null)
                syncDb.Synchronize(SyncOperation.Download, AppContext.ProgressCallback);
        }

        private ISyncableDatabase GetSyncDatabase()
        {
            ApplyChanges();
            ISyncableDatabase result = null;
            var syncDb = ResultEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb == null)
                MessageBox.Show(this, "The selected database is not syncable", "Unsupported Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                if (syncDb.DataSynchronizer == null)
                    MessageBox.Show(this, "There is no compatible database synchronizer selected for the fare database", "Unsupported Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else
                    result = syncDb;
            }

            return result;
        }

        private void ExportDatabase(DataFormat format)
        {
            ApplyChanges();
            using (var selectPath = new FolderBrowserDialog())
            {
                selectPath.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                if (selectPath.ShowDialog(this) == DialogResult.OK)
                {
                    var exportPath = selectPath.SelectedPath;
                    ProgressDialog.ExecuteTask(this, "Exporting database", "Please wait...", GetType().Name + "-ExportDb", ProgressBarStyle.Continuous, _logger, callback =>
                    {
                        callback.Begin();
                        DoExportDatabase(exportPath, format, format == DataFormat.Binary ? 500 : 30, callback);
                    });
                }
            }
        }

        private void DoExportDatabase(string exportPath, DataFormat format, int batchSize, IProgressCallback callback)
        {
            var routes = ResultEnvironment.FareDatabase.GetRoutes(true, false, false, false, callback);
            var journeyBatch = new List<Journey>();
            var routesBatch = new List<TravelRoute>();
            var jCount = routes.Sum(r => r.Journeys.Count);
            int stackCount = 0;

            callback.Begin(0, jCount);
            callback.Title = String.Format("Processing {0} journeys...", jCount);
            int routesCount = routes.Count;
            int lastRoutesIdx = routesCount - 1;

            for (int i = 0; i < routesCount; i++)
            {
                if (callback.IsAborting)
                    return;

                TravelRoute route = routes[i];
                callback.Text = String.Format("{0} - {1}", route.Departure, route.Destination);
                var journeys = route.Journeys;
                int journeysCount = journeys.Count;
                int lastJourneyIdx = journeysCount - 1;
                if (journeysCount > 0)
                {
                    for (int j = 0; j < journeysCount; j++)
                    {
                        var journey = journeys[j];
                        journeyBatch.Add(journey);
                        if (journeyBatch.Count >= batchSize || j == lastJourneyIdx) // Batch is full or this is the last item
                        {
                            var expRoute = new TravelRoute(route);  // Gather journeys into 1 exported route
                            expRoute.AddJourney(journeyBatch, false);
                            ResultEnvironment.FareDatabase.LoadData(journeyBatch, true, AppContext.ProgressCallback);
                            routesBatch.Add(expRoute);
                            stackCount += journeyBatch.Count;
                            journeyBatch.Clear();
                        }

                        if (stackCount >= batchSize || i == lastRoutesIdx)
                        {
                            if (routesBatch.Count > 0)
                            {
                                ResultEnvironment.ArchiveManager.ExportData(routesBatch, exportPath, format, AppContext.ProgressCallback);
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

        private void btnImportToDatabase_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            using (var selectPath = new FolderBrowserDialog())
            {
                selectPath.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                if (selectPath.ShowDialog(this) == DialogResult.OK)
                {
                    var importPath = selectPath.SelectedPath;
                    var dataOptions = new DataOptions { ArchiveDataFiles = false, ShowProgressDialog = true };
                    ResultEnvironment.ArchiveManager.ImportData(importPath, dataOptions, AppContext.ProgressCallback);
                }
            }
        }

        private void btnRepairDatabase_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyChanges();
                ResultEnvironment.FareDatabase.RepairDatabase(AppContext.ProgressCallback);
                MessageBox.Show(this, "Database was successfully repaired!", "Database Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                string err = "Failed to repair database: " + ex.Message;
                _logger.Error(err + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(this, err, "Database Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResetDatabase_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this,
@"Are you sure you want to empty the whole database?
(ALL DATA WILL BE LOST)", "Database Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

                try
                {
                    ApplyChanges();
                    ResultEnvironment.FareDatabase.Reset(AppContext.ProgressCallback);
                    MessageBox.Show(this, "Database was successfully reset!", "Database Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    string err = "Failed to reset database: " + ex.Message;
                    _logger.Error(err + Environment.NewLine + ex.StackTrace);
                    MessageBox.Show(this, err, "Database Maintenance", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDbStat_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyChanges();
                MessageBox.Show(this, ResultEnvironment.FareDatabase.GetStatistics(AppContext.ProgressCallback), "Database Statistics", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception ex)
            {
                string err = "Failed to gather statistics for database: " + ex.Message;
                _logger.Error(err + Environment.NewLine + ex.StackTrace);
                MessageBox.Show(this, err, "Database Query", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportDbXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportDatabase(DataFormat.XML);
        }

        private void exportDbBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportDatabase(DataFormat.Binary);
        }

        private void btnExportDatabase_Click(object sender, EventArgs e)
        {
            btnExportDatabase.ShowContextMenuStrip();
        }

        private void btnSelectAllCurrency_Click(object sender, EventArgs e)
        {
            CheckAllCurrencies(true);
        }

        private void btnSelectNoneCurrency_Click(object sender, EventArgs e)
        {
            CheckAllCurrencies(false);
        }

        private void CheckAllCurrencies(bool newState)
        {
            for (int i = 0; i < lstCurrency.Items.Count; i++)
                lstCurrency.SetItemChecked(i, newState);
        }

        private void configChanged_EventHandler(object sender, EventArgs e)
        {
            if (!_initialized)
                return;

            ApplyChanges();
            ValidateEnvironment();
        }

        private void btnDefaultCurrency_Click(object sender, EventArgs e)
        {
            if (ResultEnvironment.CurrencyProvider == null)
                MessageBox.Show(this, "There is no currency provider available", "Invalid Operation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
            {
                ResultEnvironment.CurrencyProvider.Configuration = ResultEnvironment.CurrencyProvider.DefaultConfig;
                BindCurrencies(ResultEnvironment);
            }
        }

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
                        newEnv = _env.ConfigStore.LoadEnv(selectFileDlg.FileName);
                    }
                    catch (Exception ex) { Console.Error.WriteLine(ex.ToString()); }

                    if (newEnv == null)
                        MessageBox.Show(this, "The selected configuration file is corrupted or does not have a valid file format", "Invalid configuration file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    else
                    {
                        newEnv.ConfigStore = _env.ConfigStore;
                        _env = newEnv;
                        InitializeData();
                    }
                }
            }
        }

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
                        ApplyChanges();
                        _env.ConfigStore.SaveEnv(_env, selectFileDlg.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, "Failed to export configuration file:" + Environment.NewLine + ex.Message, "Configuration export failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

    public class PluginInfoHolder
    {
        public string Name { get { return Type.GetPluginDetail().Key; } }
        public Type Type { get; private set; }
        public PluginInfoHolder(Type targetType)
        {
            if (targetType == null)
                throw new ArgumentException("Type cannot be null");

            Type = targetType;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
