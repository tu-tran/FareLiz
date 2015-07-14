namespace SkyDean.FareLiz.WinForm.Utils
{
    using System;

    using SkyDean.FareLiz.Core;

    /// <summary>The assembly loader.</summary>
    public class AssemblyLoader : MarshalByRefObject
    {
        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>Initializes a new instance of the <see cref="AssemblyLoader" /> class.</summary>
        public AssemblyLoader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLoader"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AssemblyLoader(ILogger logger)
        {
            this._logger = logger;
        }
    }
}