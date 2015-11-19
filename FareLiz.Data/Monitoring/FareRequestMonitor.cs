namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;

    using log4net;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>
    /// The monitor state.
    /// </summary>
    public enum MonitorState
    {
        /// <summary>
        /// The stopped.
        /// </summary>
        Stopped, 

        /// <summary>
        /// The stopping.
        /// </summary>
        Stopping, 

        /// <summary>
        /// The running.
        /// </summary>
        Running
    }

    /// <summary>
    /// The operation mode.
    /// </summary>
    public enum OperationMode
    {
        /// <summary>
        /// The unspecified.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        Unspecified, 

        /// <summary>
        /// The show fare.
        /// </summary>
        [Description("Display the fare data on screen")]
        ShowFare, 

        /// <summary>
        /// The get fare and save.
        /// </summary>
        [Description("Export the data and then close the browser tab")]
        GetFareAndSave, 

        /// <summary>
        /// The live monitor.
        /// </summary>
        [Description("Live monitoring of the fare data")]
        LiveMonitor
    }

    /// <summary>Monitor for fare browser requests</summary>
    public class FareRequestMonitor : IDisposable
    {
        /// <summary>
        /// The _control factory.
        /// </summary>
        protected readonly IFareBrowserControlFactory _controlFactory;

        /// <summary>
        /// The _lock obj.
        /// </summary>
        private readonly object _lockObj = new object();

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// The _pending queue.
        /// </summary>
        private readonly DataQueue<FareMonitorRequest> _pendingQueue = new DataQueue<FareMonitorRequest>();

        /// <summary>
        /// The _process queue.
        /// </summary>
        private readonly DataQueue<FareMonitorRequest> _processQueue = new DataQueue<FareMonitorRequest>();

        /// <summary>
        /// The _background worker.
        /// </summary>
        private BackgroundWorker _backgroundWorker;

        /// <summary>
        /// Initializes a new instance of the <see cref="FareRequestMonitor"/> class.
        /// </summary>
        /// <param name="controlFactory">
        /// The control factory.
        /// </param>
        public FareRequestMonitor(IFareBrowserControlFactory controlFactory)
        {
            this.RequestTimeout = TimeSpan.FromMinutes(5);
            this._logger = AppContext.Logger;
            this._controlFactory = controlFactory;
        }

        /// <summary>
        /// Gets the batch process size.
        /// </summary>
        public int BatchProcessSize
        {
            get
            {
                var result = AppContext.MonitorEnvironment.FareDataProvider.SimultaneousRequests;
                return result > 0 ? result : 1;
            }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public MonitorState State { get; private set; }

        /// <summary>
        /// Gets the monitor environment.
        /// </summary>
        public MonitorEnvironment MonitorEnvironment { get; private set; }

        /// <summary>
        /// Gets or sets the request timeout.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// Gets the pending requests.
        /// </summary>
        public DataQueue<FareMonitorRequest> PendingRequests
        {
            get
            {
                return this._pendingQueue;
            }
        }

        /// <summary>
        /// Gets the processing requests.
        /// </summary>
        public DataQueue<FareMonitorRequest> ProcessingRequests
        {
            get
            {
                return this._processQueue;
            }
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        public virtual OperationMode Mode
        {
            get
            {
                return OperationMode.ShowFare;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public virtual void Dispose()
        {
            this.Stop();
            this.Clear();
        }

        /// <summary>
        /// The request starting.
        /// </summary>
        public event FareRequestHandler RequestStarting;

        /// <summary>
        /// The request stopping.
        /// </summary>
        public event FareRequestHandler RequestStopping;

        /// <summary>
        /// The request completed.
        /// </summary>
        public event FareRequestHandler RequestCompleted;

        /// <summary>
        /// The monitor starting.
        /// </summary>
        public event FareMonitorHandler MonitorStarting;

        /// <summary>
        /// The monitor stopping.
        /// </summary>
        public event FareMonitorHandler MonitorStopping;

        /// <summary>
        /// The enqueue.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="departureDate">
        /// The departure date.
        /// </param>
        /// <param name="returnDate">
        /// The return date.
        /// </param>
        public void Enqueue(Airport origin, Airport destination, DateTime departureDate, DateTime returnDate)
        {
            var newRequest = new FareMonitorRequest(origin, destination, departureDate, returnDate);
            this.Enqueue(newRequest);
        }

        /// <summary>
        /// The enqueue.
        /// </summary>
        /// <param name="requests">
        /// The requests.
        /// </param>
        public void Enqueue(IEnumerable<FareMonitorRequest> requests)
        {
            foreach (var r in requests)
            {
                this.Enqueue(r);
            }
        }

        /// <summary>
        /// The enqueue.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void Enqueue(FareMonitorRequest request)
        {
            request.OwnerMonitor = this;
            this._pendingQueue.Enqueue(request);
        }

        /// <summary>
        /// The clear.
        /// </summary>
        public void Clear()
        {
            this._pendingQueue.Clear(true);
            this._processQueue.Clear(true);
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <exception cref="TimeoutException">
        /// </exception>
        public void Start()
        {
            lock (this._lockObj)
            {
                AppContext.Logger.DebugFormat("Start {0}...", this.GetType());
                DateTime startDate = DateTime.Now;

                // If monitor is still being stopped: Wait for it within a timeout limit
                while (this.State == MonitorState.Stopping && (DateTime.Now - startDate) < this.RequestTimeout)
                {
                    Thread.Sleep(500);
                }

                if (this.State != MonitorState.Stopped)
                {
                    // If the monitor is still not stopped: Throw exception
                    throw new TimeoutException("Could not restart monitor. Timed out waiting for the monitor to stop");
                }

                if (this.MonitorStarting != null)
                {
                    this.MonitorStarting(this);
                }

                this.State = MonitorState.Running;
                if (this._backgroundWorker == null)
                {
                    this._backgroundWorker = new BackgroundWorker();
                    this._backgroundWorker.DoWork += this.backgroundWorker_DoWork;
                    this._backgroundWorker.RunWorkerCompleted += this.backgroundWorker_RunWorkerCompleted;
                }

                // If we can reach here: The background worker should not be doing anything on background
                if (!this._backgroundWorker.IsBusy)
                {
                    this._backgroundWorker.RunWorkerAsync();
                }

                AppContext.Logger.DebugFormat("{0} started", this.GetType());
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public virtual void Stop()
        {
            lock (this._lockObj)
            {
                AppContext.Logger.DebugFormat("Stopping {0}...", this.GetType());
                this.State = this._backgroundWorker.IsBusy ? MonitorState.Stopping : MonitorState.Stopped;
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

        /// <summary>
        /// The stop.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void Stop(FareMonitorRequest request)
        {
            this._logger.Info("Stopping " + this.GetType().Name);
            if (this.RequestStopping != null)
            {
                FareBrowserRequestArg args = new FareBrowserRequestArg(request);
                this.OnRequestStopping(args);
                if (args.Cancel)
                {
                    return;
                }
            }

            request.Stop();
        }

        /// <summary>
        /// The on request starting.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
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
                                if (this.State != MonitorState.Running)
                                {
                                    return;
                                }

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

        /// <summary>
        /// The on request stopping.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected virtual void OnRequestStopping(FareBrowserRequestArg args)
        {
            this._logger.Debug("Stopping request: " + args.Request.ShortDetail);
            if (this.RequestStopping != null)
            {
                this.RequestStopping(this, args);
            }

            this.FinalizeRequest(args.Request);
        }

        /// <summary>
        /// The on request completed.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        protected virtual void OnRequestCompleted(FareBrowserRequestArg args)
        {
            this._logger.Info("Fare request completed: " + args.Request.ShortDetail);
            if (args.Request.BrowserControl != null && args.Request.BrowserControl.RequestState == DataRequestState.NoData)
            {
                this._logger.Info("No data was found!");
            }

            if (this.RequestCompleted != null)
            {
                this.RequestCompleted(this, args);
            }
        }

        /// <summary>
        /// The finalize request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public void FinalizeRequest(FareMonitorRequest request)
        {
            this._processQueue.Remove(request);
            if (this._pendingQueue.Count == 0 && this._processQueue.Count == 0)
            {
                this.Stop();
            }
        }

        /// <summary>
        /// The background worker_ do work.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            AppUtil.NameCurrentThread(this.GetType().Name);
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;
            while (this.State == MonitorState.Running)
            {
                int canProcessCount = this.BatchProcessSize - this._processQueue.Count;
                if (canProcessCount > this._pendingQueue.Count)
                {
                    canProcessCount = this._pendingQueue.Count;
                }

                if (canProcessCount > 0)
                {
                    Console.WriteLine("Starting " + canProcessCount + " requests...");
                    var itemList = this._pendingQueue.Dequeue(canProcessCount);
                    foreach (FareMonitorRequest request in itemList)
                    {
                        var args = new FareBrowserRequestArg(request);
                        this.OnRequestStarting(args);
                        if (args.Cancel)
                        {
                            continue;
                        }

                        request.Start(); // Start the request and move it to the appropriate queue
                        this._pendingQueue.Remove(request);
                        this._processQueue.Enqueue(request);
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// The background worker_ run worker completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.MonitorStopping != null)
            {
                this.MonitorStopping(this);
            }

            bool noRunningReq = this._pendingQueue.Count == 0 && this._processQueue.Count == 0;
            this.State = noRunningReq ? MonitorState.Stopped : MonitorState.Stopping;
        }

        /// <summary>
        /// The get browser control.
        /// </summary>
        /// <returns>
        /// The <see cref="IFareBrowserControl"/>.
        /// </returns>
        internal IFareBrowserControl GetBrowserControl()
        {
            return this._controlFactory.GetBrowserControl();
        }
    }

    /// <summary>
    /// The fare monitor handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    public delegate void FareMonitorHandler(FareRequestMonitor sender);

    /// <summary>
    /// The fare request handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    public delegate void FareRequestHandler(FareRequestMonitor sender, FareBrowserRequestArg args);

    /// <summary>
    /// The fare browser request arg.
    /// </summary>
    public class FareBrowserRequestArg : CancelEventArgs
    {
        /// <summary>
        /// The empty.
        /// </summary>
        public static new readonly FareBrowserRequestArg Empty = new FareBrowserRequestArg(null);

        /// <summary>
        /// The request.
        /// </summary>
        public readonly FareMonitorRequest Request;

        /// <summary>
        /// The request initiated date.
        /// </summary>
        public readonly DateTime RequestInitiatedDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="FareBrowserRequestArg"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public FareBrowserRequestArg(FareMonitorRequest request)
        {
            this.Request = request;
            if (request != null && request.BrowserControl != null)
            {
                this.RequestInitiatedDate = request.BrowserControl.LastRequestInitiatedDate;
            }
        }
    }
}