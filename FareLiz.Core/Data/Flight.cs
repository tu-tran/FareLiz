namespace SkyDean.FareLiz.Core.Data
{
    using ProtoBuf;
    using SkyDean.FareLiz.Core.Utils;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Serialization;


    /// <summary>
    /// The fare data.
    /// </summary>
    [DebuggerDisplay("{TravelAgency} - {Price}")]
    [Serializable]
    [ProtoContract]
    public class Fare
    {
        /// <summary>
        /// Gets or sets the agency.
        /// </summary>
        public TravelAgency TravelAgency { get; set; }

        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        public double Price { get; set; }
    }

    /// <summary>Business object for storing the flight fare data</summary>
    [DebuggerDisplay("{Operator} - {Price}")]
    [Serializable]
    [ProtoContract]
    public class Flight
    {
        /// <summary>Initializes a new instance of the <see cref="Flight" /> class.</summary>
        protected Flight()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Flight"/> class.
        /// </summary>
        /// <param name="baseData">
        /// The base data.
        /// </param>
        /// <param name="flightOperator">
        /// The flight operator.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <param name="travelAgency">
        /// The travel agency.
        /// </param>
        /// <param name="outboundLeg">
        /// The outbound leg.
        /// </param>
        /// <param name="inboundLeg">
        /// The inbound leg.
        /// </param>
        public Flight(
            JourneyData baseData,
            string flightOperator,
            FlightLeg outboundLeg,
            FlightLeg inboundLeg)
        {
            this.JourneyData = baseData;
            this.Operator = flightOperator;
            this.OutboundLeg = outboundLeg;
            this.InboundLeg = inboundLeg;
            this.Fares = new List<Fare>();
        }

        /// <summary>Inbound leg (return trip)</summary>
        [ProtoMember(1)]
        public FlightLeg InboundLeg { get; set; }

        /// <summary>The flight operator (airline) from which the ticket is sold</summary>
        [ProtoMember(2)]
        public string Operator { get; set; }

        /// <summary>Outbound leg (departure trip)</summary>
        [ProtoMember(3)]
        public FlightLeg OutboundLeg { get; set; }

        /// <summary>The travel agency which sells the ticket</summary>
        [ProtoMember(4)]
        public List<Fare> Fares { get; set; }

        /// <summary>The parent JourneyData which contains this flight</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public JourneyData JourneyData { get; set; }

        /// <summary>Returns a formatted string for the flight dates</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public double Price
        {
            get { return this.Fares == null ? 0.0 : this.Fares.Min(f => f.Price); }
        }

        /// <summary>Returns a formatted string for the flight dates</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public TravelAgency TravelAgency
        {
            get { return this.Fares == null || this.Fares.Count < 1 ? null : this.Fares.First().TravelAgency; }
        }

        /// <summary>Returns a formatted string for the flight dates</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string TravelDateString
        {
            get
            {
                DateTime startTime = this.OutboundLeg.Departure, endTime = this.InboundLeg == null ? DateTime.MinValue : this.InboundLeg.Departure;
                string result = StringUtil.GetPeriodString(startTime, endTime);
                return result;
            }
        }

        /// <summary>The total flight duration</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public TimeSpan Duration
        {
            get
            {
                return (this.OutboundLeg == null ? TimeSpan.Zero : this.OutboundLeg.Duration)
                       + (this.InboundLeg == null ? TimeSpan.Zero : this.InboundLeg.Duration);
            }
        }

        /// <summary>Returns formatted string which contains all flight details (multiple lines)</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string SummaryString
        {
            get
            {
                string outBound = this.OutboundLeg == null ? null : Environment.NewLine + "Outbound flight: " + this.OutboundLeg.DetailString;
                string inBound = this.InboundLeg == null ? null : Environment.NewLine + "Return flight: " + this.InboundLeg.DetailString;
                string result = "Departure: " + this.JourneyData.JourneyInfo.Route.Departure + Environment.NewLine + "Destination: "
                                + this.JourneyData.JourneyInfo.Route.Destination + Environment.NewLine + "Travel Period: " + this.TravelDateString
                                + Environment.NewLine + "Flight duration: " + this.Duration.ToHourMinuteString() + outBound + inBound
                                + Environment.NewLine + "Data date: " + this.JourneyData.DataDate + " "
                                + this.JourneyData.Currency;
                return result;
            }
        }

        /// <summary>Returns true if the flight departure date is still in the future</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public bool IsAvailable
        {
            get
            {
                DateTime deptDate = this.OutboundLeg == null ? DateTime.MinValue : this.OutboundLeg.Departure;
                return deptDate.Date >= DateTime.Now.Date;
            }
        }

        /// <summary>Returns true if the flight can be purchased (the flight date is still in the future and there is information on the travel agency)</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public bool CanBePurchased
        {
            get
            {
                bool result = this.Price > 0.0 && this.IsAvailable;
                return result;
            }
        }

        /// <summary>
        /// Check if the flight detail is the same as the other flight (ignore the price and travel agency)
        /// </summary>
        /// <param name="otherFLight">
        /// The other F Light.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsSameFlight(Flight otherFLight)
        {
            bool result = string.Equals(this.Operator, otherFLight.Operator, StringComparison.OrdinalIgnoreCase); // Operator
            if (result)
            {
                if (result = this.OutboundLeg != null && otherFLight.OutboundLeg != null)
                {
                    // Outbound leg
                    if (result = this.OutboundLeg.IsSame(otherFLight.OutboundLeg))
                    {
                        if (result = this.InboundLeg != null && otherFLight.InboundLeg != null)
                        {
                            // Inbound leg
                            result = this.InboundLeg.IsSame(otherFLight.InboundLeg);
                        }
                        else
                        {
                            result = this.InboundLeg == null && otherFLight.InboundLeg == null;
                        }
                    }
                }
                else
                {
                    result = this.OutboundLeg == null && otherFLight.OutboundLeg == null;
                }
            }

            return result;
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return this.Operator + " - " + this.JourneyData.Currency + " - " + this.Duration.ToHourMinuteString() + " ["
                   + this.OutboundLeg.Departure.ToString(CultureInfo.InvariantCulture) + "-"
                   + (this.InboundLeg == null ? null : this.InboundLeg.Departure.ToString(CultureInfo.InvariantCulture)) + "]";
        }
    }
}