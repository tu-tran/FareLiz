namespace SkyDean.FareLiz.Core
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>Interface for synchronizable fare database</summary>
    public interface ISyncableDatabase : IFareDatabase, ISyncable
    {
        /// <summary>Helper object used for synchronizing data packages from the database</summary>
        IPackageSyncer<TravelRoute> PackageSynchronizer { get; set; }

        /// <summary>
        /// Receive all new data packages
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// Number of received packages
        /// </returns>
        int ReceivePackages(IProgressCallback callback);

        /// <summary>
        /// Send data packages
        /// </summary>
        /// <param name="data">
        /// List of journey data
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// Package ID
        /// </returns>
        string SendData(IList<TravelRoute> data, IProgressCallback callback);

        /// <summary>
        /// Check if a package with specific ID has been imported
        /// </summary>
        /// <param name="packageId">
        /// Package ID
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// Package existence in database
        /// </returns>
        bool IsPackageImported(string packageId, IProgressCallback callback);

        /// <summary>
        /// Get list of imported packages' ID
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        IList<string> GetImportedPackages(IProgressCallback callback);

        /// <summary>
        /// Add the list of data as a package
        /// </summary>
        /// <param name="packageId">
        /// Target ID for data package
        /// </param>
        /// <param name="data">
        /// List of data
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        void AddPackage(string packageId, IList<TravelRoute> data, IProgressCallback callback);
    }
}