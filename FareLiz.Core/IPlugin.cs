namespace SkyDean.FareLiz.Core
{
    using SkyDean.FareLiz.Core.Config;

    /// <summary>Interface for all plugin types</summary>
    public interface IPlugin : IConfigurable
    {
        /// <summary>Gets or sets the logger.</summary>
        ILogger Logger { get; set; }

        /// <summary>Initialize the plugin. This is normally called after object creation</summary>
        void Initialize();
    }
}