namespace SkyDean.FareLiz.Core.Data
{
    using System;
    using System.Globalization;
    using System.Xml.Serialization;

    using ProtoBuf;

    /// <summary>Business object for storing the flight leg detail</summary>
    [Serializable]
    [ProtoContract]
    public class FlightLeg
    {
        /// <summary>Initializes a new instance of the <see cref="FlightLeg" /> class.</summary>
        protected FlightLeg()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightLeg"/> class.
        /// </summary>
        /// <param name="departureTime">
        /// The departure time.
        /// </param>
        /// <param name="arrivalTime">
        /// The arrival time.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        /// <param name="transit">
        /// The transit.
        /// </param>
        public FlightLeg(DateTime departureTime, DateTime arrivalTime, TimeSpan duration, int transit)
        {
            this.Departure = departureTime;
            this.Arrival = arrivalTime;
            this.Duration = duration;
            this.Transit = transit;
        }

        /// <summary>Arrival time of the flight leg (in local time of the destination airport)</summary>
        [ProtoMember(1)]
        public DateTime Arrival { get; set; }

        /// <summary>Depature time of the flight leg (in local time of the departure airport)</summary>
        [ProtoMember(2)]
        public DateTime Departure { get; set; }

        /// <summary>Total flight duration</summary>
        [ProtoMember(3)]
        public TimeSpan Duration { get; set; }

        /// <summary>Total number of transits</summary>
        [ProtoMember(5)]
        public int Transit { get; set; }

        /// <summary>Returns a formatted string for the flight leg detail</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string DetailString
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0}h - {1}", 
                    this.Duration, 
                    this.Transit < 1 ? "No transit" : this.Transit + " transit" + (this.Transit > 1 ? "s" : null));
            }
        }

        /// <summary>
        /// Returns true if the flight leg has the same departure time, arrival time, total duration and number of transits
        /// </summary>
        /// <param name="otherLeg">
        /// The other Leg.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSame(FlightLeg otherLeg)
        {
            bool result;
            if (result = otherLeg != null)
            {
                if (result = this.Departure == otherLeg.Departure)
                {
                    if (result = this.Arrival == otherLeg.Arrival)
                    {
                        if (result = this.Duration == otherLeg.Duration)
                        {
                            result = this.Transit == otherLeg.Transit;
                        }
                    }
                }
            }

            return result;
        }
    }
}