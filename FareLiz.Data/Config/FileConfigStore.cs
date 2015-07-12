using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Utils;

namespace SkyDean.FareLiz.Data.Config
{
    /// <summary>
    /// Helper class used for storing configurations in a file
    /// </summary>
    public class FileConfigStore : IConfigStore
    {
        public string ConfigFile { get; private set; }
        public ILog Logger { get; set; }

        private readonly IPluginResolver _pluginResolver;

        public FileConfigStore(string configFileName, IPluginResolver pluginResolver, ILog logger)
            : this()
        {
            ConfigFile = configFileName;
            _pluginResolver = pluginResolver;
            Logger = logger;
        }

        protected FileConfigStore() { }

        public MonitorEnvironment LoadEnv()
        {
            return LoadEnv(ConfigFile);
        }

        public MonitorEnvironment LoadEnv(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return null;

                var configs = new Dictionary<Type, ConfigInfo>();
                byte[][] rawData;
                var formatter = new TolerantBinaryFormatter(Logger);
                var typeResolver = new TypeResolver(Logger);
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var productName = reader.ReadString();
                        var productVersion = reader.ReadString();
                        Logger.DebugFormat("Loading configuration for {0} {1}...", productName, productVersion);
                        rawData = formatter.Deserialize(stream) as byte[][];
                    }
                }

                if (rawData != null)
                {
                    foreach (var data in rawData)
                    {
                        try
                        {
                            using (var ms = new MemoryStream(data))
                            {
                                var configData = formatter.Deserialize(ms) as KeyValue<Type, ConfigInfo>;
                                if (configData != null && configData.Key != null)
                                {
                                    configs.Add(configData.Key, configData.Value);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Warn("Cannot read configuration: " + ex.Message);
                        }
                    }
                }

                if (configs.Count > 0)
                {
                    IArchiveManager archiveManager = null;
                    IFareDataProvider fareDataProvider = null;
                    ISyncableDatabase syncDb = null;
                    IFareDatabase fareDatabase = null;
                    ICurrencyProvider currencyProvider = null;
                    BackgroundServiceManager backgroundServices = new BackgroundServiceManager(Logger);

                    foreach (var pair in configs)
                    {
                        Type type = pair.Key;
                        ConfigInfo info = pair.Value;

                        if (type == typeof(IArchiveManager))
                        {
                            if (info.ConfiguredType != null)
                            {
                                archiveManager = typeResolver.CreateInstance<IArchiveManager>(info.ConfiguredType);
                                if (archiveManager != null)
                                    archiveManager.Configuration = (info.TypeConfiguration ?? archiveManager.DefaultConfig);
                            }
                        }
                        else if (type == typeof(IFareDataProvider))
                        {
                            if (info.ConfiguredType != null)
                            {
                                fareDataProvider = typeResolver.CreateInstance<IFareDataProvider>(info.ConfiguredType);
                                if (fareDataProvider != null)
                                    fareDataProvider.Configuration = (info.TypeConfiguration ?? fareDataProvider.DefaultConfig);
                                if (archiveManager != null)
                                    archiveManager.FareDataProvider = fareDataProvider;
                            }
                        }
                        else if (type == typeof(IFareDatabase))
                        {
                            if (info.ConfiguredType != null)
                            {
                                fareDatabase = typeResolver.CreateInstance<IFareDatabase>(info.ConfiguredType);
                                if (fareDatabase != null)
                                    fareDatabase.Configuration = (info.TypeConfiguration ?? fareDatabase.DefaultConfig);
                                syncDb = fareDatabase as ISyncableDatabase;
                            }
                        }
                        else if (type == typeof(IDatabaseSyncer<>))
                        {
                            if (syncDb != null)
                            {
                                if (info.ConfiguredType != null)
                                {
                                    var dbSyncer = typeResolver.CreateInstance<IPlugin>(info.ConfiguredType);
                                    if (dbSyncer != null)
                                    {
                                        var dataSyncer = dbSyncer as IDataSyncer;
                                        dbSyncer.Configuration = (info.TypeConfiguration ?? dbSyncer.DefaultConfig);
                                        syncDb.DataSynchronizer = dataSyncer;
                                        syncDb.PackageSynchronizer = dbSyncer as IPackageSyncer<TravelRoute>;
                                    }
                                }
                            }
                        }
                        else if (typeof(IHelperService).IsAssignableFrom(type))
                        {
                            if (info.ConfiguredType != null)
                            {
                                var newService = typeResolver.CreateInstance<IHelperService>(info.ConfiguredType);
                                if (newService != null)
                                {
                                    newService.Configuration = (info.TypeConfiguration ?? newService.DefaultConfig);
                                    backgroundServices.Add(newService, false);
                                    var currencyService = newService as ICurrencyProvider;
                                    if (currencyService != null)
                                        currencyProvider = currencyService;
                                }
                            }
                        }
                    }

                    return new MonitorEnvironment(this, _pluginResolver, fareDataProvider, fareDatabase, archiveManager, currencyProvider, backgroundServices, Logger);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Failed to load environment data: {0}", ex);
            }

            return null;
        }

        public void SaveEnv(MonitorEnvironment env)
        {
            SaveEnv(env, ConfigFile);
        }

        public void SaveEnv(MonitorEnvironment env, string filePath)
        {
            var data = new List<KeyValue<Type, ConfigInfo>>();
            if (env.ArchiveManager != null)
                data.Add(new KeyValue<Type, ConfigInfo>(typeof(IArchiveManager), new ConfigInfo(env.ArchiveManager)));

            if (env.FareDataProvider != null)
                data.Add(new KeyValue<Type, ConfigInfo>(typeof(IFareDataProvider), new ConfigInfo(env.FareDataProvider)));

            if (env.FareDatabase != null)
                data.Add(new KeyValue<Type, ConfigInfo>(typeof(IFareDatabase), new ConfigInfo(env.FareDatabase)));

            var syncDb = env.FareDatabase as ISyncableDatabase;
            if (syncDb != null && syncDb.DataSynchronizer != null)
            {
                data.Add(new KeyValue<Type, ConfigInfo>(typeof(IDatabaseSyncer<>), new ConfigInfo(syncDb.DataSynchronizer)));
            }

            if (env.CurrencyProvider != null)
                data.Add(new KeyValue<Type, ConfigInfo>(typeof(ICurrencyProvider), new ConfigInfo(env.CurrencyProvider)));

            if (env.BackgroundServices != null)
            {
                var allServices = env.BackgroundServices.GetAll();
                foreach (var s in allServices)
                {
                    if (s != env.CurrencyProvider)
                        data.Add(new KeyValue<Type, ConfigInfo>(s.GetType(), new ConfigInfo(s)));
                }
            }

            var rawData = new byte[data.Count][];
            var formatter = new TolerantBinaryFormatter(Logger);
            using (var ms = new MemoryStream())
            {
                for (int i = 0; i < data.Count; i++)
                {
                    formatter.Serialize(ms, data[i]);
                    rawData[i] = ms.ToArray();
                    ms.SetLength(0);
                }
            }

            using (var stream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(AppUtil.ProductName);
                    writer.Write(AppUtil.ProductVersion);
                    formatter.Serialize(stream, rawData);
                }
            }
        }
    }
}
