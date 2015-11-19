namespace SkyDean.FareLiz.DropBox
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;

    using DropNet.Exceptions;

    using SkyDean.FareLiz.Core;

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

            string msg = restEx.Response.ErrorMessage;
            if (string.IsNullOrEmpty(msg))
            {
                var resp = restEx.Response.Content;
                if (!string.IsNullOrEmpty(resp))
                {
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        var data = serializer.DeserializeObject(resp) as Dictionary<string, object>;
                        object errData = null;
                        if (data != null && data.TryGetValue("error", out errData))
                        {
                            if (errData != null)
                            {
                                msg = errData + Environment.NewLine + "Make sure that you have properly configured the plugin authentication!";
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return new ApplicationException(msg, ex);
        }
    }
}