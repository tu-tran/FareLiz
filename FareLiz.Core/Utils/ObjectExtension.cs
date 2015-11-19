namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.IO;
    using System.Reflection;

    using log4net;

    /// <summary>The object extension.</summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// The shallow clone.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ShallowClone<T>(this T targetObject) where T : class
        {
            return targetObject.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(targetObject, null) as T;
        }

        /// <summary>
        /// The binary serialize deep clone.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T BinarySerializeDeepClone<T>(this T obj, ILogger logger) where T : class
        {
            if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
            {
                return (T)((ICloneable)obj).Clone();
            }

            var formatter = new TolerantBinaryFormatter(logger);
            var ms = new MemoryStream();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }

        /// <summary>
        /// The reflection deep clone.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T ReflectionDeepClone<T>(this T obj, ILogger logger) where T : class
        {
            var cloner = new ReflectionDeepCloner();
            var result = cloner.DeepClone(obj);
            return result;
        }

        /// <summary>
        /// Transfer object data to another object
        /// </summary>
        /// <param name="fromObject">
        /// The from Object.
        /// </param>
        /// <param name="toObject">
        /// The to Object.
        /// </param>
        public static void TransferDataTo<T>(this T fromObject, T toObject) where T : class
        {
            var transporter = new ObjectDataTransporter();
            transporter.TransferData(fromObject, toObject);
        }

        /// <summary>
        /// The are equals.
        /// </summary>
        /// <param name="a1">
        /// The a 1.
        /// </param>
        /// <param name="a2">
        /// The a 2.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool AreEquals(byte[] a1, byte[] a2)
        {
            if (a1 == a2)
            {
                return true;
            }

            if ((a1 != null) && (a2 != null))
            {
                if (a1.Length != a2.Length)
                {
                    return false;
                }

                for (int i = 0; i < a1.Length; i++)
                {
                    if (a1[i] != a2[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}