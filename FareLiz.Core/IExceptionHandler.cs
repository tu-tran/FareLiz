namespace SkyDean.FareLiz.Core
{
    using System;

    /// <summary>Interface for handling specific types of exception</summary>
    public interface IExceptionHandler
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
        Exception ProcessException(Exception ex);
    }
}