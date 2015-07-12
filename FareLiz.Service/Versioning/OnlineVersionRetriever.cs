using Ionic.Zip;
using log4net;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Service.LiveUpdate;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace SkyDean.FareLiz.Service.Versioning
{
    /// <summary>
    /// Retrieve new version information from online repository. This class handles the version distributed by OnlineVersionPublisher
    /// </summary>
    public class OnlineVersionRetriever : IVersionRetriever
    {
        // List of live update mirrors
        private string[] _manifestUrls = new string[] {
            "http://skydean.com/Repository/" + AppUtil.ProductName,
        };

        public string ProductName
        {
            get { return AppUtil.ProductName; }
        }

        public string PublisherName
        {
            get { return AppUtil.CompanyName; }
        }

        public ILog Logger { get; set; }

        public VersionInfo GetCurrentVersion()
        {
            return new VersionInfo(AppUtil.ProductVersion, AppUtil.ProductWriteDate, AppUtil.EntryDirectory, AppUtil.EntryDirectory);
        }

        public OnlineVersionRetriever(ILog logger)
        {
            Logger = logger;
        }

        public VersionInfo GetLatestVersion()
        {
            foreach (var host in _manifestUrls)
            {
                try
                {
                    var baseUrl = host.TrimEnd('/');
                    var manifestUrl = baseUrl + "/" + OnlineVersionPublisher.ManifestFileName + "?req=" + DateTime.UtcNow.ToString("yyyy-MM-dd-hh.mm.ss") + "&host=" + Environment.MachineName;
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

                                if (String.IsNullOrEmpty(version) || String.IsNullOrEmpty(x86) || String.IsNullOrEmpty(x64)
                                      || !DateTime.TryParseExact(dateStr, OnlineVersionPublisher.DateFormatString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out createdDate))
                                    return null;

                                string x86pkg = ProcessUri(request.RequestUri, x86);
                                string x64pkg = ProcessUri(request.RequestUri, x64);
                                return new VersionInfo(version, createdDate, x86pkg, x64pkg);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Failed to get latest version info [{0}]: {1}", host, ex.Message);
                }
            }

            return null;
        }

        public string GetChangeLog(VersionInfo fromVersion, VersionInfo toVersion)
        {
            return null;
        }

        public UpdateRequest CheckForUpdate()
        {
            UpdateRequest result = null;
            var curVersion = GetCurrentVersion();
            var latestVersion = GetLatestVersion();
            if (latestVersion != null && latestVersion.VersionNumber != null
                && latestVersion.VersionNumber > curVersion.VersionNumber)
                result = new UpdateRequest(AppUtil.ProductName, curVersion, latestVersion);

            return result;
        }

        public void DownloadPackage(UpdateRequest request, string targetLocation)
        {
            bool isX64 = IntPtr.Size == 8;
            var pkgLocation = (isX64 ? request.ToVersion.X64Package : request.ToVersion.X86Package);   // Determine the processor architecture
            Directory.CreateDirectory(targetLocation);

            var targetFile = Path.Combine(targetLocation, Guid.NewGuid().ToString() + ".zipTmp");

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

        private string ProcessUri(Uri manifestUri, string packageLocation)
        {
            string baseUrl = manifestUri.Scheme + "://" + manifestUri.Host;
            if (!packageLocation.StartsWith("/")) // Absolute path
                for (int i = 0; i < manifestUri.Segments.Length - 1; i++)
                    baseUrl += manifestUri.Segments[i];

            string result = baseUrl + packageLocation;
            return result;
        }
    }
}
