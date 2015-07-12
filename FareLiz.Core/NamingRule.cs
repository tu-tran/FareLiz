using System.Globalization;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// General naming rule, string formats...
    /// </summary>
    public static class NamingRule
    {
        public const string TIME_FORMAT = "HH:mm",
                            DATE_FORMAT = "dd.MM.yyyy",
                            DATA_DATETIME_FORMAT = "yyyy-MM-dd HH.mm.ss";

        /// <summary>
        /// The default culture for handling numeric values in en-US format
        /// </summary>
        public static readonly CultureInfo NumberCulture = new CultureInfo(CultureInfo.CurrentCulture.Name)
        {
            NumberFormat = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," }
        };
    }
}
