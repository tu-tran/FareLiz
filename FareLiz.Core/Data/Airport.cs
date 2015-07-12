using ProtoBuf;
using System;
using System.Xml.Serialization;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Information on the airport
    /// </summary>
    [Serializable, ProtoContract]
    public class Airport
    {
        /// <summary>
        /// IATA code of the airport (3 letter)
        /// </summary>
        [ProtoMember(1)]
        public string IATA { get; set; }

        /// <summary>
        /// Airport name
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string Name { get; set; }

        /// <summary>
        /// City of the airport
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string City { get; set; }

        /// <summary>
        /// Country of the airport
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public string Country { get; set; }

        /// <summary>
        /// Airport's latitude co-ordinate
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public float Latitude { get; set; }

        /// <summary>
        /// Airport's longitude co-ordinate
        /// </summary>
        [XmlIgnore, ProtoIgnore]
        public float Longitude { get; set; }

        // This constructor is needed for serialization
        internal Airport() { }

        internal Airport(string name, string city, string country, string iata, float latitude, float longitude)
        {
            Name = name;
            City = city;
            Country = country;
            IATA = iata;
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1} ({2}) {3}", City, Name, Country, IATA);
        }

        public override bool Equals(object obj)
        {
            var castObject = obj as Airport;
            if (castObject == null)
            {
                return false;
            }

            return String.Compare(IATA, castObject.IATA, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
