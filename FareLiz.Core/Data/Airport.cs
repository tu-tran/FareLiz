namespace SkyDean.FareLiz.Core.Data
{
    using System;
    using System.Xml.Serialization;

    using ProtoBuf;

    /// <summary>Information on the airport</summary>
    [Serializable]
    [ProtoContract]
    public class Airport
    {
        // This constructor is needed for serialization
        /// <summary>Initializes a new instance of the <see cref="Airport" /> class.</summary>
        internal Airport()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Airport"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <param name="country">
        /// The country.
        /// </param>
        /// <param name="iata">
        /// The iata.
        /// </param>
        /// <param name="latitude">
        /// The latitude.
        /// </param>
        /// <param name="longitude">
        /// The longitude.
        /// </param>
        internal Airport(string name, string city, string country, string iata, float latitude, float longitude)
        {
            this.Name = name;
            this.City = city;
            this.Country = country;
            this.IATA = iata;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>IATA code of the airport (3 letter)</summary>
        [ProtoMember(1)]
        public string IATA { get; set; }

        /// <summary>Airport name</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string Name { get; set; }

        /// <summary>City of the airport</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string City { get; set; }

        /// <summary>Country of the airport</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public string Country { get; set; }

        /// <summary>Airport's latitude co-ordinate</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public float Latitude { get; set; }

        /// <summary>Airport's longitude co-ordinate</summary>
        [XmlIgnore]
        [ProtoIgnore]
        public float Longitude { get; set; }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format("{0}, {1} ({2}) {3}", this.City, this.Name, this.Country, this.IATA);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var castObject = obj as Airport;
            if (castObject == null)
            {
                return false;
            }

            return string.Compare(this.IATA, castObject.IATA, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>The get hash code.</summary>
        /// <returns>The <see cref="int" />.</returns>
        public override int GetHashCode()
        {
            return this.IATA == null ? base.GetHashCode() : this.IATA.GetHashCode();
        }
    }
}