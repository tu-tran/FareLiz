using System;
using System.Collections.Generic;
using System.Reflection;

namespace SkyDean.FareLiz.Core.Utils
{
    public static class TypeExtensions
    {
        public static IEnumerable<FieldInfo> GetFieldsRecursively(this Type targetType, BindingFlags bindingFlags)
        {
            Type type = targetType;
            var result = new List<FieldInfo>();
            var objectType = typeof(object);
            while (type != null && type != objectType)
            {
                FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

                foreach (var fi in fieldInfos)
                {
                    bool exist = false;
                    foreach (var item in result)
                        if (item.Name == fi.Name)
                        {
                            exist = true;
                            break;
                        }

                    if (!exist)
                        result.Add(fi);
                }

                type = type.BaseType;
            }

            return result;
        }

        public static IEnumerable<PropertyInfo> GetPropertiesRecursively(this Type targetType, BindingFlags bindingFlags)
        {
            Type type = targetType;
            var result = new List<PropertyInfo>();

            var objectType = typeof(object);
            while (type != objectType)
            {
                PropertyInfo[] props = type.GetProperties(bindingFlags);
                foreach (var p in props)
                {
                    bool exist = false;
                    foreach (var item in result)
                        if (item.Name == p.Name)
                        {
                            exist = true;
                            break;
                        }
                    if (!exist)
                        result.Add(p);
                }

                type = type.BaseType;
            }

            return result;
        }
    }
}
