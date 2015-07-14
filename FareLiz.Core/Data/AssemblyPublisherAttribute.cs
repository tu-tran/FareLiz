namespace SkyDean.FareLiz.Core.Data
{
    using System;

    /// <summary>Attribute for storing the publisher information of the assembly</summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyPublisherAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyPublisherAttribute"/> class.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        public AssemblyPublisherAttribute(string url, string email)
        {
            this.PublisherUrl = url;
            this.PublisherEmail = email;
        }

        /// <summary>Web URL of the publisher</summary>
        public string PublisherUrl { get; private set; }

        /// <summary>Contact email of the publisher</summary>
        public string PublisherEmail { get; private set; }
    }
}