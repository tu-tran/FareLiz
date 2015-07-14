namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.Threading;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Service.LiveUpdate;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Data;

    /// <summary>The win form global context.</summary>
    internal class WinFormGlobalContext : AppContext
    {
        /// <summary>The _instance.</summary>
        private static WinFormGlobalContext _instance;

        /// <summary>The _is first start set.</summary>
        private static bool _isFirstStartSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormGlobalContext"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        private WinFormGlobalContext(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>The get instance.</summary>
        /// <returns>The <see cref="WinFormGlobalContext" />.</returns>
        internal static WinFormGlobalContext GetInstance()
        {
            return GetInstance(LogUtil.GetLogger("WinFormGlobalContext"));
        }

        /// <summary>
        /// The get instance.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="WinFormGlobalContext"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal static WinFormGlobalContext GetInstance(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentException("Logger cannot be null for Global Context instance");
            }

            if (_instance == null)
            {
                _instance = new WinFormGlobalContext(logger);
            }
            else
            {
                _instance._logger = logger;
            }

            return _instance;
        }

        /// <summary>
        /// The set first start.
        /// </summary>
        /// <param name="firstStart">
        /// The first start.
        /// </param>
        /// <exception cref="ApplicationException">
        /// </exception>
        internal void SetFirstStart(bool firstStart)
        {
            if (_isFirstStartSet)
            {
                throw new ApplicationException("This property cannot be set more than once!");
            }

            _isFirstStartSet = true;
            this._firstStart = firstStart;
        }

        /// <summary>
        /// The set environment.
        /// </summary>
        /// <param name="env">
        /// The env.
        /// </param>
        internal void SetEnvironment(MonitorEnvironment env)
        {
            this._err = null;
            if (env != null)
            {
                this._logger.Debug("Closing global environment...");
                env.Close();
            }

            this._logger.Info("Setting new global environment...");
            this.AddServices(env); // Add needed services to this environment instance
            this._env = env;

            if (this._env != null)
            {
                this._logger.Debug("Initializing global environment...");
                var result = this._env.Initialize();
                if (!result.Succeeded)
                {
                    this._err = result.ErrorMessage;
                    this._logger.Warn("Error initializing environment: " + this._err);
                }
            }
        }

        /// <summary>
        /// The add services.
        /// </summary>
        /// <param name="env">
        /// The env.
        /// </param>
        internal void AddServices(MonitorEnvironment env)
        {
            LiveUpdateService liveUpdateService = null;
            OnlineCurrencyProvider currencyProvider = null;

            var activeServices = env.BackgroundServices.GetAll();
            foreach (var svc in activeServices)
            {
                if (svc is LiveUpdateService)
                {
                    liveUpdateService = (LiveUpdateService)svc;
                }
                else if (svc is OnlineCurrencyProvider)
                {
                    currencyProvider = (OnlineCurrencyProvider)svc;
                }
            }

            if (liveUpdateService == null)
            {
                this._logger.Debug("Adding Live Update Service...");
                liveUpdateService = new LiveUpdateService();
                liveUpdateService.Configuration = liveUpdateService.DefaultConfig;
                env.BackgroundServices.Add(liveUpdateService, false);
            }

            liveUpdateService.ProductLogo = Properties.Resources.FareLiz;

            if (currencyProvider == null)
            {
                this._logger.Debug("Adding Currency Provider...");
                env.CurrencyProvider = currencyProvider = new OnlineCurrencyProvider();
                currencyProvider.Configuration = currencyProvider.DefaultConfig;
                env.BackgroundServices.Add(currencyProvider, false);
            }

            this._logger.Info("Total loaded services: " + env.BackgroundServices.Count);
        }

        /// <summary>
        /// Execute a delegate on a separate thread and show a progress dialog
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threadName">
        /// The thread Name.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="exceptionHandler">
        /// The exception Handler.
        /// </param>
        /// <param name="finalHandler">
        /// The final Handler.
        /// </param>
        /// <param name="notifyError">
        /// The notify Error In Msg Box.
        /// </param>
        /// <param name="visible">
        /// The visible.
        /// </param>
        protected override void DoExecuteTask(object owner, 
                                              string title, 
                                              string text, 
                                              string threadName, 
                                              ProgressStyle style, 
                                              ILogger logger, 
                                              Action<IProgressCallback> action, 
                                              CallbackExceptionDelegate exceptionHandler, 
                                              CallbackExceptionDelegate finalHandler, 
                                              bool notifyError, 
                                              bool visible)
        {
            using (var progressDialog = new ProgressDialog(title, text, style, true, visible))
            {
                var target = owner as IWin32Window;
                ThreadPool.QueueUserWorkItem(
                    delegate(object param)
                        {
                            threadName = string.IsNullOrEmpty(threadName) ? title.Replace(" ", string.Empty) : threadName;
                            AppUtil.NameCurrentThread(threadName);
                            Exception actionException = null;
                            var callback = (IProgressCallback)param;

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
                                        var currentTitle = callback.Title;
                                        logger.Error((string.IsNullOrEmpty(currentTitle) ? null : currentTitle + ": ") + ex);
                                        if (notifyError)
                                        {
                                            callback.Inform(owner, "An error occured: " + ex.Message, currentTitle, NotificationType.Error);
                                        }
                                    }

                                    if (exceptionHandler != null)
                                    {
                                        exceptionHandler(callback, ex);
                                    }
                                    else
                                    {
                                        throw;
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
                    progressDialog);
                progressDialog.ShowDialog(target);
            }
        }
    }
}