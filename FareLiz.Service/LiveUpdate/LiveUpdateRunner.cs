using log4net;

using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Service.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>
    /// This class plays the role of a third party application: It will update another application and restart it later on.
    /// In order to make use of this class, create an application that reference to this class
    /// </summary>
    public class LiveUpdateRunner
    {
        private readonly UpdateParameter _param;
        private readonly ILog _logger;
        private static readonly string _runnerAsmName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);

        public LiveUpdateRunner(UpdateParameter parameters, ILog logger)
        {
            _param = parameters;
            _logger = logger;
        }


        /// <summary>
        /// Start the real work: Kill process, Backup, Update and Restart the required process
        /// </summary>
        public void DoUpdate()
        {
            _logger.Info("Applying updates...");
            ProgressDialog.ExecuteTask(null, "Applying Updates", "Please wait...", "ApplyUpdate", ProgressBarStyle.Marquee, _logger, callback =>
                        {
                            callback.Begin();

                            // First, kill the running process
                            _logger.DebugFormat("Killing process [{0}]...", _param.ProcessToEnd);
                            callback.Text = "Closing active application...";
                            ProcessUtils.KillProcess(_param.ProcessToEnd, 3, 3);

                            callback.Text = "Applying updates...";
                            IOUtils.CopyFolder(_param.PackageLocation, _param.TargetUpdatePath, true, false);
                            IOUtils.DeleteDirectory(_param.PackageLocation);    // Clean package folder
                        },
                // Handle exception delegate
                        (callback, ex) =>
                        {
                            HandleException(ex);
                            string message = "Update failed! Restoring application backup...";
                            _logger.Error(message);
                            callback.Text = message;
                            IOUtils.CopyFolder(_param.BackupPath, _param.TargetUpdatePath, true, false);
                        },
                // Final delegate
                        (callback, ex) =>
                        {
                            _logger.Info("Live Update ended. Running post-process...");
                            StartPostProcess();
                        });
        }

        private void StartPostProcess()
        {
            if (String.IsNullOrEmpty(_param.RestartProcess))
                return;

            _logger.Info("Starting post-process: " + _param.RestartProcess);
            var newProcess = Process.Start(_param.RestartProcess, _param.RestartArgs);
            newProcess.WaitForInputIdle(3);
        }

        private void HandleException(Exception ex)
        {
            string message = "An error occured: " + ex.Message + Environment.NewLine + ex.StackTrace;
            _logger.Error(message);
            if (_param.NotifyMsg)
                MessageBox.Show(message, "Live Update", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static ProcessStartInfo GetUpdateRunnerProcessInfo(UpdateParameter parameters)
        {
            var runnerPath = Path.Combine(parameters.BackupPath, _runnerAsmName);
            var cmdLine = parameters.ToCommandLine();
            var startInfo = new ProcessStartInfo(runnerPath, typeof(LiveUpdateService) + " " + cmdLine) { UseShellExecute = false };    // Signal that we want to use LiveUpdateService
            return startInfo;
        }
    }
}

