namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Monitor used for exporting fare</summary>
    public class FareExportMonitor : FareRequestMonitor
    {
        /// <summary>
        /// The _archive manager.
        /// </summary>
        private readonly IArchiveManager _archiveManager;

        /// <summary>
        /// The _fare database.
        /// </summary>
        private readonly IFareDatabase _fareDatabase;

        /// <summary>
        /// The _data path.
        /// </summary>
        private readonly string _dataPath;

        /// <summary>
        /// The _cached routes.
        /// </summary>
        private readonly List<TravelRoute> _cachedRoutes = new List<TravelRoute>();

        /// <summary>
        /// The _auto sync.
        /// </summary>
        private readonly Func<bool> _autoSync;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The _lock obj.
        /// </summary>
        private readonly object _lockObj = new object();

        /// <summary>
        /// The _lock stream.
        /// </summary>
        private Stream _lockStream;

        /// <summary>
        /// The cach e_ amount.
        /// </summary>
        private const int CACHE_AMOUNT = 100;

#if DEBUG

        /// <summary>
        /// The loc k_ ext.
        /// </summary>
        private static readonly string LOCK_EXT = string.Empty;
#else
        private static readonly string LOCK_EXT = ".{2559a1f2-21d7-11d4-bdaf-00c04f60b9f0}";
#endif

        /// <summary>
        /// Gets the mode.
        /// </summary>
        public override OperationMode Mode
        {
            get
            {
                return OperationMode.GetFareAndSave;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FareExportMonitor"/> class.
        /// </summary>
        /// <param name="archiveManager">
        /// The archive manager.
        /// </param>
        /// <param name="controlFactory">
        /// The control factory.
        /// </param>
        /// <param name="autoSync">
        /// The auto sync.
        /// </param>
        public FareExportMonitor(IArchiveManager archiveManager, IFareBrowserControlFactory controlFactory, bool autoSync)
            : this(archiveManager, controlFactory, () => autoSync)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FareExportMonitor"/> class.
        /// </summary>
        /// <param name="archiveManager">
        /// The archive manager.
        /// </param>
        /// <param name="controlFactory">
        /// The control factory.
        /// </param>
        /// <param name="autoSync">
        /// The auto sync.
        /// </param>
        public FareExportMonitor(IArchiveManager archiveManager, IFareBrowserControlFactory controlFactory, Func<bool> autoSync)
            : base(controlFactory)
        {
            var env = AppContext.MonitorEnvironment;
            this._archiveManager = archiveManager;
            this._archiveManager.Initialize();
            this._fareDatabase = env.FareDatabase;
            this._autoSync = autoSync;
            this._logger = env.Logger;
            var guid = Guid.NewGuid().ToString();
            this._dataPath = AppUtil.GetLocalDataPath("Temp") + "\\" + guid + LOCK_EXT;
            Directory.CreateDirectory(this._dataPath);
            this._lockStream = File.Open(Path.Combine(this._dataPath, "_" + guid), FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            this.RequestCompleted += this.FareExportMonitor_OnRequestCompleted;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            this._logger.Debug("Disposing " + this.GetType());
            this.FinalizeData();
            base.Dispose();
        }

        /// <summary>
        /// The release lock.
        /// </summary>
        private void ReleaseLock()
        {
            this._logger.Debug("Releasing lock...");
            lock (this._lockObj)
            {
                if (this._lockStream == null)
                {
                    return;
                }

                if (this._lockStream != null)
                {
                    this._lockStream.Dispose();
                    this._lockStream = null;
                }

                if (Directory.Exists(this._dataPath))
                {
                    Directory.Delete(this._dataPath, true);
                }
            }
        }

        /// <summary>
        /// The save data to database.
        /// </summary>
        private void SaveDataToDatabase()
        {
            IList<TravelRoute> newRoutes;
            lock (this._lockObj)
            {
                this._logger.Info("Trying to save data...");
                if (!Directory.Exists(this._dataPath))
                {
                    this._logger.DebugFormat("Data path [{0}] no longer exists... Exiting...", this._dataPath);
                    return;
                }

                newRoutes = this._archiveManager.ImportData(this._dataPath, DataOptions.Default, AppContext.ProgressCallback);
            }

            if (newRoutes == null || newRoutes.Count < 1)
            {
                return;
            }

            var syncDb = this._fareDatabase as ISyncableDatabase;
            if (this._autoSync() && syncDb != null)
            {
                var newPkgId = syncDb.SendData(newRoutes, AppContext.ProgressCallback);
                syncDb.AddPackage(newPkgId, null, AppContext.ProgressCallback);
            }
        }

        /// <summary>
        /// The fare export monitor_ on request completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void FareExportMonitor_OnRequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            var browser = args.Request.BrowserControl;
            if (browser == null)
            {
                this._logger.Warn("There is no browser control associated with the request");
                return;
            }

            TravelRoute route = browser.LastRetrievedRoute;
            lock (this._lockObj)
            {
                var pendingCount = sender.PendingRequests.Count;
                var processCount = sender.ProcessingRequests.Count;
                this._logger.DebugFormat("There are {0} pending requests, {1} processing requests", pendingCount, processCount);
                bool isLastRequest = pendingCount == 0 && processCount == 0;
                if (browser.RequestState == DataRequestState.Ok && route != null && route.Journeys.Count > 0)
                {
                    this._cachedRoutes.Add(route); // Cache journeys

                    if (this._cachedRoutes.Count >= CACHE_AMOUNT || isLastRequest)
                    {
                        // Export journeys to file to reduce memory load
                        this.FlushCache();
                    }
                }

                if (isLastRequest)
                {
                    this._logger.Debug("There is no more active request for the monitor");
                    this.FinalizeData();
                }
            }
        }

        /// <summary>
        /// The flush cache.
        /// </summary>
        private void FlushCache()
        {
            if (this._cachedRoutes.Count == 0)
            {
                this._logger.Info("There is no cached journey data... Nothing to flush");
            }
            else
            {
                // For now, the number of routes is also the number of journey data
                this._logger.InfoFormat("There are {0} journey in cache... Exporting...", this._cachedRoutes.Count);
                TravelRoute.Merge(this._cachedRoutes);
                this._archiveManager.ExportData(this._cachedRoutes, this._dataPath, DataFormat.Binary, AppContext.ProgressCallback);
                this._cachedRoutes.Clear();
            }
        }

        /// <summary>
        /// The finalize data.
        /// </summary>
        private void FinalizeData()
        {
            this.FlushCache();
            this.SaveDataToDatabase();
            this.ReleaseLock();
        }
    }
}