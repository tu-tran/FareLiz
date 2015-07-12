namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Define the types of exceptions which can be handled
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExceptionHandlerAttribute : Attribute
    {
        private List<Type> _handledExpcetions;
        /// <summary>
        /// List of supported exception types
        /// </summary>
        public List<Type> HandledExpcetions { get { return this._handledExpcetions; } }

        public ExceptionHandlerAttribute(params Type[] targetExceptionTypes)
        {
            if (targetExceptionTypes != null && targetExceptionTypes.Length > 0)
            {
                this._handledExpcetions = new List<Type>();
                var exType = typeof(Exception);

                foreach (var t in targetExceptionTypes)
                {
                    if (exType.IsAssignableFrom(t))
                        this.HandledExpcetions.Add(t);
                }
            }

        }
    }
}