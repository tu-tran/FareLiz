using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Service;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace SkyDean.FareLiz.WinForm
{
    using SkyDean.FareLiz.Core;

    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            AppUtil.NameCurrentThread(String.Format("{0}_{1}_{2}_Main", AppUtil.CompanyName, AppUtil.ProductName, AppUtil.ProductVersion));
            var logger = LogUtil.GetLogger();

            var exceptionHelper = new ExceptionHelper(logger, true, false);
            Application.ThreadException += exceptionHelper.UnhandledThreadExceptionHandler;
            AppDomain.CurrentDomain.UnhandledException += exceptionHelper.UnhandledExceptionHandler;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var bootStrap = new BootStrap(logger);
            bootStrap.Run(args);
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName targetAsmName = new AssemblyName(args.Name);
            string name = targetAsmName.Name;
            var allAsms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in allAsms)
                if (String.Equals(asm.GetName().Name, name, StringComparison.OrdinalIgnoreCase))
                    return asm;

            return null;
        }
    }
}