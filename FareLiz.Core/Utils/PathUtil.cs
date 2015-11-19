namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>The path util.</summary>
    public static class PathUtil
    {
        /// <summary>The _app path.</summary>
        private static readonly string _appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        /// <summary>Gets the application path.</summary>
        public static string ApplicationPath
        {
            get
            {
                return _appPath;
            }
        }

        /// <summary>
        /// The remove invalid path chars.
        /// </summary>
        /// <param name="folderName">
        /// The folder name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveInvalidPathChars(string folderName)
        {
            return RemoveChars(folderName, Path.GetInvalidPathChars());
        }

        /// <summary>
        /// The remove invalid file name chars.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveInvalidFileNameChars(string fileName)
        {
            return RemoveChars(fileName, Path.GetInvalidFileNameChars());
        }

        /// <summary>
        /// The remove chars.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="invalidChars">
        /// The invalid chars.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveChars(string input, IEnumerable<char> invalidChars)
        {
            var result = new string(input.Where(c => !invalidChars.Contains(c)).ToArray());
            return result;
        }
    }
}