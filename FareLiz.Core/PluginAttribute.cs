using System;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Attribute used for storing the plugin details
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PluginAttribute : Attribute
    {
        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get { return _name; } }
        private readonly string _name;

        /// <summary>
        /// Plugin's description
        /// </summary>
        public string Description { get { return _description; } }
        private readonly string _description;

        public PluginAttribute(string name, string description)
        {
            _name = name;
            _description = description;
        }
    }
}
