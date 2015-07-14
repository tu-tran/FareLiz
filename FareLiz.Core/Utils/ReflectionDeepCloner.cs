namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>The reflection deep cloner.</summary>
    public class ReflectionDeepCloner
    {
        /// <summary>
        /// The deep clone.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T DeepClone<T>(T targetObject) where T : class
        {
            return this.ReflectionDeepCopy(targetObject, new Dictionary<object, object>()) as T;
        }

        /// <summary>
        /// The reflection deep copy.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        /// <param name="parentCalls">
        /// The parent calls.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        private object ReflectionDeepCopy(object targetObject, IDictionary<object, object> parentCalls)
        {
            if (targetObject == null)
            {
                return null;
            }

            var type = targetObject.GetType();
            if (type.IsValueType || type == typeof(string) || typeof(ILogger).IsAssignableFrom(type)
                || typeof(MarshalByRefObject).IsAssignableFrom(type))
            {
                // Return as-is
                return targetObject;
            }

            // We got a reference type
            foreach (var pair in parentCalls)
            {
                // If this is a reference to its parent
                if (targetObject == pair.Key)
                {
                    Debug.WriteLine("Circular reference detected for type " + pair.Key);
                    return pair.Value;
                }
            }

            var cloneable = targetObject as ICloneable;
            if (cloneable != null)
            {
                Debug.WriteLine("Found ICloneable interface: " + type);
                parentCalls.Add(targetObject, cloneable);
                return cloneable;
            }

            Debug.WriteLine("Reflection-cloning type " + type);
            object clonedObject = null;
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var array = targetObject as Array;
                var copied = Array.CreateInstance(elementType, array.Length);
                Debug.WriteLine("Found array of type {0} [{1}]", elementType, array.Length);

                for (var i = 0; i < array.Length; i++)
                {
                    copied.SetValue(this.ReflectionDeepCopy(array.GetValue(i), parentCalls), i);
                }

                clonedObject = Convert.ChangeType(copied, type);
                if (clonedObject != null)
                {
                    parentCalls.Add(targetObject, clonedObject);
                }
            }
            else
            {
                clonedObject = TypeResolver.CreateInstance(type);
                if (clonedObject != null)
                {
                    parentCalls.Add(targetObject, clonedObject);
                }

                var clonedList = clonedObject as IList;
                if (clonedList != null)
                {
                    var sourceList = targetObject as IList;
                    Debug.WriteLine("Found IList interface [{0}]", sourceList.Count);

                    foreach (var item in sourceList)
                    {
                        var clonedItem = this.ReflectionDeepCopy(item, parentCalls);
                        clonedList.Add(clonedItem);
                    }
                }
                else
                {
                    var clonedDict = clonedObject as IDictionary;
                    if (clonedDict != null)
                    {
                        if (type.FullName == "System.Collections.Hashtable+SyncHashtable")
                        {
                            var newHashtable = new Hashtable();
                            clonedObject = clonedDict = Hashtable.Synchronized(newHashtable);
                        }

                        var sourceDict = targetObject as IDictionary;
                        Debug.WriteLine("Found IDictionary interface [{0}]", sourceDict.Count);

                        foreach (var key in sourceDict.Keys)
                        {
                            var clonedKey = this.ReflectionDeepCopy(key, parentCalls);
                            var val = sourceDict[key];
                            var clonedVal = this.ReflectionDeepCopy(val, parentCalls);
                            clonedDict.Add(clonedKey, clonedVal);
                        }
                    }
                    else
                    {
                        var fields =
                            type.GetFieldsRecursively(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField);
                        foreach (var field in fields)
                        {
                            var fieldValue = field.GetValue(targetObject);
                            if (fieldValue == null)
                            {
                                continue;
                            }

                            Debug.WriteLine("Found field {0}: {1}", field, fieldValue);
                            field.SetValue(clonedObject, this.ReflectionDeepCopy(fieldValue, parentCalls));
                        }
                    }
                }
            }

            return clonedObject;
        }
    }
}