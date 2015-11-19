namespace SkyDean.FareLiz.IO
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>The file archive manager.</summary>
    [DisplayName("File Archive Manager")]
    [Description("Data Archive Manager using persistent file system storage")]
    public class FileArchiveManager : IArchiveManager
    {
        #region Constants

        /// <summary>The bi n_ extension.</summary>
        private const string BIN_EXTENSION = ".fle";

        /// <summary>The xm l_ extension.</summary>
        private const string XML_EXTENSION = ".xml";

        #endregion

        #region Fields

        /// <summary>The accepted extensions.</summary>
        public List<string> AcceptedExtensions = new List<string> { ".htm", ".html", ".xml", ".fle" };

        /// <summary>The _config.</summary>
        private FileArchiveManagerConfig _config;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="FileArchiveManager" /> class.</summary>
        public FileArchiveManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileArchiveManager"/> class.
        /// </summary>
        /// <param name="dataProvider">
        /// The data provider.
        /// </param>
        /// <param name="fareDatabase">
        /// The fare database.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public FileArchiveManager(IFareDataProvider dataProvider, IFareDatabase fareDatabase, ILogger logger)
            : this()
        {
            this.FareDataProvider = dataProvider;
            this.FareDatabase = fareDatabase;
            this.Logger = logger;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the configuration.</summary>
        public IConfig Configuration
        {
            get
            {
                return this._config;
            }

            set
            {
                this._config = value as FileArchiveManagerConfig;
            }
        }

        /// <summary>Gets the custom config builder.</summary>
        public IConfigBuilder CustomConfigBuilder
        {
            get
            {
                return null;
            }
        }

        /// <summary>Gets the default config.</summary>
        public IConfig DefaultConfig
        {
            get
            {
                return new FileArchiveManagerConfig { ArchivePath = "Archives", ProcessBatchSize = 500 };
            }
        }

        /// <summary>Gets or sets the fare data provider.</summary>
        public IFareDataProvider FareDataProvider { get; set; }

        /// <summary>Gets or sets the fare database.</summary>
        public IFareDatabase FareDatabase { get; set; }

        /// <summary>Gets a value indicating whether is configurable.</summary>
        public bool IsConfigurable
        {
            get
            {
                return false;
            }
        }

        /// <summary>Gets or sets the logger.</summary>
        public ILogger Logger { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The export data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="destinationPath">
        /// The destination path.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExportData(IList<TravelRoute> data, string destinationPath, DataFormat format, IProgressCallback callback)
        {
            if (data == null || data.Count < 1)
            {
                return null;
            }

            this.Logger.InfoFormat("Export {0} travel route", data.Count);
            if (string.IsNullOrEmpty(destinationPath))
            {
                destinationPath = ".\\";
            }

            Directory.CreateDirectory(destinationPath);
            string targetFile = null;

            if (format == DataFormat.XML)
            {
                foreach (var route in data)
                {
                    targetFile = Path.Combine(destinationPath, this.GenerateFileName(route) + XML_EXTENSION);
                    using (var fs = File.Open(targetFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
                    {
                        this.FareDataProvider.ExportData(fs, route);
                    }
                }
            }
            else
            {
                targetFile = Path.Combine(
                    destinationPath,
                    string.Format(CultureInfo.InvariantCulture, "Exp_{0}_{1}{2}", Guid.NewGuid(), data.Count, BIN_EXTENSION));
                var formatter = new ProtoBufTransfer(this.Logger);
                formatter.ToRaw(data, targetFile);
            }

            this.Logger.InfoFormat("{0} travel route was exported", data.Count);
            return targetFile;
        }

        /// <summary>
        /// The export data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="destinationPath">
        /// The destination path.
        /// </param>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string ExportData(TravelRoute data, string destinationPath, DataFormat format, IProgressCallback callback)
        {
            return this.ExportData(new List<TravelRoute> { data }, destinationPath, format, callback);
        }

        /// <summary>The initialize.</summary>
        public void Initialize()
        {
            if (this.Configuration == null)
            {
                this.Configuration = this.DefaultConfig;
            }

            if (!string.IsNullOrEmpty(this._config.ArchivePath))
            {
                Directory.CreateDirectory(this._config.ArchivePath);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The import data.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        public IList<TravelRoute> ImportData(string path, DataOptions options, IProgressCallback callback)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = ".\\";
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentException("Target path does not exist: " + path);
            }

            var result = new List<TravelRoute>();
            callback.Begin();
            callback.Text = "Getting file list...";
            string[] fileList = Directory.GetFiles(path, "*");
            var acceptedFiles = new List<string>();
            foreach (var file in fileList)
            {
                string fileExt = Path.GetExtension(file);
                foreach (var ext in this.AcceptedExtensions)
                {
                    if (string.Equals(fileExt, ext, StringComparison.OrdinalIgnoreCase))
                    {
                        acceptedFiles.Add(file);
                        break;
                    }
                }
            }

            callback.Title = string.Format(CultureInfo.InvariantCulture, "Importing {0} files...", acceptedFiles.Count);
            callback.SetRange(0, acceptedFiles.Count);

            int stackCount = 0, maxStack = this._config.ProcessBatchSize;
            long stackSizeBytes = 0, maxStackSizeBytes = 1024 * 1024 * 20;

            // If data size reaches the limit: Start processing file content
            var processRoutes = new List<TravelRoute>();
            var formatter = new ProtoBufTransfer(this.Logger);
            var processFiles = new List<string>();

            for (int i = 0; i < acceptedFiles.Count; i++)
            {
                if (callback.IsAborting)
                {
                    return result;
                }

                string file = acceptedFiles[i];
                string fileName = Path.GetFileName(file);
                string ext = Path.GetExtension(fileName); // Get file extension in order to determine the data type

                this.Logger.InfoFormat("Importing file [{0}]", fileName);
                callback.Text = fileName;
                var fileSize = new FileInfo(file).Length;
                stackSizeBytes += fileSize;

                // Handle Binary data
                if (string.Equals(ext, BIN_EXTENSION, StringComparison.OrdinalIgnoreCase))
                {
                    var newRoutes = formatter.FromRaw<List<TravelRoute>>(file);
                    if (newRoutes == null)
                    {
                        var singleRoute = formatter.FromRaw<TravelRoute>(file);
                        if (singleRoute != null && singleRoute.Journeys.Count > 0)
                        {
                            processRoutes.Add(singleRoute);
                            stackCount += singleRoute.Journeys.Count;
                        }
                    }
                    else if (newRoutes.Count > 0)
                    {
                        foreach (var r in newRoutes)
                        {
                            if (r.Journeys.Count > 0)
                            {
                                processRoutes.AddRange(newRoutes);
                                stackCount += r.Journeys.Count;
                            }
                        }
                    }
                }
                else
                {
                    // Handle the old data using the Fare Provider
                    var newRoute = this.FareDataProvider.ReadData(File.ReadAllText(file, Encoding.Default));
                    if (newRoute != null && newRoute.Journeys.Count > 0)
                    {
                        var existRoute = processRoutes.FirstOrDefault(r => r.IsSameRoute(newRoute));
                        if (existRoute == null)
                        {
                            processRoutes.Add(newRoute);
                        }
                        else
                        {
                            existRoute.AddJourney(newRoute.Journeys, true); // Merge the journeys into the same Route
                        }

                        foreach (var j in newRoute.Journeys)
                        {
                            foreach (var d in j.Data)
                            {
                                if (d.DataDate.IsUndefined())
                                {
                                    d.DataDate = DateTime.Now;
                                }
                            }
                        }

                        stackCount += newRoute.Journeys.Count;
                        stackSizeBytes += 20 * fileSize;

                        // XML file needs much more resource for processing: Add "padding" to the file size boundary
                    }
                }

                processFiles.Add(file);

                if (stackCount >= maxStack || stackSizeBytes >= maxStackSizeBytes || i == acceptedFiles.Count - 1)
                {
                    this.FareDatabase.AddData(processRoutes, callback);
                    result.AddRange(processRoutes);
                    if (options.ArchiveDataFiles)
                    {
                        callback.Text = string.Format("Archiving {0} data entries...", processFiles.Count);
                        this.ExportData(processRoutes, this._config.ArchivePath, DataFormat.Binary, callback);

                        foreach (var f in processFiles)
                        {
                            File.Delete(f);
                        }
                    }

                    processRoutes.Clear();
                    stackCount = 0;
                    stackSizeBytes = 0;
                }

                callback.Increment(1);
            }

            return result;
        }

        /// <summary>
        /// The generate file name.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GenerateFileName(TravelRoute route)
        {
            string origin = route.Departure.IATA;
            string dest = route.Destination.IATA;
            long minId = int.MaxValue, maxId = 0;

            foreach (var j in route.Journeys)
            {
                var id = j.Id;
                if (id > maxId)
                {
                    maxId = id;
                }

                if (id < minId)
                {
                    minId = id;
                }
            }

            var fileName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}.[{2}]-[{3}]-({4}-{5}){6}({7})",
                this.FareDataProvider.ServiceName,
                route.Id,
                origin,
                dest,
                minId,
                maxId,
                DateTime.Now.ToString(NamingRule.DATE_FORMAT, CultureInfo.InvariantCulture),
                route.Journeys.Count);
            var processedName = PathUtil.RemoveInvalidFileNameChars(fileName);
            return processedName;
        }

        #endregion
    }
}