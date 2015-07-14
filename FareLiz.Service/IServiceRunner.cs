namespace SkyDean.FareLiz.Service
{
    using SkyDean.FareLiz.Core;

    /// <summary>Interface for service plugin</summary>
    public interface IServiceRunner : IPlugin
    {
        /// <summary>
        /// Starts the service
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        void RunService(string[] args);
    }
}