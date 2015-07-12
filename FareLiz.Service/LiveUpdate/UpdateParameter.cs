using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SkyDean.FareLiz.Service.LiveUpdate
{
    public class UpdateParameter
    {
        public string PackageLocation { get; set; }
        public string TargetUpdatePath { get; set; }
        public string BackupPath { get; set; }
        public string ProcessToEnd { get; set; }
        public string RestartProcess { get; set; }
        public string RestartArgs { get; set; }
        public bool NotifyMsg { get; set; }

        /// <summary>
        /// Generate parameter String. Pass this string into the Updater Binary so that it will do the actual update
        /// </summary>
        /// <param name="resourceLocation">The path where to download the resource from</param>
        /// <param name="targetUpdatePath">Destination update folder</param>
        /// <param name="backupPath">The path for backup</param>
        /// <param name="processToEnd">Process to kill before running the update</param>
        /// <param name="processToRestart">Process to start after the update is finished</param>
        /// <param name="restartArg">Argument for the restarting process</param>
        /// <param name="notifyMsg">Does the updater display all messages in message box</param>
        /// <returns></returns>
        public string ToCommandLine()
        {
            return String.Format("{0} \"{1}\" {2} \"{3}\" {4} \"{5}\" {6} \"{7}\" {8} \"{9}\" {10} \"{11}\" {12} \"{13}\"",
                "Source", PackageLocation,
                "Target", TargetUpdatePath,
                "Backup", BackupPath,
                "EndProcess", ProcessToEnd,
                "StartProcess", RestartProcess,
                "Argument", RestartArgs,
                "Notify", NotifyMsg);
        }

        /// <summary>
        /// Generate new object based on parameter string array
        /// </summary>
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
                if (parameters[i] == "Source") resourceLocation = parameters[i + 1];
                if (parameters[i] == "Target") targetUpdatePath = parameters[i + 1];
                if (parameters[i] == "Backup") backupPath = parameters[i + 1];
                if (parameters[i] == "EndProcess") processToEnd = parameters[i + 1];
                if (parameters[i] == "StartProcess") processToRestart = parameters[i + 1];
                if (parameters[i] == "Argument") argumentForRestartProcess = parameters[i + 1];
                if (parameters[i] == "Notify") notifyInMessageBox = Boolean.Parse(parameters[i + 1]);
                i++;
            }

            if (String.IsNullOrEmpty(resourceLocation) || !Directory.Exists(resourceLocation))
                throw new ArgumentException("Invalid resource location: " + resourceLocation);

            if (String.IsNullOrEmpty(targetUpdatePath) || !Directory.Exists(targetUpdatePath))
                throw new ArgumentException("Invalid target update path: " + targetUpdatePath);

            if (String.IsNullOrEmpty(backupPath) || !Directory.Exists(backupPath))
                throw new ArgumentException("Invalid backup path: " + backupPath);

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
