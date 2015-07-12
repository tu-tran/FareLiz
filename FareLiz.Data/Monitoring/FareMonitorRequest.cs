using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;

    /// <summary>
    /// A request to retrieve fare data using FareBrowserControl
    /// </summary>
    public class FareMonitorRequest : FlightFareRequest
    {
        private FareRequestMonitor _ownerMonitor = null;
        public FareRequestMonitor OwnerMonitor
        {
            get { return this._ownerMonitor; }
            set
            {
                if (this._ownerMonitor != null && this._ownerMonitor != value)
                    throw new ArgumentException("Cannot assign the request to another monitor");
                this._ownerMonitor = value;
            }
        }

        public IFareBrowserControl BrowserControl { get; private set; }

        public FareMonitorRequest(FareMonitorRequest request) :
            this(request.Departure, request.Destination, request.DepartureDate, request.ReturnDate) { }

        public FareMonitorRequest(Airport origin, Airport destination, DateTime departureDate, DateTime returnDate)
            : base(origin, destination, departureDate, returnDate)
        {
        }

        public void Initialize()
        {
            if (this.BrowserControl == null)
                this.BrowserControl = this.OwnerMonitor.GetBrowserControl();
        }

        public void Start()
        {
            this.StartedDate = DateTime.Now;
            this.BrowserControl.RequestDataAsync(this);
        }

        public void Stop()
        {
            if (this.BrowserControl != null)
                this.BrowserControl.Stop();
        }

        public void Reset()
        {
            this.StartedDate = DateTime.MinValue;
            if (this.BrowserControl != null)
                this.BrowserControl.Reset();
        }
    }
}
