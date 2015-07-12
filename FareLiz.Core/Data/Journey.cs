using ProtoBuf;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Business object for representing a flight journey
    /// A journey may contain multiple historical data (whereas the actual flights are stored)
    /// </summary>
    [Serializable, ProtoContract]
    public class Journey
    {
        /// <summary>
        /// Unique ID for the journey
        /// </summary>
        [ProtoMember(1)]
        public long Id { get; set; }

        /// <summary>
        /// Journey departure date
        /// </summary>
        [ProtoMember(2)]
        public DateTime DepartureDate { get; set; }

        /// <summary>
        /// Journey return date (for round-trip)
        /// </summary>
        [ProtoMember(3)]
        public DateTime ReturnDate { get; set; }

        /// <summary>
        /// Fare data history for the journeys
        /// </summary>
        [ProtoMember(4)]
        public List<JourneyData> Data { get; private set; }

        /// <summary>
        /// The parent route assosiated with the journey
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public TravelRoute Route { get; set; }

        /// <summary>
        /// IATA code of the departure airport
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string DepartureCode { get { return Route == null ? null : (Route.Departure == null ? null : Route.Departure.IATA); } }

        /// <summary>
        /// IATA code of the destination airport
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string DestinationCode { get { return Route == null ? null : (Route.Destination == null ? null : Route.Destination.IATA); } }

        /// <summary>
        /// Total stay duration in days (for round trip). For one-way trip, this property returns 0
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public int StayDuration
        {
            get
            {
                if (ReturnDate.IsUndefined() || DepartureDate.IsUndefined())
                    return 0;

                return (int)(ReturnDate - DepartureDate).TotalDays;
            }
        }

        public Journey()
        {
            Data = new List<JourneyData>();
        }

        public Journey(long id, TravelRoute route, DateTime departureDate, DateTime returnDate)
            : this(route)
        {
            Id = id;
            DepartureDate = departureDate;
            ReturnDate = returnDate;
        }

        public Journey(TravelRoute route)
            : this()
        {
            Route = route;
        }

        /// <summary>
        /// Add fare data for the journey
        /// </summary>
        public void AddData(IList<JourneyData> data)
        {
            if (data != null && data.Count > 0)
            {
                Data.AddRange(data);
                SetJourneyDataLinks(data);
            }
        }

        /// <summary>
        /// Add fare data for the journey
        /// </summary>
        public void AddData(JourneyData data)
        {
            if (data != null)
            {
                Data.Add(data);
                data.JourneyInfo = this;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} - [{1}-{2}] {3}", Id, DepartureCode, DestinationCode, StringUtil.GetPeriodString(DepartureDate, ReturnDate));
        }

        /// <summary>
        /// Check that two journeys are the same (same departure, destination and travel dates)
        /// </summary>
        public bool IsSameTrip(Journey other)
        {
            return (String.Equals(DepartureCode, other.DepartureCode, StringComparison.OrdinalIgnoreCase) &&
                    String.Equals(DestinationCode, other.DestinationCode, StringComparison.OrdinalIgnoreCase) &&
                    DepartureDate == other.DepartureDate && ReturnDate == other.ReturnDate);
        }

        [ProtoAfterDeserialization]
        public void SetJourneyDataLinks()
        {
            SetJourneyDataLinks(Data);
        }

        private void SetJourneyDataLinks(IList<JourneyData> data)
        {
            if (data != null && data.Count > 0)
                foreach (var d in data)
                    d.JourneyInfo = this;
        }
    }
}
