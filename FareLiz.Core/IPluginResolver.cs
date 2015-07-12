using System;
using System.Collections.Generic;
namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for resolving the plugins and their handlers
    /// </summary>
    public interface IPluginResolver
    {
        /// <summary>
        /// Get the types for archive manager
        /// </summary>
        IList<Type> GetArchiveManagerTypes();

        /// <summary>
        /// Get the types for database synchronizer
        /// </summary>
        /// <param name="dbType">Target fare database type which needs to be synchronized</param>

        IList<Type> GetDbSyncerTypes(Type dbType);

        /// <summary>
        /// Get the types which handles fare database
        /// </summary>
        IList<Type> GetFareDatabaseTypes();

        /// <summary>
        /// Get the types which retrieves fare data
        /// </summary>
        IList<Type> GetFareDataProviderTypes();

        /// <summary>
        /// Load all plugins into current domain
        /// </summary>
        void LoadPlugins();
    }
}
