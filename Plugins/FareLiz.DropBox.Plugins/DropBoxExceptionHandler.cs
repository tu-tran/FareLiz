using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using DropNet.Exceptions;
using SkyDean.FareLiz.Core;

namespace SkyDean.FareLiz.DropBox
{
    [ExceptionHandler(typeof(DropboxException))]
    public class DropBoxExceptionHandler : IExceptionHandler
    {
        public Exception ProcessException(Exception ex)
        {
            return HandleException(ex);
        }

        public static Exception HandleException(Exception ex)
        {
            var restEx = ex as DropboxException;
            if (restEx == null)
                return ex;

            string msg = restEx.Response.ErrorMessage;
            if (String.IsNullOrEmpty(msg))
            {
                var resp = restEx.Response.Content;
                if (!String.IsNullOrEmpty(resp))
                    try
                    {
                        var serializer = new JavaScriptSerializer();
                        var data = serializer.DeserializeObject(resp) as Dictionary<string, object>;
                        object errData = null;
                        if (data != null && data.TryGetValue("error", out errData))
                        {
                            if (errData != null)
                                msg = errData.ToString() + Environment.NewLine + "Make sure that you have properly configured the plugin authentication!";
                        }
                    }
                    catch { }
            }

            return new ApplicationException(msg, ex);
        }
    }
}
