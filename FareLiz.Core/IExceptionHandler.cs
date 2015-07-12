using System;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for handling specific types of exception
    /// </summary>
    public interface IExceptionHandler
    {
        Exception ProcessException(Exception ex);
    }
}
