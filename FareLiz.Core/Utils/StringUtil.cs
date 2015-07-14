namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    /// <summary>The string util.</summary>
    public static class StringUtil
    {
        /// <summary>
        /// The format size.
        /// </summary>
        /// <param name="sizeInBytes">
        /// The size in bytes.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FormatSize(long sizeInBytes)
        {
            var fileSize = sizeInBytes / 1024.0;

            if (fileSize < 1024)
            {
                return fileSize.ToString("F01") + " KB";
            }

            fileSize /= 1024.0;

            if (fileSize < 1024)
            {
                return fileSize.ToString("F01") + " MB";
            }

            fileSize /= 1024;
            return fileSize.ToString("F01") + " GB";
        }

        /// <summary>
        /// The get period string.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetPeriodString(DateTime start, DateTime end)
        {
            return (start.IsUndefined() ? string.Empty : start.ToShortDayAndDateString())
                   + (end.IsUndefined() ? string.Empty : " - " + end.ToShortDayAndDateString());
        }
    }
}