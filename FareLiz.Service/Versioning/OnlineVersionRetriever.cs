namespace SkyDean.FareLiz.Service.Versioning
{
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Service.LiveUpdate;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;

    using Ionic.Zip;

    /// <summary>Retrieve new version information from online repository. This class handles the version distributed by OnlineVersionPublisher</summary>
    public class OnlineVersionRetriever : IVersionRetriever
    {
        // List of live update mirrors
        /// <summary>
        /// The _manifest urls.
        /// </summary>
        private string[] _manifestUrls = { "http://skydean.com/Repository/" + AppUtil.ProductName };

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineVersionRetriever"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public OnlineVersionRetriever(ILogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the product name.
        /// </summary>
        public string ProductName
        {
            get
            {
                return AppUtil.ProductName;
            }
        }

        /// <summary>
        /// Gets the publisher name.
        /// </summary>
        public string PublisherName
        {
            get
            {
                return AppUtil.CompanyName;
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The get current version.
        /// </summary>
        /// <returns>
        /// The <see cref="VersionInfo"/>.
        /// </returns>
        public VersionInfo GetCurrentVersion()
        {
            return new VersionInfo(AppUtil.ProductVersion, AppUtil.ProductWriteDate, AppUtil.EntryDirectory, AppUtil.EntryDirectory);
        }

        /// <summary>
        /// The get latest version.
        /// </summary>
        /// <returns>
        /// The <see cref="VersionInfo"/>.
        /// </returns>
        public VersionInfo GetLatestVersion()
        {
            foreach (var host in this._manifestUrls)
            {
                try
                {
                    var baseUrl = host.TrimEnd('/');
                    var manifestUrl = baseUrl + "/" + OnlineVersionPublisher.ManifestFileName + "?req="
                                      + DateTime.UtcNow.ToString("yyyy-MM-dd-hh.mm.ss") + "&host=" + Environment.MachineName;
                    var request = WebRequest.Create(manifestUrl);
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode == HttpStatusCode.OK && response.ContentType != "text/html")
                        {
                            var stream = response.GetResponseStream();
                            using (var reader = new StreamReader(stream, Encoding.Default))
                            {
                                var version = reader.ReadLine();
                                var dateStr = reader.ReadLine();
                                var x86 = reader.ReadLine();
                                var x64 = reader.ReadLine();
                                DateTime createdDate;

                                if (string.IsNullOrEmpty(version) || string.IsNullOrEmpty(x86) || string.IsNullOrEmpty(x64)
                                    || !DateTime.TryParseExact(
                                        dateStr,
                                        OnlineVersionPublisher.DateFormatString,
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.AssumeUniversal,
                                        out createdDate))
                                {
                                    return null;
                                }

                                string x86pkg = this.ProcessUri(request.RequestUri, x86);
                                string x64pkg = this.ProcessUri(request.RequestUri, x64);
                                return new VersionInfo(version, createdDate, x86pkg, x64pkg);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.ErrorFormat("Failed to get latest version info [{0}]: {1}", host, ex.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// The get change log.
        /// </summary>
        /// <param name="fromVersion">
        /// The from version.
        /// </param>
        /// <param name="toVersion">
        /// The to version.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetChangeLog(VersionInfo fromVersion, VersionInfo toVersion)
        {
            return null;
        }

        /// <summary>
        /// The check for update.
        /// </summary>
        /// <returns>
        /// The <see cref="UpdateRequest"/>.
        /// </returns>
        public UpdateRequest CheckForUpdate()
        {
            UpdateRequest result = null;
            var curVersion = this.GetCurrentVersion();
            var latestVersion = this.GetLatestVersion();
            if (latestVersion != null && latestVersion.VersionNumber != null && latestVersion.VersionNumber > curVersion.VersionNumber)
            {
                result = new UpdateRequest(AppUtil.ProductName, curVersion, latestVersion);
            }

            return result;
        }

        /// <summary>
        /// The download package.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="targetLocation">
        /// The target location.
        /// </param>
        public void DownloadPackage(UpdateRequest request, string targetLocation)
        {
            bool isX64 = IntPtr.Size == 8;
            var pkgLocation = isX64 ? request.ToVersion.X64Package : request.ToVersion.X86Package; // Determine the processor architecture
            Directory.CreateDirectory(targetLocation);

            var targetFile = Path.Combine(targetLocation, Guid.NewGuid() + ".zipTmp");

            using (var client = new WebClient())
            {
                client.DownloadFile(pkgLocation, targetFile);
            }

            try
            {
                using (var zip = ZipFile.Read(targetFile))
                {
                    zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                    foreach (var f in zip)
                    {
                        f.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                        f.Extract(targetLocation);
                    }
                }
            }
            finally
            {
                File.Delete(targetFile);
            }
        }

        /// <summary>
        /// The process uri.
        /// </summary>
        /// <param name="manifestUri">
        /// The manifest uri.
        /// </param>
        /// <param name="packageLocation">
        /// The package location.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ProcessUri(Uri manifestUri, string packageLocation)
        {
            string baseUrl = manifestUri.Scheme + "://" + manifestUri.Host;
            if (!packageLocation.StartsWith("/"))
            {
                // Absolute path
                for (int i = 0; i < manifestUri.Segments.Length - 1; i++)
                {
                    baseUrl += manifestUri.Segments[i];
                }
            }

            string result = baseUrl + packageLocation;
            return result;
        }
    }
}