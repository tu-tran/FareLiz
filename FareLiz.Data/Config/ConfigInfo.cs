namespace SkyDean.FareLiz.Data.Config
{
    using System;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;

    /// <summary>Contains configuration for plugin</summary>
    [Serializable]
    public class ConfigInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigInfo"/> class.
        /// </summary>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        public ConfigInfo(IPlugin targetObject)
        {
            if (targetObject != null)
            {
                this.ConfiguredType = targetObject.GetType();
                this.TypeConfiguration = targetObject.Configuration;
            }
        }

        /// <summary>Gets or sets the configured type.</summary>
        public Type ConfiguredType { get; set; }

        /// <summary>Gets or sets the type configuration.</summary>
        public IConfig TypeConfiguration { get; set; }
    }
}