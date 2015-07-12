using ProtoBuf;
using System;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Business object for storing the travel agency detail
    /// </summary>
    [Serializable, ProtoContract]
    public class TravelAgency
    {
        /// <summary>
        /// Name of the travel agency
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        /// Website URL of the travel agency (or the deeplink for ticket purchase)
        /// </summary>
        [ProtoMember(2)]
        public string Url { get; set; }

        protected TravelAgency() { }

        public TravelAgency(string name)
        {
            Name = name;
        }

        public TravelAgency(string name, string url)
            : this(name)
        {
            Url = url;
        }
    }
}