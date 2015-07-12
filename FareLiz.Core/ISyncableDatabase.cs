using SkyDean.FareLiz.Core.Data;
using System.Collections.Generic;
using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for synchronizable fare database
    /// </summary>
    public interface ISyncableDatabase : IFareDatabase, ISyncable
    {
        /// <summary>
        /// Helper object used for synchronizing data packages from the database
        /// </summary>
        IPackageSyncer<TravelRoute> PackageSynchronizer { get; set; }

        /// <summary>
        /// Receive all new data packages
        /// </summary>
        /// <returns>Number of received packages</returns>
        int ReceivePackages(IProgressCallback callback);

        /// <summary>
        /// Send data packages
        /// </summary>
        /// <param name="data">List of journey data</param>
        /// <returns>Package ID</returns>
        string SendData(IList<TravelRoute> data, IProgressCallback callback);

        /// <summary>
        /// Check if a package with specific ID has been imported
        /// </summary>
        /// <param name="packageId">Package ID</param>
        /// <returns>Package existence in database</returns>
        bool IsPackageImported(string packageId, IProgressCallback callback);

        /// <summary>
        /// Get list of imported packages' ID
        /// </summary>
        IList<string> GetImportedPackages(IProgressCallback callback);

        /// <summary>
        /// Add the list of data as a package
        /// </summary>
        /// <param name="packageId">Target ID for data package</param>
        /// <param name="data">List of data</param>
        void AddPackage(string packageId, IList<TravelRoute> data, IProgressCallback callback);
    }
}