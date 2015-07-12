using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyDean.FareLiz.Core.Config
{
    public class ConfigurationException : Exception
    {
        private readonly string _message;

        public ConfigurationException(IPlugin plugin, string message)
            : this(plugin.GetType(), message)
        { }

        public ConfigurationException(Type pluginType, string message)
        {
            var detail = pluginType.GetPluginDetail();
            _message = "Invalid configuration for " + detail.Key + ": " + message;
        }

        public override string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
