namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Interfaces;

    /// <summary>
    /// A custom date time picker control.
    /// </summary>
    [Designer(typeof(Design.DatePickerControlDesigner))]
    [ToolboxBitmap(typeof(DateTimePicker))]
    [DefaultEvent("ValueChanged")]
    public sealed partial class EnhancedDatePicker : Control
    {
        private Rectangle _buttonBounds;
        private ComboButtonState _buttonState;
        private bool _isDropped;
        private bool _cancelClosing;
        private bool _isFocused;

        private const int DropDownButtonWidth = 32;

        [Description("The currently selected date.")]
        [Category("Behavior")]
        public DateTime Value
        {
            get { return this.enhancedMonthCalendar.SelectionRange.Start; }
            set
            {
                if (this.enhancedMonthCalendar.SelectionStart == value || value < this.MinDate || value > this.MaxDate)
                    return;

                this.enhancedMonthCalendar.SelectionStart = value;
                this.dateTextBox.Date = value;
                this.enhancedMonthCalendar.EnsureSeletedDateIsVisible();
            }
        }

        [Description("The minimum selectable date.")]
        [Category("Behavior")]
        public DateTime MinDate
        {
            get { return this.enhancedMonthCalendar.MinDate; }

            set
            {
                this.enhancedMonthCalendar.MinDate = value;
                this.dateTextBox.MinDate = this.enhancedMonthCalendar.MinDate;
            }
        }

        /// <summary>
        /// Gets or sets the maximum selectable date.
        /// </summary>
        [Description("The maximum selectable date.")]
        [Category("Behavior")]
        public DateTime MaxDate
        {
            get { return this.enhancedMonthCalendar.MaxDate; }
            set
            {
                this.enhancedMonthCalendar.MaxDate = value;
                this.dateTextBox.MaxDate = this.enhancedMonthCalendar.MaxDate;
            }
        }

        /// <summary>
        /// Gets or sets the background color for invalid dates in the text field portion of the control.
        /// </summary>
        [Category("Appearance")]
        [Description("The backcolor for invalid dates in the text portion.")]
        [DefaultValue(typeof(Color), "Red")]
        public Color InvalidBackColor
        {
            get { return this.dateTextBox.InvalidBackColor; }
            set { this.dateTextBox.InvalidBackColor = value; }
        }

        /// <summary>
        /// Gets or sets the text color for invalid dates in the text field portion of the control.
        /// </summary>
        [Category("Appearance")]
        [Description("The text color for invalid dates in the text portion.")]
        public Color InvalidForeColor
        {
            get { return this.dateTextBox.InvalidForeColor; }
            set { this.dateTextBox.InvalidForeColor = value; }
        }

        /// <summary>
        /// Gets or sets the font for the days in the picker.
        /// </summary>
        [Category("Appearance")]
        [Description("The font for the days in the picker.")]
        public Font PickerDayFont
        {
            get { return this.enhancedMonthCalendar.Font; }
            set { this.enhancedMonthCalendar.Font = value; }
        }

        /// <summary>
        /// Gets or sets the picker header font.
        /// </summary>
        [Category("Appearance")]
        [Description("The font for the picker header.")]
        public Font PickerHeaderFont
        {
            get { return this.enhancedMonthCalendar.HeaderFont; }
            set { this.enhancedMonthCalendar.HeaderFont = value; }
        }

        /// <summary>
        /// Gets or sets the picker footer font.
        /// </summary>
        [Category("Appearance")]
        [Description("The font for the picker footer.")]
        public Font PickerFooterFont
        {
            get { return this.enhancedMonthCalendar.FooterFont; }
            set { this.enhancedMonthCalendar.FooterFont = value; }
        }

        /// <summary>
        /// Gets or sets the font for the picker day header.
        /// </summary>
        [Category("Appearance")]
        [Description("The font for the picker day header.")]
        public Font PickerDayHeaderFont
        {
            get { return this.enhancedMonthCalendar.DayHeaderFont; }
            set { this.enhancedMonthCalendar.DayHeaderFont = value; }
        }

        /// <summary>
        /// Gets or sets the text alignment for the days in the picker.
        /// </summary>
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        [Description("Determines the text alignment for the days in the picker.")]
        [Category("Appearance")]
        public ContentAlignment PickerDayTextAlignment
        {
            get { return this.enhancedMonthCalendar.DayTextAlignment; }
            set { this.enhancedMonthCalendar.DayTextAlignment = value; }
        }

        /// <summary>
        /// Gets or sets the list for bolded dates in the picker.
        /// </summary>
        [Description("The bolded dates in the picker.")]
        public List<DateTime> PickerBoldedDates
        {
            get { return this.enhancedMonthCalendar.BoldedDates; }
            set { this.enhancedMonthCalendar.BoldedDates = value; }
        }

        /// <summary>
        /// Gets the bolded dates.
        /// </summary>
        [Description("The bolded dates in the calendar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDatesCollection BoldedDatesCollection
        {
            get
            {
                return this.enhancedMonthCalendar.BoldedDatesCollection;
            }
        }

        /// <summary>
        /// Gets a collection holding the defined categories of bold dates.
        /// </summary>
        [Description("The bold date categories in the calendar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDateCategoryCollection BoldedDateCategoryCollection
        {
            get { return this.enhancedMonthCalendar.BoldedDateCategoryCollection; }
        }

        /// <summary>
        /// Gets or sets the culture used by the <see cref="EnhancedDatePicker"/>.
        /// </summary>
        [Category("Behavior")]
        [Description("The culture used by the EnhancedDatePicker.")]
        [TypeConverter(typeof(Design.CultureInfoCustomTypeConverter))]
        public CultureInfo Culture
        {
            get { return this.enhancedMonthCalendar.Culture; }

            set
            {
                if (value == null || value.IsNeutralCulture)
                    return;

                this.enhancedMonthCalendar.Culture = value;
                this.MinDate = this.enhancedMonthCalendar.MinDate;
                this.MaxDate = this.enhancedMonthCalendar.MaxDate;

                this.RightToLeft = this.enhancedMonthCalendar.UseRTL ? RightToLeft.Yes : RightToLeft.Inherit;

                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the used calendar.
        /// </summary>
        [Category("Behavior")]
        [Description("The calendar used by the EnhancedMonthCalendar.")]
        [Editor(typeof(Design.MonthCalendarCalendarUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(Design.MonthCalendarCalendarTypeConverter))]
        public Calendar CultureCalendar
        {
            get { return this.enhancedMonthCalendar.CultureCalendar; }
            set
            {
                this.enhancedMonthCalendar.CultureCalendar = value;
                this.MinDate = this.enhancedMonthCalendar.MinDate;
                this.MaxDate = this.enhancedMonthCalendar.MaxDate;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the interface for day name handling.
        /// </summary>
        [TypeConverter(typeof(Design.MonthCalendarNamesProviderTypeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Behavior")]
        [Description("Culture dependent settings for month/day names and date formatting.")]
        public ICustomFormatProvider FormatProvider
        {
            get { return this.enhancedMonthCalendar.FormatProvider; }
            set { this.enhancedMonthCalendar.FormatProvider = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the week header in the picker.
        /// </summary>
        [Category("Appearance")]
        [Description("Show the week header in the picker.")]
        [DefaultValue(true)]
        public bool ShowPickerWeekHeader
        {
            get { return this.enhancedMonthCalendar.ShowWeekHeader; }
            set { this.enhancedMonthCalendar.ShowWeekHeader = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to close the picker when clicking a day.
        /// </summary>
        [Category("Behavior")]
        [Description("Whether to close the picker on clicking a day or not (regardless whether the day is already selected).")]
        [DefaultValue(true)]
        public bool ClosePickerOnDayClick { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the shortest day names.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the shortest or the abbreviated day names in the day header of the picker.")]
        public bool UseShortestDayNames
        {
            get { return this.enhancedMonthCalendar.UseShortestDayNames; }
            set { this.enhancedMonthCalendar.UseShortestDayNames = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the native digits in <see cref="NumberFormatInfo.NativeDigits"/>
        /// specified by <see cref="EnhancedMonthCalendar.Culture"/>s <see cref="CultureInfo.NumberFormat"/>
        /// for number display.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the native digits as specified by the current Culture property.")]
        public bool UseNativeDigits
        {
            get { return this.enhancedMonthCalendar.UseNativeDigits; }
            set { this.enhancedMonthCalendar.UseNativeDigits = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow the input to be the current date separator.
        /// After editing is finished, tries to parse the input as specified by the ShortDatePattern.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        [Description("Allows the input to be the current date separator and tries to parse the date after the editing of the date finished.")]
        public bool AllowPromptAsInput { get; set; }

        /// <summary>
        /// Gets or sets the picker dimensions.
        /// </summary>
        [Category("Appearance")]
        [Description("The picker dimension.")]
        [DefaultValue(typeof(Size), "3,1")]
        public Size PickerDimension
        {
            get { return this.enhancedMonthCalendar.CalendarDimensions; }
            set { this.enhancedMonthCalendar.CalendarDimensions = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control has input focus.
        /// </summary>
        public override bool Focused
        {
            get { return base.Focused || this.dateTextBox.Focused || this.enhancedMonthCalendar.Focused || this.monthCalendarHost.Focused || this.dropDown.Focused; }
        }

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        public override Color BackColor
        {
            get { return base.BackColor; }
            set
            {
                base.BackColor = value;
                this.dateTextBox.BackColor = value;
            }
        }

        /// <summary>
        /// Gets the picker calendar.
        /// </summary>
        internal EnhancedMonthCalendar PickerCalendar
        {
            get { return this.enhancedMonthCalendar; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnhancedDatePicker"/> class.
        /// </summary>
        public EnhancedDatePicker()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint
                | ControlStyles.ResizeRedraw | ControlStyles.Selectable, true);
            this.InitializeComponent();
        }

        /// <summary>
        /// Shows or closes the picker according to the current picker RequestState.
        /// </summary>
        internal void SwitchPickerState()
        {
            if (this._isDropped)
            {
                this._buttonState = ComboButtonState.Hot;
                this._isDropped = false;
                this.dropDown.Close(ToolStripDropDownCloseReason.CloseCalled);
                this.Focus();
            }
            else
            {
                if (this._buttonState == ComboButtonState.Pressed)
                    this._buttonState = ComboButtonState.Hot;
                else if (this._buttonState == ComboButtonState.None)
                    this._buttonState = ComboButtonState.Hot;
                else
                {
                    this._buttonState = ComboButtonState.Pressed;
                    this.Refresh();
                    this.ShowDropDown();
                }
            }
        }

        /// <summary>
        /// Sets the bounds of the control
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="specified">true, if bounds where specified.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (width < 19)
                width = 19;

            height = this.MeasureControlSize();

            if (this.dateTextBox != null)
                this.dateTextBox.Size = new Size(this.Width - DropDownButtonWidth - 2 * SystemInformation.BorderSize.Width, this.Height - 2 * SystemInformation.BorderSize.Height);

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// Processes a dialog key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="Keys"/> value that represents the key to process.</param>
        /// <returns>true if the key was processed by the control; otherwise, false.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Space && !this.dateTextBox.InEditMode)
            {
                this.SwitchPickerState();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Is raised when the toolstrip drop down is closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="ToolStripDropDownClosingEventArgs"/> instance which holds the event data.</param>
        private void DropDownClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            if (this._cancelClosing)
            {
                this._cancelClosing = false;
                e.Cancel = true;
            }
            else
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.CloseCalled)
                {
                    this._buttonState = ComboButtonState.Hot;
                    this.Invalidate();
                }
                else
                    this._isDropped = false;
            }
        }

        /// <summary>
        /// Handles the <see cref="ToolStrip.ItemClicked"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="ToolStripItemClickedEventArgs"/> that contains the event data.</param>
        private void MenuItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this._cancelClosing = true;
        }

        private void MenuItemOpening(object sender, CancelEventArgs cancelEventArgs)
        {
            this._cancelClosing = true;
        }

        private void MenuItemClosed(object sender, ToolStripDropDownClosedEventArgs toolStripDropDownClosedEventArgs)
        {
            this._cancelClosing = false;
        }

        /// <summary>
        /// Handles the <see cref="ToolStripControlHost.LostFocus"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void MonthCalendarHostLostFocus(object sender, EventArgs e)
        {
            if (this._isDropped)
            {
                this._buttonState = ComboButtonState.None;
                this.dropDown.Close(ToolStripDropDownCloseReason.AppFocusChange);
            }

            this.FocusChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handles the <see cref="EnhancedMonthCalendar.DateSelected"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="DateRangeEventArgs"/> that contains the event data.</param>
        private void EnhancedMonthCalendarDateSelected(object sender, DateRangeEventArgs e)
        {
            this._buttonState = ComboButtonState.Normal;
            this.dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
            this.dateTextBox.Date = e.Start;
        }

        /// <summary>
        /// Handles the <see cref="EnhancedMonthCalendar.InternalDateSelected"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="DateEventArgs"/> that contains the event data.</param>
        private void EnhancedMonthCalendarInternalDateSelected(object sender, DateEventArgs e)
        {
            this.dateTextBox.Date = e.Date;
        }

        /// <summary>
        /// Handles the <see cref="EnhancedMonthCalendar.ActiveDateChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="ActiveDateChangedEventArgs"/> that contains the event data.</param>
        private void EnhancedMonthCalendarActiveDateChanged(object sender, ActiveDateChangedEventArgs e)
        {
            this.OnActiveDateChanged(e);
        }

        /// <summary>
        /// Handles the <see cref="EnhancedMonthCalendar.DateClicked"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="DateEventArgs"/> that contains the event data.</param>
        private void EnhancedMonthCalendarDateClicked(object sender, DateEventArgs e)
        {
            if (this.ClosePickerOnDayClick)
            {
                this._buttonState = ComboButtonState.Normal;
                this.dropDown.Close(ToolStripDropDownCloseReason.ItemClicked);
            }
        }

        /// <summary>
        /// Handles the <see cref="DatePickerDateTextBox.CheckDate"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="CheckDateEventArgs"/> that contains the event data.</param>
        private void DateTextBoxCheckDate(object sender, CheckDateEventArgs e)
        {
            this.enhancedMonthCalendar.SelectionRange = new SelectionRange(e.Date, e.Date);
            this.enhancedMonthCalendar.EnsureSeletedDateIsVisible();
            CheckDateEventArgs newArgs = new CheckDateEventArgs(e.Date, this.IsValidDate(e.Date));
            this.OnValueChanged(newArgs);
            e.IsValid = newArgs.IsValid;
        }

        /// <summary>
        /// Handles the <see cref="Control.KeyPress"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="KeyPressEventArgs"/> that contains the event data.</param>
        private void EnhancedMonthCalendarKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                this.SwitchPickerState();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the <see cref="Control.GotFocus"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        private void FocusChanged(object sender, EventArgs e)
        {
            if (this._isFocused != this.Focused)
            {
                this._isFocused = this.Focused;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Shows the toolstrip drop down.
        /// </summary>
        private void ShowDropDown()
        {
            if (this.dropDown != null)
            {
                this._isDropped = true;
                this.enhancedMonthCalendar.EnsureSeletedDateIsVisible();

                int borderWidth = SystemInformation.BorderSize.Width;
                int x = 0, y;

                if (this.RightToLeft == RightToLeft.Yes)
                    x = this.enhancedMonthCalendar.Size.Width + Math.Abs(this.enhancedMonthCalendar.Size.Width - this.Width);

                var screenRect = Screen.FromControl(this.enhancedMonthCalendar).WorkingArea;
                var screenLoc = this.PointToScreen(this.Location);
                var calBottom = screenLoc.Y + this.Height + this.enhancedMonthCalendar.Height;
                if (calBottom > screenRect.Height)
                    y = -this.enhancedMonthCalendar.Height - borderWidth;
                else
                    y = this.Height + borderWidth;

                this.dropDown.Show(this, x, y);
                this.enhancedMonthCalendar.Focus();
            }
        }

        /// <summary>
        /// Checks if the specified date is valid in the current context.
        /// </summary>
        /// <param name="date">The <see cref="DateTime"/> value to determine.</param>
        /// <returns>true if it is a valid date; otherwise false.</returns>
        private bool IsValidDate(DateTime date)
        {
            return date >= this.MinDate && date <= this.MaxDate;
        }

        /// <summary>
        /// Measures the height of the control.
        /// </summary>
        /// <returns>The height in pixel.</returns>
        private int MeasureControlSize()
        {
            using (Graphics g = this.CreateGraphics())
            {
                return this.MeasureControlSize(g);
            }
        }

        /// <summary>
        /// Measures the height of the control with the specified <paramref name="g"/> object.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object to measure with.</param>
        /// <returns>The height of the control in pixel.</returns>
        private int MeasureControlSize(Graphics g)
        {
            if (g == null)
                return 22;
            return Size.Round(g.MeasureString(DateTime.Today.ToShortDateString(), this.Font)).Height + 8;
        }

        private enum ComboButtonState { Normal = 0, Hot, Pressed, None }
    }
}