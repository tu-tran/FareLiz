using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Diagnostics;
using System.IO;

namespace SkyDean.FareLiz.Service.Versioning
{
    /// <summary>
    /// Paramter for publishing a new version
    /// </summary>
    internal class PublishParameter
    {
        private enum ProductPlatform { x86, x64 }
        private enum PackageType { Zip, Installer }

        internal string X86BuildLocation { get; private set; }
        internal string X64BuildLocation { get; private set; }
        internal string OutputLocation { get; private set; }
        internal string FtpServer { get; private set; }
        internal int FtpPort { get; private set; }
        internal string FtpProxyHost { get; set; }
        internal int FtpProxyPort { get; set; }
        internal string FtpUser { get; private set; }
        internal string FtpPassword { get; private set; }
        internal string InnoExecutable { get; private set; }
        internal string InnoScriptPath { get; private set; }
        internal string InnoExtraParams { get; private set; }

        private string ftpRootPath = "/";
        internal string FtpRootPath
        {
            get { return ftpRootPath; }
            set
            {
                ftpRootPath = "/" + (String.IsNullOrEmpty(value) ? null : value.Replace("\\", "/").Trim('/'));
            }
        }

        private string ftpSubDir = "";
        internal string FtpSubDirectory
        {
            get { return ftpSubDir; }
            set { ftpSubDir = (String.IsNullOrEmpty(value) ? null : value.Replace("\\", "/").Trim('/')); }
        }

        internal string ProductBasePath { get { return Path.Combine(OutputLocation, AppUtil.ProductName); } }
        internal string X86PackageFile { get { return Path.Combine(ProductBasePath, GetVersionRelativePath(ProductPlatform.x86, PackageType.Zip)); } }
        internal string X64PackageFile { get { return Path.Combine(ProductBasePath, GetVersionRelativePath(ProductPlatform.x64, PackageType.Zip)); } }

        internal string X86InstallerFile { get { return Path.Combine(ProductBasePath, GetVersionRelativePath(ProductPlatform.x86, PackageType.Installer)); } }
        internal string X64InstallerFile { get { return Path.Combine(ProductBasePath, GetVersionRelativePath(ProductPlatform.x64, PackageType.Installer)); } }

        internal string FtpBasePath { get { return FtpRootPath + (String.IsNullOrEmpty(FtpSubDirectory) ? null : "/" + FtpSubDirectory); } }
        internal string FtpProductBasePath { get { return FtpBasePath + "/" + AppUtil.ProductName; } }

        internal string FtpFullX86PackagePath { get { return FtpProductBasePath + "/" + GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Zip); } }
        internal string FtpFullX64PackagePath { get { return FtpProductBasePath + "/" + GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Zip); } }

        internal string FtpFullX86InstallerPath { get { return FtpProductBasePath + "/" + GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Installer); } }
        internal string FtpFullX64InstallerPath { get { return FtpProductBasePath + "/" + GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Installer); } }

        internal string FtpX86PackageRelativePath { get { return GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Zip); } }
        internal string FtpX64PackageRelativePath { get { return GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Zip); } }

        internal string FtpX86InstallerRelativePath { get { return GetFtpVersionRelativePath(ProductPlatform.x86, PackageType.Installer); } }
        internal string FtpX64InstallerRelativePath { get { return GetFtpVersionRelativePath(ProductPlatform.x64, PackageType.Installer); } }

        internal string DefaultInstallationPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppUtil.CompanyName) + "\\" + AppUtil.ProductName; }
        }

        public bool HasInstallers
        {
            get { return !String.IsNullOrEmpty(InnoExecutable) && !String.IsNullOrEmpty(InnoScriptPath); }
        }

        private string GetVersionRelativePath(ProductPlatform platform, PackageType type)
        {
            return String.Format("{0}\\{1}-{0}-{2}." + (type == PackageType.Zip ? "zip" : "exe"), AppUtil.ProductVersion, AppUtil.ProductName, platform);
        }

        private string GetFtpVersionRelativePath(ProductPlatform platform, PackageType type)
        {
            return GetVersionRelativePath(platform, type).Replace("\\", "/");
        }

        internal string FtpServerUrl { get { return String.IsNullOrEmpty(FtpServer) ? null : "ftp://" + FtpServer + ":" + FtpPort; } }

        internal static PublishParameter FromCommandLine(string[] parameters)
        {
            string x86 = null, x64 = null, output = null, server = null, user = null, password = null, ftpPath = "/", subDir = null, proxyHost = null,
                   innoExe = null, innoScript = null, innoExtraParams = null;
            int port = 21, proxyPort = 80;

            // Read the paramters from the command line
            for (int i = 0; i < parameters.Length; i = i + 2)
            {
                if (parameters[i] == "x86") x86 = parameters[i + 1];
                if (parameters[i] == "x64") x64 = parameters[i + 1];
                if (parameters[i] == "output") output = parameters[i + 1] ?? "";
                if (parameters[i] == "server") server = parameters[i + 1];
                if (parameters[i] == "user") user = parameters[i + 1];
                if (parameters[i] == "password") password = parameters[i + 1];
                if (parameters[i] == "port" && !int.TryParse(parameters[i + 1], out port))
                    throw new ArgumentException("Invalid port: " + parameters[i + 1]);
                if (parameters[i] == "proxyHost") proxyHost = parameters[i + 1];
                if (parameters[i] == "proxyPort" && !int.TryParse(parameters[i + 1], out proxyPort))
                    throw new ArgumentException("Invalid proxy port: " + parameters[i + 1]);
                if (parameters[i] == "ftpBase") ftpPath = parameters[i + 1];
                if (parameters[i] == "ftpDir") subDir = parameters[i + 1];
                if (parameters[i] == "innoExe") innoExe = parameters[i + 1];
                if (parameters[i] == "innoScript") innoScript = parameters[i + 1];
                if (parameters[i] == "innoExtraParams") innoExtraParams = parameters[i + 1];
            }

            // Validate the data
            if (String.IsNullOrEmpty(x86) || !Directory.Exists(x86))
                throw new ArgumentException("Invalid x86 build location: " + x86);

            if (String.IsNullOrEmpty(x64) || !Directory.Exists(x64))
                throw new ArgumentException("Invalid x64 build location: " + x64);

            if (!String.IsNullOrEmpty(innoExe) && !File.Exists(innoExe))
                throw new ArgumentException("Invalid executable location for InnoSetup: " + innoExe);

            if (!String.IsNullOrEmpty(innoScript) && !File.Exists(innoScript))
                throw new ArgumentException("Invalid script location for InnoSetup: " + innoScript);

            string coreAsmName = Path.GetFileName(typeof(IHelperService).Assembly.Location);
            string newx86CoreLoc = Path.Combine(x86, coreAsmName);
            var x86vi = FileVersionInfo.GetVersionInfo(newx86CoreLoc);
            string newx64CoreLoc = Path.Combine(x64, coreAsmName);
            var x64vi = FileVersionInfo.GetVersionInfo(newx64CoreLoc);
            if (x86vi.FileVersion != x64vi.FileVersion)
                throw new ArgumentException("The built version of x86 and x64 binaries do not match");

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
    }
}
