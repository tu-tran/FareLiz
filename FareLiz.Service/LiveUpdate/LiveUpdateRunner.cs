namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Service.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// This class plays the role of a third party application: It will update another application and restart it later on. In order to make use of this
    /// class, create an application that reference to this class
    /// </summary>
    public class LiveUpdateRunner
    {
        /// <summary>
        /// The _runner asm name.
        /// </summary>
        private static readonly string _runnerAsmName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The _param.
        /// </summary>
        private readonly UpdateParameter _param;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveUpdateRunner"/> class.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public LiveUpdateRunner(UpdateParameter parameters, ILogger logger)
        {
            this._param = parameters;
            this._logger = logger;
        }

        /// <summary>Start the real work: Kill process, Backup, Update and Restart the required process</summary>
        public void DoUpdate()
        {
            this._logger.Info("Applying updates...");
            ProgressDialog.ExecuteTask(
                null,
                "Applying Updates",
                "Please wait...",
                "ApplyUpdate",
                ProgressBarStyle.Marquee,
                this._logger,
                callback =>
                {
                    callback.Begin();

                    // First, kill the running process
                    this._logger.DebugFormat("Killing process [{0}]...", this._param.ProcessToEnd);
                    callback.Text = "Closing active application...";
                    ProcessUtils.KillProcess(this._param.ProcessToEnd, 3, 3);

                    callback.Text = "Applying updates...";
                    IOUtils.CopyFolder(this._param.PackageLocation, this._param.TargetUpdatePath, true, false);
                    IOUtils.DeleteDirectory(this._param.PackageLocation); // Clean package folder
                },

                // Handle exception delegate
                (callback, ex) =>
                {
                    this.HandleException(ex);
                    string message = "Update failed! Restoring application backup...";
                    this._logger.Error(message);
                    callback.Text = message;
                    IOUtils.CopyFolder(this._param.BackupPath, this._param.TargetUpdatePath, true, false);
                },

                // Final delegate
                (callback, ex) =>
                {
                    this._logger.Info("Live Update ended. Running post-process...");
                    this.StartPostProcess();
                });
        }

        /// <summary>
        /// The start post process.
        /// </summary>
        private void StartPostProcess()
        {
            if (string.IsNullOrEmpty(this._param.RestartProcess))
            {
                return;
            }

            this._logger.Info("Starting post-process: " + this._param.RestartProcess);
            var newProcess = Process.Start(this._param.RestartProcess, this._param.RestartArgs);
            newProcess.WaitForInputIdle(3);
        }

        /// <summary>
        /// The handle exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        private void HandleException(Exception ex)
        {
            string message = "An error occured: " + ex.Message + Environment.NewLine + ex.StackTrace;
            this._logger.Error(message);
            if (this._param.NotifyMsg)
            {
                MessageBox.Show(message, "Live Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// The get update runner process info.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="ProcessStartInfo"/>.
        /// </returns>
        public static ProcessStartInfo GetUpdateRunnerProcessInfo(UpdateParameter parameters)
        {
            var runnerPath = Path.Combine(parameters.BackupPath, _runnerAsmName);
            var cmdLine = parameters.ToCommandLine();
            var startInfo = new ProcessStartInfo(runnerPath, typeof(LiveUpdateService) + " " + cmdLine) { UseShellExecute = false };

            // Signal that we want to use LiveUpdateService
            return startInfo;
        }
    }
}