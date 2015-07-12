using System;
using System.Reflection;

namespace SkyDean.FareLiz.Core.Utils
{
    public class ObjectDataTransporter
    {
        private readonly BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public void TransferData(object fromObject, object toObject)
        {
            if (fromObject == null)
                throw new ArgumentException("Cannot transfer from null object");

            var fromType = fromObject.GetType();

            if (toObject == null)
                toObject = TypeResolver.CreateInstance(fromType);
            else
                if (fromType != toObject.GetType())
                    throw new ArgumentException("Cannot transfer data between 2 different object types");

            var fields = fromType.GetFieldsRecursively(_flags);
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
