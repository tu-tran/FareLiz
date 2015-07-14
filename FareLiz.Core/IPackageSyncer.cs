namespace SkyDean.FareLiz.Core
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>
    /// Interface for helper objects which are used to synchronize data packages
    /// </summary>
    /// <typeparam name="T">
    /// Target package type
    /// </typeparam>
    public interface IPackageSyncer<T>
    {
        /// <summary>
        /// Receive the packages
        /// </summary>
        /// <param name="importedPackages">
        /// List of imported packages ID
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// List of received packages
        /// </returns>
        IList<DataPackage<T>> Receive(IList<string> importedPackages, IProgressCallback callback);

        /// <summary>
        /// Send the selected list of packages
        /// </summary>
        /// <param name="data">
        /// List of data packages
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        void Send(DataPackage<T> data, IProgressCallback callback);
    }
}