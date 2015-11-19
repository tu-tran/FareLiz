namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    /// <summary>
    /// The Logger interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Debug(object message);

        /// <summary>
        /// The debug.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        void Debug(object message, Exception exception);

        /// <summary>
        /// The debug format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        void DebugFormat(string format, params object[] args);

        /// <summary>
        /// The debug format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        void DebugFormat(string format, object arg0);

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
        void DebugFormat(string format, object arg0, object arg1);

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
        void DebugFormat(string format, object arg0, object arg1, object arg2);

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
        void DebugFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Info(object message);

        /// <summary>
        /// The info.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        void Info(object message, Exception exception);

        /// <summary>
        /// The info format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        void InfoFormat(string format, params object[] args);

        /// <summary>
        /// The info format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        void InfoFormat(string format, object arg0);

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
        void InfoFormat(string format, object arg0, object arg1);

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
        void InfoFormat(string format, object arg0, object arg1, object arg2);

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
        void InfoFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Warn(object message);

        /// <summary>
        /// The warn.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        void Warn(object message, Exception exception);

        /// <summary>
        /// The warn format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        void WarnFormat(string format, params object[] args);

        /// <summary>
        /// The warn format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        void WarnFormat(string format, object arg0);

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
        void WarnFormat(string format, object arg0, object arg1);

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
        void WarnFormat(string format, object arg0, object arg1, object arg2);

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
        void WarnFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Error(object message);

        /// <summary>
        /// The error.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        void Error(object message, Exception exception);

        /// <summary>
        /// The error format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        void ErrorFormat(string format, params object[] args);

        /// <summary>
        /// The error format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        void ErrorFormat(string format, object arg0);

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
        void ErrorFormat(string format, object arg0, object arg1);

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
        void ErrorFormat(string format, object arg0, object arg1, object arg2);

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
        void ErrorFormat(IFormatProvider provider, string format, params object[] args);

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void Fatal(object message);

        /// <summary>
        /// The fatal.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        void Fatal(object message, Exception exception);

        /// <summary>
        /// The fatal format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        void FatalFormat(string format, params object[] args);

        /// <summary>
        /// The fatal format.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arg0">
        /// The arg 0.
        /// </param>
        void FatalFormat(string format, object arg0);

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
        void FatalFormat(string format, object arg0, object arg1);

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
        void FatalFormat(string format, object arg0, object arg1, object arg2);

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
        void FatalFormat(IFormatProvider provider, string format, params object[] args);
    }
}