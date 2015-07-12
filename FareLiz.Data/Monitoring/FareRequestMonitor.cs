using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    using log4net;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    public enum MonitorState { Stopped, Stopping, Running }
    public enum OperationMode
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        Unspecified,

        [Description("Display the fare data on screen")]
        ShowFare,

        [Description("Export the data and then close the browser tab")]
        GetFareAndSave,

        [Description("Live monitoring of the fare data")]
        LiveMonitor
    }

    /// <summary>
    /// Monitor for fare browser requests
    /// </summary>
    public class FareRequestMonitor : IDisposable
    {
        private BackgroundWorker _backgroundWorker = null;
        private readonly ILog _logger;
        protected readonly IFareBrowserControlFactory _controlFactory;
        private readonly DataQueue<FareMonitorRequest> _pendingQueue = new DataQueue<FareMonitorRequest>();
        private readonly DataQueue<FareMonitorRequest> _processQueue = new DataQueue<FareMonitorRequest>();
        private readonly object _lockObj = new object();

        public int BatchProcessSize
        {
            get
            {
                var result = AppContext.MonitorEnvironment.FareDataProvider.SimultaneousRequests;
                return result > 0 ? result : 1;
            }
        }

        public MonitorState State { get; private set; }
        public MonitorEnvironment MonitorEnvironment { get; private set; }
        public TimeSpan RequestTimeout { get; set; }

        public DataQueue<FareMonitorRequest> PendingRequests { get { return this._pendingQueue; } }
        public DataQueue<FareMonitorRequest> ProcessingRequests { get { return this._processQueue; } }

        public event FareRequestHandler RequestStarting;
        public event FareRequestHandler RequestStopping;
        public event FareRequestHandler RequestCompleted;
        public event FareMonitorHandler MonitorStarting;
        public event FareMonitorHandler MonitorStopping;

        public virtual OperationMode Mode { get { return OperationMode.ShowFare; } }

        public FareRequestMonitor(IFareBrowserControlFactory controlFactory)
        {
            this.RequestTimeout = TimeSpan.FromMinutes(5);
            this._logger = AppContext.Logger;
            this._controlFactory = controlFactory;
        }

        public void Enqueue(Airport origin, Airport destination, DateTime departureDate, DateTime returnDate)
        {
            var newRequest = new FareMonitorRequest(origin, destination, departureDate, returnDate);
            this.Enqueue(newRequest);
        }

        public void Enqueue(IEnumerable<FareMonitorRequest> requests)
        {
            foreach (var r in requests)
            {
                this.Enqueue(r);
            }
        }

        public void Enqueue(FareMonitorRequest request)
        {
            request.OwnerMonitor = this;
            this._pendingQueue.Enqueue(request);
        }

        public void Clear()
        {
            this._pendingQueue.Clear(true);
            this._processQueue.Clear(true);
        }

        public void Start()
        {
            lock (this._lockObj)
            {
                AppContext.Logger.DebugFormat("Start {0}...", this.GetType());
                DateTime startDate = DateTime.Now;

                // If monitor is still being stopped: Wait for it within a timeout limit
                while (this.State == MonitorState.Stopping && (DateTime.Now - startDate) < this.RequestTimeout)
                    Thread.Sleep(500);

                if (this.State != MonitorState.Stopped)  // If the monitor is still not stopped: Throw exception
                    throw new TimeoutException("Could not restart monitor. Timed out waiting for the monitor to stop");

                if (this.MonitorStarting != null)
                    this.MonitorStarting(this);

                this.State = MonitorState.Running;
                if (this._backgroundWorker == null)
                {
                    this._backgroundWorker = new BackgroundWorker();
                    this._backgroundWorker.DoWork += this.backgroundWorker_DoWork;
                    this._backgroundWorker.RunWorkerCompleted += this.backgroundWorker_RunWorkerCompleted;
                }

                // If we can reach here: The background worker should not be doing anything on background
                if (!this._backgroundWorker.IsBusy)
                    this._backgroundWorker.RunWorkerAsync();
                AppContext.Logger.DebugFormat("{0} started", this.GetType());
            }
        }

        public virtual void Stop()
        {
            lock (this._lockObj)
            {
                AppContext.Logger.DebugFormat("Stopping {0}...", this.GetType());
                this.State = (this._backgroundWorker.IsBusy ? MonitorState.Stopping : MonitorState.Stopped);
                if (this._pendingQueue.Count > 0)
                {
                    this._pendingQueue.ToList().ForEach(this.Stop);
                    this._pendingQueue.Clear(true);
                }

                if (this._processQueue.Count > 0)
                {
                    this._processQueue.ToList().ForEach(this.Stop);
                    this._processQueue.Clear(true);
                }
                AppContext.Logger.DebugFormat("{0} stopped", this.GetType());
            }
        }

        public void Stop(FareMonitorRequest request)
        {
            this._logger.Info("Stopping " + this.GetType().Name);
            if (this.RequestStopping != null)
            {
                FareBrowserRequestArg args = new FareBrowserRequestArg(request);
                this.OnRequestStopping(args);
                if (args.Cancel)
                    return;
            }

            request.Stop();
        }

        public virtual void Dispose()
        {
            this.Stop();
            this.Clear();
        }

        protected virtual void OnRequestStarting(FareBrowserRequestArg args)
        {
            this._logger.Debug("Starting request for journey: " + args.Request.ShortDetail);
            if (this.RequestStarting != null)
            {
                this.RequestStarting(this, args);
            }

            var request = args.Request;
            if (request.BrowserControl == null)
            {
                request.Initialize();
            }
            else
            {
                if (request.BrowserControl.RequestState == DataRequestState.Pending)
                {
                    // Assign the event handler for completed request when the request is starting
                    JourneyBrowserDelegate _requestCompletedHandler = null;
                    _requestCompletedHandler = (b, j) =>
                        {
                            try
                            {
                                if (this.State != MonitorState.Running) return;

                                lock (this._lockObj)
                                {
                                    this.OnRequestCompleted(new FareBrowserRequestArg(request));
                                    this.FinalizeRequest(request);
                                }
                            }
                            finally
                            {
                                request.BrowserControl.GetJourneyCompleted -= _requestCompletedHandler;
                            }
                        };
                    request.BrowserControl.GetJourneyCompleted += _requestCompletedHandler;
                }
            }
        }

        protected virtual void OnRequestStopping(FareBrowserRequestArg args)
        {
            this._logger.Debug("Stopping request: " + args.Request.ShortDetail);
            if (this.RequestStopping != null)
            {
                this.RequestStopping(this, args);
            }

            this.FinalizeRequest(args.Request);
        }

        protected virtual void OnRequestCompleted(FareBrowserRequestArg args)
        {
            this._logger.Info("Fare request completed: " + args.Request.ShortDetail);
            if (args.Request.BrowserControl != null
                && args.Request.BrowserControl.RequestState == DataRequestState.NoData)
            {
                this._logger.Info("No data was found!");
            }

            if (this.RequestCompleted != null)
            {
                this.RequestCompleted(this, args);
            }
        }

        public void FinalizeRequest(FareMonitorRequest request)
        {
            this._processQueue.Remove(request);
            if (this._pendingQueue.Count == 0 && this._processQueue.Count == 0)
            {
                this.Stop();
            }
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AppUtil.NameCurrentThread(this.GetType().Name);
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            while (this.State == MonitorState.Running)
            {
                int canProcessCount = this.BatchProcessSize - this._processQueue.Count;
                if (canProcessCount > this._pendingQueue.Count)
                    canProcessCount = this._pendingQueue.Count;

                if (canProcessCount > 0)
                {
                    Console.WriteLine("Starting " + canProcessCount + " requests...");
                    var itemList = this._pendingQueue.Dequeue(canProcessCount);
                    foreach (FareMonitorRequest request in itemList)
                    {
                        var args = new FareBrowserRequestArg(request);
                        this.OnRequestStarting(args);
                        if (args.Cancel)
                            continue;

                        request.Start();    // Start the request and move it to the appropriate queue
                        this._pendingQueue.Remove(request);
                        this._processQueue.Enqueue(request);
                    }
                }

                Thread.Sleep(500);
            }
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.MonitorStopping != null)
                this.MonitorStopping(this);
            bool noRunningReq = (this._pendingQueue.Count == 0 && this._processQueue.Count == 0);
            this.State = (noRunningReq ? MonitorState.Stopped : MonitorState.Stopping);
        }

        internal IFareBrowserControl GetBrowserControl()
        {
            return this._controlFactory.GetBrowserControl();
        }
    }

    public delegate void FareMonitorHandler(FareRequestMonitor sender);
    public delegate void FareRequestHandler(FareRequestMonitor sender, FareBrowserRequestArg args);
    public class FareBrowserRequestArg : CancelEventArgs
    {
        public readonly FareMonitorRequest Request;
        public readonly DateTime RequestInitiatedDate;

        public FareBrowserRequestArg(FareMonitorRequest request)
        {
            this.Request = request;
            if (request != null && request.BrowserControl != null)
                this.RequestInitiatedDate = request.BrowserControl.LastRequestInitiatedDate;
        }

        public static readonly new FareBrowserRequestArg Empty = new FareBrowserRequestArg(null);
    }
}
