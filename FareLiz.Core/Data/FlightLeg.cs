using ProtoBuf;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Business object for storing the flight leg detail
    /// </summary>
    [Serializable, ProtoContract]
    public class FlightLeg
    {
        /// <summary>
        /// Arrival time of the flight leg (in local time of the destination airport)
        /// </summary>
        [ProtoMember(1)]
        public DateTime Arrival { get; set; }

        /// <summary>
        /// Depature time of the flight leg (in local time of the departure airport)
        /// </summary>
        [ProtoMember(2)]
        public DateTime Departure { get; set; }

        /// <summary>
        /// Total flight duration
        /// </summary>
        [ProtoMember(3)]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Total number of transits
        /// </summary>
        [ProtoMember(5)]
        public int Transit { get; set; }

        /// <summary>
        /// Returns a formatted string for the flight leg detail
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string DetailString
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture, "{0}h - {1}", Duration, Transit < 1 ? "No transit" : Transit + " transit" + (Transit > 1 ? "s" : null));
            }
        }

        protected FlightLeg() { }

        public FlightLeg(DateTime departureTime, DateTime arrivalTime, TimeSpan duration, int transit)
        {
            Departure = departureTime;
            Arrival = arrivalTime;
            Duration = duration;
            Transit = transit;
        }

        /// <summary>
        /// Returns true if the flight leg has the same departure time, arrival time, total duration and number of transits
        /// </summary>
        public bool IsSame(FlightLeg otherLeg)
        {
            bool result;
            if (result = (otherLeg != null))
                if (result = (Departure == otherLeg.Departure))
                    if (result = (Arrival == otherLeg.Arrival))
                        if (result = (Duration == otherLeg.Duration))
                            result = (Transit == otherLeg.Transit);

            return result;
        }
    }
}