namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    public class ThreadResult
    {
        public Exception Exception { get; private set; }
        public bool Succeeded { get; private set; }
        public bool IsTimedout
        {
            get { return !this.Succeeded && this.Exception is TimeoutException; }
        }

        protected ThreadResult(bool succeeded, Exception ex)
        {
            if (!succeeded && ex == null)
                throw new ArgumentException("Exception cannot be nulled for failed result");
            this.Succeeded = succeeded;
            this.Exception = ex;
        }

        public static ThreadResult Timeout(int timeoutInSeconds)
        {
            return new ThreadResult(false, new TimeoutException(String.Format("Task did not finish within {0}s", timeoutInSeconds)));
        }

        public static ThreadResult Succeed()
        {
            return new ThreadResult(true, null);
        }

        public static ThreadResult Fail(Exception ex)
        {
            return new ThreadResult(false, ex);
        }
    }
}