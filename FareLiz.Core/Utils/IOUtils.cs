using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SkyDean.FareLiz.Core.Utils
{
    public static class IOUtils
    {
        /// <summary>
        /// Recursively delete a folder and all of its contents
        /// </summary>
        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            string[] dirs = Directory.GetDirectories(path);
            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            // next, delete all files in the directory
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            Directory.Delete(path);
        }

        /// <summary>
        /// Copy content of a folder to another folder (do NOT include subfolders)
        /// </summary>
        public static void CopyFolder(string sourceFolder, string destFolder, bool overwrite, bool recursively)
        {
            if (String.Compare(sourceFolder, destFolder, true) == 0)
                return;

            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, overwrite);
            }

            if (recursively)
            {
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest, overwrite, recursively);
                }
            }
        }

        /// <summary>
        /// Move file with overwrite option. (Or skip existing files)
        /// </summary>
        public static void MoveFile(string Source, string Destination, bool Overwrite)
        {
            if (Source == Destination)
                throw new ArgumentException(String.Format("Source and destination are the same. Cannot move file [{0}].", Source));

            if (File.Exists(Source))
            {
                if (IsFileReadOnly(Source))
                    File.SetAttributes(Source, FileAttributes.Normal); // Remove Read-only tag

                if (File.Exists(Destination))
                {
                    if (Overwrite)
                    {
                        if (IsFileReadOnly(Destination))
                            File.SetAttributes(Destination, FileAttributes.Normal); // Remove Read-only tag
                        File.Copy(Source, Destination, true);
                    }

                    File.Delete(Source);
                }
                else
                    File.Move(Source, Destination);
            }
            else
                throw new ArgumentException(String.Format("Source file does not exist. Cannot move file [{0}] to [{1}].", Source, Destination));
        }

        /// <summary>
        /// Check if the current processing file is read-only
        /// </summary>
        public static bool IsFileReadOnly(string filePath)
        {
            return (new FileInfo(filePath)).IsReadOnly;
        }

        /// <summary>
        /// Check if the current file is being locked
        /// </summary>
        public static bool IsFileLocked(string filePath)
        {
            var fi = new FileInfo(filePath);
            if (fi.Exists)
            {
                try
                {
                    using (var stream = fi.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    { }
                }
                catch (IOException)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
