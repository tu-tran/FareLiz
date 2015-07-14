namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using SkyDean.FareLiz.Core;

    /// <summary>Interface for retrieving the distributed versions</summary>
    public interface IVersionRetriever
    {
        /// <summary>Name of product</summary>
        string ProductName { get; }

        /// <summary>Publisher name</summary>
        string PublisherName { get; }

        /// <summary>The logger</summary>
        ILogger Logger { get; set; }

        /// <summary>Get current version of the product (which is running or installed)</summary>
        /// <returns>The <see cref="VersionInfo" />.</returns>
        VersionInfo GetCurrentVersion();

        /// <summary>Get the latest version available</summary>
        /// <returns>The <see cref="VersionInfo" />.</returns>
        VersionInfo GetLatestVersion();

        /// <summary>
        /// Get the change log between 2 specific versions
        /// </summary>
        /// <param name="fromVersion">
        /// The from Version.
        /// </param>
        /// <param name="toVersion">
        /// The to Version.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetChangeLog(VersionInfo fromVersion, VersionInfo toVersion);

        /// <summary>Check for latest update</summary>
        /// <returns>The upgradable version. Returns null if there is no newer version</returns>
        UpdateRequest CheckForUpdate();

        /// <summary>
        /// Download the version update package
        /// </summary>
        /// <param name="request">
        /// The update request information
        /// </param>
        /// <param name="targetLocation">
        /// The location to store the update package
        /// </param>
        void DownloadPackage(UpdateRequest request, string targetLocation);
    }
}