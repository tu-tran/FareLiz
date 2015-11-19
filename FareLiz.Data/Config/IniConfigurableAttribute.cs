namespace SkyDean.FareLiz.Data.Config
{
    using System;

    /// <summary>
    /// The ini configurable attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IniConfigurableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IniConfigurableAttribute"/> class.
        /// </summary>
        /// <param name="configurable">
        /// The configurable.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        public IniConfigurableAttribute(bool configurable, string key)
        {
            this.ConfigurationKey = key;
            this.IsConfigurable = configurable;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniConfigurableAttribute"/> class.
        /// </summary>
        /// <param name="configurable">
        /// The configurable.
        /// </param>
        public IniConfigurableAttribute(bool configurable)
            : this(configurable, null)
        {
        }

        /// <summary>
        /// Gets the configuration key.
        /// </summary>
        public string ConfigurationKey { get; private set; }

        /// <summary>
        /// Gets a value indicating whether is configurable.
        /// </summary>
        public bool IsConfigurable { get; private set; }
    }
}