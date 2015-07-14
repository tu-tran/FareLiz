namespace SkyDean.FareLiz.Core
{
    #region

    using System;

    using SkyDean.FareLiz.Core.Presentation;

    #endregion

    /// <summary>The app context.</summary>
    public abstract class AppContext
    {
        #region Static Fields

        /// <summary>The _instance.</summary>
        private static AppContext _instance;

        #endregion

        #region Fields

        /// <summary>The monitor environment.</summary>
        protected MonitorEnvironment _env = null;

        /// <summary>The _err.</summary>
        protected string _err = null;

        /// <summary>The _first start.</summary>
        protected bool _firstStart = true;

        /// <summary>The _logger.</summary>
        protected ILogger _logger = null;

        #endregion

        #region Public Properties

        /// <summary>Gets the error.</summary>
        public static string Error
        {
            get
            {
                return _instance._err;
            }
        }

        /// <summary>Gets a value indicating whether first start.</summary>
        public static bool FirstStart
        {
            get
            {
                return _instance._firstStart;
            }
        }

        /// <summary>Gets the logger.</summary>
        public static ILogger Logger
        {
            get
            {
                return _instance._logger;
            }
        }

        /// <summary>Gets the monitor environment.</summary>
        public static MonitorEnvironment MonitorEnvironment
        {
            get
            {
                return _instance._env;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ApplicationException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        public static void Initialize(AppContext instance)
        {
            if (_instance != null)
            {
                throw new ApplicationException("Access denied: Global context has already been initialized");
            }

            if (instance == null)
            {
                throw new ArgumentException("Global context cannot be nulled");
            }

            _instance = instance;
        }

        /// <summary>
        /// Execute a delegate on a separate thread and show a progress dialog
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threadName">
        /// The thread Name.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="exceptionHandler">
        /// The exception Handler.
        /// </param>
        /// <param name="finalHandler">
        /// The final Handler.
        /// </param>
        /// <param name="notifyError">
        /// The notify Error.
        /// </param>
        /// <param name="visible">
        /// The visible.
        /// </param>
        public static void ExecuteTask(object owner, 
                                       string title, 
                                       string text, 
                                       string threadName, 
                                       ProgressStyle style, 
                                       ILogger logger, 
                                       Action<IProgressCallback> action, 
                                       CallbackExceptionDelegate exceptionHandler = null, 
                                       CallbackExceptionDelegate finalHandler = null, 
                                       bool notifyError = true, 
                                       bool visible = true)
        {
            _instance.DoExecuteTask(owner, title, text, threadName, style, logger, action, exceptionHandler, finalHandler, notifyError, visible);
        }

        /// <summary>
        /// Execute a delegate on a separate thread and show a progress dialog
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="threadName">
        /// The thread Name.
        /// </param>
        /// <param name="style">
        /// The style.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <param name="exceptionHandler">
        /// The exception Handler.
        /// </param>
        /// <param name="finalHandler">
        /// The final Handler.
        /// </param>
        /// <param name="notifyError">
        /// The notify Error In Msg Box.
        /// </param>
        /// <param name="visible">
        /// The visible.
        /// </param>
        protected abstract void DoExecuteTask(object owner, 
                                              string title, 
                                              string text, 
                                              string threadName, 
                                              ProgressStyle style, 
                                              ILogger logger, 
                                              Action<IProgressCallback> action, 
                                              CallbackExceptionDelegate exceptionHandler, 
                                              CallbackExceptionDelegate finalHandler, 
                                              bool notifyError, 
                                              bool visible);

        #endregion
    }
}