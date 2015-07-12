using log4net;

using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Service.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>
    /// Client used for retrieving version updates
    /// </summary>
    public class LiveUpdateClient
    {
        private readonly IVersionRetriever _versionRetriever;
        private readonly string _baseAppPath = ProcessUtils.CurrentProcessDirectory;
        private const int _maxBackups = 2;
        public ILog Logger { get; set; }

        public LiveUpdateClient(IVersionRetriever versionRetriever, ILog logger)
        {
            _versionRetriever = versionRetriever;
            Logger = logger;
        }

        /// <summary>
        /// Check for latest update
        /// </summary>
        /// <returns>The upgradable version. Returns null if there is no newer version</returns>
        public UpdateRequest CheckForUpdate()
        {
            var newRequest = _versionRetriever.CheckForUpdate();
            if (newRequest != null)
            {
                var curProcess = ProcessUtils.CurrentProcessLocation;

                var param = new UpdateParameter
                {
                    BackupPath = Path.Combine(GetBackupRoot(), newRequest.FromVersion.VersionNumber.ToString()),
                    PackageLocation = AppUtil.GetLocalDataPath(GetType().Name + "\\Packages\\" + Guid.NewGuid()),
                    ProcessToEnd = curProcess,
                    RestartProcess = curProcess,
                    NotifyMsg = true,
                    TargetUpdatePath = _baseAppPath
                };
                newRequest.Parameters = param;
            }

            return newRequest;
        }

        /// <summary>
        /// Apply the update using the downloaded resource
        /// </summary>
        public void RunUpdate(UpdateRequest request)
        {
            ProgressDialog.ExecuteTask(null, "Live Update", "Downloading update package into local drive...", "RunLiveUpdate", ProgressBarStyle.Marquee, Logger,
                // Run the actual update
                callback =>
                {
                    callback.Begin();
                    Logger.InfoFormat("Upgrading {0} from {1} to {2}...", request.ProductName, request.FromVersion.VersionNumber, request.ToVersion.VersionNumber);
                    callback.Text = "Downloading data package...";
                    Logger.Info("Download version package for " + request.ToVersion.VersionNumber);
                    _versionRetriever.DownloadPackage(request, request.Parameters.PackageLocation);                    
                    callback.Text = "Backing up current version...";
                    Logger.Info("Backup current version " + request.FromVersion.VersionNumber);
                    Backup(request);
                    callback.Text = "Cleaning up old backups...";
                    Logger.Info("Cleanup old version backups");
                    CleanBackup();  // Clean old backups
                    callback.Text = "Installing update...";
                    ProcessStartInfo runnerProcessInfo = LiveUpdateRunner.GetUpdateRunnerProcessInfo(request.Parameters);   // Prepare update runner process
                    var runnerProcess = Process.Start(runnerProcessInfo);
                    runnerProcess.WaitForExit();
                }, request.Parameters.NotifyMsg);
        }

        /// <summary>
        /// Backup the installation
        /// </summary>
        public void Backup(UpdateRequest request)
        {
            var backupPath = request.Parameters.BackupPath;
            if (Directory.Exists(backupPath))
                IOUtils.DeleteDirectory(backupPath);

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

        /// <summary>
        /// Clean up old backups, only keep the pre-defined number of backups locally
        /// </summary>
        public void CleanBackup()
        {
            var backupRoot = GetBackupRoot();
            if (!Directory.Exists(backupRoot))
                return;

            var subDirs = Directory.GetDirectories(backupRoot, "*", SearchOption.TopDirectoryOnly);
            var versionDirs = new Dictionary<Version, string>();

            foreach (var dir in subDirs)
            {
                var folder = Path.GetFileName(dir);
                if (Regex.IsMatch(folder, @"\d+?\.\d+?\.\d+?\.\d+?"))
                    versionDirs.Add(new Version(folder), dir);
            }

            if (versionDirs.Count > 0)
            {
                var totalRemove = (versionDirs.Count - _maxBackups);
                if (totalRemove > 0)
                {
                    // Sort version ascending
                    var sortedDirs = versionDirs.OrderBy(p => p.Key).ToList();
                    for (int i = 0; i < totalRemove; i++)
                    {
                        Logger.InfoFormat("Remove backup folder [{0}]", Path.GetFileName(sortedDirs[i].Value));
                        IOUtils.DeleteDirectory(sortedDirs[i].Value);
                    }
                }
            }
        }


        /// <summary>
        /// Get the location to store the backups of old versions
        /// </summary>
        private string GetBackupRoot()
        {
            return Path.Combine(_baseAppPath, "Backups");
        }
    }
}
