namespace SkyDean.FareLiz.Core.Config
{
    /// <summary>
    /// Interface for helper object (which is used to build the configuration)
    /// </summary>
    public interface IConfigBuilder
    {
        /// <summary>
        /// Configure the selected object (e.g. show dialog to users, add extra logics for determining the configuration)
        /// </summary>
        /// <returns>Resulting configuration</returns>
        IConfig Configure(IPlugin targetPlugin);
    }
}
