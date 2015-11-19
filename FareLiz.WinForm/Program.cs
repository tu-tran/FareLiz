namespace SkyDean.FareLiz.WinForm
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Service;

    /// <summary>The program.</summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        [STAThread]
        private static void Main(string[] args)
        {
            AppUtil.NameCurrentThread(string.Format("{0}_{1}_{2}_Main", AppUtil.CompanyName, AppUtil.ProductName, AppUtil.ProductVersion));
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

        /// <summary>
        /// The current domain_ assembly resolve.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="Assembly"/>.
        /// </returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName targetAsmName = new AssemblyName(args.Name);
            string name = targetAsmName.Name;
            var allAsms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in allAsms)
            {
                if (string.Equals(asm.GetName().Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return asm;
                }
            }

            return null;
        }
    }
}