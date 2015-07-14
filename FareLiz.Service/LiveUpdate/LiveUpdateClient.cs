namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Service.Utils;

    /// <summary>Client used for retrieving version updates</summary>
    public class LiveUpdateClient
    {
        /// <summary>The _max backups.</summary>
        private const int _maxBackups = 2;

        /// <summary>The _base app path.</summary>
        private readonly string _baseAppPath = ProcessUtils.CurrentProcessDirectory;

        /// <summary>The _version retriever.</summary>
        private readonly IVersionRetriever _versionRetriever;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveUpdateClient"/> class.
        /// </summary>
        /// <param name="versionRetriever">
        /// The version retriever.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public LiveUpdateClient(IVersionRetriever versionRetriever, ILogger logger)
        {
            this._versionRetriever = versionRetriever;
            this.Logger = logger;
        }

        /// <summary>Gets or sets the logger.</summary>
        public ILogger Logger { get; set; }

        /// <summary>Check for latest update</summary>
        /// <returns>The upgradable version. Returns null if there is no newer version</returns>
        public UpdateRequest CheckForUpdate()
        {
            var newRequest = this._versionRetriever.CheckForUpdate();
            if (newRequest != null)
            {
                var curProcess = ProcessUtils.CurrentProcessLocation;

                var param = new UpdateParameter
                                {
                                    BackupPath = Path.Combine(this.GetBackupRoot(), newRequest.FromVersion.VersionNumber.ToString()), 
                                    PackageLocation = AppUtil.GetLocalDataPath(this.GetType().Name + "\\Packages\\" + Guid.NewGuid()), 
                                    ProcessToEnd = curProcess, 
                                    RestartProcess = curProcess, 
                                    NotifyMsg = true, 
                                    TargetUpdatePath = this._baseAppPath
                                };
                newRequest.Parameters = param;
            }

            return newRequest;
        }

        /// <summary>
        /// Apply the update using the downloaded resource
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void RunUpdate(UpdateRequest request)
        {
            AppContext.ExecuteTask(
                null, 
                "Live Update", 
                "Downloading update package into local drive...", 
                "RunLiveUpdate", 
                ProgressStyle.Marquee, 
                this.Logger, 

                // Run the actual update
                callback =>
                    {
                        callback.Begin();
                        this.Logger.Info(
                            "Upgrading {0} from {1} to {2}...", 
                            request.ProductName, 
                            request.FromVersion.VersionNumber, 
                            request.ToVersion.VersionNumber);
                        callback.Text = "Downloading data package...";
                        this.Logger.Info("Download version package for " + request.ToVersion.VersionNumber);
                        this._versionRetriever.DownloadPackage(request, request.Parameters.PackageLocation);
                        callback.Text = "Backing up current version...";
                        this.Logger.Info("Backup current version " + request.FromVersion.VersionNumber);
                        this.Backup(request);
                        callback.Text = "Cleaning up old backups...";
                        this.Logger.Info("Cleanup old version backups");
                        this.CleanBackup(); // Clean old backups
                        callback.Text = "Installing update...";
                        var runnerProcessInfo = LiveUpdateRunner.GetUpdateRunnerProcessInfo(request.Parameters);

                        // Prepare update runner process
                        var runnerProcess = Process.Start(runnerProcessInfo);
                        runnerProcess.WaitForExit();
                    }, 
                null, 
                null, 
                request.Parameters.NotifyMsg, 
                request.Parameters.NotifyMsg);
        }

        /// <summary>
        /// Backup the installation
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void Backup(UpdateRequest request)
        {
            var backupPath = request.Parameters.BackupPath;
            if (Directory.Exists(backupPath))
            {
                IOUtils.DeleteDirectory(backupPath);
            }

            if (Directory.Exists(request.Parameters.PackageLocation))
            {
                Directory.CreateDirectory(request.Parameters.BackupPath);
                var files = Directory.GetFiles(request.Parameters.PackageLocation);
                var targetUpdatePath = request.Parameters.TargetUpdatePath;
                foreach (var f in files)
                {
                    // Only backup files that are affected by the update package
                    var fileName = Path.GetFileName(f);
                    var affectFile = Path.Combine(targetUpdatePath, fileName);
                    if (File.Exists(affectFile))
                    {
                        var backupFile = Path.Combine(backupPath, fileName);
                        File.Copy(f, backupFile);
                    }
                }
            }
        }

        /// <summary>Clean up old backups, only keep the pre-defined number of backups locally</summary>
        public void CleanBackup()
        {
            var backupRoot = this.GetBackupRoot();
            if (!Directory.Exists(backupRoot))
            {
                return;
            }

            var subDirs = Directory.GetDirectories(backupRoot, "*", SearchOption.TopDirectoryOnly);
            var versionDirs = new Dictionary<Version, string>();

            foreach (var dir in subDirs)
            {
                var folder = Path.GetFileName(dir);
                if (Regex.IsMatch(folder, @"\d+?\.\d+?\.\d+?\.\d+?"))
                {
                    versionDirs.Add(new Version(folder), dir);
                }
            }

            if (versionDirs.Count > 0)
            {
                var totalRemove = versionDirs.Count - _maxBackups;
                if (totalRemove > 0)
                {
                    // Sort version ascending
                    var sortedDirs = versionDirs.OrderBy(p => p.Key).ToList();
                    for (var i = 0; i < totalRemove; i++)
                    {
                        this.Logger.Info("Remove backup folder [{0}]", Path.GetFileName(sortedDirs[i].Value));
                        IOUtils.DeleteDirectory(sortedDirs[i].Value);
                    }
                }
            }
        }

        /// <summary>Get the location to store the backups of old versions</summary>
        /// <returns>The <see cref="string" />.</returns>
        private string GetBackupRoot()
        {
            return Path.Combine(this._baseAppPath, "Backups");
        }
    }
}