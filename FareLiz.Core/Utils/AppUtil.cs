namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The app util.</summary>
    public static class AppUtil
    {
        /// <summary>The company name.</summary>
        public static readonly string CompanyName;

        /// <summary>The product name.</summary>
        public static readonly string ProductName;

        /// <summary>The product copyright.</summary>
        public static readonly string ProductCopyright;

        /// <summary>The entry directory.</summary>
        public static readonly string EntryDirectory;

        /// <summary>The entry module name.</summary>
        public static readonly string EntryModuleName;

        /// <summary>The product version.</summary>
        public static readonly string ProductVersion;

        /// <summary>The local product data path.</summary>
        public static readonly string LocalProductDataPath;

        /// <summary>The publisher url.</summary>
        public static readonly string PublisherUrl;

        /// <summary>The publisher email.</summary>
        public static readonly string PublisherEmail;

        /// <summary>The product write date.</summary>
        public static readonly DateTime ProductWriteDate;

        /// <summary>The is debug build.</summary>
        public static readonly bool IsDebugBuild;

        /// <summary>Initializes static members of the <see cref="AppUtil" /> class.</summary>
        static AppUtil()
        {
            var executeAsm = Assembly.GetExecutingAssembly();

            var customAttributes = executeAsm.GetCustomAttributes(false);
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

            LocalProductDataPath = string.Format(
                @"{0}\{1}\{2}", 
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), 
                CompanyName, 
                ProductName);
        }

        /// <summary>
        /// The get local data path.
        /// </summary>
        /// <param name="subPath">
        /// The sub path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetLocalDataPath(string subPath)
        {
            return Path.Combine(LocalProductDataPath, subPath);
        }

        /// <summary>
        /// The name current thread.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public static void NameCurrentThread(string name)
        {
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                if (name.Length > 100)
                {
                    name = name.Substring(0, 100);
                }

                Thread.CurrentThread.Name = name;
            }
        }
    }
}