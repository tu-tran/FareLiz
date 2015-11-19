namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;

    using log4net;

    /// <summary>The type resolver.</summary>
    public sealed class TypeResolver
    {
        /// <summary>The logger.</summary>
        private readonly ILog Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeResolver"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public TypeResolver(ILog logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// The get types.
        /// </summary>
        /// <param name="baseTypeOrInterface">
        /// The base type or interface.
        /// </param>
        /// <param name="genericTypesArgument">
        /// The generic types argument.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<Type> GetTypes(Type baseTypeOrInterface, params Type[] genericTypesArgument)
        {
            var loadedAsms = AppDomain.CurrentDomain.GetAssemblies();
            var allAsms = new List<Assembly>();
            foreach (var asm in loadedAsms)
            {
                if (string.Compare(asm.Location, PathUtil.ApplicationPath, StringComparison.OrdinalIgnoreCase) != 0
                    && string.Compare(Path.GetDirectoryName(asm.Location), PathUtil.ApplicationPath, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    allAsms.Add(asm);
                }
            }

            return this.GetTypes(baseTypeOrInterface, allAsms, genericTypesArgument);
        }

        /// <summary>
        /// The get types.
        /// </summary>
        /// <param name="baseTypeOrInterface">
        /// The base type or interface.
        /// </param>
        /// <param name="fromAssemblies">
        /// The from assemblies.
        /// </param>
        /// <param name="genericTypesArgument">
        /// The generic types argument.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
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
                                {
                                    result.Add(type);
                                }
                            }
                            else
                            {
                                var typeBasesAndInterfaces = new List<Type>();
                                var typeInterfaces = type.GetInterfaces();
                                if (typeInterfaces != null && typeInterfaces.Length > 0)
                                {
                                    typeBasesAndInterfaces.AddRange(typeInterfaces);
                                }

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
                                                {
                                                    break;
                                                }

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
                        this.Logger.Error(ex);
                    }

                    foreach (var type in result)
                    {
                        this.Logger.DebugFormat(
                            "Found type {0} [{1}] in assembly {2}", 
                            type.FullName, 
                            baseTypeOrInterface.FullName, 
                            type.Assembly.GetName().FullName);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The create instance.
        /// </summary>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T CreateInstance<T>(Type targetType) where T : class
        {
            return CreateInstance(targetType) as T;
        }

        /// <summary>
        /// The create instance.
        /// </summary>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object CreateInstance(Type targetType)
        {
            if (!(targetType.IsAbstract || targetType.IsInterface))
            {
                return targetType.GetConstructor(new Type[0]) == null
                           ? FormatterServices.GetUninitializedObject(targetType)
                           : Activator.CreateInstance(targetType);
            }

            return null;
        }
    }
}