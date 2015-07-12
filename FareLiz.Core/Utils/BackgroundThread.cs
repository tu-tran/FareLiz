using log4net;
using System;
using System.Threading;
using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.Core.Utils
{
    public static class BackgroundThread
    {
        public static ThreadResult DoWork(Action action, int timeoutInSeconds, string threadName, ILog _logger)
        {
            ThreadResult result = null;
            Thread workerThread = new Thread(new ThreadStart(() =>
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
            }));

            workerThread.Name = threadName;
            workerThread.Start();
            workerThread.Join(TimeSpan.FromSeconds(timeoutInSeconds));

            if (result == null) // The action did not complete
            {
                try { workerThread.Abort(); }
                catch { }

                result = ThreadResult.Timeout(timeoutInSeconds);
            }

            return result;
        }

        public static ThreadResult ExecuteTask(string title, string text, string threadName, IProgressCallback callback, ILog logger,
            CallbackDelegate action, CallbackExceptionDelegate exceptionHandler, CallbackExceptionDelegate finalHandler, bool notifyError)
        {
            var result = DoWork(delegate
            {
                threadName = String.IsNullOrEmpty(threadName) ? title.Replace(" ", "") : threadName;
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
                            logger.Error((String.IsNullOrEmpty(currentTitle) ? null : currentTitle + ": ") + ex);
                            if (notifyError)
                                callback.Inform(callback, "An error occured: " + ex.Message, currentTitle, NotificationType.Error);
                        }

                        if (exceptionHandler != null)
                            exceptionHandler(callback, ex);
                    }
                }
                finally
                {
                    if (callback != null)
                        callback.End();

                    if (finalHandler != null)
                        finalHandler(callback, actionException);
                }
            }, Int32.MaxValue, threadName, logger);

            return result;
        }
    }
}
