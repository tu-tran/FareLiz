using System;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Attribute for storing the publisher information of the assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyPublisherAttribute : Attribute
    {
        /// <summary>
        /// Web URL of the publisher
        /// </summary>
        public string PublisherUrl { get; private set; }

        /// <summary>
        /// Contact email of the publisher
        /// </summary>
        public string PublisherEmail { get; private set; }

        public AssemblyPublisherAttribute(string url, string email)
        {
            PublisherUrl = url;
            PublisherEmail = email;
        }
    }
}
