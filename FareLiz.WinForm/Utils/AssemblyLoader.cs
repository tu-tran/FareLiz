namespace SkyDean.FareLiz.WinForm.Utils
{
    using System;
    using System.Reflection;

    using log4net;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>
    /// The assembly loader.
    /// </summary>
    public class AssemblyLoader : MarshalByRefObject
    {
        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLoader"/> class.
        /// </summary>
        public AssemblyLoader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLoader"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AssemblyLoader(ILog logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// The is valid plugin assembly.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="publicKey">
        /// The public key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsValidPluginAssembly(string fileName, byte[] publicKey)
        {
            try
            {
                var refAsmKey = Assembly.ReflectionOnlyLoadFrom(fileName).GetName().GetPublicKey(); // Validate the public key
                return ObjectExtension.AreEquals(publicKey, refAsmKey);
            }
            catch (Exception ex)
            {
                this._logger.ErrorFormat("Failed to load {0}: {1}", fileName, ex.Message);
            }

            return false;
        }
    }
}