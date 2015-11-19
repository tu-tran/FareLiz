namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;

    using log4net;

    /// <summary>This class offers the ability to save the fields of types that don't have the
    /// <c ref="System.SerializableAttribute">SerializableAttribute</c>.</summary>
    public class NonSerialiazableTypeSurrogateSelector : ISerializationSurrogate, ISurrogateSelector
    {
        /// <summary>The bindin g_ flag.</summary>
        private const BindingFlags BINDING_FLAG = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>The _type resolver.</summary>
        private readonly TypeResolver _typeResolver;

        /// <summary>The _next selector.</summary>
        private ISurrogateSelector _nextSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="NonSerialiazableTypeSurrogateSelector"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public NonSerialiazableTypeSurrogateSelector(ILogger logger)
        {
            this._logger = logger;
            this._typeResolver = new TypeResolver(this._logger);
        }

        /// <summary>
        /// The get object data.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
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

        /// <summary>
        /// The set object data.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Type type = obj.GetType();
            var fieldInfos = type.GetFieldsRecursively(BINDING_FLAG);
            foreach (var fi in fieldInfos)
            {
                if (this.IsNullableType(fi.FieldType))
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

        /// <summary>
        /// The chain selector.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        public void ChainSelector(ISurrogateSelector selector)
        {
            this._nextSelector = selector;
        }

        /// <summary>The get next selector.</summary>
        /// <returns>The <see cref="ISurrogateSelector" />.</returns>
        public ISurrogateSelector GetNextSelector()
        {
            return this._nextSelector;
        }

        /// <summary>
        /// The get surrogate.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The <see cref="ISerializationSurrogate"/>.
        /// </returns>
        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            if (this.IsKnownType(type))
            {
                selector = null;
                return null;
            }

            if (type.IsClass || type.IsValueType)
            {
                selector = this;
                return FormatterServices.GetSurrogateForCyclicalReference(this);
            }

            selector = null;
            return null;
        }

        /// <summary>
        /// The is known type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsKnownType(Type type)
        {
            return type == typeof(string) || type.IsPrimitive || type.IsSerializable;
        }

        /// <summary>
        /// The is nullable type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsNullableType(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }

            return false;
        }
    }
}