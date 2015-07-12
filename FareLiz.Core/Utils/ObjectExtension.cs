namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.IO;
    using System.Reflection;

    using log4net;

    public static class ObjectExtension
    {
        public static T ShallowClone<T>(this T targetObject) where T : class
        {
            return targetObject.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(targetObject, null) as T;
        }

        public static T BinarySerializeDeepClone<T>(this T obj, ILog logger) where T : class
        {
            if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
                return (T)((ICloneable)obj).Clone();

            var formatter = new TolerantBinaryFormatter(logger);
            var ms = new MemoryStream();
            formatter.Serialize(ms, obj);
            ms.Position = 0;
            return (T)formatter.Deserialize(ms);
        }

        public static T ReflectionDeepClone<T>(this T obj, ILog logger) where T : class
        {
            var cloner = new ReflectionDeepCloner();
            var result = cloner.DeepClone(obj);
            return result;
        }

        /// <summary>
        /// Transfer object data to another object
        /// </summary>
        public static void TransferDataTo<T>(this T fromObject, T toObject) where T : class
        {
            var transporter = new ObjectDataTransporter();
            transporter.TransferData(fromObject, toObject);
        }

        public static bool AreEquals(byte[] a1, byte[] a2)
        {
            if (a1 == a2)
                return true;

            if ((a1 != null) && (a2 != null))
            {
                if (a1.Length != a2.Length)
                    return false;

                for (int i = 0; i < a1.Length; i++)
                {
                    if (a1[i] != a2[i])
                        return false;
                }
                return true;
            }

            return false;
        }       
    }
}
