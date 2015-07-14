namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    /// <summary>The thread result.</summary>
    public class ThreadResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadResult"/> class.
        /// </summary>
        /// <param name="succeeded">
        /// The succeeded.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        protected ThreadResult(bool succeeded, Exception ex)
        {
            if (!succeeded && ex == null)
            {
                throw new ArgumentException("Exception cannot be nulled for failed result");
            }

            this.Succeeded = succeeded;
            this.Exception = ex;
        }

        /// <summary>Gets the exception.</summary>
        public Exception Exception { get; private set; }

        /// <summary>Gets a value indicating whether succeeded.</summary>
        public bool Succeeded { get; private set; }

        /// <summary>Gets a value indicating whether is timedout.</summary>
        public bool IsTimedout
        {
            get
            {
                return !this.Succeeded && this.Exception is TimeoutException;
            }
        }

        /// <summary>
        /// The timeout.
        /// </summary>
        /// <param name="timeoutInSeconds">
        /// The timeout in seconds.
        /// </param>
        /// <returns>
        /// The <see cref="ThreadResult"/>.
        /// </returns>
        public static ThreadResult Timeout(int timeoutInSeconds)
        {
            return new ThreadResult(false, new TimeoutException(string.Format("Task did not finish within {0}s", timeoutInSeconds)));
        }

        /// <summary>The succeed.</summary>
        /// <returns>The <see cref="ThreadResult" />.</returns>
        public static ThreadResult Succeed()
        {
            return new ThreadResult(true, null);
        }

        /// <summary>
        /// The fail.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="ThreadResult"/>.
        /// </returns>
        public static ThreadResult Fail(Exception ex)
        {
            return new ThreadResult(false, ex);
        }
    }
}