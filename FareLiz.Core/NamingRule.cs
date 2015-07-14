namespace SkyDean.FareLiz.Core
{
    using System.Globalization;

    /// <summary>General naming rule, string formats...</summary>
    public static class NamingRule
    {
        /// <summary>The tim e_ format.</summary>
        public const string TIME_FORMAT = "HH:mm";

        /// <summary>The dat e_ format.</summary>
        public const string DATE_FORMAT = "dd.MM.yyyy";

        /// <summary>The dat a_ datetim e_ format.</summary>
        public const string DATA_DATETIME_FORMAT = "yyyy-MM-dd HH.mm.ss";

        /// <summary>The default culture for handling numeric values in en-US format</summary>
        public static readonly CultureInfo NumberCulture = new CultureInfo(CultureInfo.CurrentCulture.Name)
                                                               {
                                                                   NumberFormat =
                                                                       new NumberFormatInfo
                                                                           {
                                                                               NumberDecimalSeparator
                                                                                   = ".", 
                                                                               NumberGroupSeparator
                                                                                   = ","
                                                                           }
                                                               };
    }
}