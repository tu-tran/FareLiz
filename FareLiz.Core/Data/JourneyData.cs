namespace SkyDean.FareLiz.Core.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml.Serialization;

    using ProtoBuf;

    /// <summary>The business object for storing a historical fare data</summary>
    [Serializable]
    [ProtoContract]
    [DebuggerDisplay("{Id} - {DataDate}")]
    public class JourneyData
    {
        /// <summary>Initializes a new instance of the <see cref="JourneyData" /> class.</summary>
        protected JourneyData()
        {
            this.Flights = new List<Flight>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JourneyData"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="currency">
        /// The currency.
        /// </param>
        /// <param name="dataDate">
        /// The data date.
        /// </param>
        public JourneyData(long id, string currency, DateTime dataDate)
            : this()
        {
            this.Id = id;
            this.Currency = currency;
            this.DataDate = dataDate;
        }

        /// <summary>Unique ID for journey's fare data</summary>
        [ProtoMember(1)]
        public long Id { get; set; }

        /// <summary>The date on which the data was collected</summary>
        [ProtoMember(2)]
        public DateTime DataDate { get; set; }

        /// <summary>The list of the flights provided for the journey</summary>
        [ProtoMember(3)]
        public List<Flight> Flights { get; private set; }

        /// <summary>The currency of the fare data (mostly 3 letter code). All flights' price belonging to this object has this same currency</summary>
        [ProtoMember(4)]
        public string Currency { get; set; }

        /// <summary>Gets or sets the journey info.</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public Journey JourneyInfo { get; set; }

        /// <summary>
        /// Add flight to the journey's fare data
        /// </summary>
        /// <param name="flights">
        /// The flights.
        /// </param>
        public void AddFlights(IList<Flight> flights)
        {
            if (flights != null && flights.Count > 0)
            {
                this.Flights.AddRange(flights);
                this.SetFlightLinks(flights);
            }
        }

        /// <summary>
        /// Add flight to the journey's fare data
        /// </summary>
        /// <param name="flight">
        /// The flight.
        /// </param>
        public void AddFlight(Flight flight)
        {
            if (flight != null)
            {
                this.Flights.Add(flight);
                flight.JourneyData = this;
            }
        }

        /// <summary>The set flight links.</summary>
        [ProtoAfterDeserialization]
        public void SetFlightLinks()
        {
            this.SetFlightLinks(this.Flights);
        }

        /// <summary>
        /// The set flight links.
        /// </summary>
        /// <param name="flights">
        /// The flights.
        /// </param>
        private void SetFlightLinks(IList<Flight> flights)
        {
            if (flights != null && flights.Count > 0)
            {
                foreach (var f in flights)
                {
                    f.JourneyData = this;
                }
            }
        }
    }
}