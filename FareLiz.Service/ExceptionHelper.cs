using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace SkyDean.FareLiz.Service
{
    using SkyDean.FareLiz.WinForm.Components.Controls;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    public class ExceptionHelper
    {
        private readonly ILog _logger;

        public string RestartArgument { get; set; }
        public bool AutoRestartProcess { get; set; }
        public bool SilentMode { get; set; }

        public readonly string EntryAssemblyLocation;

        public ExceptionHelper(ILog logger, bool autoRestartProcess, bool silentMode)
        {
            _logger = logger;
            AutoRestartProcess = autoRestartProcess;
            SilentMode = silentMode;

            var asm = Assembly.GetEntryAssembly();
            if (asm == null)
                asm = Assembly.GetExecutingAssembly();
            EntryAssemblyLocation = asm.Location;
        }

        public void UnhandledThreadExceptionHandler(object sender, ThreadExceptionEventArgs ex)
        {
            HandleException(ex.Exception);
        }

        public void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs ex)
        {
            HandleException(ex.ExceptionObject as Exception);
        }

        private void HandleException(Exception ex)
        {
            string err = null;
            if (ex != null)
                err = Log(ex);

            bool shouldRestart = AutoRestartProcess;
            bool shouldExit = true;

            if (!SilentMode)
            {
                string msg = null;
                var sep = Environment.NewLine + Environment.NewLine;
                if (!String.IsNullOrEmpty(err))
                    msg = "The application has crashed because of the following error: " + sep + err
                        + sep + "How do you want to proceed?"
                        + sep + "PS: Please give us a chance to fix the problem by sending the following error report to " + AppUtil.PublisherEmail
                        + Environment.NewLine + Environment.NewLine + ex;

                var translation = new Dictionary<ExMessageBox.ButtonType, string> { 
                { ExMessageBox.ButtonType.Yes, "&Restart Application" },
                { ExMessageBox.ButtonType.No, "E&xit Application" },
                { ExMessageBox.ButtonType.Cancel, "&Ignore the Error" }};

                var dlgResult = ExMessageBox.Show(null, msg, "Oops. This is weird...",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, translation);

                shouldRestart = (dlgResult == DialogResult.Yes);
                shouldExit = (dlgResult != DialogResult.Cancel);
            }

            if (shouldRestart)
                Process.Start(EntryAssemblyLocation, RestartArgument);
            if (shouldExit)
                Environment.Exit(-1);
        }

        public string Log(Exception ex)
        {
            Exception procEx = null;
            try
            {
                IExceptionHandler exHandler = GetHandler(ex);
                if (exHandler == null)
                    exHandler = new GenericExceptionHandler();

                procEx = exHandler.ProcessException(ex);
            }
            catch (Exception e)
            {
                _logger.Fatal(e);
            }

            if (procEx == null)
                return null;

            _logger.Fatal(procEx);
            return procEx.Message;
        }

        private IExceptionHandler GetHandler(Exception ex)
        {
            if (ex != null && ex.TargetSite != null)
            {
                var exType = ex.GetType();
                var asm = ex.TargetSite.DeclaringType.Assembly;
                var types = asm.GetTypes();
                var handlerType = typeof(IExceptionHandler);
                var handlerAttrib = typeof(ExceptionHandlerAttribute);

                foreach (var t in types)
                {
                    if (handlerType.IsAssignableFrom(t))
                    {
                        var attrib = t.GetCustomAttributes(handlerAttrib, false);
                        if (attrib != null && attrib.Length > 0)
                        {
                            foreach (var a in attrib)
                            {
                                var castAttrib = (ExceptionHandlerAttribute)a;
                                if (castAttrib.HandledExpcetions != null && castAttrib.HandledExpcetions.Count > 0)
                                {
                                    foreach (var supportedEx in castAttrib.HandledExpcetions)
                                    {
                                        if (exType == supportedEx)
                                            return Activator.CreateInstance(handlerType) as IExceptionHandler;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
