namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    /// <summary>
    /// The key value.
    /// </summary>
    /// <typeparam name="K">
    /// </typeparam>
    /// <typeparam name="V">
    /// </typeparam>
    [Serializable]
    public class KeyValue<K, V>
    {
        /// <summary>Initializes a new instance of the <see cref="KeyValue{K,V}" /> class.</summary>
        public KeyValue()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValue{K,V}"/> class.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public KeyValue(K key, V value)
        {
            this.Key = key;
            this.Value = value;
        }

        /// <summary>Gets or sets the key.</summary>
        public K Key { get; set; }

        /// <summary>Gets or sets the value.</summary>
        public V Value { get; set; }
    }
}