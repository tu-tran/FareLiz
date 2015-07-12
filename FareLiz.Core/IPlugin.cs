using log4net;

using SkyDean.FareLiz.Core.Config;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for all plugin types
    /// </summary>
    public interface IPlugin : IConfigurable
    {
        /// <summary>
        /// Initialize the plugin. This is normally called after object creation
        /// </summary>
        void Initialize();

        ILog Logger { get; set; }
    }
}
