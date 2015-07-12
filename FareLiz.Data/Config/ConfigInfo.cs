using System;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;

namespace SkyDean.FareLiz.Data.Config
{
    /// <summary>
    /// Contains configuration for plugin
    /// </summary>
    [Serializable]
    public class ConfigInfo
    {
        public Type ConfiguredType { get; set; }
        public IConfig TypeConfiguration { get; set; }

        public ConfigInfo(IPlugin targetObject)
        {
            if (targetObject != null)
            {
                ConfiguredType = targetObject.GetType();
                TypeConfiguration = targetObject.Configuration;
            }
        }
    }
}
