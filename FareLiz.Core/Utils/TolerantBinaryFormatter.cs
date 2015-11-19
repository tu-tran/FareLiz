namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;

    using log4net;

    /// <summary>The tolerant binary formatter.</summary>
    public class TolerantBinaryFormatter : IFormatter
    {
        /// <summary>The _formatter.</summary>
        private readonly BinaryFormatter _formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="TolerantBinaryFormatter"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public TolerantBinaryFormatter(ILog logger)
        {
            this._formatter = new BinaryFormatter { AssemblyFormat = FormatterAssemblyStyle.Simple, Binder = new TolerantBinder() };
            this._formatter.SurrogateSelector = new SurrogateSelector();
            this._formatter.SurrogateSelector.ChainSelector(new NonSerialiazableTypeSurrogateSelector(logger));
        }

        /// <summary>Gets or sets the binder.</summary>
        public SerializationBinder Binder
        {
            get
            {
                return this._formatter.Binder;
            }

            set
            {
                this._formatter.Binder = value;
            }
        }

        /// <summary>Gets or sets the context.</summary>
        public StreamingContext Context
        {
            get
            {
                return this._formatter.Context;
            }

            set
            {
                this._formatter.Context = value;
            }
        }

        /// <summary>Gets or sets the surrogate selector.</summary>
        public ISurrogateSelector SurrogateSelector
        {
            get
            {
                return this._formatter.SurrogateSelector;
            }

            set
            {
                this._formatter.SurrogateSelector = value;
            }
        }

        /// <summary>
        /// The deserialize.
        /// </summary>
        /// <param name="serializationStream">
        /// The serialization stream.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object Deserialize(Stream serializationStream)
        {
            return this._formatter.Deserialize(serializationStream);
        }

        /// <summary>
        /// The serialize.
        /// </summary>
        /// <param name="serializationStream">
        /// The serialization stream.
        /// </param>
        /// <param name="graph">
        /// The graph.
        /// </param>
        public void Serialize(Stream serializationStream, object graph)
        {
            this._formatter.Serialize(serializationStream, graph);
        }

        /// <summary>The tolerant binder.</summary>
        private class TolerantBinder : SerializationBinder
        {
            /// <summary>The _cache.</summary>
            private readonly Dictionary<KeyValue<string, string>, Type> _cache = new Dictionary<KeyValue<string, string>, Type>();

            /// <summary>
            /// The bind to type.
            /// </summary>
            /// <param name="assemblyName">
            /// The assembly name.
            /// </param>
            /// <param name="typeName">
            /// The type name.
            /// </param>
            /// <returns>
            /// The <see cref="Type"/>.
            /// </returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                foreach (var key in this._cache.Keys)
                {
                    if (string.Equals(assemblyName, key.Key, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(typeName, key.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        return this._cache[key];
                    }
                }

                Type result = null;
                try
                {
                    result = Type.GetType(typeName); // Let AssemblyResolve work first...
                }
                catch
                {
                }

                if (result == null)
                {
                    // Be forgiving and search for another possible type candidate
                    var asmName = new AssemblyName(assemblyName);
                    string tolerantAsmName = asmName.Name;
                    string[] typeNameParts = typeName.Split('.');
                    string tolerantTypeName = typeNameParts[typeNameParts.Length - 1];
                    Type diffAsmExactType = null, candidateType = null;

                    Assembly[] domainAsm = AppDomain.CurrentDomain.GetAssemblies();

                    // Put the assembly with the same name to the beginning...
                    var loadedAssemblies = new List<Assembly>();
                    foreach (var asm in domainAsm)
                    {
                        if (string.Equals(asm.FullName, assemblyName, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(asm.GetName().Name, tolerantAsmName, StringComparison.OrdinalIgnoreCase))
                        {
                            loadedAssemblies.Insert(0, asm);
                        }
                        else
                        {
                            loadedAssemblies.Add(asm);
                        }
                    }

                    foreach (var asm in loadedAssemblies)
                    {
                        Type[] asmTypes = null;
                        try
                        {
                            asmTypes = asm.GetTypes();
                        }
                        catch
                        {
                        }

                        if (asmTypes == null || asmTypes.Length == 0)
                        {
                            continue;
                        }

                        bool matchAsm = string.Equals(asm.GetName().Name, tolerantAsmName, StringComparison.OrdinalIgnoreCase);
                        foreach (Type type in asmTypes)
                        {
                            if (type.FullName == typeName)
                            {
                                if (matchAsm)
                                {
                                    result = type; // If we found a type with exact same signature: No need to look further
                                    break;
                                }

                                diffAsmExactType = type; // We get a type with exact same signature on different assembly
                            }
                            else if (type.Name == tolerantTypeName)
                            {
                                candidateType = type; // Let's try our luck and ignore the namespace
                            }
                        }

                        if (result != null)
                        {
                            break;
                        }
                    }

                    result = result ?? (diffAsmExactType ?? candidateType);
                }

                if (result != null)
                {
                    this._cache.Add(new KeyValue<string, string>(assemblyName, typeName), result);
                }

                return result;
            }
        }
    }
}