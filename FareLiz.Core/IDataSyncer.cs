namespace SkyDean.FareLiz.Core
{
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>Interface for helper objects which are used to synchronize an object (e.g. with other services)</summary>
    public interface IDataSyncer : IPlugin
    {
        /// <summary>Target object to be synchronized</summary>
        ISyncable SyncTargetObject { get; set; }

        /// <summary>
        /// Synchronize the selected data of the target object
        /// </summary>
        /// <param name="operation">
        /// Synchronization operation
        /// </param>
        /// <param name="data">
        /// Object data to be synchronize
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// Success
        /// </returns>
        bool Synchronize(SyncOperation operation, object data, IProgressCallback callback);
    }

    /// <summary>
    /// Generic interface for IDataSyncer
    /// </summary>
    /// <typeparam name="T">
    /// Synchronizable type
    /// </typeparam>
    public interface IDataSyncer<T> : IDataSyncer
        where T : ISyncable
    {
        /// <summary>The on validate data.</summary>
        event SyncEventHandler<T> OnValidateData;
    }

    /// <summary>
    /// The sync event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <typeparam name="T">
    /// </typeparam>
    public delegate void SyncEventHandler<T>(T sender, SyncEventArgs<T> e) where T : ISyncable;
}