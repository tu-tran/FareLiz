namespace SkyDean.FareLiz.Core
{
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>Interface for synchronizable objects</summary>
    public interface ISyncable
    {
        /// <summary>Get the helper object used for synchronizing</summary>
        IDataSyncer DataSynchronizer { get; set; }

        /// <summary>
        /// Synchronize the object
        /// </summary>
        /// <param name="operation">
        /// Operation (Upload or Download)
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// Success
        /// </returns>
        bool Synchronize(SyncOperation operation, IProgressCallback callback);
    }

    /// <summary>Synchronization operation type enumeration</summary>
    public enum SyncOperation
    {
        /// <summary>The download.</summary>
        Download, 

        /// <summary>The upload.</summary>
        Upload
    }
}