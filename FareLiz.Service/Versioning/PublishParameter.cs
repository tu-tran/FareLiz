namespace SkyDean.FareLiz.Service.Versioning
{
    using System;
    using System.Diagnostics;
    using System.IO;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Paramter for publishing a new version</summary>
    internal class PublishParameter
    {
        /// <summary>
        /// The ftp root path.
        /// </summary>
        private string ftpRootPath = "/";

        /// <summary>
        /// The ftp sub dir.
        /// </summary>
        private string ftpSubDir = string.Empty;

        /// <summary>
        /// Gets the x 86 build location.
        /// </summary>
        internal string X86BuildLocation { get; private set; }

        /// <summary>
        /// Gets the x 64 build location.
        /// </summary>
        internal string X64BuildLocation { get; private set; }

        /// <summary>
        /// Gets the output location.
        /// </summary>
        internal string OutputLocation { get; private set; }

        /// <summary>
        /// Gets the ftp server.
        /// </summary>
        internal string FtpServer { get; private set; }

        /// <summary>
        /// Gets the ftp port.
        /// </summary>
        internal int FtpPort { get; private set; }

        /// <summary>
        /// Gets or sets the ftp proxy host.
        /// </summary>
        internal string FtpProxyHost { get; set; }

        /// <summary>
        /// Gets or sets the ftp proxy port.
        /// </summary>
        internal int FtpProxyPort { get; set; }

        /// <summary>
        /// Gets the ftp user.
        /// </summary>
        internal string FtpUser { get; private set; }

        /// <summary>
        /// Gets the ftp password.
        /// </summary>
        internal string FtpPassword { get; private set; }

        /// <summary>
        /// Gets the inno executable.
        /// </summary>
        internal string InnoExecutable { get; private set; }

        /// <summary>
        /// Gets the inno script path.
        /// </summary>
        internal string InnoScriptPath { get; private set; }

        /// <summary>
        /// Gets the inno extra params.
        /// </summary>
        internal string InnoExtraParams { get; private set; }

        /// <summary>
        /// Gets or sets the ftp root path.
        /// </summary>
        internal string FtpRootPath
        {
            get
            {
                return this.ftpRootPath;
            }

            set
            {
                this.ftpRootPath = "/" + (string.IsNullOrEmpty(value) ? null : value.Replace("\\", "/").Trim('/'));
            }
        }

        /// <summary>
        /// Gets or sets the ftp sub directory.
        /// </summary>
        internal string FtpSubDirectory
        {
            get
            {
                return this.ftpSubDir;
            }

            set
            {
                this.ftpSubDir = string.IsNullOrEmpty(value) ? null : value.Replace("\\", "/").Trim('/');
            }
        }

        /// <summary>
        /// Gets the product base path.
        /// </summary>
        internal string ProductBasePath
        {
            get
            {
                return Path.Combine(this.OutputLocation, AppUtil.ProductName);
            }
        }

        /// <summary>
        /// Gets the x 86 package file.
        /// </summary>
        internal string X86PackageFile
        {
            get
            {
                return Path.Combine(this.ProductBasePath, this.GetVersionRelativePath(ProductPlatform.x86, PackageType.Zip));
            }
        }

        /// <summary>
        /// Gets the x 64 package file.
        /// </summary>
        internal string X64PackageFile
        {
            get
            {
                return Path.Combine(this.ProductBasePath, this.GetVersionRelativePath(ProductPlatform.x64, PackageType.Zip));
            }
        }

        /// <summary>
        /// Gets the x 86 installer file.
        /// </summary>
        internal string X86InstallerFile
        {
            get
            {
                return Path.Combine(this.ProductBasePath, this.GetVersionRelativePath(ProductPlatform.x86, PackageType.Installer));
            }
        }

        /// <summary>
        /// Gets the x 64 installer file.
        /// </summary>
        internal string X64InstallerFile
        {
            get
            {
                return Path.Combine(this.ProductBasePath, this.GetVersionRelativePath(ProductPlatform.x64, PackageType.Installer));
            }
        }

        /// <summary>
        /// Gets the ftp base path.
        /// </summary>
        internal string FtpBasePath
        {
            get
            {
                return this.FtpRootPath + (string.IsNullOrEmpty(this.FtpSubDirectory) ? null : "/" + this.FtpSubDirectory);
            }
        }

        /// <summary>
        /// Gets the ftp product base path.
        /// </summary>
        internal string FtpProductBasePath
        {
            get
            {
                return this.FtpBasePath + "/" + AppUtil.ProductName;
            }
        }

        /// <summary>
        /// Gets the ftp full x 86 package path.
        /// </summary>
        internal string FtpFullX86PackagePath
        {
            get
            {
                return this.FtpProductBasePath + "/" + this.GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Zip);
            }
        }

        /// <summary>
        /// Gets the ftp full x 64 package path.
        /// </summary>
        internal string FtpFullX64PackagePath
        {
            get
            {
                return this.FtpProductBasePath + "/" + this.GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Zip);
            }
        }

        /// <summary>
        /// Gets the ftp full x 86 installer path.
        /// </summary>
        internal string FtpFullX86InstallerPath
        {
            get
            {
                return this.FtpProductBasePath + "/" + this.GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Installer);
            }
        }

        /// <summary>
        /// Gets the ftp full x 64 installer path.
        /// </summary>
        internal string FtpFullX64InstallerPath
        {
            get
            {
                return this.FtpProductBasePath + "/" + this.GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Installer);
            }
        }

        /// <summary>
        /// Gets the ftp x 86 package relative path.
        /// </summary>
        internal string FtpX86PackageRelativePath
        {
            get
            {
                return this.GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Zip);
            }
        }

        /// <summary>
        /// Gets the ftp x 64 package relative path.
        /// </summary>
        internal string FtpX64PackageRelativePath
        {
            get
            {
                return this.GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Zip);
            }
        }

        /// <summary>
        /// Gets the ftp x 86 installer relative path.
        /// </summary>
        internal string FtpX86InstallerRelativePath
        {
            get
            {
                return this.GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Installer);
            }
        }

        /// <summary>
        /// Gets the ftp x 64 installer relative path.
        /// </summary>
        internal string FtpX64InstallerRelativePath
        {
            get
            {
                return this.GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Installer);
            }
        }

        /// <summary>
        /// Gets the default installation path.
        /// </summary>
        internal string DefaultInstallationPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppUtil.CompanyName) + "\\"
                       + AppUtil.ProductName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether has installers.
        /// </summary>
        public bool HasInstallers
        {
            get
            {
                return !string.IsNullOrEmpty(this.InnoExecutable) && !string.IsNullOrEmpty(this.InnoScriptPath);
            }
        }

        /// <summary>
        /// Gets the ftp server url.
        /// </summary>
        internal string FtpServerUrl
        {
            get
            {
                return string.IsNullOrEmpty(this.FtpServer) ? null : "ftp://" + this.FtpServer + ":" + this.FtpPort;
            }
        }

        /// <summary>
        /// The get version relative path.
        /// </summary>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetVersionRelativePath(ProductPlatform platform, PackageType type)
        {
            return string.Format(
                "{0}\\{1}-{0}-{2}." + (type == PackageType.Zip ? "zip" : "exe"), 
                AppUtil.ProductVersion, 
                AppUtil.ProductName, 
                platform);
        }

        /// <summary>
        /// The get ftp version relative path.
        /// </summary>
        /// <param name="platform">
        /// The platform.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetFtpVersionRelativePath(ProductPlatform platform, PackageType type)
        {
            return this.GetVersionRelativePath(platform, type).Replace("\\", "/");
        }

        /// <summary>
        /// The from command line.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="PublishParameter"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal static PublishParameter FromCommandLine(string[] parameters)
        {
            string x86 = null, 
                   x64 = null, 
                   output = null, 
                   server = null, 
                   user = null, 
                   password = null, 
                   ftpPath = "/", 
                   subDir = null, 
                   proxyHost = null, 
                   innoExe = null, 
                   innoScript = null, 
                   innoExtraParams = null;
            int port = 21, proxyPort = 80;

            // Read the paramters from the command line
            for (int i = 0; i < parameters.Length; i = i + 2)
            {
                if (parameters[i] == "x86")
                {
                    x86 = parameters[i + 1];
                }

                if (parameters[i] == "x64")
                {
                    x64 = parameters[i + 1];
                }

                if (parameters[i] == "output")
                {
                    output = parameters[i + 1] ?? string.Empty;
                }

                if (parameters[i] == "server")
                {
                    server = parameters[i + 1];
                }

                if (parameters[i] == "user")
                {
                    user = parameters[i + 1];
                }

                if (parameters[i] == "password")
                {
                    password = parameters[i + 1];
                }

                if (parameters[i] == "port" && !int.TryParse(parameters[i + 1], out port))
                {
                    throw new ArgumentException("Invalid port: " + parameters[i + 1]);
                }

                if (parameters[i] == "proxyHost")
                {
                    proxyHost = parameters[i + 1];
                }

                if (parameters[i] == "proxyPort" && !int.TryParse(parameters[i + 1], out proxyPort))
                {
                    throw new ArgumentException("Invalid proxy port: " + parameters[i + 1]);
                }

                if (parameters[i] == "ftpBase")
                {
                    ftpPath = parameters[i + 1];
                }

                if (parameters[i] == "ftpDir")
                {
                    subDir = parameters[i + 1];
                }

                if (parameters[i] == "innoExe")
                {
                    innoExe = parameters[i + 1];
                }

                if (parameters[i] == "innoScript")
                {
                    innoScript = parameters[i + 1];
                }

                if (parameters[i] == "innoExtraParams")
                {
                    innoExtraParams = parameters[i + 1];
                }
            }

            // Validate the data
            if (string.IsNullOrEmpty(x86) || !Directory.Exists(x86))
            {
                throw new ArgumentException("Invalid x86 build location: " + x86);
            }

            if (string.IsNullOrEmpty(x64) || !Directory.Exists(x64))
            {
                throw new ArgumentException("Invalid x64 build location: " + x64);
            }

            if (!string.IsNullOrEmpty(innoExe) && !File.Exists(innoExe))
            {
                throw new ArgumentException("Invalid executable location for InnoSetup: " + innoExe);
            }

            if (!string.IsNullOrEmpty(innoScript) && !File.Exists(innoScript))
            {
                throw new ArgumentException("Invalid script location for InnoSetup: " + innoScript);
            }

            string coreAsmName = Path.GetFileName(typeof(IHelperService).Assembly.Location);
            string newx86CoreLoc = Path.Combine(x86, coreAsmName);
            var x86vi = FileVersionInfo.GetVersionInfo(newx86CoreLoc);
            string newx64CoreLoc = Path.Combine(x64, coreAsmName);
            var x64vi = FileVersionInfo.GetVersionInfo(newx64CoreLoc);
            if (x86vi.FileVersion != x64vi.FileVersion)
            {
                throw new ArgumentException("The built version of x86 and x64 binaries do not match");
            }

            output = Path.GetFullPath(output);

            return new PublishParameter
                       {
                           X86BuildLocation = x86, 
                           X64BuildLocation = x64, 
                           OutputLocation = output, 
                           FtpServer = server, 
                           FtpPort = port, 
                           FtpProxyHost = proxyHost, 
                           FtpProxyPort = proxyPort, 
                           FtpUser = user, 
                           FtpPassword = password, 
                           FtpRootPath = ftpPath, 
                           FtpSubDirectory = subDir, 
                           InnoExecutable = innoExe, 
                           InnoScriptPath = innoScript, 
                           InnoExtraParams = innoExtraParams
                       };
        }

        /// <summary>
        /// The product platform.
        /// </summary>
        private enum ProductPlatform
        {
            /// <summary>
            /// The x 86.
            /// </summary>
            x86, 

            /// <summary>
            /// The x 64.
            /// </summary>
            x64
        }

        /// <summary>
        /// The package type.
        /// </summary>
        private enum PackageType
        {
            /// <summary>
            /// The zip.
            /// </summary>
            Zip, 

            /// <summary>
            /// The installer.
            /// </summary>
            Installer
        }
    }
}