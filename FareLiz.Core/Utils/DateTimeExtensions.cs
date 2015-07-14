namespace SkyDean.FareLiz.Core.Utils
{
    using System;

    /// <summary>The date time extensions.</summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// The is defined.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsDefined(this DateTime date)
        {
            return !IsUndefined(date);
        }

        /// <summary>
        /// The is undefined.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsUndefined(this DateTime date)
        {
            return date == DateTime.MinValue || date == DateTime.MaxValue;
        }

        /// <summary>
        /// The to short day and date string.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToShortDayAndDateString(this DateTime date)
        {
            return date.ToString("ddd ") + date.ToShortDateString();
        }

        /// <summary>
        /// The start of week.
        /// </summary>
        /// <param name="dt">
        /// The dt.
        /// </param>
        /// <param name="startOfWeek">
        /// The start of week.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            var diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// The to readable string.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToReadableString(this TimeSpan span)
        {
            var formatted = (span.Days > 0 ? span.Days + "d " : null) + (span.Hours > 0 ? span.Hours + "h " : null)
                            + (span.Minutes > 0 ? span.Minutes + "m " : null) + (span.Seconds > 0 ? span.Seconds + "s " : null);

            return formatted.Trim();
        }

        /// <summary>
        /// The to hour minute string.
        /// </summary>
        /// <param name="span">
        /// The span.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ToHourMinuteString(this TimeSpan span)
        {
            var totalMin = (int)span.TotalMinutes;
            var hour = totalMin / 60;
            var min = totalMin % 60;
            var formatted = (hour > 0 ? hour + "h " : null) + (min > 0 ? min + "m " : null);

            return formatted.Trim();
        }
    }
}