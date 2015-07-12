using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;
using Timers = System.Timers;
using SkyDean.FareLiz.Service.Versioning;

namespace SkyDean.FareLiz.Service.LiveUpdate
{
    /// <summary>
    /// Service providing live update functionality
    /// </summary>
    public class LiveUpdateService : IServiceRunner, IHelperService, IDisposable
    {
        private Timers.Timer _updateTimer = new Timers.Timer() { AutoReset = false, Enabled = false };
        private LiveUpdateConfiguration _config;
        private LiveUpdateClient _client;
        private readonly HashSet<Version> _ignoreVersions = new HashSet<Version>();
        private const int _initialDelay = 3000;

        public IConfig Configuration { get { return _config; } set { _config = value as LiveUpdateConfiguration; } }
        public IConfig DefaultConfig { get { return new LiveUpdateConfiguration { AutoUpdate = true, AutoUpdateIntervalHours = 8, Silent = false }; } }
        public IConfigBuilder CustomConfigBuilder { get { return null; } }
        public Image ProductLogo { get; set; }
        public Form ParentForm { get; set; }
        private ILog _logger;
        public ILog Logger
        {
            get { return _logger; }
            set
            {
                _versionRetriever.Logger = _client.Logger = _logger = value;
            }
        }

        private readonly IVersionRetriever _versionRetriever;
        protected virtual IVersionRetriever VersionRetriever
        {
            get { return _versionRetriever; }
        }

        public LiveUpdateService()
        {
            _versionRetriever = new OnlineVersionRetriever(Logger);
            _client = new LiveUpdateClient(_versionRetriever, Logger);
            Configuration = DefaultConfig;
            _updateTimer.Elapsed += UpdateTimer_Elapsed;
            Status = HelperServiceStatus.Stopped;
        }

        public void RunService(string[] args)
        {
            var updateParam = UpdateParameter.FromCommandLine(args);
            var updateRunner = new LiveUpdateRunner(updateParam, Logger);
            updateRunner.DoUpdate();
        }

        public void Initialize() { }

        public void Start()
        {
            SetAutomaticUpdate(_config.AutoUpdate);
            Status = HelperServiceStatus.Running;
        }

        public void Stop()
        {
            SetAutomaticUpdate(false);
            Status = HelperServiceStatus.Stopped;
        }

        public HelperServiceStatus Status { get; private set; }

        public void SetAutomaticUpdate(bool newValue)
        {
            _updateTimer.Stop();
            if (newValue)
            {
                _updateTimer.Interval = _initialDelay;
                _updateTimer.Start();
            }
            else
                _updateTimer.Interval = int.MaxValue;
        }

        public void Dispose()
        {
            if (_updateTimer != null)
            {
                _updateTimer.Elapsed -= UpdateTimer_Elapsed;
                _updateTimer.Stop();
                _updateTimer.Dispose();
            }
        }

        /// <summary>
        /// On event: updateTimer ticks
        /// </summary>
        private void UpdateTimer_Elapsed(object sender, Timers.ElapsedEventArgs e)
        {
            lock (_client)
            {
                try
                {
                    CheckForUpdates(false, true);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message + ex.StackTrace);  // Silent
                }

                if (_updateTimer != null && _updateTimer.Interval != int.MaxValue)  // If timer was not requested to stop
                {
                    _updateTimer.Interval = _config.AutoUpdateIntervalHours * 3600000;
                    _updateTimer.Start();
                }
            }
        }

        public void CheckForUpdates(bool notifyIfNoUpdate, bool ignoreSkippedVersion)
        {
            if (!Monitor.TryEnter(_ignoreVersions))
                return;

            try
            {
                var newUpdate = _client.CheckForUpdate();
                if (newUpdate != null)
                {
                    if (!ignoreSkippedVersion || !_ignoreVersions.Contains(newUpdate.ToVersion.VersionNumber))
                    {
                        using (var liveUpdateForm = new LiveUpdateClientForm(newUpdate, _client, ProductLogo))
                            if (liveUpdateForm.ShowDialog() == DialogResult.Cancel)
                                _ignoreVersions.Add(newUpdate.ToVersion.VersionNumber);
                    }
                }
                else if (notifyIfNoUpdate)
                    MessageBox.Show("The current version is up-to-date", "No update available", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                Monitor.Exit(_ignoreVersions);
            }
        }
    }
}
