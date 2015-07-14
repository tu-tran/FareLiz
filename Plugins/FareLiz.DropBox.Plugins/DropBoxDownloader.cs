namespace SkyDean.FareLiz.DropBox
{
    using System;
    using System.IO;

    using DropNet;
    using DropNet.Models;

    using Ionic.Zip;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>The drop box downloader.</summary>
    internal class DropBoxDownloader
    {
        /// <summary>The callback.</summary>
        private readonly IProgressCallback callback;

        /// <summary>The client.</summary>
        private readonly IDropNetClient client;

        /// <summary>The data file path.</summary>
        private readonly string dataFilePath;

        /// <summary>The file meta.</summary>
        private readonly MetaData fileMeta;

        /// <summary>The logger.</summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropBoxDownloader"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="fileMetaData">
        /// The file meta data.
        /// </param>
        /// <param name="targetFile">
        /// The target file.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal DropBoxDownloader(IDropNetClient client, MetaData fileMetaData, string targetFile, IProgressCallback callback, ILogger logger)
        {
            this.client = client;
            this.fileMeta = fileMetaData;
            this.dataFilePath = targetFile;
            this.callback = callback;
            this.logger = logger;
        }

        /// <summary>The execute.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        /// <exception cref="ApplicationException"></exception>
        internal bool Execute()
        {
            this.logger.Info("Download data from DropBox");
            if (this.fileMeta == null)
            {
                throw new ApplicationException("There is no data available from DropBox!");
            }

            var confirm = this.callback.Confirm(
                this.callback, 
                string.Format(
                    "The last data was updated on {0} ({1}). Do you want to download and install this database?", 
                    this.fileMeta.ModifiedDate, 
                    this.fileMeta.Size), 
                "Download Confirmation");

            if (confirm != ConfirmationType.Yes)
            {
                return false;
            }

            this.callback.Title = "Restore Database from DropBox";
            var message = string.Format("Downloading data from DropBox ({0})...", this.fileMeta.Size);
            this.logger.Info(message);
            this.callback.Text = message;
            this.callback.Style = ProgressStyle.Blocks;
            this.callback.SetRange(0, 100);

            var backupFile = string.Format("{0}.bak.{1}", this.dataFilePath, DateTime.Now.ToString("yyyyMMddHHmmss"));
            try
            {
                if (File.Exists(this.dataFilePath))
                {
                    this.logger.Info("Backup existing data file: [{0}] to [{1}]", this.dataFilePath, backupFile);
                    if (File.Exists(backupFile))
                    {
                        File.Delete(backupFile);
                    }

                    File.Move(this.dataFilePath, backupFile);
                }
                else
                {
                    this.logger.Info("Data file [{0}] does not exist... Skip backup", this.dataFilePath);
                }

                using (var fs = File.OpenWrite(this.fileMeta.Name))
                {
                    var chunkSize = (this.fileMeta.Bytes > 5 * 1048576 && this.fileMeta.Bytes < 20 * 1048576)
                                        ? this.fileMeta.Bytes / 100 // Download in 10 chunks
                                        : 5 * 1048576; // 5MB

                    if (chunkSize > this.fileMeta.Bytes)
                    {
                        chunkSize = this.fileMeta.Bytes;
                    }

                    long downloaded = 0;
                    while (downloaded < this.fileMeta.Bytes)
                    {
                        var chunks = this.client.GetFile(this.fileMeta.Path, downloaded, downloaded + chunkSize, this.fileMeta.Rev);
                        fs.Write(chunks, 0, chunks.Length);
                        downloaded += chunks.Length;
                        var progress = 100 * ((double)downloaded / this.fileMeta.Bytes);
                        this.callback.StepTo((int)progress);
                    }
                }

                message = "Processing received data...";
                this.logger.Info(message);
                this.callback.Text = message;
                this.logger.Debug("Decompressing received file: " + this.fileMeta.Name);
                using (var zip = ZipFile.Read(this.fileMeta.Name))
                {
                    zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                    zip.ExtractProgress += (o, e) =>
                        {
                            if (e.TotalBytesToTransfer > 0)
                            {
                                var progress = 100 * e.BytesTransferred / e.TotalBytesToTransfer;
                                this.callback.StepTo((int)progress);
                            }
                        };

                    this.callback.Style = ProgressStyle.Continuous;
                    this.callback.SetRange(0, 100);

                    foreach (var entry in zip)
                    {
                        entry.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                        entry.Extract(ExtractExistingFileAction.OverwriteSilently);
                    }
                }

                this.logger.Info("Delete temporary compressed file: " + this.fileMeta.Name);
                this.callback.Text = "Post-processing data...";
                this.callback.Style = ProgressStyle.Marquee;
                File.Delete(this.fileMeta.Name);

                this.logger.Info("Delete backup file: " + backupFile);
                File.Delete(backupFile);
            }
            catch (Exception ex)
            {
                var actualErr = DropBoxExceptionHandler.Log(ex, this.logger);
                if (!this.callback.IsAborting)
                {
                    this.callback.Inform(
                        this.callback, 
                        "Failed to download database. The previous database will now be restored. The error message was:" + Environment.NewLine
                        + actualErr, 
                        "DropBox Database Download", 
                        NotificationType.Error);
                }

                // Restore the backup and then rethrow the exception to the outer loop
                if (File.Exists(backupFile))
                {
                    if (File.Exists(this.dataFilePath))
                    {
                        File.Delete(this.dataFilePath);
                    }

                    this.logger.Info("Restore backup file [{0}] to [{1}]", backupFile, this.dataFilePath);
                    File.Move(backupFile, this.dataFilePath);
                }

                return false;
            }

            return true;
        }
    }
}