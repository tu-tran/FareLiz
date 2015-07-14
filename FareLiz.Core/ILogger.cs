namespace SkyDean.FareLiz.Core
{
    /// <summary>The logger interface.</summary>
    public interface ILogger
    {
        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        void Info(string format, params object[] arguments);

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        void Debug(string format, params object[] arguments);

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        void Warn(string format, params object[] arguments);

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        void Error(string format, params object[] arguments);

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        void Fatal(string format, params object[] arguments);
    }
}