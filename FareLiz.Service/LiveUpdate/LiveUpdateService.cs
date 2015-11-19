namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Timers;
    using System.Windows.Forms;

    using log4net;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Service.Versioning;

    using Timer = System.Timers.Timer;

    /// <summary>Service providing live update functionality</summary>
    public class LiveUpdateService : IServiceRunner, IHelperService, IDisposable
    {
        /// <summary>
        /// The _initial delay.
        /// </summary>
        private const int _initialDelay = 3000;

        /// <summary>
        /// The _ignore versions.
        /// </summary>
        private readonly HashSet<Version> _ignoreVersions = new HashSet<Version>();

        /// <summary>
        /// The _version retriever.
        /// </summary>
        private readonly IVersionRetriever _versionRetriever;

        /// <summary>
        /// The _client.
        /// </summary>
        private LiveUpdateClient _client;

        /// <summary>
        /// The _config.
        /// </summary>
        private LiveUpdateConfiguration _config;

        /// <summary>
        /// The _logger.
        /// </summary>
        private ILog _logger;

        /// <summary>
        /// The _update timer.
        /// </summary>
        private Timer _updateTimer = new Timer { AutoReset = false, Enabled = false };

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveUpdateService"/> class.
        /// </summary>
        public LiveUpdateService()
        {
            this._versionRetriever = new OnlineVersionRetriever(this.Logger);
            this._client = new LiveUpdateClient(this._versionRetriever, this.Logger);
            this.Configuration = this.DefaultConfig;
            this._updateTimer.Elapsed += this.UpdateTimer_Elapsed;
            this.Status = HelperServiceStatus.Stopped;
        }

        /// <summary>
        /// Gets or sets the product logo.
        /// </summary>
        public Image ProductLogo { get; set; }

        /// <summary>
        /// Gets or sets the parent form.
        /// </summary>
        public Form ParentForm { get; set; }

        /// <summary>
        /// Gets the version retriever.
        /// </summary>
        protected virtual IVersionRetriever VersionRetriever
        {
            get
            {
                return this._versionRetriever;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this._updateTimer != null)
            {
                this._updateTimer.Elapsed -= this.UpdateTimer_Elapsed;
                this._updateTimer.Stop();
                this._updateTimer.Dispose();
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            this.SetAutomaticUpdate(this._config.AutoUpdate);
            this.Status = HelperServiceStatus.Running;
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.SetAutomaticUpdate(false);
            this.Status = HelperServiceStatus.Stopped;
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public HelperServiceStatus Status { get; private set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public IConfig Configuration
        {
            get
            {
                return this._config;
            }

            set
            {
                this._config = value as LiveUpdateConfiguration;
            }
        }

        /// <summary>
        /// Gets the default config.
        /// </summary>
        public IConfig DefaultConfig
        {
            get
            {
                return new LiveUpdateConfiguration { AutoUpdate = true, AutoUpdateIntervalHours = 8, Silent = false };
            }
        }

        /// <summary>
        /// Gets the custom config builder.
        /// </summary>
        public IConfigBuilder CustomConfigBuilder
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILog Logger
        {
            get
            {
                return this._logger;
            }

            set
            {
                this._versionRetriever.Logger = this._client.Logger = this._logger = value;
            }
        }

        /// <summary>
        /// The run service.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public void RunService(string[] args)
        {
            var updateParam = UpdateParameter.FromCommandLine(args);
            var updateRunner = new LiveUpdateRunner(updateParam, this.Logger);
            updateRunner.DoUpdate();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// The set automatic update.
        /// </summary>
        /// <param name="newValue">
        /// The new value.
        /// </param>
        public void SetAutomaticUpdate(bool newValue)
        {
            this._updateTimer.Stop();
            if (newValue)
            {
                this._updateTimer.Interval = _initialDelay;
                this._updateTimer.Start();
            }
            else
            {
                this._updateTimer.Interval = int.MaxValue;
            }
        }

        /// <summary>
        /// On event: updateTimer ticks
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (this._client)
            {
                try
                {
                    this.CheckForUpdates(false, true);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex.Message + ex.StackTrace); // Silent
                }

                if (this._updateTimer != null && this._updateTimer.Interval != int.MaxValue)
                {
                    // If timer was not requested to stop
                    this._updateTimer.Interval = this._config.AutoUpdateIntervalHours * 3600000;
                    this._updateTimer.Start();
                }
            }
        }

        /// <summary>
        /// The check for updates.
        /// </summary>
        /// <param name="notifyIfNoUpdate">
        /// The notify if no update.
        /// </param>
        /// <param name="ignoreSkippedVersion">
        /// The ignore skipped version.
        /// </param>
        public void CheckForUpdates(bool notifyIfNoUpdate, bool ignoreSkippedVersion)
        {
            if (!Monitor.TryEnter(this._ignoreVersions))
            {
                return;
            }

            try
            {
                var newUpdate = this._client.CheckForUpdate();
                if (newUpdate != null)
                {
                    if (!ignoreSkippedVersion || !this._ignoreVersions.Contains(newUpdate.ToVersion.VersionNumber))
                    {
                        using (var liveUpdateForm = new LiveUpdateClientForm(newUpdate, this._client, this.ProductLogo))
                            if (liveUpdateForm.ShowDialog() == DialogResult.Cancel)
                            {
                                this._ignoreVersions.Add(newUpdate.ToVersion.VersionNumber);
                            }
                    }
                }
                else if (notifyIfNoUpdate)
                {
                    MessageBox.Show("The current version is up-to-date", "No update available", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            finally
            {
                Monitor.Exit(this._ignoreVersions);
            }
        }
    }
}