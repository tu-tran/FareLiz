namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>A request to retrieve fare data using FareBrowserControl</summary>
    public class FareMonitorRequest : FlightFareRequest
    {
        /// <summary>
        /// The _owner monitor.
        /// </summary>
        private FareRequestMonitor _ownerMonitor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FareMonitorRequest"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public FareMonitorRequest(FareMonitorRequest request)
            : this(request.Departure, request.Destination, request.DepartureDate, request.ReturnDate)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FareMonitorRequest"/> class.
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
        public FareMonitorRequest(Airport origin, Airport destination, DateTime departureDate, DateTime returnDate)
            : base(origin, destination, departureDate, returnDate)
        {
        }

        /// <summary>
        /// Gets or sets the owner monitor.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        public FareRequestMonitor OwnerMonitor
        {
            get
            {
                return this._ownerMonitor;
            }

            set
            {
                if (this._ownerMonitor != null && this._ownerMonitor != value)
                {
                    throw new ArgumentException("Cannot assign the request to another monitor");
                }

                this._ownerMonitor = value;
            }
        }

        /// <summary>
        /// Gets the browser control.
        /// </summary>
        public IFareBrowserControl BrowserControl { get; private set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            if (this.BrowserControl == null)
            {
                this.BrowserControl = this.OwnerMonitor.GetBrowserControl();
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            this.StartedDate = DateTime.Now;
            this.BrowserControl.RequestDataAsync(this);
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            if (this.BrowserControl != null)
            {
                this.BrowserControl.Stop();
            }
        }

        /// <summary>
        /// The reset.
        /// </summary>
        public void Reset()
        {
            this.StartedDate = DateTime.MinValue;
            if (this.BrowserControl != null)
            {
                this.BrowserControl.Reset();
            }
        }
    }
}