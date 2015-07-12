using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace SkyDean.FareLiz.Core.Utils
{
    public class TolerantBinaryFormatter : IFormatter
    {
        readonly BinaryFormatter _formatter;
        public TolerantBinaryFormatter(ILog logger)
        {
            _formatter = new BinaryFormatter() { AssemblyFormat = FormatterAssemblyStyle.Simple, Binder = new TolerantBinder() };
            _formatter.SurrogateSelector = new SurrogateSelector();
            _formatter.SurrogateSelector.ChainSelector(new NonSerialiazableTypeSurrogateSelector(logger));
        }

        public SerializationBinder Binder
        {
            get { return _formatter.Binder; }
            set { _formatter.Binder = value; }
        }

        public StreamingContext Context
        {
            get { return _formatter.Context; }
            set { _formatter.Context = value; }
        }

        public ISurrogateSelector SurrogateSelector
        {
            get { return _formatter.SurrogateSelector; }
            set { _formatter.SurrogateSelector = value; }
        }

        public object Deserialize(Stream serializationStream)
        {
            return _formatter.Deserialize(serializationStream);
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            _formatter.Serialize(serializationStream, graph);
        }   

        private class TolerantBinder : SerializationBinder
        {
            private readonly Dictionary<KeyValue<string, string>, Type> _cache = new Dictionary<KeyValue<string, string>, Type>();

            public override Type BindToType(string assemblyName, string typeName)
            {
                foreach (var key in _cache.Keys)
                {
                    if (String.Equals(assemblyName, key.Key, StringComparison.OrdinalIgnoreCase)
                            && String.Equals(typeName, key.Value, StringComparison.OrdinalIgnoreCase))
                        return _cache[key];
                }


                Type result = null;
                try
                {
                    result = Type.GetType(typeName);  // Let AssemblyResolve work first...
                }
                catch { }

                if (result == null)
                {
                    // Be forgiving and search for another possible type candidate
                    var asmName = new AssemblyName(assemblyName);
                    string tolerantAsmName = asmName.Name;
                    string[] typeNameParts = typeName.Split('.');
                    string tolerantTypeName = typeNameParts[typeNameParts.Length - 1];
                    Type diffAsmExactType = null,
                         candidateType = null;

                    Assembly[] domainAsm = AppDomain.CurrentDomain.GetAssemblies();
                    // Put the assembly with the same name to the beginning...
                    var loadedAssemblies = new List<Assembly>();
                    foreach (var asm in domainAsm)
                    {
                        if (String.Equals(asm.FullName, assemblyName, StringComparison.OrdinalIgnoreCase)
                            || String.Equals(asm.GetName().Name, tolerantAsmName, StringComparison.OrdinalIgnoreCase))
                            loadedAssemblies.Insert(0, asm);
                        else
                            loadedAssemblies.Add(asm);
                    }

                    foreach (var asm in loadedAssemblies)
                    {
                        Type[] asmTypes = null;
                        try
                        {
                            asmTypes = asm.GetTypes();
                        }
                        catch { }

                        if (asmTypes == null || asmTypes.Length == 0)
                            continue;

                        bool matchAsm = String.Equals(asm.GetName().Name, tolerantAsmName, StringComparison.OrdinalIgnoreCase);
                        foreach (Type type in asmTypes)
                        {
                            if (type.FullName == typeName)
                            {
                                if (matchAsm)
                                {
                                    result = type;    // If we found a type with exact same signature: No need to look further
                                    break;
                                }
                                else
                                    diffAsmExactType = type;    // We get a type with exact same signature on different assembly
                            }
                            else if (type.Name == tolerantTypeName)
                                candidateType = type;   // Let's try our luck and ignore the namespace
                        }

                        if (result != null)
                            break;
                    }

                    result = result ?? (diffAsmExactType ?? candidateType);
                }

                if (result != null)
                    _cache.Add(new KeyValue<string, string>(assemblyName, typeName), result);

                return result;
            }
        }
    }
}
