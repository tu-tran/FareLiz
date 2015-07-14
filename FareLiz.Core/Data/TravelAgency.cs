namespace SkyDean.FareLiz.Core.Data
{
    using System;

    using ProtoBuf;

    /// <summary>Business object for storing the travel agency detail</summary>
    [Serializable]
    [ProtoContract]
    public class TravelAgency
    {
        /// <summary>Initializes a new instance of the <see cref="TravelAgency" /> class.</summary>
        protected TravelAgency()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TravelAgency"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public TravelAgency(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TravelAgency"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        public TravelAgency(string name, string url)
            : this(name)
        {
            this.Url = url;
        }

        /// <summary>Name of the travel agency</summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>Website URL of the travel agency (or the deeplink for ticket purchase)</summary>
        [ProtoMember(2)]
        public string Url { get; set; }
    }
}