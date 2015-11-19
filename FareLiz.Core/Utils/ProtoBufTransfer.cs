namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.IO;
    using System.Reflection;

    using log4net;

    using ProtoBuf;
    using ProtoBuf.Meta;

    /// <summary>A generic class for opbject serialization</summary>
    public class ProtoBufTransfer
    {
        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtoBufTransfer"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ProtoBufTransfer(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Deserialize from base64 string (data presentation of binary bytes)
        /// </summary>
        /// <param name="dataString">
        /// The data String.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T FromBase64<T>(string dataString)
        {
            try
            {
                if (!string.IsNullOrEmpty(dataString))
                {
                    using (var ms = new MemoryStream(Convert.FromBase64String(dataString)))
                    {
                        return Serializer.Deserialize<T>(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.ToString());
            }

            return default(T);
        }

        /// <summary>
        /// Deserialize a bytes array
        /// </summary>
        /// <param name="bytesArray">
        /// The bytes Array.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T FromRaw<T>(byte[] bytesArray)
        {
            using (var ms = new MemoryStream(bytesArray))
            {
                return this.FromRaw<T>(ms);
            }
        }

        /// <summary>
        /// Deserialize from data stream
        /// </summary>
        /// <param name="dataStream">
        /// The data Stream.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T FromRaw<T>(Stream dataStream)
        {
            try
            {
                if (dataStream != null)
                {
                    return Serializer.Deserialize<T>(dataStream);
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.ToString());
            }

            return default(T);
        }

        /// <summary>
        /// Deserialize from binary file
        /// </summary>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T FromRaw<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (var fs = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite))
                    {
                        return Serializer.Deserialize<T>(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.ToString());
            }

            return default(T);
        }

        /// <summary>
        /// Serialize object into base64 string
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ToBase64<T>(T instance)
        {
            if (instance == null)
            {
                return string.Empty;
            }

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, instance);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        /// <summary>
        /// Serialize object into data stream
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
        /// </returns>
        public MemoryStream ToRawStream<T>(T instance)
        {
            if (instance == null)
            {
                return null;
            }

            var ms = new MemoryStream();
            Serializer.Serialize(ms, instance);
            return ms;
        }

        /// <summary>
        /// Serialize object into bytes array
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] ToRaw<T>(T instance)
        {
            using (var result = this.ToRawStream(instance)) return result == null ? null : result.ToArray();
        }

        /// <summary>
        /// Serialize object into binary file
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        public void ToRaw<T>(T instance, string filePath)
        {
            if (instance == null)
            {
                return;
            }

            using (var fileStream = File.OpenWrite(filePath))
            {
                Serializer.Serialize(fileStream, instance);
            }
        }

        /// <summary>Initialize the metadata for class which does not have data contract with ProtoBuf</summary>
        public void Initialize<T>()
        {
            Type objectType = typeof(T);
            var typeMeta = RuntimeTypeModel.Default.Add(objectType, true);

            foreach (var prop in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                typeMeta.Add(prop.Name);
            }

            Serializer.PrepareSerializer<T>();
        }
    }
}