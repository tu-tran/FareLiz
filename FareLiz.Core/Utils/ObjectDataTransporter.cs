namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Reflection;

    /// <summary>The object data transporter.</summary>
    public class ObjectDataTransporter
    {
        /// <summary>The _flags.</summary>
        private readonly BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// The transfer data.
        /// </summary>
        /// <param name="fromObject">
        /// The from object.
        /// </param>
        /// <param name="toObject">
        /// The to object.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void TransferData(object fromObject, object toObject)
        {
            if (fromObject == null)
            {
                throw new ArgumentException("Cannot transfer from null object");
            }

            var fromType = fromObject.GetType();

            if (toObject == null)
            {
                toObject = TypeResolver.CreateInstance(fromType);
            }
            else if (fromType != toObject.GetType())
            {
                throw new ArgumentException("Cannot transfer data between 2 different object types");
            }

            var fields = fromType.GetFieldsRecursively(this._flags);
            foreach (var f in fields)
            {
                var fieldType = f.FieldType;
                if (fieldType.GetCustomAttributes(typeof(UniqueDataAttribute), false).Length == 0)
                {
                    var fromVal = f.GetValue(fromObject);
                    f.SetValue(toObject, fromVal);
                }
            }
        }
    }
}