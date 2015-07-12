using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SkyDean.FareLiz.Core.Utils
{
    public static class PathUtil
    {
        private static readonly string _appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public static string ApplicationPath { get { return _appPath; } }

        public static string RemoveInvalidPathChars(string folderName)
        {
            return RemoveChars(folderName, Path.GetInvalidPathChars());
        }

        public static string RemoveInvalidFileNameChars(string fileName)
        {
            return RemoveChars(fileName, Path.GetInvalidFileNameChars());
        }

        public static string RemoveChars(string input, IEnumerable<char> invalidChars)
        {
            var result = new String(input.Where(c => !invalidChars.Contains(c)).ToArray());
            return result;
        }
    }
}
