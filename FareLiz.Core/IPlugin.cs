namespace SkyDean.FareLiz.Core
{
    using log4net;

    using SkyDean.FareLiz.Core.Config;

    /// <summary>Interface for all plugin types</summary>
    public interface IPlugin : IConfigurable
    {
        /// <summary>Gets or sets the logger.</summary>
        ILog Logger { get; set; }

        /// <summary>Initialize the plugin. This is normally called after object creation</summary>
        void Initialize();
    }
}