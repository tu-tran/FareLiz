namespace SkyDean.FareLiz.SQLite
{
    using System;
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>
    /// The sq lite fare database.
    /// </summary>
    public sealed partial class SQLiteFareDatabase
    {
        /// <summary>
        /// The synchronize.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <exception cref="ApplicationException">
        /// </exception>
        public bool Synchronize(SyncOperation operation, IProgressCallback callback)
        {
            if (this.DataSynchronizer == null)
            {
                throw new ApplicationException("There is no available file synchronizer. Please make sure that the proper plugin is installed");
            }

            return this.DataSynchronizer.Synchronize(operation, this.DataFileName, callback);
        }

        /// <summary>
        /// The receive packages.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ReceivePackages(IProgressCallback callback)
        {
            var pkgSyncer = this.DataSynchronizer as IPackageSyncer<TravelRoute>;
            int newPackageCount = 0;

            if (pkgSyncer != null)
            {
                var importedPkg = this.GetImportedPackages(callback);
                var newData = pkgSyncer.Receive(importedPkg, callback);
                if (newData != null && newData.Count > 0)
                {
                    this.AddData(newData, callback);
                    newPackageCount = newData.Count;
                }
            }

            return newPackageCount;
        }

        /// <summary>
        /// The send data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string SendData(IList<TravelRoute> data, IProgressCallback callback)
        {
            if (data == null || data.Count < 1)
            {
                return null;
            }

            var newPkg = new DataPackage<TravelRoute>(data);
            var pkgSyncer = this.DataSynchronizer as IPackageSyncer<TravelRoute>;
            if (pkgSyncer != null)
            {
                pkgSyncer.Send(newPkg, callback);
            }

            return newPkg.Id;
        }
    }
}