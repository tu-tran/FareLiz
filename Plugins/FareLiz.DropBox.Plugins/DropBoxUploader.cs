namespace SkyDean.FareLiz.DropBox
{
    using System;
    using System.IO;

    using DropNet;

    using Ionic.Zip;
    using Ionic.Zlib;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>The drop box uploader.</summary>
    internal class DropBoxUploader
    {
        /// <summary>The callback.</summary>
        private readonly IProgressCallback callback;

        /// <summary>The client.</summary>
        private readonly IDropNetClient client;

        /// <summary>The delete exist.</summary>
        private readonly bool deleteExist;

        /// <summary>The local file.</summary>
        private readonly string localFile;

        /// <summary>The logger.</summary>
        private readonly ILogger logger;

        /// <summary>The remote file.</summary>
        private readonly string remoteFile;

        /// <summary>The remote path.</summary>
        private readonly string remotePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropBoxUploader"/> class.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="localFile">
        /// The local file.
        /// </param>
        /// <param name="remoteFile">
        /// The remote file.
        /// </param>
        /// <param name="remotePath">
        /// The remote path.
        /// </param>
        /// <param name="deleteExist">
        /// The delete exist.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal DropBoxUploader(IDropNetClient client, 
                                 string localFile, 
                                 string remoteFile, 
                                 string remotePath, 
                                 bool deleteExist, 
                                 IProgressCallback callback, 
                                 ILogger logger)
        {
            this.client = client;
            this.localFile = localFile;
            this.remoteFile = remoteFile;
            this.remotePath = remotePath;
            this.deleteExist = deleteExist;
            this.callback = callback;
            this.logger = logger;
        }

        /// <summary>The execute.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        /// <exception cref="ApplicationException"></exception>
        internal bool Execute()
        {
            this.logger.Info("Upload data to DropBox");
            if (!File.Exists(this.localFile))
            {
                throw new ApplicationException("There is no local data available");
            }

            if (this.callback.Confirm(
                this.callback, 
                string.Format(
                    "The current data size is {0}. Do you want to proceed ? (Data will be compressed)", 
                    StringUtil.FormatSize(new FileInfo(this.localFile).Length)), 
                "Backup database to DropBox") != ConfirmationType.Yes)
            {
                return false;
            }

            this.callback.Title = "Database Compression";
            var message = "Processing offline data for uploading...";
            this.logger.Info(message);
            this.callback.Text = message;
            var tempDataFile = this.localFile + DateTime.Now.ToString(".yyyyMMddHHmmss");
            this.logger.Info("Compress [{0}] into [{1}]", this.localFile, tempDataFile);
            this.callback.Style = ProgressStyle.Continuous;
            this.callback.SetRange(0, 100);
            using (var zip = new ZipFile(tempDataFile))
            {
                zip.CompressionLevel = CompressionLevel.BestCompression;
                zip.CompressionMethod = CompressionMethod.BZip2;
                zip.AddFile(this.localFile);
                zip.SaveProgress += (o, e) =>
                    {
                        if (e.TotalBytesToTransfer > 0)
                        {
                            var progress = 100 * e.BytesTransferred / e.TotalBytesToTransfer;
                            this.callback.StepTo((int)progress);
                        }
                    };
                zip.Save();
            }

            var dropBoxDbFilePath = this.remotePath + "/" + this.remoteFile;
            message = string.Format("Sending data ({0})...", StringUtil.FormatSize(new FileInfo(tempDataFile).Length));
            this.logger.Info(message);
            this.callback.Title = "Backup Database to DropBox";
            this.callback.Text = message;
            this.callback.Style = ProgressStyle.Marquee;
            this.client.UploadFile(dropBoxDbFilePath, tempDataFile, File.ReadAllBytes(tempDataFile));
            this.logger.Info("Delete temporary file: " + tempDataFile);
            File.Delete(tempDataFile);

            if (this.deleteExist)
            {
                this.logger.Info("Delete DropBox online file: " + dropBoxDbFilePath);
                this.client.Delete(dropBoxDbFilePath);
            }

            var dropBoxTempFile = string.Format("{0}/{1}", this.remotePath, tempDataFile);
            this.logger.Info("Move DropBox online file: [{0}] -> [{1}]", dropBoxTempFile, dropBoxDbFilePath);
            this.client.Move(dropBoxTempFile, dropBoxDbFilePath);
            return true;
        }
    }
}