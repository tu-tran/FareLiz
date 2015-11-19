namespace SkyDean.FareLiz.Service
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
    /// The exception helper.
    /// </summary>
    public class ExceptionHelper
    {
        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The entry assembly location.
        /// </summary>
        public readonly string EntryAssemblyLocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHelper"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="autoRestartProcess">
        /// The auto restart process.
        /// </param>
        /// <param name="silentMode">
        /// The silent mode.
        /// </param>
        public ExceptionHelper(ILogger logger, bool autoRestartProcess, bool silentMode)
        {
            this._logger = logger;
            this.AutoRestartProcess = autoRestartProcess;
            this.SilentMode = silentMode;

            var asm = Assembly.GetEntryAssembly();
            if (asm == null)
            {
                asm = Assembly.GetExecutingAssembly();
            }

            this.EntryAssemblyLocation = asm.Location;
        }

        /// <summary>
        /// Gets or sets the restart argument.
        /// </summary>
        public string RestartArgument { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether auto restart process.
        /// </summary>
        public bool AutoRestartProcess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether silent mode.
        /// </summary>
        public bool SilentMode { get; set; }

        /// <summary>
        /// The unhandled thread exception handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public void UnhandledThreadExceptionHandler(object sender, ThreadExceptionEventArgs ex)
        {
            this.HandleException(ex.Exception);
        }

        /// <summary>
        /// The unhandled exception handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs ex)
        {
            this.HandleException(ex.ExceptionObject as Exception);
        }

        /// <summary>
        /// The handle exception.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        private void HandleException(Exception ex)
        {
            string err = null;
            if (ex != null)
            {
                err = this.Log(ex);
            }

            bool shouldRestart = this.AutoRestartProcess;
            bool shouldExit = true;

            if (!this.SilentMode)
            {
                string msg = null;
                var sep = Environment.NewLine + Environment.NewLine;
                if (!string.IsNullOrEmpty(err))
                {
                    msg = "The application has crashed because of the following error: " + sep + err + sep + "How do you want to proceed?" + sep
                          + "PS: Please give us a chance to fix the problem by sending the following error report to " + AppUtil.PublisherEmail
                          + Environment.NewLine + Environment.NewLine + ex;
                }

                var translation = new Dictionary<ExMessageBox.ButtonType, string>
                                      {
                                          { ExMessageBox.ButtonType.Yes, "&Restart Application" }, 
                                          { ExMessageBox.ButtonType.No, "E&xit Application" }, 
                                          { ExMessageBox.ButtonType.Cancel, "&Ignore the Error" }
                                      };

                var dlgResult = ExMessageBox.Show(
                    null,
                    msg,
                    "Oops. This is weird...",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1,
                    translation);

                shouldRestart = dlgResult == DialogResult.Yes;
                shouldExit = dlgResult != DialogResult.Cancel;
            }

            if (shouldRestart)
            {
                Process.Start(this.EntryAssemblyLocation, this.RestartArgument);
            }

            if (shouldExit)
            {
                Environment.Exit(-1);
            }
        }

        /// <summary>
        /// The log.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Log(Exception ex)
        {
            Exception procEx = null;
            try
            {
                IExceptionHandler exHandler = this.GetHandler(ex);
                if (exHandler == null)
                {
                    exHandler = new GenericExceptionHandler();
                }

                procEx = exHandler.ProcessException(ex);
            }
            catch (Exception e)
            {
                this._logger.Fatal(e);
            }

            if (procEx == null)
            {
                return null;
            }

            this._logger.Fatal(procEx);
            return procEx.Message;
        }

        /// <summary>
        /// The get handler.
        /// </summary>
        /// <param name="ex">
        /// The ex.
        /// </param>
        /// <returns>
        /// The <see cref="IExceptionHandler"/>.
        /// </returns>
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
                                        {
                                            return Activator.CreateInstance(handlerType) as IExceptionHandler;
                                        }
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