using System;

namespace SkyDean.FareLiz.Data.Config
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IniConfigurableAttribute : Attribute
    {
        public string ConfigurationKey { get; private set; }
        public bool IsConfigurable { get; private set; }

        public IniConfigurableAttribute(bool configurable, string key)
            : base()
        {
            ConfigurationKey = key;
            IsConfigurable = configurable;
        }

        public IniConfigurableAttribute(bool configurable) : this(configurable, null) { }
    }
}
