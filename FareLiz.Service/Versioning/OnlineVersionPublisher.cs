using Ionic.Zip;
using Ionic.Zlib;
using log4net;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Service.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SkyDean.FareLiz.Service.Versioning
{
    /// <summary>
    /// Helper service used for creating distribution package and version installer
    /// </summary>
    internal class OnlineVersionPublisher
    {
        internal static readonly string ManifestFileName = "Version.manifest";
        internal static readonly string DateFormatString = "yyyyMMddHHmmss";
        internal ILog _logger;

        internal OnlineVersionPublisher(ILog logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Publish a new distribution version
        /// </summary>
        internal void Publish(PublishParameter parameters)
        {
            if (!Directory.Exists(parameters.OutputLocation))
                Directory.CreateDirectory(parameters.OutputLocation);

            // Comrpess Zip package
            _logger.Info("Creating x86 distribtution package into " + parameters.X86PackageFile);
            CompressBinariesDir(parameters.X86BuildLocation, parameters.X86PackageFile);
            _logger.Info("Creating x64 distribtution package into " + parameters.X64PackageFile);
            CompressBinariesDir(parameters.X64BuildLocation, parameters.X64PackageFile);

            bool hasInstaller = parameters.HasInstallers;
            if (hasInstaller)
            {
                _logger.Info("Creating x86 installer into " + parameters.X86InstallerFile);
                CreateInstaller(parameters.InnoExecutable, parameters.InnoScriptPath, parameters.InnoExtraParams, parameters.X86BuildLocation, parameters.X86InstallerFile);
                _logger.Info("Creating x64 installer into " + parameters.X64InstallerFile);
                CreateInstaller(parameters.InnoExecutable, parameters.InnoScriptPath, parameters.InnoExtraParams, parameters.X64BuildLocation, parameters.X64InstallerFile);
            }
            else
                _logger.Info("There is no parameter for creating Installers... Skipped");

            // Write version manifest file locally
            WriteManifest(parameters, null);

            if (!String.IsNullOrEmpty(parameters.FtpServer))    // If parameters for FTP were specified
            {
                _logger.InfoFormat("Using FTP server {0} [{1}]", parameters.FtpServerUrl, (String.IsNullOrEmpty(parameters.FtpProxyHost) ? null : parameters.FtpProxyHost + ":" + parameters.FtpProxyPort));
                var client = new FtpClient(_logger, parameters.FtpServer, parameters.FtpPort, parameters.FtpProxyHost, parameters.FtpProxyPort, parameters.FtpUser, parameters.FtpPassword);

                _logger.Info("Uploading x86 package into " + parameters.FtpFullX86PackagePath);
                client.Upload(parameters.X86PackageFile, parameters.FtpFullX86PackagePath);
                _logger.Info("Uploading x64 package into " + parameters.FtpFullX64PackagePath);
                client.Upload(parameters.X64PackageFile, parameters.FtpFullX64PackagePath);

                if (hasInstaller)
                {
                    _logger.Info("Uploading x86 installer into " + parameters.FtpFullX86InstallerPath);
                    client.Upload(parameters.X86InstallerFile, parameters.FtpFullX86InstallerPath);
                    _logger.Info("Uploading x64 installer into " + parameters.FtpFullX64InstallerPath);
                    client.Upload(parameters.X64InstallerFile, parameters.FtpFullX64InstallerPath);
                }

                // Write version manifest file to server
                WriteManifest(parameters, client);

                // Remove old versions
                RemoveOldVersions(client, parameters);
            }
        }

        /// <summary>
        /// Write the manifest file
        /// </summary>
        /// <param name="parameters">Publish parameter</param>
        /// <param name="client">FTP client. If this parameter is null, the file will only be stored locally according to the publish parameter</param>
        private void WriteManifest(PublishParameter parameters, FtpClient client)
        {
            // Write version manifest file
            var manifest = Path.Combine(parameters.ProductBasePath, ManifestFileName);
            var sb = new StringBuilder();
            sb.AppendLine(AppUtil.ProductVersion);
            sb.AppendLine(AppUtil.ProductWriteDate.ToUniversalTime().ToString(DateFormatString));
            sb.AppendLine(parameters.FtpX86PackageRelativePath);
            sb.AppendLine(parameters.FtpX64PackageRelativePath);
            sb.AppendLine(parameters.HasInstallers ? parameters.FtpX86InstallerRelativePath : "");
            sb.AppendLine(parameters.HasInstallers ? parameters.FtpX64InstallerRelativePath : "");
            var manifestContent = sb.ToString();
            File.WriteAllText(manifest, manifestContent);

            if (client != null)
            {
                string remoteManifest = parameters.FtpProductBasePath + "/" + ManifestFileName;
                _logger.Info("Creating version manifest for version " + AppUtil.ProductVersion + " at " + remoteManifest);
                client.Upload(manifest, remoteManifest);
            }
        }

        /// <summary>
        /// Remove older versions and only keep the latest version
        /// </summary>
        private void RemoveOldVersions(FtpClient client, PublishParameter parameters)
        {
            var dirs = client.ListDirectory(parameters.FtpProductBasePath);
            if (dirs != null)
            {
                var versions = new Dictionary<Version, string>();
                foreach (var f in dirs)
                {
                    var name = Path.GetFileName(f);
                    if (Regex.IsMatch(name, @"\d+?\.\d+?\.\d+?\.\d+?"))
                    {
                        var v = new Version(name);
                        versions.Add(v, name);
                    }
                }

                if (versions.Count > 1)
                {
                    var sortedVersions = versions.OrderBy(p => p.Key).ToList(); ;
                    for (int i = 0; i < sortedVersions.Count - 1; i++)
                    {
                        var pair = sortedVersions[i];
                        var delDir = parameters.FtpProductBasePath + "/" + pair.Value;
                        _logger.Info("Removing all version folder at: " + delDir);
                        client.RemoveDirectory(delDir);
                    }
                }
            }
        }

        /// <summary>
        /// Compress the binaries into ZIP file
        /// </summary>
        private void CompressBinariesDir(string path, string outFile)
        {
            var dir = Path.GetDirectoryName(outFile);
            Directory.CreateDirectory(dir);
            if (File.Exists(outFile))
                File.Delete(outFile);

            string inDir = Path.GetFullPath(path);
            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.BestCompression;
                zip.AddSelectedFiles("name != *.pdb", inDir, "", true);
                zip.Save(outFile);
            }
        }

        /// <summary>
        /// Create innoSetup installer
        /// </summary>
        private void CreateInstaller(string innoExe, string innoScript, string innoExtraParams, string inputPath, string outFile)
        {
            // Start innoSetup and pass the dynamic paramters as preprocessor directives
            var icon = VersionPublishService.InstallerIcon;
            var tempIconDir = AppUtil.GetLocalDataPath("Temp");
            Directory.CreateDirectory(tempIconDir);
            var tempIconPath = tempIconDir + "\\" + Guid.NewGuid().ToString() + ".ico";
            using (var iconStream = File.Open(tempIconPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                icon.Save(iconStream);
            }

            try
            {
                var outDir = Path.GetDirectoryName(outFile);
                Directory.CreateDirectory(outDir);
                if (File.Exists(outFile))
                    File.Delete(outFile);

                string args = "/dICON_PATH=" + Wrap(tempIconPath)
                            + " /dINPUT_PATH=" + Wrap(inputPath)
                            + " /dOUTPUT_PATH=" + Wrap(outDir)
                            + " /dOUTPUT_FILENAME=" + Wrap(Path.GetFileNameWithoutExtension(outFile))
                            + " /dAPP_COMPANY=" + Wrap(AppUtil.CompanyName)
                            + " /dAPP_COPYRIGHT=" + Wrap(AppUtil.ProductCopyright)
                            + " /dAPP_NAME=" + Wrap(AppUtil.ProductName)
                            + " /dAPP_VERSION=" + Wrap(AppUtil.ProductVersion)
                            + " /dAPP_URL=" + Wrap(AppUtil.PublisherUrl)
                            + " /dAPP_EMAIL=" + Wrap(AppUtil.PublisherEmail)
                            + (String.IsNullOrEmpty(innoExtraParams) ? null : " " + innoExtraParams)
                            + " " + Wrap(innoScript);

                Console.WriteLine(String.Format("Starting process [{0}] with argument: {1}", innoExe, args));
                var startInfo = new ProcessStartInfo(innoExe, args)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                var innoProc = new Process { StartInfo = startInfo };
                var sb = new StringBuilder();
                var outputHandler = new DataReceivedEventHandler((o, e) =>
                    {
                        Console.WriteLine(e.Data);
                        sb.AppendLine(e.Data);
                    });
                innoProc.OutputDataReceived += outputHandler;   // Catch the console output
                innoProc.ErrorDataReceived += outputHandler;
                innoProc.Start();
                innoProc.BeginOutputReadLine();
                innoProc.BeginErrorReadLine();
                innoProc.WaitForExit();
                if (innoProc.ExitCode != 0)
                    throw new ApplicationException(String.Format("InnoSetup [{0}][{1}] failed for [{2}]: {3}", innoExe, innoScript, inputPath, sb.ToString()));

                if (!File.Exists(outFile))
                    throw new ApplicationException("Double-check the script. Installer did not exist at: " + outFile);
            }
            finally
            {
                if (File.Exists(tempIconPath))
                    File.Delete(tempIconPath);
            }
        }

        /// <summary>
        /// Wrap stirng in double-quotation mark as necessary
        /// </summary>
        private string Wrap(string input)
        {
            return "\"" + input.Trim('"') + "\"";
        }
    }
}
