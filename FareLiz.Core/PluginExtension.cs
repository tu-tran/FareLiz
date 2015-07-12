namespace SkyDean.FareLiz.Core
{
    using System;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core.Utils;

    public static class PluginExtension
    {
        public static KeyValue<string, string> GetPluginDetail(this Type pluginType)
        {
            var result = new KeyValue<string, string>(String.Empty, String.Empty);
            var attribs = pluginType.GetCustomAttributes(false);
            foreach (var a in attribs)
            {
                var name = a as DisplayNameAttribute;
                if (name != null)
                    result.Key = name.DisplayName;

                var desc = a as DescriptionAttribute;
                if (desc != null)
                    result.Value = desc.Description;
            }

            if (String.IsNullOrEmpty(result.Key))
                result.Key = pluginType.Name;

            return result;
        }

        public static KeyValue<string, string> GetDetail<T>(this T plugin) where T : IPlugin
        {
            return GetPluginDetail(plugin.GetType());
        }
    }
}