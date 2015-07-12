using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Service.LiveUpdate;
using SkyDean.FareLiz.WinForm.Components.Dialog;
using SkyDean.FareLiz.WinForm.Data;
using System;

namespace SkyDean.FareLiz.WinForm
{
    using log4net;

    internal class WinFormGlobalContext : AppContext
    {
        private static WinFormGlobalContext _instance = null;

        internal static WinFormGlobalContext GetInstance()
        {
            return GetInstance(LogManager.GetLogger("WinFormGlobalContext"));
        }
        internal static WinFormGlobalContext GetInstance(ILog logger)
        {
            if (logger == null)
                throw new ArgumentException("Logger cannot be null for Global Context instance");
            if (_instance == null)
                _instance = new WinFormGlobalContext(logger);
            else
                _instance._logger = logger;

            return _instance;
        }

        private WinFormGlobalContext(ILog logger)
        {
            _logger = logger;
            _progressCallback = new ProgressDialog();
        }

        internal void SetFirstStart(bool firstStart)
        {
            if (_isFirstStartSet)
                throw new ApplicationException("This property cannot be set more than once!");
            _isFirstStartSet = true;
            _firstStart = firstStart;
        }
        private static bool _isFirstStartSet = false;

        internal void SetEnvironment(MonitorEnvironment env)
        {
            _err = null;
            if (env != null)
            {
                _logger.Debug("Closing global environment...");
                env.Close();
            }

            _logger.Info("Setting new global environment...");
            AddServices(env);   // Add needed services to this environment instance
            _env = env;

            if (_env != null)
            {
                _logger.Debug("Initializing global environment...");
                var result = _env.Initialize();
                if (!result.Succeeded)
                {
                    _err = result.ErrorMessage;
                    _logger.Warn("Error initializing environment: " + _err);
                }
            }
        }

        internal void AddServices(MonitorEnvironment env)
        {
            LiveUpdateService liveUpdateService = null;
            OnlineCurrencyProvider currencyProvider = null;

            var activeServices = env.BackgroundServices.GetAll();
            foreach (var svc in activeServices)
            {
                if (svc is LiveUpdateService)
                    liveUpdateService = (LiveUpdateService)svc;
                else if (svc is OnlineCurrencyProvider)
                    currencyProvider = (OnlineCurrencyProvider)svc;
            }

            if (liveUpdateService == null)
            {
                _logger.Debug("Adding Live Update Service...");
                liveUpdateService = new LiveUpdateService();
                liveUpdateService.Configuration = liveUpdateService.DefaultConfig;
                env.BackgroundServices.Add(liveUpdateService, false);
            }
            liveUpdateService.ProductLogo = Properties.Resources.FareLiz;

            if (currencyProvider == null)
            {
                _logger.Debug("Adding Currency Provider...");
                env.CurrencyProvider = currencyProvider = new OnlineCurrencyProvider();
                currencyProvider.Configuration = currencyProvider.DefaultConfig;
                env.BackgroundServices.Add(currencyProvider, false);
            }

            _logger.Info("Total loaded services: " + env.BackgroundServices.Count);
        }
    }
}