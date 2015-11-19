namespace SkyDean.FareLiz.WinForm
{
    using System;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Service.LiveUpdate;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Data;
    using SkyDean.FareLiz.WinForm.Properties;

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
            this._progressCallback = new ProgressDialog();
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

            liveUpdateService.ProductLogo = Resources.FareLiz;

            if (currencyProvider == null)
            {
                this._logger.Debug("Adding Currency Provider...");
                env.CurrencyProvider = currencyProvider = new OnlineCurrencyProvider();
                currencyProvider.Configuration = currencyProvider.DefaultConfig;
                env.BackgroundServices.Add(currencyProvider, false);
            }

            this._logger.Info("Total loaded services: " + env.BackgroundServices.Count);
        }
    }
}