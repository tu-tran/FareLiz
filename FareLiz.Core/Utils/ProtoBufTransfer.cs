using log4net;
using ProtoBuf;
using System;
using System.IO;
using System.Reflection;

namespace SkyDean.FareLiz.Core.Utils
{
    /// <summary>
    /// A generic class for opbject serialization
    /// </summary>
    public class ProtoBufTransfer
    {
        private readonly ILog _logger;

        public ProtoBufTransfer(ILog logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Deserialize from base64 string (data presentation of binary bytes)
        /// </summary>
        public T FromBase64<T>(string dataString)
        {
            try
            {
                if (!String.IsNullOrEmpty(dataString))
                    using (var ms = new MemoryStream(Convert.FromBase64String(dataString)))
                    {
                        return Serializer.Deserialize<T>(ms);
                    }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            return default(T);
        }

        /// <summary>
        /// Deserialize a bytes array
        /// </summary>
        public T FromRaw<T>(byte[] bytesArray)
        {
            using (var ms = new MemoryStream(bytesArray))
            {
                return FromRaw<T>(ms);
            }
        }

        /// <summary>
        /// Deserialize from data stream
        /// </summary>
        public T FromRaw<T>(Stream dataStream)
        {
            try
            {
                if (dataStream != null)
                    return Serializer.Deserialize<T>(dataStream);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            return default(T);
        }

        /// <summary>
        /// Deserialize from binary file
        /// </summary>
        public T FromRaw<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    using (var fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        return Serializer.Deserialize<T>(fs);
                    }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }

            return default(T);
        }

        /// <summary>
        /// Serialize object into base64 string
        /// </summary>
        public string ToBase64<T>(T instance)
        {
            if (instance == null)
                return String.Empty;

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, instance);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Serialize object into data stream
        /// </summary>
        public MemoryStream ToRawStream<T>(T instance)
        {
            if (instance == null)
                return null;

            var ms = new MemoryStream();
            Serializer.Serialize<T>(ms, instance);
            return ms;
        }

        /// <summary>
        /// Serialize object into bytes array
        /// </summary>
        public byte[] ToRaw<T>(T instance)
        {
            using (var result = ToRawStream(instance))
                return result == null ? null : result.ToArray();
        }

        /// <summary>
        /// Serialize object into binary file
        /// </summary>
        public void ToRaw<T>(T instance, string filePath)
        {
            if (instance == null)
                return;

            using (var fileStream = File.OpenWrite(filePath))
            {
                Serializer.Serialize<T>(fileStream, instance);
            }
        }

        /// <summary>
        /// Initialize the metadata for class which does not have data contract with ProtoBuf
        /// </summary>
        public void Initialize<T>()
        {
            Type objectType = typeof(T);
            var typeMeta = ProtoBuf.Meta.RuntimeTypeModel.Default.Add(objectType, true);

            foreach (var prop in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                typeMeta.Add(prop.Name);

            ProtoBuf.Serializer.PrepareSerializer<T>();
        }
    }
}
