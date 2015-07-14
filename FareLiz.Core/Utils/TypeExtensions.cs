namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>The type extensions.</summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// The get fields recursively.
        /// </summary>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<FieldInfo> GetFieldsRecursively(this Type targetType, BindingFlags bindingFlags)
        {
            var type = targetType;
            var result = new List<FieldInfo>();
            var objectType = typeof(object);
            while (type != null && type != objectType)
            {
                var fieldInfos = type.GetFields(bindingFlags);

                foreach (var fi in fieldInfos)
                {
                    var exist = false;
                    foreach (var item in result)
                    {
                        if (item.Name == fi.Name)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        result.Add(fi);
                    }
                }

                type = type.BaseType;
            }

            return result;
        }

        /// <summary>
        /// The get properties recursively.
        /// </summary>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="bindingFlags">
        /// The binding flags.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetPropertiesRecursively(this Type targetType, BindingFlags bindingFlags)
        {
            var type = targetType;
            var result = new List<PropertyInfo>();

            var objectType = typeof(object);
            while (type != objectType)
            {
                var props = type.GetProperties(bindingFlags);
                foreach (var p in props)
                {
                    var exist = false;
                    foreach (var item in result)
                    {
                        if (item.Name == p.Name)
                        {
                            exist = true;
                            break;
                        }
                    }

                    if (!exist)
                    {
                        result.Add(p);
                    }
                }

                type = type.BaseType;
            }

            return result;
        }
    }
}