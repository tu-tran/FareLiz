using SkyDean.FareLiz.Core;

namespace SkyDean.FareLiz.Service
{
    /// <summary>
    /// Interface for service plugin
    /// </summary>
    public interface IServiceRunner : IPlugin
    {
        /// <summary>
        /// Starts the service
        /// </summary>
        void RunService(string[] args);
    }
}
