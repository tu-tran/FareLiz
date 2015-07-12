using SkyDean.FareLiz.Core;
using System;
using System.Reflection;
using System.Text;

namespace SkyDean.FareLiz.Service
{
    /// <summary>
    /// Generic handler for unhandled exception. Print out the details for some nested exception types
    /// </summary>
    public class GenericExceptionHandler : IExceptionHandler
    {
        /// <summary>
        /// Process the exception and return the result exception type. Return null if the exception can be ignored
        /// </summary>
        /// <param name="ex">Target exception</param>
        public Exception ProcessException(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine(ex.GetType() + ": " + ex.Message);

            var loadException = ex as ReflectionTypeLoadException;
            if (loadException != null)
            {
                sb.AppendLine("Type Load Exception: ");
                foreach (var loadEx in loadException.LoaderExceptions)
                    sb.AppendLine(loadEx.Message);
            }

            var innerEx = ex.InnerException;
            if (innerEx != null)
                sb.AppendLine("Inner Exception - " + innerEx.GetType() + ": " + innerEx.Message);

            string err = sb.ToString().Trim(Environment.NewLine.ToCharArray());
            return new UnhandledExcetion(err, ex);
        }
    }

    /// <summary>
    /// Specific class used to determine an exception which can not be properly handled
    /// </summary>
    public class UnhandledExcetion : Exception
    {
        public UnhandledExcetion(string message, Exception innerException) : base(message, innerException) { }
    }
}
