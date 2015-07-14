namespace SkyDean.FareLiz.Core
{
    using System;

    /// <summary>Result of validation</summary>
    public struct ValidateResult
    {
        /// <summary>Create a new object for successful validation</summary>
        public static readonly ValidateResult Success = new ValidateResult(true, null);

        /// <summary>Returns the error message for failed validation</summary>
        public readonly string ErrorMessage;

        /// <summary>Returns true if the validation succeeded</summary>
        public readonly bool Succeeded;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateResult"/> struct.
        /// </summary>
        /// <param name="succeeded">
        /// The succeeded.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public ValidateResult(bool succeeded, string error)
        {
            if (!succeeded && string.IsNullOrEmpty(error))
            {
                throw new ArgumentException("Error cannot be empty for failed validation");
            }

            this.Succeeded = succeeded;
            this.ErrorMessage = error;
        }
    }
}