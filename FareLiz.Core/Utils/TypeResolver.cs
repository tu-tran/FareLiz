using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace SkyDean.FareLiz.Core.Utils
{
    public sealed class TypeResolver
    {
        private readonly ILog Logger;

        public TypeResolver(ILog logger)
        {
            Logger = logger;
        }

        public IList<Type> GetTypes(Type baseTypeOrInterface, params Type[] genericTypesArgument)
        {
            var loadedAsms = AppDomain.CurrentDomain.GetAssemblies();
            var allAsms = new List<Assembly>();
            foreach (var asm in loadedAsms)
            {
                if (String.Compare(asm.Location, PathUtil.ApplicationPath, StringComparison.OrdinalIgnoreCase) != 0
                    && String.Compare(Path.GetDirectoryName(asm.Location), PathUtil.ApplicationPath, StringComparison.OrdinalIgnoreCase) == 0)
                    allAsms.Add(asm);
            }

            return GetTypes(baseTypeOrInterface, allAsms, genericTypesArgument);
        }

        public IList<Type> GetTypes(Type baseTypeOrInterface, IEnumerable<Assembly> fromAssemblies, params Type[] genericTypesArgument)
        {
            var result = new List<Type>();

            if (baseTypeOrInterface != null && fromAssemblies != null)
            {
                foreach (var asm in fromAssemblies)
                {
                    try
                    {
                        var typeList = asm.GetTypes();
                        foreach (var type in typeList)
                        {
                            if (genericTypesArgument == null || genericTypesArgument.Length == 0)
                            {
                                if (baseTypeOrInterface.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                                    result.Add(type);
                            }
                            else
                            {
                                var typeBasesAndInterfaces = new List<Type>();
                                var typeInterfaces = type.GetInterfaces();
                                if (typeInterfaces != null && typeInterfaces.Length > 0)
                                    typeBasesAndInterfaces.AddRange(typeInterfaces);

                                Type baseType = type.BaseType;
                                while (baseType != null)
                                {
                                    typeBasesAndInterfaces.Add(baseType);
                                    baseType = baseType.BaseType;
                                }

                                foreach (var parentType in typeBasesAndInterfaces)
                                {
                                    bool found = false;
                                    if (parentType.IsGenericType && parentType.GetGenericTypeDefinition() == baseTypeOrInterface)
                                    {
                                        Type[] genericArgs = parentType.GetGenericArguments();
                                        if (genericArgs != null && genericArgs.Length == genericTypesArgument.Length)
                                        {
                                            for (int i = 0; i < genericArgs.Length; i++)
                                            {
                                                if (genericArgs[i] != genericTypesArgument[i])
                                                    break;

                                                found = true;
                                            }
                                        }
                                    }

                                    if (found)
                                    {
                                        result.Add(type);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                    foreach (var type in result)
                        Logger.DebugFormat("Found type {0} [{1}] in assembly {2}", type.FullName, baseTypeOrInterface.FullName, type.Assembly.GetName().FullName);
                }
            }

            return result;
        }

        public T CreateInstance<T>(Type targetType) where T : class
        {
            return CreateInstance(targetType) as T;
        }

        public static object CreateInstance(Type targetType)
        {
            if (!(targetType.IsAbstract || targetType.IsInterface))
                return (targetType.GetConstructor(new Type[0]) == null
                    ? FormatterServices.GetUninitializedObject(targetType)
                    : Activator.CreateInstance(targetType));

            return null;
        }
    }
}
