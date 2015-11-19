using log4net.Config;

[assembly: XmlConfigurator]

namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.IO;

    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Filter;
    using log4net.Layout;
    using log4net.Repository.Hierarchy;

    /// <summary>The log util.</summary>
    public static class LogUtil
    {
        /// <summary>The lo g_ ext.</summary>
        private static readonly string LOG_EXT = ".log";

        /// <summary>Initializes static members of the <see cref="LogUtil" /> class.</summary>
        static LogUtil()
        {
            Console.WriteLine("Configuring logger...");
            var logConfigFI = new FileInfo(AppUtil.GetLocalDataPath("Logging.config"));

            if (logConfigFI.Exists)
            {
                XmlConfigurator.Configure(logConfigFI);
            }
            else
            {
                ConfigureLog4Net();
            }
        }

        /// <summary>The get logger.</summary>
        /// <returns>The <see cref="ILog" />.</returns>
        public static ILog GetLogger()
        {
            return GetLogger(AppUtil.EntryModuleName);
        }

        /// <summary>
        /// The get logger.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ILog"/>.
        /// </returns>
        public static ILog GetLogger(string name)
        {
            return LogManager.GetLogger(name);
        }

        /// <summary>The configure log 4 net.</summary>
        private static void ConfigureLog4Net()
        {
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date\t%level\t%logger\t[%thread] - %message%newline";
            patternLayout.ActivateOptions();

            AddConsoleAppender(hierarchy, patternLayout);
            AddFileAppender(hierarchy, patternLayout);

            hierarchy.Root.Level = AppUtil.IsDebugBuild ? Level.Debug : Level.Info;
            hierarchy.Configured = true;
        }

        /// <summary>
        /// The add console appender.
        /// </summary>
        /// <param name="hierarchy">
        /// The hierarchy.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        private static void AddConsoleAppender(Hierarchy hierarchy, ILayout layout)
        {
            var consoleAppender = new ColoredConsoleAppender { Layout = layout };
            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                    {
                        Level = Level.Fatal, 
                        BackColor =
                            ColoredConsoleAppender.Colors.Purple | ColoredConsoleAppender.Colors.HighIntensity, 
                        ForeColor = ColoredConsoleAppender.Colors.White
                    });

            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                    {
                        Level = Level.Error, 
                        ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity
                    });

            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                    {
                        Level = Level.Warn, 
                        ForeColor =
                            ColoredConsoleAppender.Colors.Yellow | ColoredConsoleAppender.Colors.HighIntensity
                    });

            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors
                    {
                        Level = Level.Info, 
                        ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity
                    });

            consoleAppender.AddMapping(
                new ColoredConsoleAppender.LevelColors { Level = Level.Debug, ForeColor = ColoredConsoleAppender.Colors.White });

            consoleAppender.AddFilter(
                new LevelRangeFilter { AcceptOnMatch = true, LevelMin = AppUtil.IsDebugBuild ? Level.Debug : Level.Info, LevelMax = Level.Fatal });

            consoleAppender.ActivateOptions();
            hierarchy.Root.AddAppender(consoleAppender);
        }

        /// <summary>
        /// The add file appender.
        /// </summary>
        /// <param name="hierarchy">
        /// The hierarchy.
        /// </param>
        /// <param name="layout">
        /// The layout.
        /// </param>
        private static void AddFileAppender(Hierarchy hierarchy, ILayout layout)
        {
            string logFileName = AppUtil.GetLocalDataPath("Logs\\" + AppUtil.EntryModuleName);
            string actualLogFileName = logFileName;

            int tryCount = 0;
            while (IOUtils.IsFileLocked(actualLogFileName + LOG_EXT))
            {
                if (tryCount++ == 0)
                {
                    logFileName = actualLogFileName += DateTime.Now.ToString("_yyyyMMdd");
                }
                else
                {
                    actualLogFileName = logFileName + "_" + (tryCount - 1);
                }
            }

            string targetFile = actualLogFileName + LOG_EXT;
            Console.WriteLine("Initializing log file: " + targetFile);
            var fileAppender = new RollingFileAppender
                                   {
                                       Layout = layout, 
                                       File = targetFile, 
                                       AppendToFile = true, 
                                       RollingStyle = RollingFileAppender.RollingMode.Size, 
                                       MaxSizeRollBackups = 10, 
                                       MaximumFileSize = "10MB", 
                                       StaticLogFileName = true
                                   };
            fileAppender.AddFilter(
                new LevelRangeFilter { AcceptOnMatch = true, LevelMin = AppUtil.IsDebugBuild ? Level.Debug : Level.Info, LevelMax = Level.Fatal });

            fileAppender.ActivateOptions();
            hierarchy.Root.AddAppender(fileAppender);
        }
    }
}