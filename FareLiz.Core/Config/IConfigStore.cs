namespace SkyDean.FareLiz.Core.Config
{
    /// <summary>Interface for storing the configuration objects</summary>
    public interface IConfigStore
    {
        /// <summary>Load the working environment based on stored configurations</summary>
        /// <returns>Working environment</returns>
        MonitorEnvironment LoadEnv();

        /// <summary>
        /// Load the working environment from an existing configuration file
        /// </summary>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        /// <returns>
        /// Working environment
        /// </returns>
        MonitorEnvironment LoadEnv(string filePath);

        /// <summary>
        /// Save the environment configurations
        /// </summary>
        /// <param name="env">
        /// Target working environment
        /// </param>
        void SaveEnv(MonitorEnvironment env);

        /// <summary>
        /// Save the environment configurations to specific file
        /// </summary>
        /// <param name="env">
        /// Target working environment
        /// </param>
        /// <param name="filePath">
        /// Target file path
        /// </param>
        void SaveEnv(MonitorEnvironment env, string filePath);
    }
}