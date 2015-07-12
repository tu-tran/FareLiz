using log4net;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Reflection;

namespace SkyDean.FareLiz.WinForm.Utils
{
    public class AssemblyLoader : MarshalByRefObject
    {
        private readonly ILog _logger;

        public AssemblyLoader() { }
        public AssemblyLoader(ILog logger)
        {
            _logger = logger;
        }

        public bool IsValidPluginAssembly(string fileName, byte[] publicKey)
        {
            try
            {
                var refAsmKey = Assembly.ReflectionOnlyLoadFrom(fileName).GetName().GetPublicKey();   // Validate the public key
                return (ObjectExtension.AreEquals(publicKey, refAsmKey));
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("Failed to load {0}: {1}", fileName, ex.Message);
            }

            return false;
        }
    }
}
