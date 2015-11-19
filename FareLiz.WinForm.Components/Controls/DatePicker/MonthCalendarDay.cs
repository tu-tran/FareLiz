namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker
{
    using System;
    using System.Drawing;

    /// <summary>Represents a day in the <see cref="Calendar" />.</summary>
    public class MonthCalendarDay
    {
        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthCalendarDay"/> class.
        /// </summary>
        /// <param name="month">
        /// The <see cref="MonthCalendarMonth"/> in which the day is in.
        /// </param>
        /// <param name="date">
        /// The <see cref="DateTime"/> the <see cref="MonthCalendarDay"/> represents.
        /// </param>
        public MonthCalendarDay(MonthCalendarMonth month, DateTime date)
        {
            this.Month = month;
            this.Date = date;
            this.Calendar = month.Calendar;
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the bounds of the day.</summary>
        public Rectangle Bounds { get; set; }

        /// <summary>Gets the date the <see cref="MonthCalendarDay" /> represents.</summary>
        public DateTime Date { get; private set; }

        /// <summary>Gets the <see cref="MonthCalendarMonth" /> the day is in.</summary>
        public MonthCalendarMonth Month { get; private set; }

        /// <summary>Gets the <see cref="Calendar" /> the <see cref="MonthCalendarMonth" /> is in.</summary>
        public EnhancedMonthCalendar Calendar { get; private set; }

        /// <summary>Gets a value indicating whether the represented date is selected.</summary>
        public bool Selected
        {
            get
            {
                return this.Calendar.IsSelected(this.Date);
            }
        }

        /// <summary>Gets a value indicating whether the mouse is over the represented date.</summary>
        public bool MouseOver
        {
            get
            {
                return this.Date == this.Calendar.MouseOverDay;
            }
        }

        /// <summary>Gets a value indicating whether the represented date is a trailing one.</summary>
        public bool TrailingDate
        {
            get
            {
                return this.Calendar.CultureCalendar.GetMonth(this.Date) != this.Calendar.CultureCalendar.GetMonth(this.Month.Date);
            }
        }

        /// <summary>Gets a value indicating whether the represented date is visible.</summary>
        public bool Visible
        {
            get
            {
                if (this.Date == this.Calendar.ViewStart && this.Calendar.ViewStart == this.Calendar.MinDate)
                {
                    return true;
                }

                return this.Date >= this.Calendar.MinDate && this.Date <= this.Calendar.MaxDate
                       && !(this.TrailingDate && this.Date >= this.Calendar.ViewStart && this.Date <= this.Calendar.ViewEnd);
            }
        }

        #endregion
    }
}