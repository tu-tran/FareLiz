using System;
using System.Collections.Generic;
using ProtoBuf;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Package for transporting binary data
    /// </summary>
    [Serializable, ProtoContract]
    public class DataPackage<T>
    {
        [ProtoMember(1)]
        public string Id { get; private set; }

        [ProtoMember(2)]
        public List<T> Data { get; private set; }

        [ProtoMember(3)]
        public DateTime CreatedDate { get; private set; }

        protected DataPackage()
            : this(null) { }

        public DataPackage(IEnumerable<T> initialData)
            : this(Guid.NewGuid().ToString(), initialData) { }

        public DataPackage(string id, IEnumerable<T> initialData)
        {
            Id = id;
            CreatedDate = DateTime.Now;
            if (initialData != null)
                AddData(initialData);
        }

        public void AddData(IEnumerable<T> newData)
        {
            if (Data == null)
                Data = new List<T>();
            Data.AddRange(newData);
        }

        public void AddData(T newData)
        {
            AddData(new T[] { newData });
        }
    }

}
