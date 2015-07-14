namespace SkyDean.FareLiz.Core.Utils
{
    using log4net;

    /// <summary>The logger wrapper using log4net.</summary>
    internal class Log4netLogger : ILogger
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILog logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4netLogger"/> class.
        /// </summary>
        /// <param name="wrappedLogger">
        /// The wrapped logger.
        /// </param>
        internal Log4netLogger(ILog wrappedLogger)
        {
            this.logger = wrappedLogger;
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public void Info(string format, params object[] arguments)
        {
            if (arguments == null || arguments.Length < 1)
            {
                this.logger.Info(format);
            }
            else
            {
                this.logger.InfoFormat(format, arguments);
            }
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public void Debug(string format, params object[] arguments)
        {
            if (arguments == null || arguments.Length < 1)
            {
                this.logger.Debug(format);
            }
            else
            {
                this.logger.DebugFormat(format, arguments);
            }
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public void Warn(string format, params object[] arguments)
        {
            if (arguments == null || arguments.Length < 1)
            {
                this.logger.Warn(format);
            }
            else
            {
                this.logger.WarnFormat(format, arguments);
            }
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public void Error(string format, params object[] arguments)
        {
            if (arguments == null || arguments.Length < 1)
            {
                this.logger.Error(format);
            }
            else
            {
                this.logger.ErrorFormat(format, arguments);
            }
        }

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        public void Fatal(string format, params object[] arguments)
        {
            if (arguments == null || arguments.Length < 1)
            {
                this.logger.Fatal(format);
            }
            else
            {
                this.logger.FatalFormat(format, arguments);
            }
        }
    }
}