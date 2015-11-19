namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Threading;

    using log4net;

    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>The background thread.</summary>
    public static class BackgroundThread
    {
        /// <summary>
        /// The do work.
        /// </summary>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="timeoutInSeconds">
        /// The timeout in seconds.
        /// </param>
        /// <param name="threadName">
        /// The thread name.
        /// </param>
        /// <param name="_logger">
        /// The _logger.
        /// </param>
        /// <returns>
        /// The <see cref="ThreadResult"/>.
        /// </returns>
        public static ThreadResult DoWork(Action action, int timeoutInSeconds, string threadName, ILog _logger)
        {
            ThreadResult result = null;
            Thread workerThread = new Thread(
                () =>
                    {
                        try
                        {
                            action();
                            result = ThreadResult.Succeed();
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex);
                            result = ThreadResult.Fail(ex);
                        }
                    });

            workerThread.Name = threadName;
            workerThread.Start();
            workerThread.Join(TimeSpan.FromSeconds(timeoutInSeconds));

            if (result == null)
            {
                // The action did not complete
                try
                {
                    workerThread.Abort();
                }
                catch
                {
                }

                result = ThreadResult.Timeout(timeoutInSeconds);
            }

            return result;
        }

        /// <summary>
        /// The execute task.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threadName">
        /// The thread name.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="exceptionHandler">
        /// The exception handler.
        /// </param>
        /// <param name="finalHandler">
        /// The final handler.
        /// </param>
        /// <param name="notifyError">
        /// The notify error.
        /// </param>
        /// <returns>
        /// The <see cref="ThreadResult"/>.
        /// </returns>
        public static ThreadResult ExecuteTask(
            string title, 
            string text, 
            string threadName, 
            IProgressCallback callback, 
            ILog logger, 
            CallbackDelegate action, 
            CallbackExceptionDelegate exceptionHandler, 
            CallbackExceptionDelegate finalHandler, 
            bool notifyError)
        {
            var result = DoWork(
                delegate
                    {
                        threadName = string.IsNullOrEmpty(threadName) ? title.Replace(" ", string.Empty) : threadName;
                        AppUtil.NameCurrentThread(threadName);
                        Exception actionException = null;

                        try
                        {
                            action(callback);
                        }
                        catch (Exception ex)
                        {
                            actionException = ex;
                            if (!callback.IsAborting)
                            {
                                if (logger != null)
                                {
                                    string currentTitle = callback.Title;
                                    logger.Error((string.IsNullOrEmpty(currentTitle) ? null : currentTitle + ": ") + ex);
                                    if (notifyError)
                                    {
                                        callback.Inform(callback, "An error occured: " + ex.Message, currentTitle, NotificationType.Error);
                                    }
                                }

                                if (exceptionHandler != null)
                                {
                                    exceptionHandler(callback, ex);
                                }
                            }
                        }
                        finally
                        {
                            if (callback != null)
                            {
                                callback.End();
                            }

                            if (finalHandler != null)
                            {
                                finalHandler(callback, actionException);
                            }
                        }
                    }, 
                int.MaxValue, 
                threadName, 
                logger);

            return result;
        }
    }
}