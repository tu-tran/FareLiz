namespace SkyDean.FareLiz.Core.Config
{
    /// <summary>Interface for objects which are configurable</summary>
    public interface IConfigurable
    {
        /// <summary>Get current configuration</summary>
        IConfig Configuration { get; set; }

        /// <summary>Get initial/default configuration</summary>
        IConfig DefaultConfig { get; }

        /// <summary>Custom builder object to configure this object</summary>
        IConfigBuilder CustomConfigBuilder { get; }
    }
}