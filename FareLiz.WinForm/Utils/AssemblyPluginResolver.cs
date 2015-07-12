using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SkyDean.FareLiz.WinForm.Utils
{
    /// <summary>
    /// Helper class used for resolving installed plugins
    /// </summary>
    public sealed class AssemblyPluginResolver : IPluginResolver
    {
        private readonly ILog _logger;
        private readonly TypeResolver _typeResolver;

        private static readonly HashSet<Assembly> _loadedPlugins = new HashSet<Assembly>();
        public static HashSet<Assembly> LoadedPlugins { get { return _loadedPlugins; } }

        static AssemblyPluginResolver()
        {
            var exeAsm = Assembly.GetExecutingAssembly();
            _loadedPlugins.Add(exeAsm);
            var entryAsm = Assembly.GetEntryAssembly();
            if (entryAsm != exeAsm)
                _loadedPlugins.Add(entryAsm);
        }

        public AssemblyPluginResolver() { }
        public AssemblyPluginResolver(ILog logger)
        {
            _logger = logger;
            _typeResolver = new TypeResolver(_logger);
        }

        public void LoadPlugins()
        {
            string[] pluginFiles = Directory.GetFiles(PathUtil.ApplicationPath, String.Format(CultureInfo.InvariantCulture, "{0}.*.Plugins.dll", AppUtil.ProductName));
            var publicKey = Assembly.GetExecutingAssembly().GetName().GetPublicKey();

            using (var asmLoaderProxy = AppDomainProxy<AssemblyLoader>.GetProxy(AppDomain.CurrentDomain.BaseDirectory))
            {
                var loader = asmLoaderProxy.Instance;
                foreach (var f in pluginFiles)
                {
                    try
                    {
                        if (loader.IsValidPluginAssembly(f, publicKey))
                            LoadedPlugins.Add(Assembly.LoadFile(f));
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorFormat("Failed to load assembly [{0}]: {1}", f, ex);
                    }
                }
            }
        }

        public IList<Type> GetArchiveManagerTypes()
        {
            return _typeResolver.GetTypes(typeof(IArchiveManager), LoadedPlugins);
        }

        public IList<Type> GetFareDataProviderTypes()
        {
            return _typeResolver.GetTypes(typeof(IFareDataProvider));
        }

        public IList<Type> GetFareDatabaseTypes()
        {
            return _typeResolver.GetTypes(typeof(ISyncableDatabase));
        }

        public IList<Type> GetDbSyncerTypes(Type dbType)
        {
            return _typeResolver.GetTypes(typeof(IDatabaseSyncer<>), dbType);
        }

        public object CreateAnyObject(Type interfaceType, bool throwOnNull, params object[] constructorParams)
        {
            return CreateAnyObject(interfaceType, null, throwOnNull, constructorParams);
        }

        public object CreateAnyObject(Type interfaceType, Type[] genericType, bool throwOnNull, params object[] constructorParams)
        {
            IList<Type> classTypes = _typeResolver.GetTypes(interfaceType, genericType);
            if (classTypes.Count > 0)
                return Activator.CreateInstance(classTypes.First(), constructorParams);

            if (throwOnNull)
                throw new ArgumentException("There is no implementation for interface [" + interfaceType + "]. Make sure that the plugins are properly installed!");

            return null;
        }
    }
}
