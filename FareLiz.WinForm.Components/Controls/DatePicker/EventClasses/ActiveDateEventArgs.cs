namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses
{
    using System;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker;

    /// <summary>Provides data for the <see cref="EnhancedMonthCalendar.ActiveDateChanged" /> or <see cref="EnhancedDatePicker.ActiveDateChanged" /> events.</summary>
    public class ActiveDateChangedEventArgs : DateEventArgs
    {
        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveDateChangedEventArgs"/> class.
        /// </summary>
        /// <param name="date">
        /// The corresponding date.
        /// </param>
        /// <param name="boldDate">
        /// true if the <paramref name="date"/> value is a bolded date.
        /// </param>
        public ActiveDateChangedEventArgs(DateTime date, bool boldDate)
            : base(date)
        {
            this.IsBoldDate = boldDate;
        }

        #endregion

        #region Properties

        /// <summary>Gets a value indicating whether the <see cref="DateEventArgs.Date" /> value is a bolded date.</summary>
        public bool IsBoldDate { get; private set; }

        #endregion
    }
}