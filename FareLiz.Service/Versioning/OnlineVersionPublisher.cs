namespace SkyDean.FareLiz.Service.Versioning
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Ionic.Zip;
    using Ionic.Zlib;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Service.Utils;

    /// <summary>Helper service used for creating distribution package and version installer</summary>
    internal class OnlineVersionPublisher
    {
        /// <summary>The manifest file name.</summary>
        internal static readonly string ManifestFileName = "Version.manifest";

        /// <summary>The date format string.</summary>
        internal static readonly string DateFormatString = "yyyyMMddHHmmss";

        /// <summary>The _logger.</summary>
        internal ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineVersionPublisher"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        internal OnlineVersionPublisher(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Publish a new distribution version
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        internal void Publish(PublishParameter parameters)
        {
            if (!Directory.Exists(parameters.OutputLocation))
            {
                Directory.CreateDirectory(parameters.OutputLocation);
            }

            // Comrpess Zip package
            this._logger.Info("Creating x86 distribtution package into " + parameters.X86PackageFile);
            this.CompressBinariesDir(parameters.X86BuildLocation, parameters.X86PackageFile);
            this._logger.Info("Creating x64 distribtution package into " + parameters.X64PackageFile);
            this.CompressBinariesDir(parameters.X64BuildLocation, parameters.X64PackageFile);

            var hasInstaller = parameters.HasInstallers;
            if (hasInstaller)
            {
                this._logger.Info("Creating x86 installer into " + parameters.X86InstallerFile);
                this.CreateInstaller(
                    parameters.InnoExecutable, 
                    parameters.InnoScriptPath, 
                    parameters.InnoExtraParams, 
                    parameters.X86BuildLocation, 
                    parameters.X86InstallerFile);
                this._logger.Info("Creating x64 installer into " + parameters.X64InstallerFile);
                this.CreateInstaller(
                    parameters.InnoExecutable, 
                    parameters.InnoScriptPath, 
                    parameters.InnoExtraParams, 
                    parameters.X64BuildLocation, 
                    parameters.X64InstallerFile);
            }
            else
            {
                this._logger.Info("There is no parameter for creating Installers... Skipped");
            }

            // Write version manifest file locally
            this.WriteManifest(parameters, null);

            if (!string.IsNullOrEmpty(parameters.FtpServer))
            {
                // If parameters for FTP were specified
                this._logger.Info(
                    "Using FTP server {0} [{1}]", 
                    parameters.FtpServerUrl, 
                    string.IsNullOrEmpty(parameters.FtpProxyHost) ? null : parameters.FtpProxyHost + ":" + parameters.FtpProxyPort);
                var client = new FtpClient(
                    this._logger, 
                    parameters.FtpServer, 
                    parameters.FtpPort, 
                    parameters.FtpProxyHost, 
                    parameters.FtpProxyPort, 
                    parameters.FtpUser, 
                    parameters.FtpPassword);

                this._logger.Info("Uploading x86 package into " + parameters.FtpFullX86PackagePath);
                client.Upload(parameters.X86PackageFile, parameters.FtpFullX86PackagePath);
                this._logger.Info("Uploading x64 package into " + parameters.FtpFullX64PackagePath);
                client.Upload(parameters.X64PackageFile, parameters.FtpFullX64PackagePath);

                if (hasInstaller)
                {
                    this._logger.Info("Uploading x86 installer into " + parameters.FtpFullX86InstallerPath);
                    client.Upload(parameters.X86InstallerFile, parameters.FtpFullX86InstallerPath);
                    this._logger.Info("Uploading x64 installer into " + parameters.FtpFullX64InstallerPath);
                    client.Upload(parameters.X64InstallerFile, parameters.FtpFullX64InstallerPath);
                }

                // Write version manifest file to server
                this.WriteManifest(parameters, client);

                // Remove old versions
                this.RemoveOldVersions(client, parameters);
            }
        }

        /// <summary>
        /// Write the manifest file
        /// </summary>
        /// <param name="parameters">
        /// Publish parameter
        /// </param>
        /// <param name="client">
        /// FTP client. If this parameter is null, the file will only be stored locally according to the publish parameter
        /// </param>
        private void WriteManifest(PublishParameter parameters, FtpClient client)
        {
            // Write version manifest file
            var manifest = Path.Combine(parameters.ProductBasePath, ManifestFileName);
            var sb = new StringBuilder();
            sb.AppendLine(AppUtil.ProductVersion);
            sb.AppendLine(AppUtil.ProductWriteDate.ToUniversalTime().ToString(DateFormatString));
            sb.AppendLine(parameters.FtpX86PackageRelativePath);
            sb.AppendLine(parameters.FtpX64PackageRelativePath);
            sb.AppendLine(parameters.HasInstallers ? parameters.FtpX86InstallerRelativePath : string.Empty);
            sb.AppendLine(parameters.HasInstallers ? parameters.FtpX64InstallerRelativePath : string.Empty);
            var manifestContent = sb.ToString();
            File.WriteAllText(manifest, manifestContent);

            if (client != null)
            {
                var remoteManifest = parameters.FtpProductBasePath + "/" + ManifestFileName;
                this._logger.Info("Creating version manifest for version " + AppUtil.ProductVersion + " at " + remoteManifest);
                client.Upload(manifest, remoteManifest);
            }
        }

        /// <summary>
        /// Remove older versions and only keep the latest version
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
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
                    var sortedVersions = versions.OrderBy(p => p.Key).ToList();

                    for (var i = 0; i < sortedVersions.Count - 1; i++)
                    {
                        var pair = sortedVersions[i];
                        var delDir = parameters.FtpProductBasePath + "/" + pair.Value;
                        this._logger.Info("Removing all version folder at: " + delDir);
                        client.RemoveDirectory(delDir);
                    }
                }
            }
        }

        /// <summary>
        /// Compress the binaries into ZIP file
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="outFile">
        /// The out File.
        /// </param>
        private void CompressBinariesDir(string path, string outFile)
        {
            var dir = Path.GetDirectoryName(outFile);
            Directory.CreateDirectory(dir);
            if (File.Exists(outFile))
            {
                File.Delete(outFile);
            }

            var inDir = Path.GetFullPath(path);
            using (var zip = new ZipFile())
            {
                zip.CompressionLevel = CompressionLevel.BestCompression;
                zip.AddSelectedFiles("name != *.pdb", inDir, string.Empty, true);
                zip.Save(outFile);
            }
        }

        /// <summary>
        /// Create innoSetup installer
        /// </summary>
        /// <param name="innoExe">
        /// The inno Exe.
        /// </param>
        /// <param name="innoScript">
        /// The inno Script.
        /// </param>
        /// <param name="innoExtraParams">
        /// The inno Extra Params.
        /// </param>
        /// <param name="inputPath">
        /// The input Path.
        /// </param>
        /// <param name="outFile">
        /// The out File.
        /// </param>
        private void CreateInstaller(string innoExe, string innoScript, string innoExtraParams, string inputPath, string outFile)
        {
            // Start innoSetup and pass the dynamic paramters as preprocessor directives
            var icon = VersionPublishService.InstallerIcon;
            var tempIconDir = AppUtil.GetLocalDataPath("Temp");
            Directory.CreateDirectory(tempIconDir);
            var tempIconPath = tempIconDir + "\\" + Guid.NewGuid() + ".ico";
            using (var iconStream = File.Open(tempIconPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                icon.Save(iconStream);
            }

            try
            {
                var outDir = Path.GetDirectoryName(outFile);
                Directory.CreateDirectory(outDir);
                if (File.Exists(outFile))
                {
                    File.Delete(outFile);
                }

                var args = "/dICON_PATH=" + this.Wrap(tempIconPath) + " /dINPUT_PATH=" + this.Wrap(inputPath) + " /dOUTPUT_PATH=" + this.Wrap(outDir)
                           + " /dOUTPUT_FILENAME=" + this.Wrap(Path.GetFileNameWithoutExtension(outFile)) + " /dAPP_COMPANY="
                           + this.Wrap(AppUtil.CompanyName) + " /dAPP_COPYRIGHT=" + this.Wrap(AppUtil.ProductCopyright) + " /dAPP_NAME="
                           + this.Wrap(AppUtil.ProductName) + " /dAPP_VERSION=" + this.Wrap(AppUtil.ProductVersion) + " /dAPP_URL="
                           + this.Wrap(AppUtil.PublisherUrl) + " /dAPP_EMAIL=" + this.Wrap(AppUtil.PublisherEmail)
                           + (string.IsNullOrEmpty(innoExtraParams) ? null : " " + innoExtraParams) + " " + this.Wrap(innoScript);

                Console.WriteLine("Starting process [{0}] with argument: {1}", innoExe, args);
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
                var outputHandler = new DataReceivedEventHandler(
                    (o, e) =>
                        {
                            Console.WriteLine(e.Data);
                            sb.AppendLine(e.Data);
                        });
                innoProc.OutputDataReceived += outputHandler; // Catch the console output
                innoProc.ErrorDataReceived += outputHandler;
                innoProc.Start();
                innoProc.BeginOutputReadLine();
                innoProc.BeginErrorReadLine();
                innoProc.WaitForExit();
                if (innoProc.ExitCode != 0)
                {
                    throw new ApplicationException(string.Format("InnoSetup [{0}][{1}] failed for [{2}]: {3}", innoExe, innoScript, inputPath, sb));
                }

                if (!File.Exists(outFile))
                {
                    throw new ApplicationException("Double-check the script. Installer did not exist at: " + outFile);
                }
            }
            finally
            {
                if (File.Exists(tempIconPath))
                {
                    File.Delete(tempIconPath);
                }
            }
        }

        /// <summary>
        /// Wrap stirng in double-quotation mark as necessary
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string Wrap(string input)
        {
            return "\"" + input.Trim('"') + "\"";
        }
    }
}