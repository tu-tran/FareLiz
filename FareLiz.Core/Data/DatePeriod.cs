namespace SkyDean.FareLiz.Core.Data
{
    using System;
    using System.Globalization;

    /// <summary>Utility class for storing the time period (with start and end date)</summary>
    public class DatePeriod : IComparable<DatePeriod>
    {
        /// <summary>The dat e_ format.</summary>
        public static readonly string DATE_FORMAT = "ddd dd/MM/yyyy";

        /// <summary>
        /// Initializes a new instance of the <see cref="DatePeriod"/> class.
        /// </summary>
        /// <param name="departureDate">
        /// The departure date.
        /// </param>
        /// <param name="returnDate">
        /// The return date.
        /// </param>
        public DatePeriod(DateTime departureDate, DateTime returnDate)
        {
            this.StartDate = departureDate;
            this.EndDate = returnDate;
        }

        /// <summary>Start date of the period</summary>
        public DateTime StartDate { get; set; }

        /// <summary>End date of the period</summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Compare with other DatePeriod by start date and then end date. This method does not compare the total duration of the period
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int CompareTo(DatePeriod other)
        {
            if (other == null)
            {
                return 1;
            }

            int result = this.StartDate.CompareTo(other.StartDate);
            if (result == 0)
            {
                return this.EndDate.CompareTo(other.EndDate);
            }

            return result;
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, 
                "{0} - {1}", 
                this.StartDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture), 
                this.EndDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The is equals.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsEquals(object obj)
        {
            return this.CompareTo(obj as DatePeriod) == 0;
        }

        /// <summary>
        /// Create DatePeriod object based on string. This method only works if the string is well-formatted or created using ToString() method
        /// </summary>
        /// <param name="text">
        /// </param>
        /// <returns>
        /// The <see cref="DatePeriod"/>.
        /// </returns>
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