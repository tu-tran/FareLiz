using ProtoBuf;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Business object for storing the flight fare data
    /// </summary>
    [DebuggerDisplay("{Operator} - {Price}")]
    [Serializable, ProtoContract]
    public class Flight
    {
        /// <summary>
        /// Inbound leg (return trip)
        /// </summary>
        [ProtoMember(1)]
        public FlightLeg InboundLeg { get; set; }

        /// <summary>
        /// The flight operator (airline) from which the ticket is sold
        /// </summary>
        [ProtoMember(2)]
        public string Operator { get; set; }

        /// <summary>
        /// Outbound leg (departure trip)
        /// </summary>
        [ProtoMember(3)]
        public FlightLeg OutboundLeg { get; set; }

        /// <summary>
        /// Price of the flight (in the currency of the parent JourneyData)
        /// </summary>
        [ProtoMember(4)]
        public float Price { get; set; }

        /// <summary>
        /// The travel agency which sells the ticket
        /// </summary>
        [ProtoMember(5)]
        public TravelAgency TravelAgency { get; set; }

        /// <summary>
        /// The parent JourneyData which contains this flight
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public JourneyData JourneyData { get; set; }

        /// <summary>
        /// Returns a formatted string for the flight dates
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string TravelDateString
        {
            get
            {
                DateTime startTime = OutboundLeg.Departure,
                         endTime = (InboundLeg == null ? DateTime.MinValue : InboundLeg.Departure);
                string result = StringUtil.GetPeriodString(startTime, endTime);
                return result;
            }
        }

        /// <summary>
        /// The total flight duration
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public TimeSpan Duration
        {
            get { return (OutboundLeg == null ? TimeSpan.Zero : OutboundLeg.Duration) + (InboundLeg == null ? TimeSpan.Zero : InboundLeg.Duration); }
        }

        /// <summary>
        /// Returns formatted string which contains all flight details (multiple lines)
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string SummaryString
        {
            get
            {
                string outBound = OutboundLeg == null ? null : Environment.NewLine + "Outbound flight: " + OutboundLeg.DetailString;
                string inBound = InboundLeg == null ? null : Environment.NewLine + "Return flight: " + InboundLeg.DetailString;
                string agency = TravelAgency == null ? null : Environment.NewLine + "Travel agency: " + TravelAgency.Name;
                string result =
"Departure: " + JourneyData.JourneyInfo.Route.Departure + Environment.NewLine +
"Destination: " + JourneyData.JourneyInfo.Route.Destination + Environment.NewLine +
"Travel Period: " + TravelDateString + Environment.NewLine +
"Flight duration: " + Duration.ToHourMinuteString() +
outBound + inBound + agency + Environment.NewLine +
"Data date: " + JourneyData.DataDate + Environment.NewLine +
"Price: " + Price + " " + JourneyData.Currency;
                return result;
            }
        }

        /// <summary>
        /// Returns true if the flight departure date is still in the future
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public bool IsAvailable
        {
            get
            {
                DateTime deptDate = OutboundLeg == null ? DateTime.MinValue : OutboundLeg.Departure;
                return deptDate.Date >= DateTime.Now.Date;
            }
        }

        /// <summary>
        /// Returns true if the flight can be purchased (the flight date is still in the future and there is information on the travel agency)
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public bool CanBePurchased
        {
            get
            {
                bool result = TravelAgency != null && !String.IsNullOrEmpty(TravelAgency.Url) && IsAvailable;
                return result;
            }
        }


        protected Flight() { }

        public Flight(JourneyData baseData, string flightOperator, float price, TravelAgency travelAgency, FlightLeg outboundLeg, FlightLeg inboundLeg)
        {
            JourneyData = baseData;
            Operator = flightOperator;
            Price = price;
            TravelAgency = travelAgency;
            OutboundLeg = outboundLeg;
            InboundLeg = inboundLeg;
        }

        /// <summary>
        /// Check if the flight detail is the same as the other flight (ignore the price and travel agency)
        /// </summary>
        public bool IsSameFlight(Flight otherFLight)
        {
            bool result = String.Equals(Operator, otherFLight.Operator, StringComparison.OrdinalIgnoreCase);    // Operator
            if (result)
            {
                if (result = (OutboundLeg != null && otherFLight.OutboundLeg != null))  // Outbound leg
                {
                    if (result = (OutboundLeg.IsSame(otherFLight.OutboundLeg)))
                        if (result = (InboundLeg != null && otherFLight.InboundLeg != null))    // Inbound leg
                            result = InboundLeg.IsSame(otherFLight.InboundLeg);
                        else
                            result = (InboundLeg == null && otherFLight.InboundLeg == null);
                }
                else
                    result = (OutboundLeg == null && otherFLight.OutboundLeg == null);
            }

            return result;
        }

        public override string ToString()
        {
            return Operator + " - " + Price + JourneyData.Currency + " - " + Duration.ToHourMinuteString() + " [" + OutboundLeg.Departure.ToString(CultureInfo.InvariantCulture) + "-" +
                (InboundLeg == null ? null : InboundLeg.Departure.ToString(CultureInfo.InvariantCulture)) + "]";
        }
    }
}