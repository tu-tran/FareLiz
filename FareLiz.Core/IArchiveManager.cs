namespace SkyDean.FareLiz.Core
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;

    /// <summary>Archive Manager Interface: Used for storing old data which is not accessed frequently</summary>
    public interface IArchiveManager : IPlugin
    {
        /// <summary>Gets or sets the fare data provider.</summary>
        IFareDataProvider FareDataProvider { get; set; }

        /// <summary>Gets or sets the fare database.</summary>
        IFareDatabase FareDatabase { get; set; }

        /// <summary>
        /// Import data from selected path into database using assigned FareDataProvider and FareDatabase object
        /// </summary>
        /// <param name="path">
        /// Selected path
        /// </param>
        /// <param name="options">
        /// Options for handling the data
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The imported journeys
        /// </returns>
        IList<TravelRoute> ImportData(string path, DataOptions options, IProgressCallback callback);

        /// <summary>
        /// Export data journey into selected destination
        /// </summary>
        /// <param name="data">
        /// Journey data
        /// </param>
        /// <param name="destinationPath">
        /// Destination path for export
        /// </param>
        /// <param name="format">
        /// Data format of the output data
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// Path of the output data
        /// </returns>
        string ExportData(TravelRoute data, string destinationPath, DataFormat format, IProgressCallback callback);

        /// <summary>
        /// Export a list of data journey into selected destination
        /// </summary>
        /// <param name="journeyData">
        /// List of journey data
        /// </param>
        /// <param name="destinationPath">
        /// Destination path for export
        /// </param>
        /// <param name="format">
        /// Data format of the output data
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// Path of the output data
        /// </returns>
        string ExportData(IList<TravelRoute> journeyData, string destinationPath, DataFormat format, IProgressCallback callback);
    }

    /// <summary>The data format.</summary>
    public enum DataFormat
    {
        /// <summary>The binary.</summary>
        Binary, 

        /// <summary>The xml.</summary>
        XML
    }
}