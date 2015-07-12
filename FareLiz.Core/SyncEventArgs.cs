namespace SkyDean.FareLiz.Core
{
    using System;

    public class SyncEventArgs<T> : EventArgs where T : ISyncable
    {
        public IDataSyncer<T> Syncer { get; set; }
        public object Data { get; set; }

        public SyncEventArgs(IDataSyncer<T> syncer, object data)
        {
            this.Syncer = syncer;
            this.Data = data;
        }
    }
}