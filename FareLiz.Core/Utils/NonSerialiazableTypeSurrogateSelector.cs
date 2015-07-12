using log4net;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace SkyDean.FareLiz.Core.Utils
{
    /// <summary>
    /// This class offers the ability to save the fields
    /// of types that don't have the <c ref="System.SerializableAttribute">SerializableAttribute</c>.
    /// </summary>
    public class NonSerialiazableTypeSurrogateSelector : ISerializationSurrogate, ISurrogateSelector
    {
        private const BindingFlags BINDING_FLAG = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private readonly TypeResolver _typeResolver;
        private readonly ILog _logger;

        public NonSerialiazableTypeSurrogateSelector(ILog logger)
        {
            _logger = logger;
            _typeResolver = new TypeResolver(_logger);
        }

        private bool IsKnownType(Type type)
        {
            return type == typeof(string) || type.IsPrimitive || type.IsSerializable;
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Type type = obj.GetType();

            var fieldInfos = type.GetFieldsRecursively(BINDING_FLAG);
            foreach (var fi in fieldInfos)
            {
                object val = fi.GetValue(obj);
                info.AddValue(fi.Name, val);
            }
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Type type = obj.GetType();
            var fieldInfos = type.GetFieldsRecursively(BINDING_FLAG);
            foreach (var fi in fieldInfos)
            {
                if (IsNullableType(fi.FieldType))
                {
                    Type argumentValueForTheNullableType = fi.FieldType.GetGenericArguments()[0];
                    fi.SetValue(obj, info.GetValue(fi.Name, argumentValueForTheNullableType));
                }
                else
                {
                    fi.SetValue(obj, info.GetValue(fi.Name, fi.FieldType));
                }
            }
            return null;
        }

        private bool IsNullableType(Type type)
        {
            if (type.IsGenericType)
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);
            return false;
        }

        ISurrogateSelector _nextSelector;
        public void ChainSelector(ISurrogateSelector selector)
        {
            _nextSelector = selector;
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _nextSelector;
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (IsKnownType(type))
            {
                selector = null;
                return null;
            }
            if (type.IsClass || type.IsValueType)
            {
                selector = this;
                return FormatterServices.GetSurrogateForCyclicalReference(this);
            }
            else
            {
                selector = null;
                return null;
            }
        }
    }
}