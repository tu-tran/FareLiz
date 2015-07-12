namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for helper service
    /// </summary>
    public interface IHelperService : IPlugin
    {
        /// <summary>
        /// Start the helper service (in background)
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the background helper service
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns the current status of service
        /// </summary>
        HelperServiceStatus Status { get; }
    }

    /// <summary>
    /// The status of helper service
    /// </summary>
    public enum HelperServiceStatus { Stopped, Stopping, Running }
}
