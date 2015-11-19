namespace SkyDean.FareLiz.Core
{
    #region

    using System;

    using log4net;

    using SkyDean.FareLiz.Core.Presentation;

    #endregion

    /// <summary>The app context.</summary>
    public abstract class AppContext
    {
        #region Static Fields

        /// <summary>The _instance.</summary>
        private static AppContext _instance;

        #endregion

        #region Public Methods and Operators

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

        #endregion

        #region Fields

        /// <summary>The monitor environment.</summary>
        protected MonitorEnvironment _env = null;

        /// <summary>The _err.</summary>
        protected string _err = null;

        /// <summary>The _first start.</summary>
        protected bool _firstStart = true;

        /// <summary>The _logger.</summary>
        protected ILog _logger = null;

        /// <summary> The progress callback. </summary>
        protected IProgressCallback _progressCallback = null;

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
        public static ILog Logger
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

        /// <summary> Gets the progress callback.</summary>
        public static IProgressCallback ProgressCallback
        {
            get
            {
                return _instance._progressCallback;
            }
        }

        #endregion
    }
}