namespace SkyDean.FareLiz.Core
{
    using System;

    /// <summary>
    /// The sync event args.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    public class SyncEventArgs<T> : EventArgs
        where T : ISyncable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncEventArgs{T}"/> class.
        /// </summary>
        /// <param name="syncer">
        /// The syncer.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        public SyncEventArgs(IDataSyncer<T> syncer, object data)
        {
            this.Syncer = syncer;
            this.Data = data;
        }

        /// <summary>Gets or sets the syncer.</summary>
        public IDataSyncer<T> Syncer { get; set; }

        /// <summary>Gets or sets the data.</summary>
        public object Data { get; set; }
    }
}