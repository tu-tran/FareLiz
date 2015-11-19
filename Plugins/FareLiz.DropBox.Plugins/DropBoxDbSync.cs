namespace SkyDean.FareLiz.DropBox
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using DropNet;
    using DropNet.Models;

    using Ionic.Zip;
    using Ionic.Zlib;

    using log4net;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>
    /// Abstract class used for synchronizing object with DropBox
    /// </summary>
    /// <typeparam name="T">
    /// The type of the target fare database.
    /// </typeparam>
    public abstract class DropBoxDbSync<T> : IDatabaseSyncer<T>
        where T : IFareDatabase, ISyncable
    {
        /// <summary>
        /// The pk g_ ext.
        /// </summary>
        private const string PKG_EXT = ".dbpkg";

        /// <summary>
        /// The pk g_ dateformat.
        /// </summary>
        private const string PKG_DATEFORMAT = "yyyyMMddHHmmss";

        /// <summary>
        /// The pk g_ separator.
        /// </summary>
        private const char PKG_SEPARATOR = '_';

        /// <summary>
        /// The _config.
        /// </summary>
        private DropBoxSyncerConfig _config = new DropBoxSyncerConfig();

        /// <summary>
        /// The _formatter.
        /// </summary>
        private StringFormatter _formatter = new StringFormatter(DropBoxSyncConfigBuilder.Seed);

        /// <summary>
        /// Gets the sync target.
        /// </summary>
        public T SyncTarget
        {
            get
            {
                return (T)this.SyncTargetObject;
            }
        }

        /// <summary>
        /// Gets the drop box path.
        /// </summary>
        public string DropBoxPath
        {
            get
            {
                return this._config.DropBoxBaseFolder;
            }
        }

        /// <summary>
        /// The on validate data.
        /// </summary>
        public event SyncEventHandler<T> OnValidateData;

        /// <summary>
        /// Gets or sets the sync target object.
        /// </summary>
        public ISyncable SyncTargetObject { get; set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public IConfig Configuration
        {
            get
            {
                return this._config;
            }

            set
            {
                this._config = value as DropBoxSyncerConfig;
            }
        }

        /// <summary>
        /// Gets the default config.
        /// </summary>
        public IConfig DefaultConfig
        {
            get
            {
                return new DropBoxSyncerConfig();
            }
        }

        /// <summary>
        /// Gets the custom config builder.
        /// </summary>
        public IConfigBuilder CustomConfigBuilder
        {
            get
            {
                return new DropBoxSyncConfigBuilder(this.Logger);
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        public virtual void Initialize()
        {
            if (this.Configuration == null)
            {
                this.Configuration = this.DefaultConfig;
            }
        }

        /// <summary>
        /// The synchronize.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="ApplicationException">
        /// </exception>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool Synchronize(SyncOperation operation, object data, IProgressCallback callback)
        {
            if (data == null)
            {
                throw new ArgumentException("Synchronize data cannot be null");
            }

            this.Logger.InfoFormat("Synchronizing data [{0}]: {1}", operation, data);

            bool success = false;
            try
            {
                callback.Begin();
                callback.Text = "Retrieving DropBox metadata...";
                string dataFilePath = data.ToString();
                string dropBoxDbFileName = Path.GetFileName(data.ToString()) + ".compressed";
                string dropBoxDbFilePath = string.Format("{0}/{1}", this.DropBoxPath, dropBoxDbFileName);
                DropNetClient client = this.GetClient();
                MetaData baseMeta = this.GetOrCreateBaseMetaData(client), fileMeta = null;

                if (baseMeta.Contents != null)
                {
                    foreach (MetaData m in baseMeta.Contents)
                    {
                        if (string.Compare(m.Name, dropBoxDbFileName, StringComparison.OrdinalIgnoreCase) == 0 && !m.Is_Dir)
                        {
                            // File already exists
                            fileMeta = m;
                            break;
                        }
                    }
                }

                switch (operation)
                {
                    case SyncOperation.Download: // Download and Decompress from DropBox
                        if (fileMeta == null)
                        {
                            throw new ApplicationException("There is no data available from DropBox!");
                        }

                        ConfirmationType confirm = callback.Confirm(
                            callback, 
                            string.Format(
                                "The last data was updated on {0} ({1}). Do you want to download and install this database?", 
                                fileMeta.ModifiedDate, 
                                fileMeta.Size), 
                            "Download Confirmation");

                        if (confirm != ConfirmationType.Yes)
                        {
                            return false;
                        }

                        callback.Title = "Restore Database from DropBox";
                        string message = string.Format("Downloading data from DropBox ({0})...", fileMeta.Size);
                        this.Logger.Info(message);
                        callback.Text = message;

                        string backupFile = string.Format("{0}.bak.{1}", dataFilePath, DateTime.Now.ToString("yyyyMMddHHmmss"));
                        try
                        {
                            byte[] dataBytes = client.GetFile(dropBoxDbFilePath);
                            File.WriteAllBytes(dropBoxDbFileName, dataBytes);
                            if (File.Exists(dataFilePath))
                            {
                                this.Logger.InfoFormat("Backup existing data file: [{0}] to [{1}]", dataFilePath, backupFile);
                                File.Move(dataFilePath, backupFile);
                            }
                            else
                            {
                                this.Logger.InfoFormat("Data file [{0}] does not exist... Skip backup", dataFilePath);
                            }

                            message = "Processing received data...";
                            this.Logger.Info(message);
                            callback.Text = message;
                            this.Logger.Debug("Decompressing received file: " + dropBoxDbFileName);
                            using (var zip = ZipFile.Read(dropBoxDbFileName))
                            {
                                zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                                zip.ExtractProgress += (o, e) =>
                                    {
                                        if (e.TotalBytesToTransfer > 0)
                                        {
                                            callback.StepTo((int)(100 * e.BytesTransferred / e.TotalBytesToTransfer));
                                        }
                                    };

                                callback.Style = ProgressStyle.Continuous;
                                callback.SetRange(0, 100);

                                foreach (var entry in zip)
                                {
                                    entry.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                                    entry.Extract(ExtractExistingFileAction.OverwriteSilently);
                                }
                            }

                            this.Logger.Info("Delete temporary compressed file: " + dropBoxDbFileName);
                            callback.Text = "Post-processing data...";
                            callback.Style = ProgressStyle.Marquee;
                            File.Delete(dropBoxDbFileName);

                            if (this.OnValidateData != null)
                            {
                                this.OnValidateData(this.SyncTarget, new SyncEventArgs<T>(this, data));
                            }

                            this.Logger.Info("Delete backup file: " + backupFile);
                            File.Delete(backupFile);
                        }
                        catch (Exception ex)
                        {
                            string actualErr = this.LogException(ex);
                            if (!callback.IsAborting)
                            {
                                callback.Inform(
                                    callback, 
                                    "Failed to download database. The previous database will now be restored. The error message was:"
                                    + Environment.NewLine + actualErr, 
                                    "DropBox Database Download", 
                                    NotificationType.Error);
                            }

                            // Restore the backup and then rethrow the exception to the outer loop
                            if (File.Exists(backupFile))
                            {
                                if (File.Exists(dataFilePath))
                                {
                                    File.Delete(dataFilePath);
                                }

                                this.Logger.InfoFormat("Restore backup file [{0}] to [{1}]", backupFile, dataFilePath);
                                File.Move(backupFile, dataFilePath);
                            }
                        }

                        break;

                    case SyncOperation.Upload: // Compress and Upload to DropBox
                        this.Logger.Info("Upload data to DropBox");
                        if (!File.Exists(dataFilePath))
                        {
                            throw new ApplicationException("There is no local data available");
                        }

                        if (callback.Confirm(
                            callback, 
                            string.Format(
                                "The current data size is {0}. Do you want to proceed ? (Data will be compressed)", 
                                StringUtil.FormatSize(new FileInfo(dataFilePath).Length)), 
                            "Backup database to DropBox") != ConfirmationType.Yes)
                        {
                            return false;
                        }

                        callback.Title = "Database Compression";
                        message = "Processing offline data for uploading...";
                        this.Logger.Info(message);
                        callback.Text = message;
                        string tempDataFile = dropBoxDbFileName + DateTime.Now.ToString(".yyyyMMddHHmmss");
                        this.Logger.InfoFormat("Compress [{0}] into [{1}]", dataFilePath, tempDataFile);
                        callback.Style = ProgressStyle.Continuous;
                        callback.SetRange(0, 100);
                        using (var zip = new ZipFile(tempDataFile))
                        {
                            zip.CompressionLevel = CompressionLevel.BestCompression;
                            zip.CompressionMethod = CompressionMethod.BZip2;
                            zip.AddFile(dataFilePath);
                            zip.SaveProgress += (o, e) =>
                                {
                                    if (e.TotalBytesToTransfer > 0)
                                    {
                                        callback.StepTo((int)(100 * e.BytesTransferred / e.TotalBytesToTransfer));
                                    }
                                };
                            zip.Save();
                        }

                        message = string.Format("Sending data ({0})...", StringUtil.FormatSize(new FileInfo(tempDataFile).Length));
                        this.Logger.Info(message);
                        callback.Title = "Backup Database to DropBox";
                        callback.Text = message;
                        callback.Style = ProgressStyle.Marquee;
                        client.UploadFile(this.DropBoxPath, tempDataFile, File.ReadAllBytes(tempDataFile));
                        this.Logger.Info("Delete temporary file: " + tempDataFile);
                        File.Delete(tempDataFile);

                        if (fileMeta != null)
                        {
                            this.Logger.Info("Delete DropBox online file: " + dropBoxDbFilePath);
                            client.Delete(dropBoxDbFilePath);
                        }

                        string dropBoxTempFile = string.Format("{0}/{1}", this.DropBoxPath, tempDataFile);
                        this.Logger.InfoFormat("Move DropBox online file: [{0}] -> [{1}]", dropBoxTempFile, dropBoxDbFilePath);
                        client.Move(dropBoxTempFile, dropBoxDbFilePath);
                        break;

                    default:
                        throw new NotImplementedException("This operation is not implemented!");
                }

                this.Logger.InfoFormat("DropBox {0} completed", operation);
                success = true;
            }
            catch (Exception ex)
            {
                var message = this.LogException(ex);
                callback.Inform(callback, message, "DropBox Database Synchronizing", NotificationType.Error);
            }

            this.Logger.InfoFormat("Synchronization [{0}] ended", operation);
            return success;
        }

        /// <summary>
        /// The receive.
        /// </summary>
        /// <param name="importedPackages">
        /// The imported packages.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<DataPackage<TravelRoute>> Receive(IList<string> importedPackages, IProgressCallback callback)
        {
            IList<DataPackage<TravelRoute>> result = null;
            this.Logger.Info("Receiving packages from DropBox");
            ProgressDialog.ExecuteTask(
                null, 
                "DropBox Synchronization", 
                "Retrieving data packages from configured DropBox account...", 
                this.GetType().Name + "-Receive", 
                ProgressBarStyle.Marquee, 
                this.Logger, 
                cb => { result = this.DoReceive(cb as ProgressDialog, importedPackages); });

            return result;
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void Send(DataPackage<TravelRoute> data, IProgressCallback callback)
        {
            this.Logger.InfoFormat("Send data to DropBox");
            var client = this.GetClient();
            MetaData baseData = this.GetOrCreateBaseMetaData(client);
            if (baseData == null)
            {
                this.Logger.Warn("There is no data to be sent");
            }
            else
            {
                var formatter = new ProtoBufTransfer(this.Logger);
                byte[] rawData = formatter.ToRaw(data);
                if (rawData != null)
                {
                    byte[] compressedRaw = this.Compress(rawData, data.Id);
                    if (compressedRaw != null)
                    {
                        this.Logger.InfoFormat("Upload package [{0}] to DropBox", data.Id);
                        string newFile = data.CreatedDate.ToString(PKG_DATEFORMAT) + PKG_SEPARATOR + data.Id + PKG_EXT;
                        client.UploadFile(baseData.Path, newFile, compressedRaw);
                        this.Logger.InfoFormat("Package [{0}] was uploaded ({1})", data.Id, StringUtil.FormatSize(compressedRaw.LongLength));
                    }
                }
            }
        }

        /// <summary>
        /// The do receive.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="importedPackages">
        /// The imported packages.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        private IList<DataPackage<TravelRoute>> DoReceive(ProgressDialog callback, IList<string> importedPackages)
        {
            this.Logger.Info("Receive DropBox packages");
            IList<DataPackage<TravelRoute>> result = null;
            callback.Begin();
            callback.Text = "Retrieving DropBox metadata...";
            var client = this.GetClient();
            MetaData baseData = this.GetOrCreateBaseMetaData(client);
            if (baseData != null && baseData.Contents != null)
            {
                callback.Text = "Processing DropBox metadata...";
                var pkgIds = new List<string>();
                foreach (MetaData m in baseData.Contents)
                {
                    if (!m.Is_Dir && m.Extension == PKG_EXT)
                    {
                        pkgIds.Add(m.Path);
                    }
                }

                // Remove imported packages from the list
                if (importedPackages != null && importedPackages.Count > 0)
                {
                    for (int i = 0; i < pkgIds.Count; i++)
                    {
                        var pkgName = Path.GetFileNameWithoutExtension(pkgIds[i]);
                        var parts = pkgName.Split(PKG_SEPARATOR);
                        bool exist = false;
                        if (parts != null && parts.Length == 2)
                        {
                            string curId = parts[1];
                            for (int j = 0; j < importedPackages.Count; j++)
                            {
                                if (string.Equals(curId, importedPackages[j], StringComparison.OrdinalIgnoreCase))
                                {
                                    // Package was already imported: Flag it to be removed
                                    exist = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            exist = true; // Invalid file name: Consider it to be exist and remove it later
                        }

                        if (exist)
                        {
                            this.Logger.DebugFormat("Package [{0}] already existed", pkgIds[i]);
                            pkgIds.RemoveAt(i--);
                        }
                    }
                }

                if (pkgIds.Count > 0)
                {
                    callback.Style = ProgressStyle.Continuous;
                    callback.SetRange(0, pkgIds.Count);
                    result = new List<DataPackage<TravelRoute>>();
                    foreach (var f in pkgIds)
                    {
                        callback.Text = f;
                        byte[] rawData = client.GetFile(f);
                        byte[] extractData = this.Decompress(rawData);
                        if (extractData == null)
                        {
                            this.Logger.WarnFormat("Package [{0}] is corrupted", f);
                        }
                        else
                        {
                            this.Logger.InfoFormat("Import package [{0}] from DropBox", f);
                            var formatter = new ProtoBufTransfer(this.Logger);
                            var newPkg = formatter.FromRaw<DataPackage<TravelRoute>>(extractData);
                            if (newPkg != null && newPkg.Data != null && newPkg.Data.Count > 0)
                            {
                                result.Add(newPkg);
                            }
                        }

                        callback.Increment(1);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>
        /// The <see cref="DropNetClient"/>.
        /// </returns>
        /// <exception cref="ConfigurationException">
        /// </exception>
        private DropNetClient GetClient()
        {
            string apiKey = this.Convert(this._config.ApiKey);
            string apiSecret = this.Convert(this._config.ApiSecret);
            string userToken = this.Convert(this._config.UserToken);
            string userSecret = this.Convert(this._config.UserSecret);

            bool isError = string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret) || string.IsNullOrEmpty(userToken)
                            || string.IsNullOrEmpty(userSecret);
            if (isError)
            {
                throw new ConfigurationException(
                    this, 
                    "The plugin has not been properly configured. Please make sure that you have authenticated with DropBox");
            }

            var result = new DropNetClient(apiKey, apiSecret, userToken, userSecret) { UseSandbox = true };
            return result;
        }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string Convert(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var hexStr = Encoding.UTF8.GetString(data);
            return this._formatter.Untag(hexStr);
        }

        /// <summary>
        /// The get or create base meta data.
        /// </summary>
        /// <param name="dropBoxClient">
        /// The drop box client.
        /// </param>
        /// <returns>
        /// The <see cref="MetaData"/>.
        /// </returns>
        private MetaData GetOrCreateBaseMetaData(DropNetClient dropBoxClient)
        {
            this.Logger.Info("Check DropBox folder status");
            MetaData rootMeta = null;

            try
            {
                rootMeta = dropBoxClient.GetMetaData(this.DropBoxPath);
                if (rootMeta.Is_Deleted)
                {
                    rootMeta = null;
                }
            }
            catch (Exception ex)
            {
                var realEx = DropBoxExceptionHandler.HandleException(ex);
                if (realEx != null)
                {
                    this.Logger.Warn("Could not get DropBox base data path: " + realEx.Message);
                }
            }

            if (rootMeta == null)
            {
                try
                {
                    rootMeta = dropBoxClient.CreateFolder(this.DropBoxPath);
                }
                catch (Exception ex)
                {
                    var realEx = DropBoxExceptionHandler.HandleException(ex);
                    if (realEx != null)
                    {
                        string err = "Failed to access DropBox: " + realEx.Message;
                        MessageBox.Show(err, "DropBox Sync Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Logger.Error(err);
                    }
                }
            }

            return rootMeta;
        }

        /// <summary>
        /// The compress.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="entryName">
        /// The entry name.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private byte[] Compress(byte[] input, string entryName)
        {
            if (input == null)
            {
                return null;
            }

            using (var outStream = new MemoryStream())
            {
                using (var zip = new ZipFile())
                {
                    zip.CompressionMethod = CompressionMethod.BZip2;
                    zip.CompressionLevel = CompressionLevel.BestCompression;
                    zip.AddEntry(entryName, input);
                    zip.Save(outStream);

                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// The decompress.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        private byte[] Decompress(byte[] input)
        {
            using (var inputStream = new MemoryStream(input))
            {
                using (var extractor = ZipFile.Read(inputStream))
                {
                    foreach (var entry in extractor)
                    {
                        var outStream = new MemoryStream();
                        entry.Extract(outStream);
                        return outStream.ToArray();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// The log exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string LogException(Exception ex)
        {
            var realEx = DropBoxExceptionHandler.HandleException(ex);
            string err = "Failed to synchronize data with DropBox: ";
            if (realEx == null)
            {
                err += ex.Message;
            }
            else
            {
                err += realEx.Message;
                this.Logger.Error(err);
            }

            return err;
        }
    }
}