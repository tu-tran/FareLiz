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

namespace SkyDean.FareLiz.DropBox
{
    /// <summary>
    /// Abstract class used for synchronizing object with DropBox
    /// </summary>
    /// <typeparam name="T">The type of the target fare database.</typeparam>
    public abstract class DropBoxDbSync<T> : IDatabaseSyncer<T> where T : IFareDatabase, ISyncable
    {
        private const string PKG_EXT = ".dbpkg",
                             PKG_DATEFORMAT = "yyyyMMddHHmmss";
        private const char PKG_SEPARATOR = '_';
        private StringFormatter _formatter = new StringFormatter(DropBoxSyncConfigBuilder.Seed);

        public event SyncEventHandler<T> OnValidateData;

        public ISyncable SyncTargetObject { get; set; }
        public T SyncTarget
        {
            get { return (T)SyncTargetObject; }
        }

        private DropBoxSyncerConfig _config = new DropBoxSyncerConfig();
        public IConfig Configuration
        {
            get { return _config; }
            set { _config = value as DropBoxSyncerConfig; }
        }

        public IConfig DefaultConfig { get { return new DropBoxSyncerConfig(); } }
        public IConfigBuilder CustomConfigBuilder { get { return new DropBoxSyncConfigBuilder(Logger); } }
        public ILog Logger { get; set; }

        public string DropBoxPath { get { return _config.DropBoxBaseFolder; } }

        public virtual void Initialize()
        {
            if (Configuration == null)
                Configuration = DefaultConfig;
        }

        public bool Synchronize(SyncOperation operation, object data, IProgressCallback callback)
        {
            if (data == null)
                throw new ArgumentException("Synchronize data cannot be null");

            Logger.InfoFormat("Synchronizing data [{0}]: {1}", operation, data);

            bool success = false;
            try
            {
                callback.Begin();
                callback.Text = "Retrieving DropBox metadata...";
                string dataFilePath = data.ToString();
                string dropBoxDbFileName = Path.GetFileName(data.ToString()) + ".compressed";
                string dropBoxDbFilePath = String.Format("{0}/{1}", DropBoxPath, dropBoxDbFileName);
                DropNetClient client = GetClient();
                MetaData baseMeta = GetOrCreateBaseMetaData(client),
                         fileMeta = null;

                if (baseMeta.Contents != null)
                    foreach (MetaData m in baseMeta.Contents)
                        if (String.Compare(m.Name, dropBoxDbFileName, StringComparison.OrdinalIgnoreCase) == 0 && !m.Is_Dir)
                        // File already exists
                        {
                            fileMeta = m;
                            break;
                        }

                switch (operation)
                {
                    case SyncOperation.Download: // Download and Decompress from DropBox
                        if (fileMeta == null)
                            throw new ApplicationException("There is no data available from DropBox!");

                        ConfirmationType confirm = callback.Confirm(
                                    callback,
                                    String.Format(
                                        "The last data was updated on {0} ({1}). Do you want to download and install this database?",
                                        fileMeta.ModifiedDate, fileMeta.Size),
                                    "Download Confirmation");

                        if (confirm != ConfirmationType.Yes)
                            return false;

                        callback.Title = "Restore Database from DropBox";
                        string message = String.Format("Downloading data from DropBox ({0})...", fileMeta.Size);
                        Logger.Info(message);
                        callback.Text = message;

                        string backupFile = String.Format("{0}.bak.{1}", dataFilePath, DateTime.Now.ToString("yyyyMMddHHmmss"));
                        try
                        {
                            byte[] dataBytes = client.GetFile(dropBoxDbFilePath);
                            File.WriteAllBytes(dropBoxDbFileName, dataBytes);
                            if (File.Exists(dataFilePath))
                            {
                                Logger.InfoFormat("Backup existing data file: [{0}] to [{1}]", dataFilePath, backupFile);
                                File.Move(dataFilePath, backupFile);
                            }
                            else
                                Logger.InfoFormat("Data file [{0}] does not exist... Skip backup", dataFilePath);

                            message = "Processing received data...";
                            Logger.Info(message);
                            callback.Text = message;
                            Logger.Debug("Decompressing received file: " + dropBoxDbFileName);
                            using (var zip = ZipFile.Read(dropBoxDbFileName))
                            {
                                zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                                zip.ExtractProgress += (o, e) =>
                                {
                                    if (e.TotalBytesToTransfer > 0)
                                        callback.StepTo((int)(100 * e.BytesTransferred / e.TotalBytesToTransfer));
                                };

                                callback.Style = ProgressStyle.Continuous;
                                callback.SetRange(0, 100);

                                foreach (var entry in zip)
                                {
                                    entry.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                                    entry.Extract(ExtractExistingFileAction.OverwriteSilently);
                                }
                            }

                            Logger.Info("Delete temporary compressed file: " + dropBoxDbFileName);
                            callback.Text = "Post-processing data...";
                            callback.Style = ProgressStyle.Marquee;
                            File.Delete(dropBoxDbFileName);

                            if (OnValidateData != null)
                                OnValidateData(SyncTarget, new SyncEventArgs<T>(this, data));

                            Logger.Info("Delete backup file: " + backupFile);
                            File.Delete(backupFile);
                        }
                        catch (Exception ex)
                        {
                            string actualErr = LogException(ex);
                            if (!callback.IsAborting)
                                callback.Inform(callback, "Failed to download database. The previous database will now be restored. The error message was:" + Environment.NewLine + actualErr, "DropBox Database Download", NotificationType.Error);

                            // Restore the backup and then rethrow the exception to the outer loop
                            if (File.Exists(backupFile))
                            {
                                if (File.Exists(dataFilePath))
                                    File.Delete(dataFilePath);
                                Logger.InfoFormat("Restore backup file [{0}] to [{1}]", backupFile, dataFilePath);
                                File.Move(backupFile, dataFilePath);
                            }
                        }
                        break;

                    case SyncOperation.Upload: // Compress and Upload to DropBox
                        Logger.Info("Upload data to DropBox");
                        if (!File.Exists(dataFilePath))
                            throw new ApplicationException("There is no local data available");

                        if (callback.Confirm(callback, String.Format("The current data size is {0}. Do you want to proceed ? (Data will be compressed)", StringUtil.FormatSize(new FileInfo(dataFilePath).Length)),
                                            "Backup database to DropBox") != ConfirmationType.Yes)
                            return false;

                        callback.Title = "Database Compression";
                        message = "Processing offline data for uploading...";
                        Logger.Info(message);
                        callback.Text = message;
                        string tempDataFile = dropBoxDbFileName + DateTime.Now.ToString(".yyyyMMddHHmmss");
                        Logger.InfoFormat("Compress [{0}] into [{1}]", dataFilePath, tempDataFile);
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
                                    callback.StepTo((int)(100 * e.BytesTransferred / e.TotalBytesToTransfer));
                            };
                            zip.Save();
                        }

                        message = String.Format("Sending data ({0})...", StringUtil.FormatSize(new FileInfo(tempDataFile).Length));
                        Logger.Info(message);
                        callback.Title = "Backup Database to DropBox";
                        callback.Text = message;
                        callback.Style = ProgressStyle.Marquee;
                        client.UploadFile(DropBoxPath, tempDataFile, File.ReadAllBytes(tempDataFile));
                        Logger.Info("Delete temporary file: " + tempDataFile);
                        File.Delete(tempDataFile);

                        if (fileMeta != null)
                        {
                            Logger.Info("Delete DropBox online file: " + dropBoxDbFilePath);
                            client.Delete(dropBoxDbFilePath);
                        }

                        string dropBoxTempFile = String.Format("{0}/{1}", DropBoxPath, tempDataFile);
                        Logger.InfoFormat("Move DropBox online file: [{0}] -> [{1}]", dropBoxTempFile, dropBoxDbFilePath);
                        client.Move(dropBoxTempFile, dropBoxDbFilePath);
                        break;

                    default:
                        throw new NotImplementedException("This operation is not implemented!");
                }

                Logger.InfoFormat("DropBox {0} completed", operation);
                success = true;
            }
            catch (Exception ex)
            {
                var message = LogException(ex);
                callback.Inform(callback, message, "DropBox Database Synchronizing", NotificationType.Error);
            }

            Logger.InfoFormat("Synchronization [{0}] ended", operation);
            return success;
        }

        public IList<DataPackage<TravelRoute>> Receive(IList<string> importedPackages, IProgressCallback callback)
        {
            IList<DataPackage<TravelRoute>> result = null;
            Logger.Info("Receiving packages from DropBox");
            ProgressDialog.ExecuteTask(null, "DropBox Synchronization", "Retrieving data packages from configured DropBox account...", GetType().Name + "-Receive", ProgressBarStyle.Marquee, Logger, cb =>
                        {
                            result = DoReceive(cb as ProgressDialog, importedPackages);
                        });

            return result;
        }

        private IList<DataPackage<TravelRoute>> DoReceive(ProgressDialog callback, IList<string> importedPackages)
        {
            Logger.Info("Receive DropBox packages");
            IList<DataPackage<TravelRoute>> result = null;
            callback.Begin();
            callback.Text = "Retrieving DropBox metadata...";
            var client = GetClient();
            MetaData baseData = GetOrCreateBaseMetaData(client);
            if (baseData != null && baseData.Contents != null)
            {
                callback.Text = "Processing DropBox metadata...";
                var pkgIds = new List<string>();
                foreach (MetaData m in baseData.Contents)
                    if (!m.Is_Dir && m.Extension == PKG_EXT)
                    {
                        pkgIds.Add(m.Path);
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
                                if (String.Equals(curId, importedPackages[j], StringComparison.OrdinalIgnoreCase))
                                {
                                    // Package was already imported: Flag it to be removed
                                    exist = true;
                                    break;
                                }
                            }
                        }
                        else
                            exist = true;   // Invalid file name: Consider it to be exist and remove it later

                        if (exist)
                        {
                            Logger.DebugFormat("Package [{0}] already existed", pkgIds[i]);
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
                        byte[] extractData = Decompress(rawData);
                        if (extractData == null)
                            Logger.WarnFormat("Package [{0}] is corrupted", f);
                        else
                        {
                            Logger.InfoFormat("Import package [{0}] from DropBox", f);
                            var formatter = new ProtoBufTransfer(Logger);
                            var newPkg = formatter.FromRaw<DataPackage<TravelRoute>>(extractData);
                            if (newPkg != null && newPkg.Data != null && newPkg.Data.Count > 0)
                                result.Add(newPkg);
                        }
                        callback.Increment(1);
                    }
                }
            }

            return result;
        }

        public void Send(DataPackage<TravelRoute> data, IProgressCallback callback)
        {
            Logger.InfoFormat("Send data to DropBox");
            var client = GetClient();
            MetaData baseData = GetOrCreateBaseMetaData(client);
            if (baseData == null)
                Logger.Warn("There is no data to be sent");
            else
            {
                var formatter = new ProtoBufTransfer(Logger);
                byte[] rawData = formatter.ToRaw(data);
                if (rawData != null)
                {
                    byte[] compressedRaw = Compress(rawData, data.Id);
                    if (compressedRaw != null)
                    {
                        Logger.InfoFormat("Upload package [{0}] to DropBox", data.Id);
                        string newFile = data.CreatedDate.ToString(PKG_DATEFORMAT) + PKG_SEPARATOR + data.Id + PKG_EXT;
                        client.UploadFile(baseData.Path, newFile, compressedRaw);
                        Logger.InfoFormat("Package [{0}] was uploaded ({1})", data.Id, StringUtil.FormatSize(compressedRaw.LongLength));
                    }
                }
            }
        }

        private DropNetClient GetClient()
        {
            string apiKey = Convert(_config.ApiKey);
            string apiSecret = Convert(_config.ApiSecret);
            string userToken = Convert(_config.UserToken);
            string userSecret = Convert(_config.UserSecret);

            bool isError = (String.IsNullOrEmpty(apiKey) || String.IsNullOrEmpty(apiSecret) || String.IsNullOrEmpty(userToken) || String.IsNullOrEmpty(userSecret));
            if (isError)
                throw new ConfigurationException(this, "The plugin has not been properly configured. Please make sure that you have authenticated with DropBox");

            var result = new DropNetClient(apiKey, apiSecret, userToken, userSecret) { UseSandbox = true };
            return result;
        }

        private string Convert(byte[] data)
        {
            if (data == null)
                return null;

            var hexStr = Encoding.UTF8.GetString(data);
            return _formatter.Untag(hexStr);
        }

        MetaData GetOrCreateBaseMetaData(DropNetClient dropBoxClient)
        {
            Logger.Info("Check DropBox folder status");
            MetaData rootMeta = null;

            try
            {
                rootMeta = dropBoxClient.GetMetaData(DropBoxPath);
                if (rootMeta.Is_Deleted)
                    rootMeta = null;
            }
            catch (Exception ex)
            {
                var realEx = DropBoxExceptionHandler.HandleException(ex);
                if (realEx != null)
                    Logger.Warn("Could not get DropBox base data path: " + realEx.Message);
            }

            if (rootMeta == null)
                try
                {
                    rootMeta = dropBoxClient.CreateFolder(DropBoxPath);
                }
                catch (Exception ex)
                {
                    var realEx = DropBoxExceptionHandler.HandleException(ex);
                    if (realEx != null)
                    {
                        string err = "Failed to access DropBox: " + realEx.Message;
                        MessageBox.Show(err, "DropBox Sync Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Logger.Error(err);
                    }
                }

            return rootMeta;
        }

        private byte[] Compress(byte[] input, string entryName)
        {
            if (input == null)
                return null;

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

        private string LogException(Exception ex)
        {
            var realEx = DropBoxExceptionHandler.HandleException(ex);
            string err = "Failed to synchronize data with DropBox: ";
            if (realEx == null)
                err += ex.Message;
            else
            {
                err += realEx.Message;
                Logger.Error(err);
            }

            return err;
        }
    }
}