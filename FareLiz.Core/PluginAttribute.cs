namespace SkyDean.FareLiz.Core
{
    using System;

    /// <summary>Attribute used for storing the plugin details</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PluginAttribute : Attribute
    {
        /// <summary>The _description.</summary>
        private readonly string _description;

        /// <summary>The _name.</summary>
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginAttribute"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public PluginAttribute(string name, string description)
        {
            this._name = name;
            this._description = description;
        }

        /// <summary>Name of the plugin</summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        /// <summary>Plugin's description</summary>
        public string Description
        {
            get
            {
                return this._description;
            }
        }
    }
}