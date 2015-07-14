namespace SkyDean.FareLiz.Core
{
    using System;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>A new request to scan for air fare</summary>
    public class FlightFareRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlightFareRequest"/> class.
        /// </summary>
        /// <param name="departure">
        /// The departure.
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
        /// <exception cref="ArgumentException">
        /// </exception>
        public FlightFareRequest(Airport departure, Airport destination, DateTime departureDate, DateTime returnDate)
        {
            if (departure == null)
            {
                throw new ArgumentException("Departure cannot be null");
            }

            if (destination == null)
            {
                throw new ArgumentException("Destination cannot be null");
            }

            if (departureDate.IsUndefined())
            {
                throw new ArgumentException("Departure date is not defined");
            }

            this.Departure = departure;
            this.Destination = destination;
            this.DepartureDate = departureDate;
            this.ReturnDate = returnDate;
        }

        /// <summary>Departure airport</summary>
        public Airport Departure { get; private set; }

        /// <summary>Destination airport</summary>
        public Airport Destination { get; private set; }

        /// <summary>Departure date</summary>
        public DateTime DepartureDate { get; private set; }

        /// <summary>Return date</summary>
        public DateTime ReturnDate { get; private set; }

        /// <summary>Is the request for round trip</summary>
        public bool IsRoundTrip
        {
            get
            {
                return this.ReturnDate.IsDefined();
            }
        }

        /// <summary>The time when the request was started</summary>
        public DateTime StartedDate { get; set; }

        /// <summary>The stay duration of the journey</summary>
        public TimeSpan Duration
        {
            get
            {
                return this.StartedDate.IsDefined() ? (DateTime.Now - this.StartedDate) : TimeSpan.Zero;
            }
        }

        /// <summary>Returns a formatted string of all the request details (multiline)</summary>
        public string Detail
        {
            get
            {
                var result = "Departure: " + this.Departure + Environment.NewLine + "Destination: " + this.Destination + Environment.NewLine
                             + "Departure Date: " + this.DepartureDate.ToShortDayAndDateString() + Environment.NewLine
                             + (this.IsRoundTrip
                                    ? "Return Date: " + this.ReturnDate.ToShortDayAndDateString() + Environment.NewLine + "Stay duration: "
                                      + (this.ReturnDate - this.DepartureDate).TotalDays + " days"
                                    : string.Empty);
                return result;
            }
        }

        /// <summary>Returns a short description of the request in a single line</summary>
        public string ShortDetail
        {
            get
            {
                var result = string.Format(
                    "[{0}-{1}] - [{2}]", 
                    this.Departure.IATA, 
                    this.Destination.IATA, 
                    StringUtil.GetPeriodString(this.DepartureDate, this.ReturnDate));
                return result;
            }
        }

        /// <summary>
        /// Check if two requests are similar (same location and same travel time)
        /// </summary>
        /// <param name="other">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSame(FlightFareRequest other)
        {
            bool result;
            if (other == null)
            {
                result = false;
            }
            else
            {
                if (result = string.Equals(this.Departure.IATA, other.Departure.IATA, StringComparison.OrdinalIgnoreCase))
                {
                    if (result = string.Equals(this.Destination.IATA, other.Destination.IATA, StringComparison.OrdinalIgnoreCase))
                    {
                        if (result = this.DepartureDate == other.DepartureDate)
                        {
                            result = this.ReturnDate == other.ReturnDate;
                        }
                    }
                }
            }

            return result;
        }
    }
}