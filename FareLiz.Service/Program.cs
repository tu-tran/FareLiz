using log4net;
using System;
using System.Windows.Forms;
using SkyDean.FareLiz.Core.Utils;

namespace SkyDean.FareLiz.Service
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            var logger = LogUtil.GetLogger();
            var exceptionHelper = new ExceptionHelper(logger, false, true);
            AppDomain.CurrentDomain.UnhandledException += exceptionHelper.UnhandledExceptionHandler;
            Application.ThreadException += exceptionHelper.UnhandledThreadExceptionHandler;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var svcName = args[0];
            var service = GetService(svcName, logger);

            if (service == null)
            {
                logger.Warn("Invalid service name: " + svcName);
                return;
            }

            // Get arguments and ignore the first argument
            string[] svcArgs = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
                svcArgs[i - 1] = args[i];

            service.Logger = logger;
            service.Initialize();
            service.RunService(svcArgs);
        }

        private static IServiceRunner GetService(string name, ILog logger)
        {
            var targetType = Type.GetType(name);
            if (targetType == null)
                return null;

            var typeResolver = new TypeResolver(logger);
            if (targetType.IsClass && !targetType.IsAbstract && typeof(IServiceRunner).IsAssignableFrom(targetType))
            {
                var result = typeResolver.CreateInstance<IServiceRunner>(targetType);
                result.Logger = logger;
                return result;
            }

            return null;
        }
    }
}