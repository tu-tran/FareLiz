namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.IO;

    /// <summary>The io utils.</summary>
    public static class IOUtils
    {
        /// <summary>
        /// Recursively delete a folder and all of its contents
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            // next, delete all files in the directory
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            Directory.Delete(path);
        }

        /// <summary>
        /// Copy content of a folder to another folder (do NOT include subfolders)
        /// </summary>
        /// <param name="sourceFolder">
        /// The source Folder.
        /// </param>
        /// <param name="destFolder">
        /// The dest Folder.
        /// </param>
        /// <param name="overwrite">
        /// The overwrite.
        /// </param>
        /// <param name="recursively">
        /// The recursively.
        /// </param>
        public static void CopyFolder(string sourceFolder, string destFolder, bool overwrite, bool recursively)
        {
            if (string.Compare(sourceFolder, destFolder, true) == 0)
            {
                return;
            }

            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            var files = Directory.GetFiles(sourceFolder);
            foreach (var file in files)
            {
                var name = Path.GetFileName(file);
                var dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, overwrite);
            }

            if (recursively)
            {
                var folders = Directory.GetDirectories(sourceFolder);
                foreach (var folder in folders)
                {
                    var name = Path.GetFileName(folder);
                    var dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest, overwrite, recursively);
                }
            }
        }

        /// <summary>
        /// Move file with overwrite option. (Or skip existing files)
        /// </summary>
        /// <param name="Source">
        /// The Source.
        /// </param>
        /// <param name="Destination">
        /// The Destination.
        /// </param>
        /// <param name="Overwrite">
        /// The Overwrite.
        /// </param>
        public static void MoveFile(string Source, string Destination, bool Overwrite)
        {
            if (Source == Destination)
            {
                throw new ArgumentException(string.Format("Source and destination are the same. Cannot move file [{0}].", Source));
            }

            if (File.Exists(Source))
            {
                if (IsFileReadOnly(Source))
                {
                    File.SetAttributes(Source, FileAttributes.Normal); // Remove Read-only tag
                }

                if (File.Exists(Destination))
                {
                    if (Overwrite)
                    {
                        if (IsFileReadOnly(Destination))
                        {
                            File.SetAttributes(Destination, FileAttributes.Normal); // Remove Read-only tag
                        }

                        File.Copy(Source, Destination, true);
                    }

                    File.Delete(Source);
                }
                else
                {
                    File.Move(Source, Destination);
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Source file does not exist. Cannot move file [{0}] to [{1}].", Source, Destination));
            }
        }

        /// <summary>
        /// Check if the current processing file is read-only
        /// </summary>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsFileReadOnly(string filePath)
        {
            return (new FileInfo(filePath)).IsReadOnly;
        }

        /// <summary>
        /// Check if the current file is being locked
        /// </summary>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsFileLocked(string filePath)
        {
            var fi = new FileInfo(filePath);
            if (fi.Exists)
            {
                try
                {
                    using (var stream = fi.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                    }
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