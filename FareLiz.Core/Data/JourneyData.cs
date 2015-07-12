using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// The business object for storing a historical fare data
    /// </summary>
    [Serializable, ProtoContract]
    [DebuggerDisplay("{Id} - {DataDate}")]
    public class JourneyData
    {
        /// <summary>
        /// Unique ID for journey's fare data
        /// </summary>
        [ProtoMember(1)]
        public long Id { get; set; }

        /// <summary>
        /// The date on which the data was collected
        /// </summary>
        [ProtoMember(2)]
        public DateTime DataDate { get; set; }

        /// <summary>
        /// The list of the flights provided for the journey
        /// </summary>
        [ProtoMember(3)]
        public List<Flight> Flights { get; private set; }

        /// <summary>
        /// The currency of the fare data (mostly 3 letter code). All flights' price belonging to this object has this same currency
        /// </summary>
        [ProtoMember(4)]
        public string Currency { get; set; }

        [XmlIgnore, ProtoIgnore]
        public Journey JourneyInfo { get; set; }

        protected JourneyData()
        {
            Flights = new List<Flight>();
        }

        public JourneyData(long id, string currency, DateTime dataDate)
            : this()
        {
            Id = id;
            Currency = currency;
            DataDate = dataDate;
        }

        /// <summary>
        /// Add flight to the journey's fare data
        /// </summary>
        public void AddFlights(IList<Flight> flights)
        {
            if (flights != null && flights.Count > 0)
            {
                Flights.AddRange(flights);
                SetFlightLinks(flights);
            }
        }

        /// <summary>
        /// Add flight to the journey's fare data
        /// </summary>
        public void AddFlight(Flight flight)
        {
            if (flight != null)
            {
                Flights.Add(flight);
                flight.JourneyData = this;
            }
        }

        [ProtoAfterDeserialization]
        public void SetFlightLinks()
        {
            SetFlightLinks(Flights);
        }

        private void SetFlightLinks(IList<Flight> flights)
        {
            if (flights != null && flights.Count > 0)
                foreach (var f in flights)
                    f.JourneyData = this;
        }
    }
}
