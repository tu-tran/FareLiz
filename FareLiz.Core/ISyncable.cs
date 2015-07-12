using System.Collections.Generic;
using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for synchronizable objects
    /// </summary>
    public interface ISyncable
    {
        /// <summary>
        /// Synchronize the object
        /// </summary>
        /// <param name="operation">Operation (Upload or Download)</param>
        /// <returns>Success</returns>
        bool Synchronize(SyncOperation operation, IProgressCallback callback);

        /// <summary>
        /// Get the helper object used for synchronizing
        /// </summary>
        IDataSyncer DataSynchronizer { get; set; }
    }

    /// <summary>
    /// Synchronization operation type enumeration
    /// </summary>
    public enum SyncOperation
    {
        Download,
        Upload
    }
}