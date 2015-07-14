namespace SkyDean.FareLiz.Core.Config
{
    using System;

    /// <summary>The configuration exception.</summary>
    public class ConfigurationException : Exception
    {
        /// <summary>The _message.</summary>
        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public ConfigurationException(IPlugin plugin, string message)
            : this(plugin.GetType(), message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        /// <param name="pluginType">
        /// The plugin type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public ConfigurationException(Type pluginType, string message)
        {
            var detail = pluginType.GetPluginDetail();
            this._message = "Invalid configuration for " + detail.Key + ": " + message;
        }

        /// <summary>Gets the message.</summary>
        public override string Message
        {
            get
            {
                return this._message;
            }
        }
    }
}