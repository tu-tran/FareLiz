using System;

namespace SkyDean.FareLiz.Core.Utils
{
    [Serializable]
    public class KeyValue<K, V>
    {
        public K Key { get; set; }
        public V Value { get; set; }

        public KeyValue() { }
        public KeyValue(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
}
