using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Data.Config;
using SkyDean.FareLiz.Data.Monitoring;
using SkyDean.FareLiz.WinForm.Config;
using SkyDean.FareLiz.WinForm.Presentation;
using SkyDean.FareLiz.WinForm.Presentation.Controllers;
using SkyDean.FareLiz.WinForm.Presentation.Views;
using SkyDean.FareLiz.WinForm.Utils;
using System;
using System.IO;
using System.Windows.Forms;

namespace SkyDean.FareLiz.WinForm
{
    internal class BootStrap
    {
        private readonly ILog Logger;

        private readonly string ENV_CONFIG_FILENAME = "Environment.bin",
                                GUI_INI_FILENAME = AppUtil.ProductName + ".ini",
                                HISTORY_FILENAME = "History.txt";

        internal BootStrap(ILog logger)
        {
            if (logger == null)
                throw new ArgumentException("Logger cannot be null");

            Logger = logger;
        }

        internal void Run(string[] args)
        {
            ExecutionParam executionParam;
            if (!ExecutionParam.Parse(args, AppUtil.GetLocalDataPath(GUI_INI_FILENAME), Logger, out executionParam))
            {
                ExecutionParam.ShowHelp();
                return;
            }

            Logger.InfoFormat("Application {0} {1} was started!", AppUtil.ProductName, AppUtil.ProductVersion);
            MuteApplicationVolume();

            if (args.Length < 1 && !SingleInstance.Start()) // Show up other instance if no parameter was specified
            {
                if (MessageBox.Show("Another instance was already running. Do you want to start a new instance ?", AppUtil.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    SingleInstance.ShowFirstInstance();
                    return;
                }
            }

            string historyFile = AppUtil.GetLocalDataPath(HISTORY_FILENAME);
            bool firstStart = !File.Exists(historyFile);
            if (firstStart)
            {
                Directory.CreateDirectory(AppUtil.LocalProductDataPath);
                File.Create(historyFile);
                using (var introForm = new IntroForm())
                    introForm.ShowDialog();
            }

            var globalContext = WinFormGlobalContext.GetInstance(Logger);
            globalContext.SetFirstStart(firstStart);

            MonitorEnvironment env = GetEnvironment(executionParam, globalContext);
            if (env == null)
                Environment.Exit(1);

            env.Logger = Logger;
            globalContext.SetEnvironment(env);
            AppContext.Initialize(globalContext);
            var mainForm = new FlightStatisticForm(null, executionParam, true);
            CheckFareForm checkFareForm = null;

            if (executionParam != null && executionParam.OperationMode != OperationMode.Unspecified)
            {
                mainForm.Hide();
                mainForm.WindowState = FormWindowState.Minimized;
                var controller = new CheckFareController(executionParam);
                checkFareForm = new CheckFareForm(executionParam);
                checkFareForm.Attach(controller);

                if (executionParam.IsMinimized)
                {
                    checkFareForm.WindowState = FormWindowState.Minimized;
                    checkFareForm.ShowInTaskbar = false;
                }
                checkFareForm.Show();
            }

            Application.Run(mainForm);
            SingleInstance.Stop();
            Logger.Info("Application stopped");
        }

        private MonitorEnvironment GetEnvironment(ExecutionParam executionParam, WinFormGlobalContext globalContext)
        {
            var pluginResolver = new AssemblyPluginResolver(Logger);
            pluginResolver.LoadPlugins();
            var configStore = new FileConfigStore(AppUtil.GetLocalDataPath(ENV_CONFIG_FILENAME), pluginResolver, Logger);

            MonitorEnvironment env = configStore.LoadEnv();

            if (env == null || env.ArchiveManager == null || env.FareDatabase == null || env.FareDataProvider == null)
            {
                env = new MonitorEnvironment(configStore, pluginResolver, new BackgroundServiceManager(Logger), Logger);
                globalContext.AddServices(env);
                using (var configDialog = new EnvConfiguratorDialog(env, executionParam))
                {
                    if (configDialog.ShowDialog() == DialogResult.OK)
                        env = configDialog.ResultEnvironment;
                    else
                    {
                        env.Close();
                        env = null;
                    }
                }
            }

            return env;
        }

        private void MuteApplicationVolume()
        {
            try
            {
                WinApi.SetVolume(0);
                WinApi.CoInternetSetFeatureEnabled(WinApi.FEATURE_DISABLE_NAVIGATION_SOUNDS,
                                                   WinApi.SET_FEATURE_ON_PROCESS, true);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to mute volume: " + e);
            }
        }
    }
}
