namespace SkyDean.FareLiz.Core
{
    using System;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>The plugin extension.</summary>
    public static class PluginExtension
    {
        /// <summary>
        /// The get plugin detail.
        /// </summary>
        /// <param name="pluginType">
        /// The plugin type.
        /// </param>
        /// <returns>
        /// The <see cref="KeyValue"/>.
        /// </returns>
        public static KeyValue<string, string> GetPluginDetail(this Type pluginType)
        {
            var result = new KeyValue<string, string>(string.Empty, string.Empty);
            var attribs = pluginType.GetCustomAttributes(false);
            foreach (var a in attribs)
            {
                var name = a as DisplayNameAttribute;
                if (name != null)
                {
                    result.Key = name.DisplayName;
                }

                var desc = a as DescriptionAttribute;
                if (desc != null)
                {
                    result.Value = desc.Description;
                }
            }

            if (string.IsNullOrEmpty(result.Key))
            {
                result.Key = pluginType.Name;
            }

            return result;
        }

        /// <summary>
        /// The get detail.
        /// </summary>
        /// <param name="plugin">
        /// The plugin.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="KeyValue"/>.
        /// </returns>
        public static KeyValue<string, string> GetDetail<T>(this T plugin) where T : IPlugin
        {
            return GetPluginDetail(plugin.GetType());
        }
    }
}