namespace SkyDean.FareLiz.Core.Data
{
    using System;
    using System.Collections.Generic;

    using ProtoBuf;

    /// <summary>
    /// Package for transporting binary data
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    [Serializable]
    [ProtoContract]
    public class DataPackage<T>
    {
        /// <summary>Initializes a new instance of the <see cref="DataPackage{T}" /> class.</summary>
        protected DataPackage()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPackage{T}"/> class.
        /// </summary>
        /// <param name="initialData">
        /// The initial data.
        /// </param>
        public DataPackage(IEnumerable<T> initialData)
            : this(Guid.NewGuid().ToString(), initialData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataPackage{T}"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="initialData">
        /// The initial data.
        /// </param>
        public DataPackage(string id, IEnumerable<T> initialData)
        {
            this.Id = id;
            this.CreatedDate = DateTime.Now;
            if (initialData != null)
            {
                this.AddData(initialData);
            }
        }

        /// <summary>Gets the id.</summary>
        [ProtoMember(1)]
        public string Id { get; private set; }

        /// <summary>Gets the data.</summary>
        [ProtoMember(2)]
        public List<T> Data { get; private set; }

        /// <summary>Gets the created date.</summary>
        [ProtoMember(3)]
        public DateTime CreatedDate { get; private set; }

        /// <summary>
        /// The add data.
        /// </summary>
        /// <param name="newData">
        /// The new data.
        /// </param>
        public void AddData(IEnumerable<T> newData)
        {
            if (this.Data == null)
            {
                this.Data = new List<T>();
            }

            this.Data.AddRange(newData);
        }

        /// <summary>
        /// The add data.
        /// </summary>
        /// <param name="newData">
        /// The new data.
        /// </param>
        public void AddData(T newData)
        {
            this.AddData(new[] { newData });
        }
    }
}