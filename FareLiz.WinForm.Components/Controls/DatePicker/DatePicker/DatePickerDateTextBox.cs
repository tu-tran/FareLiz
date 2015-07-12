namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Helper;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Interfaces;

    /// <summary>
    /// Control that handles displaying and entering a date.
    /// </summary>
    internal sealed partial class DatePickerDateTextBox : Control
    {
        #region Fields
        /// <summary>
        /// The text box for manual input.
        /// </summary>
        private readonly InputDateTextBox _inputBox;
        internal InputDateTextBox InputBox { get { return this._inputBox; } }

        /// <summary>
        /// The parent date picker control.
        /// </summary>
        private readonly EnhancedDatePicker _enhancedDatePicker;

        /// <summary>
        /// Indicates that a date part is entered manually.
        /// </summary>
        private bool _inEditMode;

        /// <summary>
        /// Indicates whether the selected date is valid.
        /// </summary>
        private bool _isValidDate = true;

        /// <summary>
        /// The selected date part.
        /// </summary>
        private SelectedDatePart _selectedPart = SelectedDatePart.None;

        /// <summary>
        /// The day bounds.
        /// </summary>
        private RectangleF _dayBounds;

        /// <summary>
        /// The month bounds.
        /// </summary>
        private RectangleF _monthBounds;

        /// <summary>
        /// The year bounds.
        /// </summary>
        private RectangleF _yearBounds;

        /// <summary>
        /// The current date.
        /// </summary>
        private DateTime _currentDate;

        /// <summary>
        /// The background color for invalid dates.
        /// </summary>
        private Color _invalidDateBackColor;

        /// <summary>
        /// The text color for invalid dates.
        /// </summary>
        private Color _invalidDateForeColor;

        /// <summary>
        /// The day part index of the date string.
        /// </summary>
        private int _dayPartIndex;

        /// <summary>
        /// The month part index of the date string.
        /// </summary>
        private int _monthPartIndex;

        /// <summary>
        /// The year part index of the date string.
        /// </summary>
        private int _yearPartIndex;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DatePickerDateTextBox"/> class.
        /// </summary>
        /// <param name="picker">The parent <see cref="EnhancedDatePicker"/> control.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="picker"/> is <c>null</c>.</exception>
        public DatePickerDateTextBox(EnhancedDatePicker picker)
        {
            if (picker == null)
                throw new ArgumentNullException("picker", "parameter 'picker' cannot be null.");

            this._enhancedDatePicker = picker;

            this.SetStyle(
               ControlStyles.AllPaintingInWmPaint
               | ControlStyles.OptimizedDoubleBuffer
               | ControlStyles.Selectable
               | ControlStyles.Opaque
               | ControlStyles.ResizeRedraw
               | ControlStyles.UserPaint,
               true);

            this._currentDate = DateTime.Today;
            this._invalidDateBackColor = Color.Red;
            this._invalidDateForeColor = this.ForeColor;

            this._inputBox = new InputDateTextBox(this)
            {
                Visible = false,
                Multiline = true,
                ShortcutsEnabled = false
            };

            this._inputBox.FinishedEditing += this.InputBoxFinishedEditing;

            this.Controls.Add(this._inputBox);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="DatePickerDateTextBox"/> class from being created.
        /// </summary>
        private DatePickerDateTextBox()
        { }
        #endregion

        /// <summary>
        /// Event that is raised if a new date is to be set and provides the possibility to check
        /// if the date to be set is valid in a custom situation or not.
        /// </summary>
        public event EventHandler<CheckDateEventArgs> CheckDate;

        #region Properties

        /// <summary>
        /// Gets or sets the currently displayed date.
        /// </summary>
        public DateTime Date
        {
            get { return this._currentDate; }

            set
            {
                if (value == this._currentDate)
                    return;

                if (value < this.MinDate)
                    value = this.MinDate;
                else if (value > this.MaxDate)
                    value = this.MaxDate;
                this.SetNewDate(value);
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the background color if an entered date is invalid.
        /// </summary>
        public Color InvalidBackColor
        {
            get { return this._invalidDateBackColor; }
            set
            {
                if (value.IsEmpty || value == this._invalidDateBackColor)
                    return;
                this._invalidDateBackColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the text color if an entered date is invalid.
        /// </summary>
        public Color InvalidForeColor
        {
            get { return this._invalidDateForeColor; }
            set
            {
                if (value.IsEmpty || value == this._invalidDateForeColor)
                    return;
                this._invalidDateForeColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the minimum visible date.
        /// </summary>
        public DateTime MinDate { get; set; }

        /// <summary>
        /// Gets or sets the maximum visible date.
        /// </summary>
        public DateTime MaxDate { get; set; }

        /// <summary>
        /// Gets the currently used culture.
        /// </summary>
        public CultureInfo Culture
        {
            get { return this._enhancedDatePicker.Culture; }
        }

        /// <summary>
        /// Gets a value indicating whether the control has input focus.
        /// </summary>
        public override bool Focused
        {
            get { return base.Focused || this._inputBox.Focused; }
        }

        /// <summary>
        /// Gets or sets the forecolor.
        /// </summary>
        public sealed override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in edit mode.
        /// </summary>
        public bool InEditMode
        {
            get { return this._inEditMode; }
        }
        #endregion

        #region methods
        #region protected methods
        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();

            var dayDist = (int)Math.Min(Math.Abs(this._dayBounds.Left - e.Location.X), Math.Abs(this._dayBounds.Right - e.Location.X));
            var monthDist = (int)Math.Min(Math.Abs(this._monthBounds.Left - e.Location.X), Math.Abs(this._monthBounds.Right - e.Location.X));
            var yearDist = (int)Math.Min(Math.Abs(this._yearBounds.Left - e.Location.X), Math.Abs(this._yearBounds.Right - e.Location.X));

            var min = Math.Min(dayDist, Math.Min(monthDist, yearDist));

            if (this._dayBounds.Contains(e.Location) || min == dayDist)
            {
                this._selectedPart = SelectedDatePart.Day;
            }
            else if (this._monthBounds.Contains(e.Location) || min == monthDist)
            {
                this._selectedPart = SelectedDatePart.Month;
            }
            else if (this._yearBounds.Contains(e.Location) || min == yearDist)
            {
                this._selectedPart = SelectedDatePart.Year;
            }

            this.Refresh();

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Processes a dialog key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>true if the key was processed by the control; otherwise, false.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.Right)
            {
                this.SetDatePart(keyData == Keys.Left ? this.RightToLeft == RightToLeft.No : this.RightToLeft == RightToLeft.Yes);

                return true;
            }

            Calendar cal = this._enhancedDatePicker.PickerCalendar.CultureCalendar;

            MonthCalendarDate dt = new MonthCalendarDate(cal, this._currentDate);

            DateTime date = this.Date;

            if (keyData == Keys.Up || keyData == Keys.Down)
            {
                bool up = keyData == Keys.Up;

                switch (this._selectedPart)
                {
                    case SelectedDatePart.Day:
                        {
                            int day = dt.Day + (up ? 1 : -1);

                            int daysInMonth = DateMethods.GetDaysInMonth(dt);

                            if (day > daysInMonth)
                            {
                                day = 1;
                            }
                            else if (day < 1)
                            {
                                day = daysInMonth;
                            }

                            date = new DateTime(dt.Year, dt.Month, day, cal);

                            break;
                        }

                    case SelectedDatePart.Month:
                        {
                            int day = dt.Day;

                            int month = dt.Month + (up ? 1 : -1);

                            int monthsInYear = cal.GetMonthsInYear(dt.Year);

                            if (month > monthsInYear)
                            {
                                month = 1;
                            }
                            else if (month < 1)
                            {
                                month = monthsInYear;
                            }

                            DateTime newDate = new DateTime(dt.Year, month, 1, cal);

                            dt = new MonthCalendarDate(cal, newDate);

                            int daysInMonth = DateMethods.GetDaysInMonth(dt);

                            newDate = daysInMonth < day ? cal.AddDays(newDate, daysInMonth - 1) : cal.AddDays(newDate, day - 1);

                            date = newDate;

                            break;
                        }

                    case SelectedDatePart.Year:
                        {
                            int year = dt.Year + (up ? 1 : -1);
                            int minYear = cal.GetYear(this.MinDate);
                            int maxYear = cal.GetYear(this.MaxDate);

                            year = Math.Max(minYear, Math.Min(year, maxYear));

                            int yearDiff = year - dt.Year;

                            date = cal.AddYears(this._currentDate, yearDiff);

                            break;
                        }
                }

                this.Date = date < this.MinDate ? this.MinDate : (date > this.MaxDate ? this.MaxDate : date);

                this.Refresh();

                return true;
            }

            if (keyData == Keys.Home || keyData == Keys.End)
            {
                bool first = keyData == Keys.Home;

                switch (this._selectedPart)
                {
                    case SelectedDatePart.Day:
                        {
                            date = first ? new DateTime(dt.Year, dt.Month, 1, cal)
                               : new DateTime(dt.Year, dt.Month, DateMethods.GetDaysInMonth(dt), cal);

                            break;
                        }

                    case SelectedDatePart.Month:
                        {
                            int day = dt.Day;

                            date = first ? new DateTime(dt.Year, 1, 1, cal)
                               : new DateTime(dt.Year, cal.GetMonthsInYear(dt.Year), 1, cal);

                            int daysInMonth = DateMethods.GetDaysInMonth(dt);

                            date = day > daysInMonth ? cal.AddDays(date, daysInMonth - 1)
                               : cal.AddDays(date, day - 1);

                            break;
                        }

                    case SelectedDatePart.Year:
                        {
                            date = first ? this.MinDate.Date : this.MaxDate.Date;

                            break;
                        }
                }

                this.Date = date < this.MinDate ? this.MinDate : (date > this.MaxDate ? this.MaxDate : date);

                this.Refresh();

                return true;
            }

            if (keyData == Keys.Space && !this._inEditMode)
            {
                this._enhancedDatePicker.SwitchPickerState();

                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;

            if ((e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
               && (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9))
            {
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.KeyPress"/> event.
        /// </summary>
        /// <param name="e">A <see cref="KeyPressEventArgs"/> that contains the event data.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;

            if (!char.IsDigit(e.KeyChar))
            {
                return;
            }

            if (!this._inEditMode)
            {
                this._inEditMode = true;
                this.Refresh();

                var keyCharString = e.KeyChar.ToString(CultureInfo.InvariantCulture);

                if (this._enhancedDatePicker.UseNativeDigits)
                {
                    var number = int.Parse(keyCharString);
                    keyCharString = DateMethods.GetNativeNumberString(number, this._enhancedDatePicker.Culture.NumberFormat.NativeDigits, false);
                }

                this._inputBox.Font = this.Font;
                this._inputBox.Location = new Point(0, 2);
                this._inputBox.Size = this.Size;
                this._inputBox.Text = keyCharString;
                this._inputBox.Visible = true;
                this._inputBox.SelectionStart = 1;
                this._inputBox.SelectionLength = 0;
                this._inputBox.BringToFront();
                this._inputBox.Focus();
            }

            this.Refresh();
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this._inEditMode)
            {
                e.Graphics.Clear(this.BackColor);
                base.OnPaint(e);
                return;
            }

            e.Graphics.Clear(this.Enabled ? (this._isValidDate ? this.BackColor : this._invalidDateBackColor) : SystemColors.Window);

            using (StringFormat format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoClip | StringFormatFlags.NoWrap))
            {
                format.LineAlignment = StringAlignment.Center;

                if (this.RightToLeft == RightToLeft.Yes)
                    format.Alignment = StringAlignment.Far;

                using (SolidBrush foreBrush = new SolidBrush(this.Enabled ? (this._isValidDate ? this.ForeColor : this._invalidDateForeColor) : SystemColors.GrayText),
                   selectedBrush = new SolidBrush(SystemColors.HighlightText),
                   selectedBack = new SolidBrush(SystemColors.Highlight))
                {
                    EnhancedMonthCalendar cal = this._enhancedDatePicker.PickerCalendar;
                    ICustomFormatProvider provider = cal.FormatProvider;

                    MonthCalendarDate date = new MonthCalendarDate(cal.CultureCalendar, this._currentDate);

                    DatePatternParser parser = new DatePatternParser(provider.LongDatePattern, provider);

                    string dateString = parser.ParsePattern(date, this._enhancedDatePicker.UseNativeDigits ? this._enhancedDatePicker.Culture.NumberFormat.NativeDigits : null);

                    this._dayPartIndex = parser.DayPartIndex;
                    this._monthPartIndex = parser.MonthPartIndex;
                    this._yearPartIndex = parser.YearPartIndex;

                    List<CharacterRange> rangeList = new List<CharacterRange>();

                    int dayIndex = parser.DayIndex;
                    int monthIndex = parser.MonthIndex;
                    int yearIndex = parser.YearIndex;

                    if (!string.IsNullOrEmpty(parser.DayString))
                        rangeList.Add(new CharacterRange(dayIndex, parser.DayString.Length));

                    if (!string.IsNullOrEmpty(parser.MonthString))
                        rangeList.Add(new CharacterRange(monthIndex, parser.MonthString.Length));

                    if (!string.IsNullOrEmpty(parser.YearString))
                        rangeList.Add(new CharacterRange(yearIndex, parser.YearString.Length));

                    format.SetMeasurableCharacterRanges(rangeList.ToArray());

                    Rectangle layoutRect = this.ClientRectangle;

                    e.Graphics.DrawString(dateString, this.Font, foreBrush, layoutRect, format);

                    Region[] dateRegions = e.Graphics.MeasureCharacterRanges(dateString, this.Font, layoutRect, format);

                    this._dayBounds = dateRegions[0].GetBounds(e.Graphics);
                    this._monthBounds = dateRegions[1].GetBounds(e.Graphics);
                    this._yearBounds = dateRegions[2].GetBounds(e.Graphics);

                    if (this._selectedPart == SelectedDatePart.Day)
                    {
                        e.Graphics.FillRectangle(selectedBack, this._dayBounds.X, this._dayBounds.Y - 2, this._dayBounds.Width + 1, this._dayBounds.Height + 1);
                        e.Graphics.DrawString(parser.DayString, this.Font, selectedBrush, this._dayBounds.X - 2, this._dayBounds.Y - 2);
                    }

                    if (this._selectedPart == SelectedDatePart.Month)
                    {
                        e.Graphics.FillRectangle(selectedBack, this._monthBounds.X, this._monthBounds.Y - 2, this._monthBounds.Width + 1, this._monthBounds.Height + 1);
                        e.Graphics.DrawString(parser.MonthString, this.Font, selectedBrush, this._monthBounds.X - 2, this._monthBounds.Y - 2);
                    }

                    if (this._selectedPart == SelectedDatePart.Year)
                    {
                        e.Graphics.FillRectangle(selectedBack, this._yearBounds.X, this._yearBounds.Y - 2, this._yearBounds.Width + 1, this._yearBounds.Height + 1);
                        e.Graphics.DrawString(parser.YearString, this.Font, selectedBrush, this._yearBounds.X - 2, this._yearBounds.Y - 2);
                    }
                }
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the System.Windows.Forms.Control and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._inputBox.FinishedEditing -= this.InputBoxFinishedEditing;

                this._inputBox.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.GotFocus"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            this._selectedPart = SelectedDatePart.Day;

            this.Refresh();
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.LostFocus"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            this._selectedPart = SelectedDatePart.None;

            this.Refresh();
        }

        /// <summary>
        /// Raises the <see cref="Control.BackColorChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            if (this._inputBox != null)
            {
                this._inputBox.BackColor = this.BackColor;
            }

            base.OnBackColorChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="Control.ForeColorChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            if (this._inputBox != null)
            {
                this._inputBox.ForeColor = this.ForeColor;
            }

            base.OnForeColorChanged(e);
        }
        #endregion

        #region private methods
        /// <summary>
        /// Determines if the specified <paramref name="day"/> is a valid day in regard to the <paramref name="date"/> value.
        /// </summary>
        /// <param name="day">The day value.</param>
        /// <param name="date">The year and month value.</param>
        /// <param name="cal">The calendar to use.</param>
        /// <returns>true if it's a valid day; false otherwise.</returns>
        private static bool IsValidDay(int day, DateTime date, Calendar cal)
        {
            return day >= 1 && day <= DateMethods.GetDaysInMonth(new MonthCalendarDate(cal, date));
        }

        /// <summary>
        /// Determines if the specified <paramref name="day"/> is a valid day in regard to the <paramref name="date"/> value.
        /// </summary>
        /// <param name="day">The day value.</param>
        /// <param name="date">The year and month value.</param>
        /// <param name="cal">The calendar to use.</param>
        /// <returns>true if it's a valid day; false otherwise.</returns>
        private static bool IsValidDay(string day, DateTime date, Calendar cal)
        {
            return IsValidDay(string.IsNullOrEmpty(day) ? 0 : int.Parse(day), date, cal);
        }

        /// <summary>
        /// Determines if the specified <paramref name="month"/> is a valid month value.
        /// </summary>
        /// <param name="month">The month value.</param>
        /// <param name="year">The year value.</param>
        /// <param name="cal">The calendar to use.</param>
        /// <returns>true if it's a valid month value; false otherwise.</returns>
        private static bool IsValidMonth(int month, int year, Calendar cal)
        {
            year = cal.ToFourDigitYear(year);

            return month >= 1 && month <= cal.GetMonthsInYear(year);
        }

        /// <summary>
        /// Determines if the specified <paramref name="month"/> is a valid month value.
        /// </summary>
        /// <param name="month">The month value.</param>
        /// <param name="year">The year value.</param>
        /// <param name="cal">The calendar to use.</param>
        /// <returns>true if it's a valid month value; false otherwise.</returns>
        private static bool IsValidMonth(string month, int year, Calendar cal)
        {
            return IsValidMonth(string.IsNullOrEmpty(month) ? 0 : int.Parse(month), year, cal);
        }

        /// <summary>
        /// Determines if the specified <paramref name="year"/> is a valid year value.
        /// </summary>
        /// <param name="year">The year value.</param>
        /// <param name="cal">The calendar to use.</param>
        /// <param name="era">The era the year belongs to.</param>
        /// <returns>true if it's a valid year value; false otherwise.</returns>
        private static bool IsValidYear(int year, Calendar cal, int era)
        {
            int minYear = cal.GetYear(cal.MinSupportedDateTime.Date);
            int maxYear = cal.GetYear(cal.MaxSupportedDateTime.Date);

            if (cal.Eras.Length > 1)
            {
                DateTime? minDate = null, maxDate = null;

                DateTime date = cal.MinSupportedDateTime;

                while (date < cal.MaxSupportedDateTime.Date)
                {
                    int e = cal.GetEra(date);

                    if (e == era)
                    {
                        if (minDate == null)
                        {
                            minDate = date;
                        }

                        maxDate = date;
                    }

                    date = cal.AddDays(date, 1);
                }

                minYear = cal.GetYear(minDate.GetValueOrDefault(cal.MinSupportedDateTime.Date));
                maxYear = cal.GetYear(maxDate.GetValueOrDefault(cal.MaxSupportedDateTime.Date));
            }

            year = cal.ToFourDigitYear(year);

            return year >= minYear && year <= maxYear;
        }

        /// <summary>
        /// Sets the next date part specified by the <paramref name="left"/>.
        /// </summary>
        /// <param name="left">true for selecting the next left date part; false for the next right date part.</param>
        private void SetDatePart(bool left)
        {
            int index = -1;

            switch (this._selectedPart)
            {
                case SelectedDatePart.Day:
                    {
                        index = this._dayPartIndex;

                        break;
                    }

                case SelectedDatePart.Month:
                    {
                        index = this._monthPartIndex;

                        break;
                    }

                case SelectedDatePart.Year:
                    {
                        index = this._yearPartIndex;

                        break;
                    }
            }

            if (index != -1)
            {
                this._selectedPart = this.GetNextSelectedPart(index, left);
            }

            this.Refresh();
        }

        /// <summary>
        /// Gets the selected date part for the specified direction and the current index.
        /// </summary>
        /// <param name="currentIndex">The index of the currently selected date part.</param>
        /// <param name="left">The moving direction.</param>
        /// <returns>The next selected date part.</returns>
        private SelectedDatePart GetNextSelectedPart(int currentIndex, bool left)
        {
            int newIndex = currentIndex + (left ? -1 : 1);

            if (newIndex < 0)
            {
                newIndex = 2;
            }
            else if (newIndex > 2)
            {
                newIndex = 0;
            }

            if (this._dayPartIndex == newIndex)
            {
                return SelectedDatePart.Day;
            }

            if (this._monthPartIndex == newIndex)
            {
                return SelectedDatePart.Month;
            }

            if (this._yearPartIndex == newIndex)
            {
                return SelectedDatePart.Year;
            }

            return SelectedDatePart.None;
        }

        /// <summary>
        /// Sets the specified <paramref name="date"/> as the currently displayed date.
        /// </summary>
        /// <param name="date">The date to set.</param>
        private void SetNewDate(DateTime date)
        {
            this._currentDate = date;

            if (this.CheckDate != null)
            {
                CheckDateEventArgs checkEventArgs = new CheckDateEventArgs(date, true);

                this.CheckDate(this, checkEventArgs);

                this._isValidDate = checkEventArgs.IsValid;

                this.Invalidate();
            }
        }

        /// <summary>
        /// Handles the <see cref="InputDateTextBox.FinishedEditing"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void InputBoxFinishedEditing(object sender, EventArgs e)
        {
            this._inputBox.Visible = false;

            string inputStr = this._inputBox.GetCurrentText();

            bool containsSeparator = inputStr.Contains(this._enhancedDatePicker.FormatProvider.DateSeparator);
            string aggregate = string.Empty;

            Dictionary<int, string> dic;

            if (containsSeparator)
            {
                aggregate = this._enhancedDatePicker.FormatProvider.DateSeparator;

                dic = new Dictionary<int, string>
            {
               { this._yearPartIndex, @"(?<year>\d{2,4})" },
               { this._dayPartIndex, @"(?<day>\d\d?)" },
               { this._monthPartIndex, @"(?<month>\d\d?)" }
            };
            }
            else
            {
                var yearLength = inputStr.Length == 8 ? 4 : 2;

                dic = new Dictionary<int, string>
            {
               { this._yearPartIndex, string.Format(@"(?<year>\d{{{0}}})", yearLength) },
               { this._dayPartIndex, @"(?<day>\d\d)" },
               { this._monthPartIndex, @"(?<month>\d\d)" }
            };
            }

            var sortedKeys = dic.Keys.ToList();

            sortedKeys.Sort();

            var regexPattern = sortedKeys.ConvertAll(i => dic[i]).Aggregate((s1, s2) => s1 + aggregate + s2);
            var match = System.Text.RegularExpressions.Regex.Match(inputStr, regexPattern);

            var groups = match.Groups;

            var dayString = groups["day"].Value;
            var monthString = groups["month"].Value;
            var yearString = groups["year"].Value;

            if (match.Success && !string.IsNullOrEmpty(dayString) && !string.IsNullOrEmpty(monthString) && !string.IsNullOrEmpty(yearString))
            {
                int year = int.Parse(yearString);

                Calendar cal = this._enhancedDatePicker.PickerCalendar.CultureCalendar;

                year = cal.ToFourDigitYear(year);

                if (IsValidYear(year, cal, cal.GetEra(DateTime.Today)) && IsValidMonth(monthString, year, cal))
                {
                    DateTime date = new DateTime(year, int.Parse(monthString), 1, cal);

                    if (IsValidDay(dayString, date, cal))
                    {
                        this.Date = cal.AddDays(date, int.Parse(dayString) - 1);
                    }
                }
            }

            this._inEditMode = false;

            this.Focus();
        }
        #endregion

        #endregion

        private enum SelectedDatePart { Day, Month, Year, None }
    }
}