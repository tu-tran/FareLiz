namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data.Config;
    using SkyDean.FareLiz.Data.Monitoring;
    using SkyDean.FareLiz.WinForm.Presentation.Controllers;
    using SkyDean.FareLiz.WinForm.Presentation.Views;
    using SkyDean.FareLiz.WinForm.Utils;

    /// <summary>The boot strap.</summary>
    internal class BootStrap
    {
        /// <summary>The en v_ confi g_ filename.</summary>
        private readonly string ENV_CONFIG_FILENAME = "Environment.bin";

        /// <summary>The gu i_ in i_ filename.</summary>
        private readonly string GUI_INI_FILENAME = AppUtil.ProductName + ".ini";

        /// <summary>The histor y_ filename.</summary>
        private readonly string HISTORY_FILENAME = "History.txt";

        /// <summary>The logger.</summary>
        private readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BootStrap"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal BootStrap(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentException("Logger cannot be null");
            }

            this.Logger = logger;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        internal void Run(string[] args)
        {
            ExecutionParam executionParam;
            if (!ExecutionParam.Parse(args, AppUtil.GetLocalDataPath(this.GUI_INI_FILENAME), this.Logger, out executionParam))
            {
                ExecutionParam.ShowHelp();
                return;
            }

            this.Logger.Info("Application {0} {1} was started!", AppUtil.ProductName, AppUtil.ProductVersion);
            this.MuteApplicationVolume();

            if (args.Length < 1 && !SingleInstance.Start())
            {
                // Show up other instance if no parameter was specified
                if (MessageBox.Show(
                    "Another instance was already running. Do you want to start a new instance ?", 
                    AppUtil.ProductName, 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question) == DialogResult.No)
                {
                    SingleInstance.ShowFirstInstance();
                    return;
                }
            }

            var historyFile = AppUtil.GetLocalDataPath(this.HISTORY_FILENAME);
            var firstStart = !File.Exists(historyFile);
            if (firstStart)
            {
                Directory.CreateDirectory(AppUtil.LocalProductDataPath);
                File.Create(historyFile);
                using (var introForm = new IntroForm()) introForm.ShowDialog();
            }

            var globalContext = WinFormGlobalContext.GetInstance(this.Logger);
            globalContext.SetFirstStart(firstStart);

            var env = this.GetEnvironment(executionParam, globalContext);
            if (env == null)
            {
                Environment.Exit(1);
            }

            env.Logger = this.Logger;
            AppContext.Initialize(globalContext);
            globalContext.SetEnvironment(env);            
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
            this.Logger.Info("Application stopped");
        }

        /// <summary>
        /// The get environment.
        /// </summary>
        /// <param name="executionParam">
        /// The execution param.
        /// </param>
        /// <param name="globalContext">
        /// The global context.
        /// </param>
        /// <returns>
        /// The <see cref="MonitorEnvironment"/>.
        /// </returns>
        private MonitorEnvironment GetEnvironment(ExecutionParam executionParam, WinFormGlobalContext globalContext)
        {
            var pluginResolver = new AssemblyPluginResolver(this.Logger);
            pluginResolver.LoadPlugins();
            var configStore = new FileConfigStore(AppUtil.GetLocalDataPath(this.ENV_CONFIG_FILENAME), pluginResolver, this.Logger);

            var env = configStore.LoadEnv();

            if (env == null || env.ArchiveManager == null || env.FareDatabase == null || env.FareDataProvider == null)
            {
                env = new MonitorEnvironment(configStore, pluginResolver, new BackgroundServiceManager(this.Logger), this.Logger);
                globalContext.AddServices(env);
                using (var configDialog = new EnvConfiguratorDialog(env, executionParam))
                {
                    if (configDialog.ShowDialog() == DialogResult.OK)
                    {
                        env = configDialog.ResultEnvironment;
                    }
                    else
                    {
                        env.Close();
                        env = null;
                    }
                }
            }

            return env;
        }

        /// <summary>The mute application volume.</summary>
        private void MuteApplicationVolume()
        {
            try
            {
                WinApi.SetVolume(0);
                WinApi.CoInternetSetFeatureEnabled(WinApi.FEATURE_DISABLE_NAVIGATION_SOUNDS, WinApi.SET_FEATURE_ON_PROCESS, true);
            }
            catch (Exception e)
            {
                this.Logger.Error("Failed to mute volume: " + e);
            }
        }
    }
}