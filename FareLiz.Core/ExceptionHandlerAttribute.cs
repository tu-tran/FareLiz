namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>Define the types of exceptions which can be handled</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExceptionHandlerAttribute : Attribute
    {
        /// <summary>The _handled expcetions.</summary>
        private List<Type> _handledExpcetions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlerAttribute"/> class.
        /// </summary>
        /// <param name="targetExceptionTypes">
        /// The target exception types.
        /// </param>
        public ExceptionHandlerAttribute(params Type[] targetExceptionTypes)
        {
            if (targetExceptionTypes != null && targetExceptionTypes.Length > 0)
            {
                this._handledExpcetions = new List<Type>();
                var exType = typeof(Exception);

                foreach (var t in targetExceptionTypes)
                {
                    if (exType.IsAssignableFrom(t))
                    {
                        this.HandledExpcetions.Add(t);
                    }
                }
            }
        }

        /// <summary>List of supported exception types</summary>
        public List<Type> HandledExpcetions
        {
            get
            {
                return this._handledExpcetions;
            }
        }
    }
}