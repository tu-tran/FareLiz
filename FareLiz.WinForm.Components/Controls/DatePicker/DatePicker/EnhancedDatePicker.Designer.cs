namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    partial class EnhancedDatePicker
    {
        /// <summary>
        /// Disposes the control.
        /// </summary>
        /// <param name="disposing">true for releasing both managed and unmanaged resources, false for releasing only managed resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dropDown != null)
                {
                    this.dropDown.Closing -= this.DropDownClosing;
                    this.dropDown.GotFocus -= this.FocusChanged;
                    this.dropDown.LostFocus -= this.FocusChanged;
                    this.dropDown.Dispose();
                    this.dropDown = null;
                }

                this.enhancedMonthCalendar.DateSelected -= this.EnhancedMonthCalendarDateSelected;
                this.enhancedMonthCalendar.InternalDateSelected -= this.EnhancedMonthCalendarInternalDateSelected;
                this.enhancedMonthCalendar.ActiveDateChanged -= this.EnhancedMonthCalendarActiveDateChanged;
                this.enhancedMonthCalendar.DateClicked -= this.EnhancedMonthCalendarDateClicked;
                this.enhancedMonthCalendar.GotFocus -= this.FocusChanged;
                this.enhancedMonthCalendar.LostFocus -= this.FocusChanged;
                this.enhancedMonthCalendar.MonthMenu.ItemClicked -= this.MenuItemClicked;
                this.enhancedMonthCalendar.YearMenu.ItemClicked -= this.MenuItemClicked;
                this.monthCalendarHost.LostFocus -= this.MonthCalendarHostLostFocus;
                this.monthCalendarHost.GotFocus -= this.FocusChanged;

                this.enhancedMonthCalendar.Dispose();

                this.monthCalendarHost.Dispose();

                this.dateTextBox.CheckDate -= this.DateTextBoxCheckDate;
                this.dateTextBox.GotFocus -= this.FocusChanged;
                this.dateTextBox.LostFocus -= this.FocusChanged;

                this.dateTextBox.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.enhancedMonthCalendar = new EnhancedMonthCalendar();
            this.enhancedMonthCalendar.SelectionMode = MonthCalendarSelectionMode.Day;
            this.enhancedMonthCalendar.KeyPress += this.EnhancedMonthCalendarKeyPress;
            this.enhancedMonthCalendar.DateSelected += this.EnhancedMonthCalendarDateSelected;
            this.enhancedMonthCalendar.ActiveDateChanged += this.EnhancedMonthCalendarActiveDateChanged;
            this.enhancedMonthCalendar.DateClicked += this.EnhancedMonthCalendarDateClicked;
            this.enhancedMonthCalendar.InternalDateSelected += this.EnhancedMonthCalendarInternalDateSelected;
            this.enhancedMonthCalendar.GotFocus += this.FocusChanged;
            this.enhancedMonthCalendar.LostFocus += this.FocusChanged;
            this.enhancedMonthCalendar.MonthMenu.OwnerItem = this.monthCalendarHost;
            this.enhancedMonthCalendar.YearMenu.OwnerItem = this.monthCalendarHost;
            this.enhancedMonthCalendar.MonthMenu.Opening += this.MenuItemOpening;
            this.enhancedMonthCalendar.YearMenu.Opening += this.MenuItemOpening;
            this.enhancedMonthCalendar.MonthMenu.Closed += this.MenuItemClosed;
            this.enhancedMonthCalendar.YearMenu.Closed += this.MenuItemClosed;
            this.enhancedMonthCalendar.MonthMenu.ItemClicked += this.MenuItemClicked;
            this.enhancedMonthCalendar.YearMenu.ItemClicked += this.MenuItemClicked;

            this.monthCalendarHost = new ToolStripControlHost(this.enhancedMonthCalendar);
            this.monthCalendarHost.Margin = Padding.Empty;
            this.monthCalendarHost.Padding = Padding.Empty;
            this.monthCalendarHost.LostFocus += this.MonthCalendarHostLostFocus;
            this.monthCalendarHost.GotFocus += this.FocusChanged;

            this.dateTextBox = new DatePickerDateTextBox(this);
            this.dateTextBox.Date = DateTime.Today;
            this.dateTextBox.Location = new Point(2, 2);
            this.dateTextBox.Height = this.Height - 2;
            this.dateTextBox.MinDate = this.enhancedMonthCalendar.MinDate;
            this.dateTextBox.MaxDate = this.enhancedMonthCalendar.MaxDate;
            this.dateTextBox.InputBox.BorderStyle = BorderStyle.None;
            this.dateTextBox.CheckDate += this.DateTextBoxCheckDate;
            this.dateTextBox.GotFocus += this.FocusChanged;
            this.dateTextBox.LostFocus += this.FocusChanged;

            this.dropDown = new ToolStripDropDown();
            this.dropDown.DropShadowEnabled = true;
            this.dropDown.Closing += this.DropDownClosing;
            this.dropDown.GotFocus += this.FocusChanged;
            this.dropDown.LostFocus += this.FocusChanged;
            this.dropDown.Items.Add(this.monthCalendarHost);
            this.dropDown.Padding = Padding.Empty;
            this.dropDown.Margin = Padding.Empty;

            this.AllowPromptAsInput = false;
            this.BackColor = SystemColors.Window;
            this.Controls.Add(this.dateTextBox);
            this.ClosePickerOnDayClick = true;
            this.Font = System.Drawing.SystemFonts.DefaultFont;
        }

        private ToolStripControlHost monthCalendarHost;
        private EnhancedMonthCalendar enhancedMonthCalendar;
        private DatePickerDateTextBox dateTextBox;
        private ToolStripDropDown dropDown;
    }
}