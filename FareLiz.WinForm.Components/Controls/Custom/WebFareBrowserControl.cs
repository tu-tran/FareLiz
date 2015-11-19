namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The web fare browser control.
    /// </summary>
    public sealed class WebFareBrowserControl : Control, IFareBrowserControl
    {
        /// <summary>
        /// The _sync object.
        /// </summary>
        private readonly object _syncObject = new object();

        /// <summary>
        /// The lv flight data.
        /// </summary>
        private readonly FlightDataListView lvFlightData;

        /// <summary>
        /// The _last retrieved route.
        /// </summary>
        private TravelRoute _lastRetrievedRoute;

        /// <summary>
        /// The _request state.
        /// </summary>
        private DataRequestState _requestState = DataRequestState.Pending;

        /// <summary>
        /// Prevents a default instance of the <see cref="WebFareBrowserControl"/> class from being created.
        /// </summary>
        private WebFareBrowserControl()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebFareBrowserControl"/> class.
        /// </summary>
        /// <param name="fareDataProvider">
        /// The fare data provider.
        /// </param>
        public WebFareBrowserControl(IFareDataProvider fareDataProvider)
        {
            this.DataHandler = fareDataProvider;
            this.lvFlightData = new FlightDataListView { Dock = DockStyle.Fill };
            this.lvFlightData.SetWatermark("Idle");
            this.Controls.Add(this.lvFlightData);
        }

        /// <summary>
        /// Gets a value indicating whether is stopping.
        /// </summary>
        public bool IsStopping { get; private set; }

        /// <summary>
        /// Gets the timeout in seconds.
        /// </summary>
        public int TimeoutInSeconds
        {
            get
            {
                return this.DataHandler == null ? 0 : this.DataHandler.TimeoutInSeconds;
            }
        }

        /// <summary>
        /// Gets the data handler.
        /// </summary>
        public IFareDataProvider DataHandler { get; private set; }

        /// <summary>
        /// Gets the last data date.
        /// </summary>
        public DateTime LastDataDate { get; private set; }

        /// <summary>
        /// Gets the last request initiated date.
        /// </summary>
        public DateTime LastRequestInitiatedDate { get; private set; }

        /// <summary>
        /// Gets the last request started date.
        /// </summary>
        public DateTime LastRequestStartedDate { get; private set; }

        /// <summary>
        /// Gets the last exception.
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                return AppContext.MonitorEnvironment.Logger;
            }
        }

        /// <summary>
        /// Gets the last retrieved route.
        /// </summary>
        public TravelRoute LastRetrievedRoute
        {
            get
            {
                return this._lastRetrievedRoute;
            }
        }

        /// <summary>
        /// Gets the request state.
        /// </summary>
        public DataRequestState RequestState
        {
            get
            {
                return this._requestState;
            }

            private set
            {
                this._requestState = value;
            }
        }

        /// <summary>
        /// The get journey completed.
        /// </summary>
        public event JourneyBrowserDelegate GetJourneyCompleted;

        /// <summary>
        /// The request data async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="TravelRoute"/>.
        /// </returns>
        public TravelRoute RequestDataAsync(FlightFareRequest request)
        {
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    AppUtil.NameCurrentThread(this.GetType().Name + "-RequestDataAsync");
                    this.DoRequestData(request);
                });

            return this.LastRetrievedRoute;
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public void Reset()
        {
            this.LastDataDate = this.LastRequestStartedDate = DateTime.MinValue;
            this._lastRetrievedRoute = null;
            this.RequestState = DataRequestState.Pending;
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.IsStopping = true;
        }

        /// <summary>
        /// The do request data.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        private void DoRequestData(FlightFareRequest request)
        {
            if (!Monitor.TryEnter(this._syncObject))
            {
                return;
            }

            try
            {
                this.IsStopping = false;
                if (this.IsDestructed() || this.IsStopping)
                {
                    return;
                }

                // First, reset all data
                this.Reset();
                var reqName = StringUtil.GetPeriodString(request.DepartureDate, request.ReturnDate);
                this.lvFlightData.SafeInvoke(
                    new Action(
                        () =>
                        {
                            this.lvFlightData.Name = reqName;
                            this.lvFlightData.SetWatermark("Requesting data... Please wait...");
                        }));

                // Create a thread to get the data
                DataRequestResult dataRequestResult = DataRequestResult.Empty;
                var workResult = BackgroundThread.DoWork(
                    () =>
                    {
                        this.lvFlightData.SetWatermark(
                            "Data request was started on " + DateTime.Now + Environment.NewLine
                            + "Please wait while the application retrieves fare data...");
                        this.RequestState = DataRequestState.Requested;
                        this.LastRequestStartedDate = DateTime.Now;
                        dataRequestResult = this.DataHandler.QueryData(request, this.OnProgressChanged);
                    },
                    this.TimeoutInSeconds,
                    reqName,
                    AppContext.Logger);

                if (workResult.Succeeded)
                {
                    this.RequestState = dataRequestResult.ResultRoute == null ? DataRequestState.NoData : DataRequestState.Ok;
                }
                else
                {
                    this.RequestState = DataRequestState.Failed;
                    if (workResult.IsTimedout)
                    {
                        string err = "Request timed out after " + this.TimeoutInSeconds + "s";
                        this.LastException = new TimeoutException(err);
                        this.Logger.ErrorFormat(err);
                    }
                    else if (workResult.Exception != null)
                    {
                        this.LastException = workResult.Exception;
                        this.Logger.Error("Failed to request journey data: " + workResult.Exception.Message);
                    }
                }

                this.RequestState = dataRequestResult.RequestState;
                this._lastRetrievedRoute = dataRequestResult.ResultRoute;
                if (this.RequestState > DataRequestState.Requested)
                {
                    this.LastDataDate = DateTime.Now;
                    if (this._lastRetrievedRoute != null && this._lastRetrievedRoute.Journeys.Count > 0)
                    {
                        foreach (var j in this._lastRetrievedRoute.Journeys)
                        {
                            foreach (var d in j.Data)
                            {
                                if (d.DataDate.IsUndefined())
                                {
                                    d.DataDate = this.LastDataDate;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.LastException = ex;
                this.RequestState = DataRequestState.Failed;
                this.Logger.Error("Failed to request data: " + ex);
            }
            finally
            {
                Monitor.Exit(this._syncObject); // Release the lock at the end
                this.OnGetJourneyCompleted(new JourneyEventArgs(this.LastRetrievedRoute, this.RequestState, this.LastRequestInitiatedDate));
                this.IsStopping = false;
            }
        }

        /// <summary>
        /// The on progress changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnProgressChanged(object sender, JourneyProgressChangedEventArgs e)
        {
            if (e.ResultRoute != null)
            {
                this.lvFlightData.SafeInvoke(
                    new Action(
                        () =>
                        {
                            this.BindData(e.ResultRoute);
                            this.lvFlightData.Enabled = e.ProgressPercentage < 100;
                        }));
            }
        }

        /// <summary>
        /// The on get journey completed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnGetJourneyCompleted(JourneyEventArgs e)
        {
            if (this.IsDestructed() || this.LastRequestInitiatedDate != e.RequestInitiatedDate)
            {
                return;
            }

            this.BindData(e.ResultRoute);
            this.lvFlightData.Enabled = true;

            if (!this.IsStopping && this.GetJourneyCompleted != null)
            {
                this.GetJourneyCompleted(this, e);
            }
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        private void BindData(TravelRoute route)
        {
            if (route != null && route.Journeys.Count > 0 && route.Journeys[0].Data.Count > 0)
            {
                DateTime dataDate = DateTime.Now;
                var flights = new List<Flight>();
                var jData = route.Journeys[0].Data;
                foreach (var d in jData)
                {
                    d.DataDate = dataDate;
                    if (d.Flights != null && d.Flights.Count > 0)
                    {
                        flights.AddRange(d.Flights);
                    }
                }

                this.lvFlightData.SetDataSourceAsync(flights, true);
                this.lvFlightData.SetWatermark(null);
            }
            else if (this.RequestState == DataRequestState.NoData)
            {
                this.lvFlightData.SetWatermark(
                    "There is no data for selected journey. Probably there is no flight on that date or there is no flight between two destinations!");
            }
            else if (this.LastException != null)
            {
                this.lvFlightData.SetWatermark("Failed to retrieve data: " + this.LastException.Message);
            }
            else
            {
                this.lvFlightData.SetWatermark("The request was not properly handled or aborted");
            }
        }
    }
}