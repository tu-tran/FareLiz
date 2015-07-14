namespace SkyDean.FareLiz.DropBox
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using DropNet;
    using DropNet.Models;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using Ionic.Zip;
    using Ionic.Zlib;

    /// <summary>
    /// Abstract class used for synchronizing object with DropBox
    /// </summary>
    /// <typeparam name="T">
    /// The type of the target fare database.
    /// </typeparam>
    public abstract class DropBoxDbSync<T> : IDatabaseSyncer<T>
        where T : IFareDatabase, ISyncable
    {
        /// <summary>The pk g_ ext.</summary>
        private const string PKG_EXT = ".dbpkg";

        /// <summary>The pk g_ dateformat.</summary>
        private const string PKG_DATEFORMAT = "yyyyMMddHHmmss";

        /// <summary>The pk g_ separator.</summary>
        private const char PKG_SEPARATOR = '_';

        /// <summary>The _formatter.</summary>
        private readonly StringFormatter _formatter = new StringFormatter(DropBoxSyncConfigBuilder.Seed);

        /// <summary>The _config.</summary>
        private DropBoxSyncerConfig _config = new DropBoxSyncerConfig();

        /// <summary>Gets the sync target.</summary>
        public T SyncTarget
        {
            get
            {
                return (T)this.SyncTargetObject;
            }
        }

        /// <summary>Gets the drop box path.</summary>
        public string DropBoxPath
        {
            get
            {
                return this._config.DropBoxBaseFolder;
            }
        }

        /// <summary>The on validate data.</summary>
        public event SyncEventHandler<T> OnValidateData;

        /// <summary>Gets or sets the sync target object.</summary>
        public ISyncable SyncTargetObject { get; set; }

        /// <summary>Gets or sets the configuration.</summary>
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

        /// <summary>Gets the default config.</summary>
        public IConfig DefaultConfig
        {
            get
            {
                return new DropBoxSyncerConfig();
            }
        }

        /// <summary>Gets the custom config builder.</summary>
        public IConfigBuilder CustomConfigBuilder
        {
            get
            {
                return new DropBoxSyncConfigBuilder(this.Logger);
            }
        }

        /// <summary>Gets or sets the logger.</summary>
        public ILogger Logger { get; set; }

        /// <summary>The initialize.</summary>
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
        /// <exception cref="NotImplementedException">
        /// </exception>
        public bool Synchronize(SyncOperation operation, object data, IProgressCallback callback)
        {
            if (data == null)
            {
                throw new ArgumentException("Synchronize data cannot be null");
            }

            this.Logger.Info("Synchronizing data [{0}]: {1}", operation, data);

            var success = false;
            try
            {
                callback.Begin();
                callback.Text = "Retrieving DropBox metadata...";
                var dataFilePath = data.ToString();
                var dropBoxDbFileName = Path.GetFileName(data.ToString()) + ".compressed";
                var client = this.GetClient();
                MetaData baseMeta = this.GetOrCreateBaseMetaData(client), fileMeta = null;

                if (baseMeta.Contents != null)
                {
                    foreach (var m in baseMeta.Contents)
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
                        success = new DropBoxDownloader(client, fileMeta, dropBoxDbFileName, callback, this.Logger).Execute();
                        if (success && this.OnValidateData != null)
                        {
                            this.OnValidateData(this.SyncTarget, new SyncEventArgs<T>(this, data));
                        }

                        break;

                    case SyncOperation.Upload: // Compress and Upload to DropBox
                        success =
                            new DropBoxUploader(client, dataFilePath, dropBoxDbFileName, this.DropBoxPath, fileMeta != null, callback, this.Logger)
                                .Execute();
                        break;

                    default:
                        throw new NotImplementedException("This operation is not implemented!");
                }

                this.Logger.Info("DropBox {0} completed", operation);
                success = true;
            }
            catch (Exception ex)
            {
                var message = DropBoxExceptionHandler.Log(ex, this.Logger);
                callback.Inform(callback, message, "DropBox Database Synchronizing", NotificationType.Error);
            }

            this.Logger.Info("Synchronization [{0}] ended", operation);
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
            AppContext.ExecuteTask(
                null, 
                "DropBox Synchronization", 
                "Retrieving data packages from configured DropBox account...", 
                this.GetType().Name + "-Receive", 
                ProgressStyle.Marquee, 
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
            this.Logger.Info("Send data to DropBox");
            var client = this.GetClient();
            var baseData = this.GetOrCreateBaseMetaData(client);
            if (baseData == null)
            {
                this.Logger.Warn("There is no data to be sent");
            }
            else
            {
                var formatter = new ProtoBufTransfer(this.Logger);
                var rawData = formatter.ToRaw(data);
                if (rawData != null)
                {
                    var compressedRaw = this.Compress(rawData, data.Id);
                    if (compressedRaw != null)
                    {
                        this.Logger.Info("Upload package [{0}] to DropBox", data.Id);
                        var newFile = data.CreatedDate.ToString(PKG_DATEFORMAT) + PKG_SEPARATOR + data.Id + PKG_EXT;
                        client.UploadFile(baseData.Path, newFile, compressedRaw);
                        this.Logger.Info("Package [{0}] was uploaded ({1})", data.Id, StringUtil.FormatSize(compressedRaw.LongLength));
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
            var baseData = this.GetOrCreateBaseMetaData(client);
            if (baseData != null && baseData.Contents != null)
            {
                callback.Text = "Processing DropBox metadata...";
                var pkgIds = new List<string>();
                foreach (var m in baseData.Contents)
                {
                    if (!m.Is_Dir && m.Extension == PKG_EXT)
                    {
                        pkgIds.Add(m.Path);
                    }
                }

                // Remove imported packages from the list
                if (importedPackages != null && importedPackages.Count > 0)
                {
                    for (var i = 0; i < pkgIds.Count; i++)
                    {
                        var pkgName = Path.GetFileNameWithoutExtension(pkgIds[i]);
                        var parts = pkgName.Split(PKG_SEPARATOR);
                        var exist = false;
                        if (parts != null && parts.Length == 2)
                        {
                            var curId = parts[1];
                            for (var j = 0; j < importedPackages.Count; j++)
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
                            this.Logger.Debug("Package [{0}] already existed", pkgIds[i]);
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
                        var rawData = client.GetFile(f);
                        var extractData = this.Decompress(rawData);
                        if (extractData == null)
                        {
                            this.Logger.Warn("Package [{0}] is corrupted", f);
                        }
                        else
                        {
                            this.Logger.Info("Import package [{0}] from DropBox", f);
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

        /// <summary>The get client.</summary>
        /// <returns>The <see cref="DropNetClient" />.</returns>
        /// <exception cref="ConfigurationException"></exception>
        private DropNetClient GetClient()
        {
            var apiKey = this.Convert(this._config.ApiKey);
            var apiSecret = this.Convert(this._config.ApiSecret);
            var userToken = this.Convert(this._config.UserToken);
            var userSecret = this.Convert(this._config.UserSecret);

            var isError = string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret) || string.IsNullOrEmpty(userToken)
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
                rootMeta = dropBoxClient.GetMetaData(this.DropBoxPath, false);
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
                        var err = "Failed to access DropBox: " + realEx.Message;
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
    }
}