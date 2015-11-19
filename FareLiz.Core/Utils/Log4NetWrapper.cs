namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    using log4net;

    /// <summary>
    /// The log 4 net wrapper.
    /// </summary>
    internal class Log4NetWrapper : ILogger
    {
        /// <summary>
        /// The _logger.
        /// </summary>
        private ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetWrapper"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public Log4NetWrapper(ILog logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Debug(object message)
        {
            this._logger.Debug(message);
        }

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Debug(object message, Exception exception)
        {
            this._logger.Debug(message, exception);
        }

        /// <summary>
        /// The debug format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void DebugFormat(string format, params object[] args)
        {
            this._logger.DebugFormat(format, args);
        }

        /// <summary>
        /// The debug format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public void DebugFormat(string format, object arg0)
        {
            this._logger.DebugFormat(format, arg0);
        }

        /// <summary>
        /// The debug format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public void DebugFormat(string format, object arg0, object arg1)
        {
            this._logger.DebugFormat(format, arg0, arg1);
        }

        /// <summary>
        /// The debug format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            this._logger.DebugFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// The debug format.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._logger.DebugFormat(provider, format, args);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Info(object message)
        {
            this._logger.Info(message);
        }

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Info(object message, Exception exception)
        {
            this._logger.Info(message, exception);
        }

        /// <summary>
        /// The info format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void InfoFormat(string format, params object[] args)
        {
            this._logger.InfoFormat(format, args);
        }

        /// <summary>
        /// The info format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public void InfoFormat(string format, object arg0)
        {
            this._logger.InfoFormat(format, arg0);
        }

        /// <summary>
        /// The info format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public void InfoFormat(string format, object arg0, object arg1)
        {
            this._logger.InfoFormat(format, arg0, arg1);
        }

        /// <summary>
        /// The info format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            this._logger.InfoFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// The info format.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._logger.InfoFormat(provider, format, args);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Warn(object message)
        {
            this._logger.Warn(message);
        }

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Warn(object message, Exception exception)
        {
            this._logger.Warn(message, exception);
        }

        /// <summary>
        /// The warn format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void WarnFormat(string format, params object[] args)
        {
            this._logger.WarnFormat(format, args);
        }

        /// <summary>
        /// The warn format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public void WarnFormat(string format, object arg0)
        {
            this._logger.WarnFormat(format, arg0);
        }

        /// <summary>
        /// The warn format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public void WarnFormat(string format, object arg0, object arg1)
        {
            this._logger.WarnFormat(format, arg0, arg1);
        }

        /// <summary>
        /// The warn format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            this._logger.WarnFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// The warn format.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._logger.WarnFormat(provider, format, args);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Error(object message)
        {
            this._logger.Error(message);
        }

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Error(object message, Exception exception)
        {
            this._logger.Error(message, exception);
        }

        /// <summary>
        /// The error format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void ErrorFormat(string format, params object[] args)
        {
            this._logger.ErrorFormat(format, args);
        }

        /// <summary>
        /// The error format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public void ErrorFormat(string format, object arg0)
        {
            this._logger.ErrorFormat(format, arg0);
        }

        /// <summary>
        /// The error format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public void ErrorFormat(string format, object arg0, object arg1)
        {
            this._logger.ErrorFormat(format, arg0, arg1);
        }

        /// <summary>
        /// The error format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            this._logger.ErrorFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// The error format.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._logger.ErrorFormat(provider, format, args);
        }

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Fatal(object message)
        {
            this._logger.Fatal(message);
        }

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void Fatal(object message, Exception exception)
        {
            this._logger.Fatal(message, exception);
        }

        /// <summary>
        /// The fatal format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void FatalFormat(string format, params object[] args)
        {
            this._logger.FatalFormat(format, args);
        }

        /// <summary>
        /// The fatal format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        public void FatalFormat(string format, object arg0)
        {
            this._logger.FatalFormat(format, arg0);
        }

        /// <summary>
        /// The fatal format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        public void FatalFormat(string format, object arg0, object arg1)
        {
            this._logger.FatalFormat(format, arg0, arg1);
        }

        /// <summary>
        /// The fatal format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <param name="arg2">
        /// The arg 2.
        /// </param>
        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            this._logger.FatalFormat(format, arg0, arg1, arg2);
        }

        /// <summary>
        /// The fatal format.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            this._logger.FatalFormat(provider, format, args);
        }
    }
}