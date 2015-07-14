namespace SkyDean.FareLiz.Core
{
    /// <summary>Interface for helper service</summary>
    public interface IHelperService : IPlugin
    {
        /// <summary>Returns the current status of service</summary>
        HelperServiceStatus Status { get; }

        /// <summary>Start the helper service (in background)</summary>
        void Start();

        /// <summary>Stop the background helper service</summary>
        void Stop();
    }

    /// <summary>The status of helper service</summary>
    public enum HelperServiceStatus
    {
        /// <summary>The stopped.</summary>
        Stopped, 

        /// <summary>The stopping.</summary>
        Stopping, 

        /// <summary>The running.</summary>
        Running
    }
}