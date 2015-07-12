using SkyDean.FareLiz.Core.Data;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace SkyDean.FareLiz.Core.Utils
{
    public static class AppUtil
    {
        public static readonly string CompanyName;
        public static readonly string ProductName;
        public static readonly string ProductCopyright;
        public static readonly string EntryDirectory;
        public static readonly string EntryModuleName;
        public static readonly string ProductVersion;
        public static readonly string LocalProductDataPath;
        public static readonly string PublisherUrl;
        public static readonly string PublisherEmail;
        public static readonly DateTime ProductWriteDate;
        public static readonly bool IsDebugBuild;

        static AppUtil()
        {
            var executeAsm = Assembly.GetExecutingAssembly();

            object[] customAttributes = executeAsm.GetCustomAttributes(false);
            foreach (var attrib in customAttributes)
            {
                var attribute = attrib as AssemblyCompanyAttribute;
                if (attribute != null)
                {
                    CompanyName = attribute.Company;
                    continue;
                }

                var productAttribute = attrib as AssemblyProductAttribute;
                if (productAttribute != null)
                {
                    ProductName = productAttribute.Product;
                    continue;
                }

                var copyrightAttribute = attrib as AssemblyCopyrightAttribute;
                if (copyrightAttribute != null)
                {
                    ProductCopyright = copyrightAttribute.Copyright;
                    continue;
                }

                var publisherAttribute = attrib as AssemblyPublisherAttribute;
                if (publisherAttribute != null)
                {
                    PublisherUrl = publisherAttribute.PublisherUrl;
                    PublisherEmail = publisherAttribute.PublisherEmail;
                }
            }

            ProductVersion = executeAsm.GetName().Version.ToString();
            ProductWriteDate = File.GetLastWriteTime(executeAsm.Location);

            var entryAsm = Assembly.GetEntryAssembly() ?? executeAsm;
            EntryDirectory = Path.GetDirectoryName(entryAsm.Location);
            EntryModuleName = Path.GetFileNameWithoutExtension(entryAsm.Location);

#if DEBUG
            IsDebugBuild = true;
#endif
#if !DEBUG
            IsDebugBuild = false;
#endif

            LocalProductDataPath = String.Format(@"{0}\{1}\{2}",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CompanyName, ProductName);
        }

        public static string GetLocalDataPath(string subPath)
        {
            return Path.Combine(LocalProductDataPath, subPath);
        }

        public static void NameCurrentThread(string name)
        {
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                if (name.Length > 100)
                    name = name.Substring(0, 100);
                Thread.CurrentThread.Name = name;
            }
        }
    }
}
