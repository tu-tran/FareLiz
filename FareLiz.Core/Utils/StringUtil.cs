using System;

namespace SkyDean.FareLiz.Core.Utils
{
    public static class StringUtil
    {
        public static string FormatSize(long sizeInBytes)
        {
            double fileSize = (sizeInBytes / 1024.0);

            if (fileSize < 1024)
                return fileSize.ToString("F01") + " KB";
            else
            {
                fileSize /= 1024.0;

                if (fileSize < 1024)
                    return fileSize.ToString("F01") + " MB";
                else
                {
                    fileSize /= 1024;
                    return fileSize.ToString("F01") + " GB";
                }
            }
        }

        public static string GetPeriodString(DateTime start, DateTime end)
        {
            return (start.IsUndefined() ? "" : start.ToShortDayAndDateString()) +
                (end.IsUndefined() ? "" : " - " + end.ToShortDayAndDateString());
        }
    }
}
