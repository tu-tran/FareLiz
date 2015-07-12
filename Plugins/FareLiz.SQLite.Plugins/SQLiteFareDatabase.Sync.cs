using System;
using System.Collections.Generic;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.SQLite
{
    public sealed partial class SQLiteFareDatabase
    {
        public bool Synchronize(SyncOperation operation, IProgressCallback callback)
        {
            if (DataSynchronizer == null)
                throw new ApplicationException("There is no available file synchronizer. Please make sure that the proper plugin is installed");

            return DataSynchronizer.Synchronize(operation, DataFileName, callback);
        }

        public int ReceivePackages(IProgressCallback callback)
        {
            var pkgSyncer = DataSynchronizer as IPackageSyncer<TravelRoute>;
            int newPackageCount = 0;

            if (pkgSyncer != null)
            {
                var importedPkg = GetImportedPackages(callback);
                var newData = pkgSyncer.Receive(importedPkg, callback);
                if (newData != null && newData.Count > 0)
                {
                    AddData(newData, callback);
                    newPackageCount = newData.Count;
                }
            }

            return newPackageCount;
        }

        public string SendData(IList<TravelRoute> data, IProgressCallback callback)
        {
            if (data == null || data.Count < 1)
                return null;

            var newPkg = new DataPackage<TravelRoute>(data);
            var pkgSyncer = DataSynchronizer as IPackageSyncer<TravelRoute>;
            if (pkgSyncer != null)
            {
                pkgSyncer.Send(newPkg, callback);
            }

            return newPkg.Id;
        }
    }
}