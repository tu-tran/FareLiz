namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using System;
    using System.IO;

    /// <summary>
    /// The update parameter.
    /// </summary>
    public class UpdateParameter
    {
        /// <summary>
        /// Gets or sets the package location.
        /// </summary>
        public string PackageLocation { get; set; }

        /// <summary>
        /// Gets or sets the target update path.
        /// </summary>
        public string TargetUpdatePath { get; set; }

        /// <summary>
        /// Gets or sets the backup path.
        /// </summary>
        public string BackupPath { get; set; }

        /// <summary>
        /// Gets or sets the process to end.
        /// </summary>
        public string ProcessToEnd { get; set; }

        /// <summary>
        /// Gets or sets the restart process.
        /// </summary>
        public string RestartProcess { get; set; }

        /// <summary>
        /// Gets or sets the restart args.
        /// </summary>
        public string RestartArgs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether notify msg.
        /// </summary>
        public bool NotifyMsg { get; set; }

        /// <summary>
        /// Generate parameter String. Pass this string into the Updater Binary so that it will do the actual update
        /// </summary>
        /// <param name="resourceLocation">
        /// The path where to download the resource from
        /// </param>
        /// <param name="targetUpdatePath">
        /// Destination update folder
        /// </param>
        /// <param name="backupPath">
        /// The path for backup
        /// </param>
        /// <param name="processToEnd">
        /// Process to kill before running the update
        /// </param>
        /// <param name="processToRestart">
        /// Process to start after the update is finished
        /// </param>
        /// <param name="restartArg">
        /// Argument for the restarting process
        /// </param>
        /// <param name="notifyMsg">
        /// Does the updater display all messages in message box
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToCommandLine()
        {
            return string.Format(
                "{0} \"{1}\" {2} \"{3}\" {4} \"{5}\" {6} \"{7}\" {8} \"{9}\" {10} \"{11}\" {12} \"{13}\"", 
                "Source", 
                this.PackageLocation, 
                "Target", 
                this.TargetUpdatePath, 
                "Backup", 
                this.BackupPath, 
                "EndProcess", 
                this.ProcessToEnd, 
                "StartProcess", 
                this.RestartProcess, 
                "Argument", 
                this.RestartArgs, 
                "Notify", 
                this.NotifyMsg);
        }

        /// <summary>
        /// Generate new object based on parameter string array
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="UpdateParameter"/>.
        /// </returns>
        public static UpdateParameter FromCommandLine(string[] parameters)
        {
            string resourceLocation = null, 
                   targetUpdatePath = null, 
                   backupPath = null, 
                   processToEnd = null, 
                   processToRestart = null, 
                   argumentForRestartProcess = null;

            bool notifyInMessageBox = true;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == "Source")
                {
                    resourceLocation = parameters[i + 1];
                }

                if (parameters[i] == "Target")
                {
                    targetUpdatePath = parameters[i + 1];
                }

                if (parameters[i] == "Backup")
                {
                    backupPath = parameters[i + 1];
                }

                if (parameters[i] == "EndProcess")
                {
                    processToEnd = parameters[i + 1];
                }

                if (parameters[i] == "StartProcess")
                {
                    processToRestart = parameters[i + 1];
                }

                if (parameters[i] == "Argument")
                {
                    argumentForRestartProcess = parameters[i + 1];
                }

                if (parameters[i] == "Notify")
                {
                    notifyInMessageBox = bool.Parse(parameters[i + 1]);
                }

                i++;
            }

            if (string.IsNullOrEmpty(resourceLocation) || !Directory.Exists(resourceLocation))
            {
                throw new ArgumentException("Invalid resource location: " + resourceLocation);
            }

            if (string.IsNullOrEmpty(targetUpdatePath) || !Directory.Exists(targetUpdatePath))
            {
                throw new ArgumentException("Invalid target update path: " + targetUpdatePath);
            }

            if (string.IsNullOrEmpty(backupPath) || !Directory.Exists(backupPath))
            {
                throw new ArgumentException("Invalid backup path: " + backupPath);
            }

            return new UpdateParameter
                       {
                           PackageLocation = resourceLocation, 
                           TargetUpdatePath = targetUpdatePath, 
                           BackupPath = backupPath, 
                           ProcessToEnd = processToEnd, 
                           RestartProcess = processToRestart, 
                           RestartArgs = argumentForRestartProcess, 
                           NotifyMsg = notifyInMessageBox
                       };
        }
    }
}