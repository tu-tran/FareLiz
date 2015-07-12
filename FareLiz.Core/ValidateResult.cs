using System;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Result of validation
    /// </summary>
    public struct ValidateResult
    {
        /// <summary>
        /// Returns true if the validation succeeded
        /// </summary>
        public readonly bool Succeeded;

        /// <summary>
        /// Returns the error message for failed validation
        /// </summary>
        public readonly string ErrorMessage;

        public ValidateResult(bool succeeded, string error)
        {
            if (!succeeded && String.IsNullOrEmpty(error))
                throw new ArgumentException("Error cannot be empty for failed validation");

            Succeeded = succeeded;
            ErrorMessage = error;
        }

        /// <summary>
        /// Create a new object for successful validation
        /// </summary>
        public static readonly ValidateResult Success = new ValidateResult(true, null);
    }
}
