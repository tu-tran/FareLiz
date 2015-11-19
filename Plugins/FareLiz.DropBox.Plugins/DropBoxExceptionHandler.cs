namespace SkyDean.FareLiz.DropBox
{
    using DropNet.Exceptions;
    using SkyDean.FareLiz.Core;
    using System;

    /// <summary>
    /// The drop box exception handler.
    /// </summary>
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

            string msg = restEx.Message;
            return new ApplicationException(msg, ex);
        }
    }
}