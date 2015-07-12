using System;
using System.Globalization;

namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Utility class for storing the time period (with start and end date)
    /// </summary>
    public class DatePeriod : IComparable<DatePeriod>
    {
        public static readonly string DATE_FORMAT = "ddd dd/MM/yyyy";

        /// <summary>
        /// Start date of the period
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the period
        /// </summary>
        public DateTime EndDate { get; set; }        

        public DatePeriod(DateTime departureDate, DateTime returnDate)
        {
            StartDate = departureDate;
            EndDate = returnDate;
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} - {1}", StartDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture), EndDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Compare with other DatePeriod by start date and then end date. This method does not compare the total duration of the period
        /// </summary>
        public int CompareTo(DatePeriod other)
        {
            if (other == null)
                return 1;

            int result = StartDate.CompareTo(other.StartDate);
            if (result == 0)
                return EndDate.CompareTo(other.EndDate);
            return result;
        }

        public bool IsEquals(object obj)
        {
            return CompareTo(obj as DatePeriod) == 0;
        }

        /// <summary>
        /// Create DatePeriod object based on string. This method only works if the string is well-formatted or created using ToString() method
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static DatePeriod Parse(string text)
        {
            var delim = new[] { " - " };
            string[] dateStr = text.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            DateTime departDate = DateTime.ParseExact(dateStr[0], DATE_FORMAT, CultureInfo.InvariantCulture),
                     returnDate = DateTime.ParseExact(dateStr[0], DATE_FORMAT, CultureInfo.InvariantCulture);

            return new DatePeriod(departDate, returnDate);
        }
    }
}
