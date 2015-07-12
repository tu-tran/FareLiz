using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Utils;
using System;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// A new request to scan for air fare
    /// </summary>
    public class FlightFareRequest
    {
        /// <summary>
        /// Departure airport
        /// </summary>
        public Airport Departure { get; private set; }

        /// <summary>
        /// Destination airport
        /// </summary>
        public Airport Destination { get; private set; }

        /// <summary>
        /// Departure date
        /// </summary>
        public DateTime DepartureDate { get; private set; }

        /// <summary>
        /// Return date
        /// </summary>
        public DateTime ReturnDate { get; private set; }

        /// <summary>
        /// Is the request for round trip
        /// </summary>
        public bool IsRoundTrip { get { return ReturnDate.IsDefined(); } }

        /// <summary>
        /// The time when the request was started
        /// </summary>
        public DateTime StartedDate { get; set; }

        /// <summary>
        /// The stay duration of the journey
        /// </summary>
        public TimeSpan Duration { get { return StartedDate.IsDefined() ? (DateTime.Now - StartedDate) : TimeSpan.Zero; } }

        public FlightFareRequest(Airport departure, Airport destination, DateTime departureDate, DateTime returnDate)
        {
            if (departure == null)
                throw new ArgumentException("Departure cannot be null");
            if (destination == null)
                throw new ArgumentException("Destination cannot be null");
            if (departureDate.IsUndefined())
                throw new ArgumentException("Departure date is not defined");

            Departure = departure;
            Destination = destination;
            DepartureDate = departureDate;
            ReturnDate = returnDate;
        }

        /// <summary>
        /// Check if two requests are similar (same location and same travel time)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsSame(FlightFareRequest other)
        {
            bool result;
            if (other == null)
                result = false;
            else
            {
                if (result = String.Equals(Departure.IATA, other.Departure.IATA, StringComparison.OrdinalIgnoreCase))
                    if (result = String.Equals(Destination.IATA, other.Destination.IATA, StringComparison.OrdinalIgnoreCase))
                        if (result = (DepartureDate == other.DepartureDate))
                            result = (ReturnDate == other.ReturnDate);
            }

            return result;
        }

        /// <summary>
        /// Returns a formatted string of all the request details (multiline)
        /// </summary>
        public string Detail
        {
            get
            {
                string result = "Departure: " + Departure + Environment.NewLine +
                                "Destination: " + Destination + Environment.NewLine +
                                "Departure Date: " + DepartureDate.ToShortDayAndDateString() + Environment.NewLine +
                                (IsRoundTrip ? "Return Date: " + ReturnDate.ToShortDayAndDateString() + Environment.NewLine
                                             + "Stay duration: " + (ReturnDate - DepartureDate).TotalDays + " days" : "");
                return result;
            }
        }

        /// <summary>
        /// Returns a short description of the request in a single line
        /// </summary>
        public string ShortDetail
        {
            get
            {
                string result = String.Format("[{0}-{1}] - [{2}]", Departure.IATA, Destination.IATA, StringUtil.GetPeriodString(DepartureDate, ReturnDate));
                return result;
            }
        }
    }
}
