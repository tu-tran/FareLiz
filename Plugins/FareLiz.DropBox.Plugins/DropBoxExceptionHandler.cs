namespace SkyDean.FareLiz.DropBox
{
    using System;

    using DropNet.Exceptions;

    using SkyDean.FareLiz.Core;

    /// <summary>The drop box exception handler.</summary>
    [ExceptionHandler(typeof(DropboxException))]
    public class DropBoxExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// The process exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="Exception"/>.
        /// </returns>
        public Exception ProcessException(Exception ex)
        {
            return HandleException(ex);
        }

        /// <summary>
        /// The handle exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="Exception"/>.
        /// </returns>
        public static Exception HandleException(Exception ex)
        {
            var restEx = ex as DropboxException;
            if (restEx == null)
            {
                return ex;
            }

            var msg = restEx.Message + Environment.NewLine + "Make sure that you have properly configured the plugin authentication!";
            return new ApplicationException(msg, ex);
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Log(Exception ex, ILogger logger)
        {
            var realEx = HandleException(ex);
            var err = "Failed to synchronize data with DropBox: ";
            if (realEx == null)
            {
                err += ex.Message;
            }
            else
            {
                err += realEx.Message;
                logger.Error(err);
            }

            return err;
        }
    }
}