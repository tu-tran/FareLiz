namespace SkyDean.FareLiz.WinForm.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Helper class used for resolving installed plugins</summary>
    public sealed class AssemblyPluginResolver : IPluginResolver
    {
        /// <summary>The _loaded plugins.</summary>
        private static readonly HashSet<Assembly> _loadedPlugins = new HashSet<Assembly>();

        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>The _type resolver.</summary>
        private readonly TypeResolver _typeResolver;

        /// <summary>Initializes static members of the <see cref="AssemblyPluginResolver" /> class.</summary>
        static AssemblyPluginResolver()
        {
            var exeAsm = Assembly.GetExecutingAssembly();
            _loadedPlugins.Add(exeAsm);
            var entryAsm = Assembly.GetEntryAssembly();
            if (entryAsm != exeAsm)
            {
                _loadedPlugins.Add(entryAsm);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="AssemblyPluginResolver" /> class.</summary>
        public AssemblyPluginResolver()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyPluginResolver"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public AssemblyPluginResolver(ILogger logger)
        {
            this._logger = logger;
            this._typeResolver = new TypeResolver(this._logger);
        }

        /// <summary>Gets the loaded plugins.</summary>
        public static HashSet<Assembly> LoadedPlugins
        {
            get
            {
                return _loadedPlugins;
            }
        }

        /// <summary>The load plugins.</summary>
        public void LoadPlugins()
        {
            string[] pluginFiles = Directory.GetFiles(
                PathUtil.ApplicationPath, 
                string.Format(CultureInfo.InvariantCulture, "{0}.*.Plugins.dll", AppUtil.ProductName));
            var publicKey = Assembly.GetExecutingAssembly().GetName().GetPublicKey();

            using (var asmLoaderProxy = AppDomainProxy<AssemblyLoader>.GetProxy(AppDomain.CurrentDomain.BaseDirectory))
            {
                var loader = asmLoaderProxy.Instance;
                foreach (var f in pluginFiles)
                {
                    try
                    {
                        if (loader.IsValidPluginAssembly(f, publicKey))
                        {
                            LoadedPlugins.Add(Assembly.LoadFile(f));
                        }
                    }
                    catch (Exception ex)
                    {
                        this._logger.ErrorFormat("Failed to load assembly [{0}]: {1}", f, ex);
                    }
                }
            }
        }

        /// <summary>The get archive manager types.</summary>
        /// <returns>The <see cref="IList" />.</returns>
        public IList<Type> GetArchiveManagerTypes()
        {
            return this._typeResolver.GetTypes(typeof(IArchiveManager), LoadedPlugins);
        }

        /// <summary>The get fare data provider types.</summary>
        /// <returns>The <see cref="IList" />.</returns>
        public IList<Type> GetFareDataProviderTypes()
        {
            return this._typeResolver.GetTypes(typeof(IFareDataProvider));
        }

        /// <summary>The get fare database types.</summary>
        /// <returns>The <see cref="IList" />.</returns>
        public IList<Type> GetFareDatabaseTypes()
        {
            return this._typeResolver.GetTypes(typeof(ISyncableDatabase));
        }

        /// <summary>
        /// The get db syncer types.
        /// </summary>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<Type> GetDbSyncerTypes(Type dbType)
        {
            return this._typeResolver.GetTypes(typeof(IDatabaseSyncer<>), dbType);
        }

        /// <summary>
        /// The create any object.
        /// </summary>
        /// <param name="interfaceType">
        /// The interface type.
        /// </param>
        /// <param name="throwOnNull">
        /// The throw on null.
        /// </param>
        /// <param name="constructorParams">
        /// The constructor params.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object CreateAnyObject(Type interfaceType, bool throwOnNull, params object[] constructorParams)
        {
            return this.CreateAnyObject(interfaceType, null, throwOnNull, constructorParams);
        }

        /// <summary>
        /// The create any object.
        /// </summary>
        /// <param name="interfaceType">
        /// The interface type.
        /// </param>
        /// <param name="genericType">
        /// The generic type.
        /// </param>
        /// <param name="throwOnNull">
        /// The throw on null.
        /// </param>
        /// <param name="constructorParams">
        /// The constructor params.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public object CreateAnyObject(Type interfaceType, Type[] genericType, bool throwOnNull, params object[] constructorParams)
        {
            IList<Type> classTypes = this._typeResolver.GetTypes(interfaceType, genericType);
            if (classTypes.Count > 0)
            {
                return Activator.CreateInstance(classTypes.First(), constructorParams);
            }

            if (throwOnNull)
            {
                throw new ArgumentException(
                    "There is no implementation for interface [" + interfaceType + "]. Make sure that the plugins are properly installed!");
            }

            return null;
        }
    }
}