namespace SkyDean.FareLiz.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using ProtoBuf;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Business object for representing a flight journey A journey may contain multiple historical data (whereas the actual flights are stored)</summary>
    [Serializable]
    [ProtoContract]
    public class Journey
    {
        /// <summary>Initializes a new instance of the <see cref="Journey" /> class.</summary>
        public Journey()
        {
            this.Data = new List<JourneyData>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Journey"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="departureDate">
        /// The departure date.
        /// </param>
        /// <param name="returnDate">
        /// The return date.
        /// </param>
        public Journey(long id, TravelRoute route, DateTime departureDate, DateTime returnDate)
            : this(route)
        {
            this.Id = id;
            this.DepartureDate = departureDate;
            this.ReturnDate = returnDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Journey"/> class.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        public Journey(TravelRoute route)
            : this()
        {
            this.Route = route;
        }

        /// <summary>Unique ID for the journey</summary>
        [ProtoMember(1)]
        public long Id { get; set; }

        /// <summary>Journey departure date</summary>
        [ProtoMember(2)]
        public DateTime DepartureDate { get; set; }

        /// <summary>Journey return date (for round-trip)</summary>
        [ProtoMember(3)]
        public DateTime ReturnDate { get; set; }

        /// <summary>Fare data history for the journeys</summary>
        [ProtoMember(4)]
        public List<JourneyData> Data { get; private set; }

        /// <summary>The parent route assosiated with the journey</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public TravelRoute Route { get; set; }

        /// <summary>IATA code of the departure airport</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string DepartureCode
        {
            get
            {
                return this.Route == null ? null : (this.Route.Departure == null ? null : this.Route.Departure.IATA);
            }
        }

        /// <summary>IATA code of the destination airport</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string DestinationCode
        {
            get
            {
                return this.Route == null ? null : (this.Route.Destination == null ? null : this.Route.Destination.IATA);
            }
        }

        /// <summary>Total stay duration in days (for round trip). For one-way trip, this property returns 0</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public int StayDuration
        {
            get
            {
                if (this.ReturnDate.IsUndefined() || this.DepartureDate.IsUndefined())
                {
                    return 0;
                }

                return (int)(this.ReturnDate - this.DepartureDate).TotalDays;
            }
        }

        /// <summary>
        /// Add fare data for the journey
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void AddData(IList<JourneyData> data)
        {
            if (data != null && data.Count > 0)
            {
                this.Data.AddRange(data);
                this.SetJourneyDataLinks(data);
            }
        }

        /// <summary>
        /// Add fare data for the journey
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public void AddData(JourneyData data)
        {
            if (data != null)
            {
                this.Data.Add(data);
                data.JourneyInfo = this;
            }
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format(
                "{0} - [{1}-{2}] {3}", 
                this.Id, 
                this.DepartureCode, 
                this.DestinationCode, 
                StringUtil.GetPeriodString(this.DepartureDate, this.ReturnDate));
        }

        /// <summary>
        /// Check that two journeys are the same (same departure, destination and travel dates)
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSameTrip(Journey other)
        {
            return string.Equals(this.DepartureCode, other.DepartureCode, StringComparison.OrdinalIgnoreCase)
                   && string.Equals(this.DestinationCode, other.DestinationCode, StringComparison.OrdinalIgnoreCase)
                   && this.DepartureDate == other.DepartureDate && this.ReturnDate == other.ReturnDate;
        }

        /// <summary>The set journey data links.</summary>
        [ProtoAfterDeserialization]
        public void SetJourneyDataLinks()
        {
            this.SetJourneyDataLinks(this.Data);
        }

        /// <summary>
        /// The set journey data links.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        private void SetJourneyDataLinks(IList<JourneyData> data)
        {
            if (data != null && data.Count > 0)
            {
                foreach (var d in data)
                {
                    d.JourneyInfo = this;
                }
            }
        }
    }
}