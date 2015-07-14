namespace SkyDean.FareLiz.Service.LiveUpdate
{
    /// <summary>The update request.</summary>
    public class UpdateRequest
    {
        /// <summary>The from version.</summary>
        public readonly VersionInfo FromVersion;

        /// <summary>The product name.</summary>
        public readonly string ProductName;

        /// <summary>The to version.</summary>
        public readonly VersionInfo ToVersion;

        /// <summary>The parameters.</summary>
        public UpdateParameter Parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRequest"/> class.
        /// </summary>
        /// <param name="productName">
        /// The product name.
        /// </param>
        /// <param name="fromVersion">
        /// The from version.
        /// </param>
        /// <param name="toVersion">
        /// The to version.
        /// </param>
        public UpdateRequest(string productName, VersionInfo fromVersion, VersionInfo toVersion)
        {
            this.ProductName = productName;
            this.FromVersion = fromVersion;
            this.ToVersion = toVersion;
        }
    }
}