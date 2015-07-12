namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    public static class DateTimeExtensions
    {
        public static bool IsDefined(this DateTime date)
        {
            return !IsUndefined(date);
        }

        public static bool IsUndefined(this DateTime date)
        {
            return (date == DateTime.MinValue || date == DateTime.MaxValue);
        }

        public static string ToShortDayAndDateString(this DateTime date)
        {
            return date.ToString("ddd ") + date.ToShortDateString();
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static string ToReadableString(this TimeSpan span)
        {
            string formatted =
                  (span.Days > 0 ? span.Days + "d " : null)
                + (span.Hours > 0 ? span.Hours + "h " : null)
                + (span.Minutes > 0 ? span.Minutes + "m " : null)
                + (span.Seconds > 0 ? span.Seconds + "s " : null);

            return formatted.Trim();
        }

        public static string ToHourMinuteString(this TimeSpan span)
        {
            int totalMin = (int)span.TotalMinutes;
            int hour = totalMin / 60;
            int min = totalMin % 60;
            string formatted =
                  (hour > 0 ? hour + "h " : null)
                + (min > 0 ? min + "m " : null);

            return formatted.Trim();
        }
    }
}
