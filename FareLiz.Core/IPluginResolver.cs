namespace SkyDean.FareLiz.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>Interface for resolving the plugins and their handlers</summary>
    public interface IPluginResolver
    {
        /// <summary>Get the types for archive manager</summary>
        /// <returns>The <see cref="IList" />.</returns>
        IList<Type> GetArchiveManagerTypes();

        /// <summary>
        /// Get the types for database synchronizer
        /// </summary>
        /// <param name="dbType">
        /// Target fare database type which needs to be synchronized
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        IList<Type> GetDbSyncerTypes(Type dbType);

        /// <summary>Get the types which handles fare database</summary>
        /// <returns>The <see cref="IList" />.</returns>
        IList<Type> GetFareDatabaseTypes();

        /// <summary>Get the types which retrieves fare data</summary>
        /// <returns>The <see cref="IList" />.</returns>
        IList<Type> GetFareDataProviderTypes();

        /// <summary>Load all plugins into current domain</summary>
        void LoadPlugins();
    }
}