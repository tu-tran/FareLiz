namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Design;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Helper;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Interfaces;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Renderer;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>A highly customizable and culture dependent month calendar control.</summary>
    [Designer(typeof(MonthCalendarControlDesigner))]
    [DefaultEvent("DateSelected")]
    [DefaultProperty("ViewStart")]
    [ToolboxBitmap(typeof(MonthCalendar))]
    public sealed partial class EnhancedMonthCalendar : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnhancedMonthCalendar"/> class.
        /// </summary>
        public EnhancedMonthCalendar()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, 
                true);

            this.InitializeComponent();

            this._eraRanges = GetEraRanges(this.cultureCalendar);
            this._formatProvider = new MonthCalendarFormatProvider(this.culture, null, this.culture.TextInfo.IsRightToLeft)
                                       {
                                           EnhancedMonthCalendar
                                               = this
                                       };
            this._minDate = this.cultureCalendar.MinSupportedDateTime.Date < new DateTime(1900, 1, 1)
                                ? new DateTime(1900, 1, 1)
                                : this.cultureCalendar.MinSupportedDateTime.Date;
            this._maxDate = this.cultureCalendar.MaxSupportedDateTime.Date > new DateTime(9998, 12, 31)
                                ? new DateTime(9998, 12, 31)
                                : this.cultureCalendar.MaxSupportedDateTime.Date;
            this._renderer = new MonthCalendarRenderer(this);

            this.UpdateYearMenu(DateTime.Today.Year); // update year menu
            this.SetStartDate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)); // set start date
            this.CalculateSize(this.SizeMode == CalendarSizeMode.CalendarSize);
        }

        #region Fields

        /// <summary>
        /// The _bold dates collection.
        /// </summary>
        private readonly BoldedDatesCollection _boldDatesCollection = new BoldedDatesCollection();

        /// <summary>
        /// The setredraw.
        /// </summary>
        private const int SETREDRAW = 11;

        /// <summary>
        /// The extend selection.
        /// </summary>
        private bool extendSelection;

        /// <summary>
        /// The selection started.
        /// </summary>
        private bool selectionStarted;

        /// <summary>
        /// The mouse location.
        /// </summary>
        private Point mouseLocation = Point.Empty;

        /// <summary>
        /// The current move bounds.
        /// </summary>
        private Rectangle currentMoveBounds = Rectangle.Empty;

        /// <summary>
        /// The _day width.
        /// </summary>
        private int _dayWidth;

        /// <summary>
        /// The _day height.
        /// </summary>
        private int _dayHeight;

        /// <summary>
        /// The _day name height.
        /// </summary>
        private int _dayNameHeight;

        /// <summary>
        /// The _month width.
        /// </summary>
        private int _monthWidth;

        /// <summary>
        /// The _month height.
        /// </summary>
        private int _monthHeight;

        /// <summary>
        /// The _week number width.
        /// </summary>
        private int _weekNumberWidth;

        /// <summary>
        /// The _header height.
        /// </summary>
        private int _headerHeight;

        /// <summary>
        /// The _footer height.
        /// </summary>
        private int _footerHeight;

        /// <summary>
        /// The _last visible date.
        /// </summary>
        private DateTime _lastVisibleDate;

        /// <summary>
        /// The _month selected.
        /// </summary>
        private DateTime _monthSelected;

        /// <summary>
        /// The _year selected.
        /// </summary>
        private DateTime _yearSelected;

        /// <summary>
        /// The _footer rect.
        /// </summary>
        private Rectangle _footerRect;

        /// <summary>
        /// The _left arrow rect.
        /// </summary>
        private Rectangle _leftArrowRect;

        /// <summary>
        /// The _right arrow rect.
        /// </summary>
        private Rectangle _rightArrowRect;

        /// <summary>
        /// The _renderer.
        /// </summary>
        private MonthCalendarRenderer _renderer;

        /// <summary>The selection start range if in week selection mode.</summary>
        private SelectionRange _selectionStartRange;

        /// <summary>The selection range for backup purposes.</summary>
        private SelectionRange _backupRange;

        /// <summary>Indicates that an menu is currently displayed.</summary>
        private bool _showingMenu;

        /// <summary>Indicates whether the control is calculating it's sizes.</summary>
        private bool _inUpdate;

        #endregion

        #region Properties

        /// <summary>The months displayed.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MonthCalendarMonth[] Months { get; private set; }

        /// <summary>
        /// The calendar size mode.
        /// </summary>
        public enum CalendarSizeMode
        {
            /// <summary>
            /// The calendar dimension.
            /// </summary>
            CalendarDimension, 

            /// <summary>
            /// The calendar size.
            /// </summary>
            CalendarSize
        }

        /// <summary>
        /// The size mode.
        /// </summary>
        private CalendarSizeMode sizeMode = CalendarSizeMode.CalendarDimension;

        /// <summary>
        /// Gets or sets the size mode.
        /// </summary>
        [Category("Appearance")]
        [Description("Sets how calendar is filled on screen.")]
        public CalendarSizeMode SizeMode
        {
            get
            {
                return this.sizeMode;
            }

            set
            {
                this.sizeMode = value;
                this.Refresh();
            }
        }

        /// <summary>Gets or sets the start month and year.</summary>
        [Category("Appearance")]
        [Description("Sets the first displayed month and year.")]
        public DateTime ViewStart
        {
            get
            {
                return this.viewStart;
            }

            set
            {
                if (value == this.viewStart || value < this._minDate || value > this._maxDate)
                {
                    return;
                }

                this.SetStartDate(value);
                this.UpdateMonths();
                this.Refresh();
            }
        }

        /// <summary>
        /// The view start.
        /// </summary>
        private DateTime viewStart;

        /// <summary>Gets the last in-month date of the last displayed month.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime ViewEnd
        {
            get
            {
                MonthCalendarDate dt =
                    new MonthCalendarDate(this.cultureCalendar, this.viewStart).GetEndDateOfWeek(this._formatProvider)
                        .FirstOfMonth.AddMonths(this.Months != null ? this.Months.Length - 1 : 1)
                        .FirstOfMonth;
                int daysInMonth = dt.DaysInMonth;
                dt = dt.AddDays(daysInMonth - 1);
                return dt.Date;
            }
        }

        /// <summary>Gets the real start date of the month calendar.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime RealStartDate
        {
            get
            {
                return this.realStart;
            }
        }

        /// <summary>
        /// The real start.
        /// </summary>
        private DateTime realStart;

        /// <summary>Gets or sets the lower limit of the visible month and year.</summary>
        [Category("Behavior")]
        [Description("The viewable minimum month and year.")]
        public DateTime MinDate
        {
            get
            {
                return this._minDate;
            }

            set
            {
                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime || value >= this._maxDate)
                {
                    return;
                }

                value = this.GetMinDate(value);
                this._minDate = value.Date;
                this.UpdateMonths();
                int dim1 = Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height);
                int dim2 = this.Months != null ? this.Months.Length : 1;
                if (dim1 != dim2)
                {
                    this.SetStartDate(new MonthCalendarDate(this.CultureCalendar, this.viewStart).AddMonths(dim2 - dim1).Date);
                }

                this.Invalidate();
            }
        }

        /// <summary>
        /// The _min date.
        /// </summary>
        private DateTime _minDate;

        /// <summary>Gets or sets the upper limit of the visible month and year.</summary>
        [Category("Behavior")]
        [Description("The viewable maximum month and year.")]
        public DateTime MaxDate
        {
            get
            {
                return this._maxDate;
            }

            set
            {
                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime || value <= this._minDate)
                {
                    return;
                }

                value = this.GetMaxDate(value);
                this._maxDate = value.Date;
                this.UpdateMonths();
                int dim1 = Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height);
                int dim2 = this.Months != null ? this.Months.Length : 1;
                if (dim1 != dim2)
                {
                    this.SetStartDate(new MonthCalendarDate(this.CultureCalendar, this.viewStart).AddMonths(dim2 - dim1).Date);
                }

                this.Invalidate();
            }
        }

        /// <summary>
        /// The _max date.
        /// </summary>
        private DateTime _maxDate;

        /// <summary>Gets or sets the calendar dimensions.</summary>
        [Category("Appearance")]
        [Description("The number of rows and columns of months in the calendar.")]
        [DefaultValue(typeof(Size), "3,1")]
        public Size CalendarDimensions
        {
            get
            {
                return this.calendarDimensions;
            }

            set
            {
                if (value == this.calendarDimensions || value.IsEmpty)
                {
                    return;
                }

                value.Width = Math.Max(1, Math.Min(value.Width, 7)); // get number of months in a row
                value.Height = Math.Max(1, Math.Min(value.Height, 7)); // get number of months in a column
                this.calendarDimensions = value; // set new dimension
                this._inUpdate = true; // update width and height of control
                this.Width = value.Width * this._monthWidth;
                this.Height = (value.Height * this._monthHeight) + (this.showFooter ? this._footerHeight : 0);
                this._inUpdate = false;
                this.scrollChange = Math.Max(0, Math.Min(this.scrollChange, this.calendarDimensions.Width * this.calendarDimensions.Height));

                    // adjust scroll change
                this.CalculateSize(false); // calculate sizes
            }
        }

        /// <summary>
        /// The calendar dimensions.
        /// </summary>
        private Size calendarDimensions = new Size(3, 1);

        /// <summary>Gets or sets the header font.</summary>
        [Category("Appearance")]
        [Description("The font for the header.")]
        public Font HeaderFont
        {
            get
            {
                return this.headerFont;
            }

            set
            {
                if (value == this.headerFont || value == null)
                {
                    return;
                }

                this.BeginUpdate();
                if (this.headerFont != null)
                {
                    this.headerFont.Dispose();
                }

                this.headerFont = value;
                this.CalculateSize(false);
                this.EndUpdate();
            }
        }

        /// <summary>
        /// The header font.
        /// </summary>
        private Font headerFont = new Font(SystemFonts.CaptionFont, FontStyle.Bold);

        /// <summary>Gets or sets the footer font.</summary>
        [Category("Appearance")]
        [Description("The font for the footer.")]
        public Font FooterFont
        {
            get
            {
                return this.footerFont;
            }

            set
            {
                if (value == this.footerFont || value == null)
                {
                    return;
                }

                this.BeginUpdate();
                if (this.footerFont != null)
                {
                    this.footerFont.Dispose();
                }

                this.footerFont = value;
                this.CalculateSize(false);
                this.EndUpdate();
            }
        }

        /// <summary>
        /// The footer font.
        /// </summary>
        private Font footerFont = new Font(SystemFonts.DefaultFont, FontStyle.Bold);

        /// <summary>Gets or sets the font for the day header.</summary>
        [Category("Appearance")]
        [Description("The font for the day header.")]
        public Font DayHeaderFont
        {
            get
            {
                return this.dayHeaderFont;
            }

            set
            {
                if (value == this.dayHeaderFont || value == null)
                {
                    return;
                }

                this.BeginUpdate();
                if (this.dayHeaderFont != null)
                {
                    this.dayHeaderFont.Dispose();
                }

                this.dayHeaderFont = value;
                this.CalculateSize(false);
                this.EndUpdate();
            }
        }

        /// <summary>
        /// The day header font.
        /// </summary>
        private Font dayHeaderFont = SystemFonts.DefaultFont;

        /// <summary>Gets or sets the font used for the week header and days.</summary>
        public override Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                this.BeginUpdate();
                base.Font = value;
                this.CalculateSize(false);
                this.EndUpdate();
            }
        }

        /// <summary>Gets or sets the size of the control.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size Size
        {
            get
            {
                return base.Size;
            }

            set
            {
                base.Size = value;
            }
        }

        /// <summary>Gets or sets the text alignment for the days.</summary>
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        [Description("Determines the text alignment for the days.")]
        [Category("Appearance")]
        public ContentAlignment DayTextAlignment
        {
            get
            {
                return this.dayTextAlign;
            }

            set
            {
                if (value == this.dayTextAlign)
                {
                    return;
                }

                this.dayTextAlign = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The day text align.
        /// </summary>
        private ContentAlignment dayTextAlign = ContentAlignment.MiddleCenter;

        /// <summary>Gets or sets a value indicating whether to use a right to left layout.</summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the RTL layout.")]
        public bool RightToLeftLayout
        {
            get
            {
                return this.rightToLeftLayout;
            }

            set
            {
                if (value == this.rightToLeftLayout)
                {
                    return;
                }

                this.rightToLeftLayout = value;
                this._formatProvider.IsRTLLanguage = this.UseRTL;
                Size calDim = this.calendarDimensions;
                this.UpdateMonths();
                this.CalendarDimensions = calDim;
                this.Refresh();
            }
        }

        /// <summary>
        /// The right to left layout.
        /// </summary>
        private bool rightToLeftLayout;

        /// <summary>Gets or sets a value indicating whether to show the footer.</summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether to show the footer.")]
        public bool ShowFooter
        {
            get
            {
                return this.showFooter;
            }

            set
            {
                if (value == this.showFooter)
                {
                    return;
                }

                this.showFooter = value;
                this.Height += value ? this._footerHeight : -this._footerHeight;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The show footer.
        /// </summary>
        private bool showFooter = true;

        /// <summary>Gets or sets a value indicating whether to show the week header.</summary>
        [DefaultValue(true)]
        [Category("Appearance")]
        [Description("Indicates whether to show the week header.")]
        public bool ShowWeekHeader
        {
            get
            {
                return this.showWeekHeader;
            }

            set
            {
                if (this.showWeekHeader == value)
                {
                    return;
                }

                this.showWeekHeader = value;
                this.CalculateSize(false);
            }
        }

        /// <summary>
        /// The show week header.
        /// </summary>
        private bool showWeekHeader = true;

        /// <summary>Gets or sets a value indicating whether to use the shortest or the abbreviated day names in the day header.</summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the shortest or the abbreviated day names in the day header.")]
        public bool UseShortestDayNames
        {
            get
            {
                return this.useShortestDayNames;
            }

            set
            {
                this.useShortestDayNames = value;
                this.CalculateSize(false);
            }
        }

        /// <summary>
        /// The use shortest day names.
        /// </summary>
        private bool useShortestDayNames;

        /// <summary>
        /// Gets or sets a value indicating whether to use the native digits in <see cref="NumberFormatInfo.NativeDigits" />
        /// specified by <see cref="EnhancedMonthCalendar.Culture" />s <see cref="CultureInfo.NumberFormat" />
        /// for number display.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        [Description("Indicates whether to use the native digits as specified by the current Culture property.")]
        public bool UseNativeDigits
        {
            get
            {
                return this.useNativeDigits;
            }

            set
            {
                if (value == this.useNativeDigits)
                {
                    return;
                }

                this.useNativeDigits = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// The use native digits.
        /// </summary>
        private bool useNativeDigits;

        /// <summary>Gets or sets the list for bolded dates.</summary>
        [Description("The bolded dates in the month calendar.")]
        public List<DateTime> BoldedDates
        {
            get
            {
                return this.boldedDates;
            }

            set
            {
                this.boldedDates = value ?? new List<DateTime>();
            }
        }

        /// <summary>
        /// The bolded dates.
        /// </summary>
        private List<DateTime> boldedDates = new List<DateTime>();

        /// <summary>Gets the bolded dates.</summary>
        [Description("The bolded dates in the calendar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDatesCollection BoldedDatesCollection
        {
            get
            {
                return this._boldDatesCollection;
            }
        }

        /// <summary>Gets a collection holding the defined categories of bold dates.</summary>
        [Description("The bold date categories in the calendar.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BoldedDateCategoryCollection BoldedDateCategoryCollection { get; private set; }

        /// <summary>Gets or sets the selection start date.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectionStart
        {
            get
            {
                return this.selectionStart;
            }

            set
            {
                value = value.Date;

                // valid value ?
                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime)
                {
                    return;
                }

                if (value < this._minDate)
                {
                    value = this._minDate;
                }
                else if (value > this._maxDate)
                {
                    value = this._maxDate;
                }

                switch (this.daySelectionMode)
                {
                    case MonthCalendarSelectionMode.Day:
                        this.selectionStart = value;
                        this.selectionEnd = value;

                        break;

                    case MonthCalendarSelectionMode.WorkWeek:
                    case MonthCalendarSelectionMode.FullWeek:
                        MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, value).GetFirstDayInWeek(this._formatProvider);
                        this.selectionStart = dt.Date;
                        this.selectionEnd = dt.GetEndDateOfWeek(this._formatProvider).Date;

                        break;

                    case MonthCalendarSelectionMode.Month:
                        dt = new MonthCalendarDate(this.CultureCalendar, value).FirstOfMonth;
                        this.selectionStart = dt.Date;
                        this.selectionEnd = dt.AddMonths(1).AddDays(-1).Date;

                        break;

                    case MonthCalendarSelectionMode.Manual:
                        value = this.GetSelectionDate(this.selectionEnd, value);

                        if (value == DateTime.MinValue)
                        {
                            this.selectionEnd = value;
                            this.selectionStart = value;
                        }
                        else
                        {
                            this.selectionStart = value;
                            if (this.selectionEnd == DateTime.MinValue)
                            {
                                this.selectionEnd = value;
                            }
                        }

                        break;
                }

                this.Refresh();
            }
        }

        /// <summary>
        /// The selection start.
        /// </summary>
        private DateTime selectionStart = DateTime.Today;

        /// <summary>Gets or sets the selection end date.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectionEnd
        {
            get
            {
                return this.selectionEnd;
            }

            set
            {
                value = value.Date;

                if (value < this.CultureCalendar.MinSupportedDateTime || value > this.CultureCalendar.MaxSupportedDateTime
                    || this.daySelectionMode != MonthCalendarSelectionMode.Manual)
                {
                    return;
                }

                if (value < this._minDate)
                {
                    value = this._minDate;
                }
                else if (value > this._maxDate)
                {
                    value = this._maxDate;
                }

                value = this.GetSelectionDate(this.selectionStart, value);

                if (value == DateTime.MinValue || this.selectionStart == DateTime.MinValue)
                {
                    this.selectionStart = value;
                    this.selectionEnd = value;
                    this.Refresh();
                    return;
                }

                this.selectionEnd = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// The selection end.
        /// </summary>
        private DateTime selectionEnd = DateTime.Today;

        /// <summary>Gets or sets the selection range of the selected dates.</summary>
        [Category("Behavior")]
        [Description("The selection range.")]
        public SelectionRange SelectionRange
        {
            get
            {
                return new SelectionRange(this.selectionStart, this.selectionEnd);
            }

            set
            {
                if (value == null)
                {
                    this.selectionEnd = DateTime.Today;
                    this.selectionStart = DateTime.Today;

                    this.Refresh();
                    return;
                }

                switch (this.daySelectionMode)
                {
                    case MonthCalendarSelectionMode.Day:
                    case MonthCalendarSelectionMode.WorkWeek:
                    case MonthCalendarSelectionMode.FullWeek:
                    case MonthCalendarSelectionMode.Month:
                        this.SelectionStart = this.selectionStart == value.Start ? value.End : value.Start;

                        break;

                    case MonthCalendarSelectionMode.Manual:
                        this.selectionStart = value.Start;
                        this.SelectionEnd = value.End;

                        break;
                }
            }
        }

        /// <summary>Gets the selection ranges.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<SelectionRange> SelectionRanges
        {
            get
            {
                return this.selectionRanges;
            }
        }

        /// <summary>
        /// The selection ranges.
        /// </summary>
        private readonly List<SelectionRange> selectionRanges = new List<SelectionRange>();

        /// <summary>Gets or sets the scroll change if clicked on an arrow button.</summary>
        [DefaultValue(0)]
        [Category("Behavior")]
        [Description(
            "The number of months the calendar is going for- or backwards if clicked on an arrow."
            + "A value of 0 indicates the last visible month is the first month (forwards) and vice versa.")]
        public int ScrollChange
        {
            get
            {
                return this.scrollChange;
            }

            set
            {
                if (value == this.scrollChange)
                {
                    return;
                }

                this.scrollChange = value;
            }
        }

        /// <summary>
        /// The scroll change.
        /// </summary>
        private int scrollChange;

        /// <summary>Gets or sets the maximum selectable days.</summary>
        [DefaultValue(0)]
        [Category("Behavior")]
        [Description("The maximum selectable days. A value of 0 means no limit.")]
        public int MaxSelectionCount
        {
            get
            {
                return this.maxSelectionCount;
            }

            set
            {
                if (value == this.maxSelectionCount)
                {
                    return;
                }

                this.maxSelectionCount = Math.Max(0, value);
            }
        }

        /// <summary>
        /// The max selection count.
        /// </summary>
        private int maxSelectionCount;

        /// <summary>Gets or sets the day selection mode.</summary>
        [DefaultValue(MonthCalendarSelectionMode.Manual)]
        [Category("Behavior")]
        [Description("Sets the day selection mode.")]
        public MonthCalendarSelectionMode SelectionMode
        {
            get
            {
                return this.daySelectionMode;
            }

            set
            {
                if (value == this.daySelectionMode || !Enum.IsDefined(typeof(MonthCalendarSelectionMode), value))
                {
                    return;
                }

                this.daySelectionMode = value;
                this.SetSelectionRange(value);
                this.Refresh();
            }
        }

        /// <summary>
        /// The day selection mode.
        /// </summary>
        private MonthCalendarSelectionMode daySelectionMode = MonthCalendarSelectionMode.Manual;

        /// <summary>Gets or sets the non working days.</summary>
        [DefaultValue(CalendarDayOfWeek.Saturday | CalendarDayOfWeek.Sunday)]
        [Category("Behavior")]
        [Description("Sets the non working days.")]
        [Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
        public CalendarDayOfWeek NonWorkDays
        {
            get
            {
                return this.nonWorkDays;
            }

            set
            {
                if (value == this.nonWorkDays)
                {
                    return;
                }

                this.nonWorkDays = value;
                if (this.daySelectionMode == MonthCalendarSelectionMode.WorkWeek)
                {
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// The non work days.
        /// </summary>
        private CalendarDayOfWeek nonWorkDays = CalendarDayOfWeek.Saturday | CalendarDayOfWeek.Sunday;

        /// <summary>Gets or sets the used _renderer.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MonthCalendarRenderer Renderer
        {
            get
            {
                return this._renderer;
            }

            set
            {
                if (value == null)
                {
                    return;
                }

                this._renderer = value;
                this.Refresh();
            }
        }

        /// <summary>Gets or sets the culture used by the <see cref="EnhancedMonthCalendar" />.</summary>
        [Category("Behavior")]
        [Description("The culture used by the Calendar.")]
        [TypeConverter(typeof(CultureInfoCustomTypeConverter))]
        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }

            set
            {
                if (value == null || value.IsNeutralCulture)
                {
                    return;
                }

                this.culture = value;
                this._formatProvider.DateTimeFormat = value.DateTimeFormat;
                this.CultureCalendar = null;
                if (DateMethods.IsRTLCulture(value))
                {
                    this.RightToLeft = RightToLeft.Yes;
                    this.RightToLeftLayout = true;
                }
                else
                {
                    this.RightToLeftLayout = false;
                    this.RightToLeft = RightToLeft.Inherit;
                }

                this._formatProvider.IsRTLLanguage = this.UseRTL;
            }
        }

        /// <summary>
        /// The culture.
        /// </summary>
        private CultureInfo culture = CultureInfo.CurrentCulture;

        /// <summary>Gets or sets the used calendar.</summary>
        [Category("Behavior")]
        [Description("The calendar used by the Calendar.")]
        [Editor(typeof(MonthCalendarCalendarUIEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(MonthCalendarCalendarTypeConverter))]
        public Calendar CultureCalendar
        {
            get
            {
                return this.cultureCalendar;
            }

            set
            {
                if (value == null)
                {
                    value = this.culture.Calendar;
                }

                this.cultureCalendar = value;
                this._formatProvider.Calendar = value;

                if (value.GetType() == typeof(PersianCalendar) && !value.IsReadOnly)
                {
                    value.TwoDigitYearMax = 1410;
                }

                foreach (Calendar c in this.culture.OptionalCalendars)
                {
                    if (value.GetType() == c.GetType())
                    {
                        if (value.GetType() == typeof(GregorianCalendar))
                        {
                            GregorianCalendar g1 = (GregorianCalendar)value;
                            GregorianCalendar g2 = (GregorianCalendar)c;

                            if (g1.CalendarType != g2.CalendarType)
                            {
                                continue;
                            }
                        }

                        this.culture.DateTimeFormat.Calendar = c;
                        this._formatProvider.DateTimeFormat = this.culture.DateTimeFormat;
                        this.cultureCalendar = c;

                        value = c;

                        break;
                    }
                }

                this._eraRanges = GetEraRanges(value);
                this.ReAssignSelectionMode();
                this._minDate = this.GetMinDate(value.MinSupportedDateTime.Date);
                this._maxDate = this.GetMaxDate(value.MaxSupportedDateTime.Date);
                this.SetStartDate(DateTime.Today);
                this.CalculateSize(false);
            }
        }

        /// <summary>
        /// The culture calendar.
        /// </summary>
        private Calendar cultureCalendar = CultureInfo.CurrentUICulture.DateTimeFormat.Calendar;

        /// <summary>Gets or sets the interface for day name handling.</summary>
        [TypeConverter(typeof(MonthCalendarNamesProviderTypeConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category("Behavior")]
        [Description("Culture dependent settings for month/day names and date formatting.")]
        public ICustomFormatProvider FormatProvider
        {
            get
            {
                if (this._formatProvider == null)
                {
                    this._formatProvider = new MonthCalendarFormatProvider(this.culture, null, this.culture.TextInfo.IsRightToLeft);
                }

                return this._formatProvider;
            }

            set
            {
                this._formatProvider = value;
            }
        }

        /// <summary>
        /// The _format provider.
        /// </summary>
        private ICustomFormatProvider _formatProvider;

        /// <summary>Gets the size of a single <see cref="MonthCalendarMonth" />.</summary>
        internal Size MonthSize
        {
            get
            {
                return new Size(this._monthWidth, this._monthHeight);
            }
        }

        /// <summary>Gets the size of a single day.</summary>
        internal Size DaySize
        {
            get
            {
                return new Size(this._dayWidth, this._dayHeight);
            }
        }

        /// <summary>Gets the footer size.</summary>
        internal Size FooterSize
        {
            get
            {
                return new Size(this.Width, this._footerHeight);
            }
        }

        /// <summary>Gets the header size.</summary>
        internal Size HeaderSize
        {
            get
            {
                return new Size(this._monthWidth, this._headerHeight);
            }
        }

        /// <summary>Gets the size of the day names.</summary>
        internal Size DayNamesSize
        {
            get
            {
                return new Size(this._dayWidth * 7, this._dayNameHeight);
            }
        }

        /// <summary>Gets the size of the week numbers.</summary>
        internal Size WeekNumberSize
        {
            get
            {
                return new Size(this._weekNumberWidth, this._dayHeight * 7);
            }
        }

        /// <summary>Gets the date for the current day the mouse is over.</summary>
        internal DateTime MouseOverDay
        {
            get
            {
                return this.mouseMoveFlags.Day;
            }
        }

        /// <summary>
        /// The mouse move flags.
        /// </summary>
        private MonthCalendarMouseMoveFlags mouseMoveFlags = new MonthCalendarMouseMoveFlags();

        /// <summary>Gets a value indicating whether the control is using the RTL layout.</summary>
        internal bool UseRTL
        {
            get
            {
                return this.RightToLeft == RightToLeft.Yes && this.rightToLeftLayout;
            }
        }

        /// <summary>Gets the current left button RequestState.</summary>
        internal ButtonState LeftButtonState
        {
            get
            {
                return this.mouseMoveFlags.LeftArrow ? ButtonState.Pushed : ButtonState.Normal;
            }
        }

        /// <summary>Gets the current right button RequestState.</summary>
        internal ButtonState RightButtonState
        {
            get
            {
                return this.mouseMoveFlags.RightArrow ? ButtonState.Pushed : ButtonState.Normal;
            }
        }

        /// <summary>Gets the current hit type result.</summary>
        internal MonthCalendarHitType CurrentHitType
        {
            get
            {
                return this.currentHitType;
            }
        }

        /// <summary>
        /// The current hit type.
        /// </summary>
        private MonthCalendarHitType currentHitType = MonthCalendarHitType.None;

        /// <summary>Gets the month menu.</summary>
        internal ContextMenuStrip MonthMenu
        {
            get
            {
                return this.monthMenu;
            }
        }

        /// <summary>Gets the year menu.</summary>
        internal ContextMenuStrip YearMenu
        {
            get
            {
                return this.yearMenu;
            }
        }

        /// <summary>Gets the era date ranges for the current calendar.</summary>
        internal MonthCalendarEraRange[] EraRanges
        {
            get
            {
                return this._eraRanges;
            }
        }

        /// <summary>
        /// The _era ranges.
        /// </summary>
        private MonthCalendarEraRange[] _eraRanges;

        #endregion

        #region methods

        /// <summary>Prevents a drawing of the control until <see cref="EndUpdate" /> is called.</summary>
        public void BeginUpdate()
        {
            NativeMethods.SendMessage(this.Handle, SETREDRAW, false, 0);
        }

        /// <summary>Ends the updating process and the control can draw itself again.</summary>
        public void EndUpdate()
        {
            NativeMethods.SendMessage(this.Handle, SETREDRAW, true, 0);
            this.Refresh();
        }

        /// <summary>
        /// Performs a hit test with the specified <paramref name="location"/> as mouse location.
        /// </summary>
        /// <param name="location">
        /// The mouse location.
        /// </param>
        /// <returns>
        /// A <see cref="MonthCalendarHitTest"/> object.
        /// </returns>
        public MonthCalendarHitTest HitTest(Point location)
        {
            if (!this.ClientRectangle.Contains(location))
            {
                return MonthCalendarHitTest.Empty;
            }

            if (this._rightArrowRect.Contains(location))
            {
                return new MonthCalendarHitTest(this.GetNewScrollDate(false), MonthCalendarHitType.Arrow, this._rightArrowRect);
            }

            if (this._leftArrowRect.Contains(location))
            {
                return new MonthCalendarHitTest(this.GetNewScrollDate(true), MonthCalendarHitType.Arrow, this._leftArrowRect);
            }

            if (this.showFooter && this._footerRect.Contains(location))
            {
                return new MonthCalendarHitTest(DateTime.Today, MonthCalendarHitType.Footer, this._footerRect);
            }

            foreach (MonthCalendarMonth month in this.Months)
            {
                MonthCalendarHitTest hit = month.HitTest(location);

                if (!hit.IsEmpty)
                {
                    return hit;
                }
            }

            return MonthCalendarHitTest.Empty;
        }

        /// <summary>Gets all bolded dates.</summary>
        /// <returns>A generic List of type <see cref="DateTime" /> with the bolded dates.</returns>
        internal List<DateTime> GetBoldedDates()
        {
            List<DateTime> dates = new List<DateTime>();

            // remove all duplicate dates
            this.boldedDates.ForEach(
                date =>
                    {
                        if (!dates.Contains(date))
                        {
                            dates.Add(date);
                        }
                    });

            return dates;
        }

        /// <summary>
        /// Determines if the <paramref name="date"/> is selected.
        /// </summary>
        /// <param name="date">
        /// The date to determine if checked.
        /// </param>
        /// <returns>
        /// true if <paramref name="date"/> is selected; false otherwise.
        /// </returns>
        internal bool IsSelected(DateTime date)
        {
            // get if date is in first selection range
            bool selected = this.SelectionRange.Contains(date);

            // get if date is in all other selection ranges (only in WorkWeek day selection mode)
            this.selectionRanges.ForEach(range => { selected |= range.Contains(date); });

            // if in WorkWeek day selection mode a date is only selected if a work day
            if (this.daySelectionMode == MonthCalendarSelectionMode.WorkWeek)
            {
                // get all work days
                List<DayOfWeek> workDays = DateMethods.GetWorkDays(this.nonWorkDays);

                // return if date is selected
                return workDays.Contains(date.DayOfWeek) && selected;
            }

            // return if date is selected
            return selected;
        }

        /// <summary>Scrolls to the selection start.</summary>
        internal void EnsureSeletedDateIsVisible()
        {
            if (this.RealStartDate < this.MinDate || this.RealStartDate > this.selectionStart || this.selectionStart > this._lastVisibleDate)
            {
                this.SetStartDate(new DateTime(this.selectionStart.Year, this.selectionStart.Month, 1));
                this.UpdateMonths();
            }
        }

        /// <summary>
        /// Sets the bounds of the left arrow.
        /// </summary>
        /// <param name="rect">
        /// The bounds of the left arrow.
        /// </param>
        internal void SetLeftArrowRect(Rectangle rect)
        {
            // if in RTL mode
            if (this.UseRTL)
            {
                // the left arrow bounds are the right ones
                this._rightArrowRect = rect;
            }
            else
            {
                this._leftArrowRect = rect;
            }
        }

        /// <summary>
        /// Sets the bounds of the right arrow.
        /// </summary>
        /// <param name="rect">
        /// The bounds of the right arrow.
        /// </param>
        internal void SetRightArrowRect(Rectangle rect)
        {
            // if in RTL mode
            if (this.UseRTL)
            {
                // the right arrow bounds are the left ones
                this._leftArrowRect = rect;
            }
            else
            {
                this._rightArrowRect = rect;
            }
        }

        /// <summary>
        /// Processes a dialog key.
        /// </summary>
        /// <param name="keyData">
        /// One of the <see cref="Keys"/> value that represents the key to process.
        /// </param>
        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.daySelectionMode != MonthCalendarSelectionMode.Day)
            {
                return base.ProcessDialogKey(keyData);
            }

            MonthCalendarDate dt = new MonthCalendarDate(this.cultureCalendar, this.selectionStart);
            bool retValue = false;

            if (keyData == Keys.Left)
            {
                this.selectionStart = dt.AddDays(-1).Date;
                retValue = true;
            }
            else if (keyData == Keys.Right)
            {
                this.selectionStart = dt.AddDays(1).Date;
                retValue = true;
            }
            else if (keyData == Keys.Up)
            {
                this.selectionStart = dt.AddDays(-7).Date;
                retValue = true;
            }
            else if (keyData == Keys.Down)
            {
                this.selectionStart = dt.AddDays(7).Date;
                retValue = true;
            }

            if (retValue)
            {
                if (this.selectionStart < this._minDate)
                {
                    this.selectionStart = this._minDate;
                }
                else if (this.selectionStart > this._maxDate)
                {
                    this.selectionStart = this._maxDate;
                }

                this.SetSelectionRange(this.daySelectionMode);
                this.EnsureSeletedDateIsVisible();
                this.RaiseInternalDateSelected();
                this.Invalidate();
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Performs the work of setting the specified bounds of this control.
        /// </summary>
        /// <param name="x">
        /// The new <see cref="System.Windows.Forms.Control.Left"/> property value of the control.
        /// </param>
        /// <param name="y">
        /// The new <see cref="System.Windows.Forms.Control.Top"/> property value of the control.
        /// </param>
        /// <param name="width">
        /// The new <see cref="System.Windows.Forms.Control.Width"/> property value of the control.
        /// </param>
        /// <param name="height">
        /// The new <see cref="System.Windows.Forms.Control.Height"/> property value of the control.
        /// </param>
        /// <param name="specified">
        /// A bitwise combination of the <see cref="System.Windows.Forms.BoundsSpecified"/> values.
        /// </param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
            if (this.Created || this.DesignMode)
            {
                this.CalculateSize(this.SizeMode == CalendarSizeMode.CalendarSize);
            }
        }

        /// <summary>
        /// Builds an array of <see cref="MonthCalendarEraRange"/> to store the min and max date of the eras of the specified
        /// <see cref="System.Globalization.Calendar"/>.
        /// </summary>
        /// <param name="cal">
        /// The <see cref="System.Globalization.Calendar"/> to retrieve the era ranges for.
        /// </param>
        /// <returns>
        /// An array of type <see cref="MonthCalendarEraRange"/>.
        /// </returns>
        private static MonthCalendarEraRange[] GetEraRanges(Calendar cal)
        {
            if (cal.Eras.Length == 1)
            {
                return new[] { new MonthCalendarEraRange(cal.Eras[0], cal.MinSupportedDateTime.Date, cal.MaxSupportedDateTime.Date) };
            }

            List<MonthCalendarEraRange> ranges = new List<MonthCalendarEraRange>();
            DateTime date = cal.MinSupportedDateTime.Date;
            int currentEra = -1;
            while (date < cal.MaxSupportedDateTime.Date)
            {
                int era = cal.GetEra(date);

                if (era != currentEra)
                {
                    ranges.Add(new MonthCalendarEraRange(era, date));
                    if (currentEra != -1)
                    {
                        ranges[ranges.Count - 2].MaxDate = cal.AddDays(date, -1);
                    }

                    currentEra = era;
                }

                date = cal.AddDays(date, 1);
            }

            ranges[ranges.Count - 1].MaxDate = date;
            return ranges.ToArray();
        }

        /// <summary>
        /// Gets the era range for the specified era.
        /// </summary>
        /// <param name="era">
        /// The era to get the date range for.
        /// </param>
        /// <returns>
        /// A <see cref="MonthCalendarEraRange"/> object.
        /// </returns>
        private MonthCalendarEraRange GetEraRange(int era)
        {
            foreach (MonthCalendarEraRange e in this._eraRanges)
            {
                if (e.Era == era)
                {
                    return e;
                }
            }

            return new MonthCalendarEraRange(
                this.CultureCalendar.GetEra(DateTime.Today), 
                this.CultureCalendar.MinSupportedDateTime.Date, 
                this.CultureCalendar.MaxSupportedDateTime.Date);
        }

        /// <summary>Gets the era range for the era the current date is in.</summary>
        /// <returns>A <see cref="MonthCalendarEraRange" />.</returns>
        private MonthCalendarEraRange GetEraRange()
        {
            return this.GetEraRange(this.CultureCalendar.GetEra(DateTime.Today));
        }

        /// <summary>
        /// Calculates the various sizes of a single month view and the global size of the control.
        /// </summary>
        /// <param name="changeDimension">
        /// The change Dimension.
        /// </param>
        private void CalculateSize(bool changeDimension)
        {
            // if already calculating - return
            if (this._inUpdate)
            {
                return;
            }

            this._inUpdate = true;

            using (Graphics g = this.CreateGraphics())
            {
                // get sizes for different elements of the calendar
                SizeF daySize = g.MeasureString("30", this.Font);
                SizeF weekNumSize = g.MeasureString("59", this.Font);

                MonthCalendarDate date = new MonthCalendarDate(this.CultureCalendar, this.viewStart);

                SizeF monthNameSize = g.MeasureString(this._formatProvider.GetMonthName(date.Year, date.Month), this.headerFont);
                SizeF yearStringSize = g.MeasureString(this.viewStart.ToString("yyyy"), this.headerFont);
                SizeF footerStringSize = g.MeasureString(this.viewStart.ToShortDateString(), this.footerFont);

                // calculate the header height
                this._headerHeight = Math.Max((int)Math.Max(monthNameSize.Height + 3, yearStringSize.Height) + 1, 15);

                // calculate the width of a single day
                this._dayWidth = Math.Max(12, (int)daySize.Width + 1) + 5;

                // calculate the height of a single day
                this._dayHeight = Math.Max(Math.Max(12, (int)weekNumSize.Height + 1), (int)daySize.Height + 1) + 2;

                // calculate the height of the footer
                this._footerHeight = Math.Max(20, (int)footerStringSize.Height + 2);

                // calculate the width of the week number header
                this._weekNumberWidth = this.showWeekHeader ? Math.Max(12, (int)weekNumSize.Width + 1) + 2 : 0;

                // set minimal height of the day name header
                this._dayNameHeight = this.dayHeaderFont.Height + 10;

                // loop through all day names
                foreach (string str in DateMethods.GetDayNames(this._formatProvider, this.useShortestDayNames ? 2 : 1))
                {
                    // get the size of the name
                    SizeF dayNameSize = g.MeasureString(str, this.dayHeaderFont);

                    // adjust the width of the day and the day name header height
                    this._dayWidth = Math.Max(this._dayWidth, (int)dayNameSize.Width + 1);
                    this._dayNameHeight = Math.Max(this._dayNameHeight, (int)dayNameSize.Height + 1);
                }

                // calculate the width and height of a MonthCalendarMonth element
                this._monthWidth = this._weekNumberWidth + (this._dayWidth * 7) + 2;
                this._monthHeight = this._headerHeight + this._dayNameHeight + (this._dayHeight * 6) + 2;

                if (changeDimension)
                {
                    // calculate the dimension of the control
                    int calWidthDim = Math.Max(1, this.Width / this._monthWidth);
                    int calHeightDim = Math.Max(1, this.Height / this._monthHeight);

                    // set the dimensions
                    this.CalendarDimensions = new Size(calWidthDim, calHeightDim);
                }

                // set the width and height of the control
                this.Height = (this._monthHeight * this.calendarDimensions.Height) + (this.showFooter ? this._footerHeight : 0) + this.Padding.Top
                              + this.Padding.Bottom;
                this.Width = this._monthWidth * this.calendarDimensions.Width + this.Padding.Top + this.Padding.Bottom;

                // calculate the footer bounds
                this._footerRect = new Rectangle(1, this.Height - this._footerHeight - 1, this.Width - 2, this._footerHeight);

                // update the months
                this.UpdateMonths();
            }

            this._inUpdate = false;
            this.Refresh();
        }

        /// <summary>
        /// Sets the start date.
        /// </summary>
        /// <param name="start">
        /// The start date.
        /// </param>
        /// <returns>
        /// true if <paramref name="start"/> is valid; false otherwise.
        /// </returns>
        private bool SetStartDate(DateTime start)
        {
            if (start < DateTime.MinValue.Date || start > DateTime.MaxValue.Date)
            {
                return false;
            }

            DayOfWeek firstDayOfWeek = this._formatProvider.FirstDayOfWeek;
            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this._maxDate);

            if (start > this._maxDate)
            {
                start = dt.AddMonths(1 - this.Months.Length).FirstOfMonth.Date;
            }

            if (start < this._minDate)
            {
                start = this._minDate;
            }

            dt = new MonthCalendarDate(this.CultureCalendar, start);
            int length = this.Months != null ? this.Months.Length - 1 : 0;

            while (dt.Date > this._minDate && dt.Day != 1)
            {
                dt = dt.AddDays(-1);
            }

            MonthCalendarDate endDate = dt.AddMonths(length);
            MonthCalendarDate endDateDay = endDate.AddDays(endDate.DaysInMonth - 1 - (endDate.Day - 1));

            if (endDate.Date >= this._maxDate || endDateDay.Date >= this._maxDate)
            {
                dt = new MonthCalendarDate(this.CultureCalendar, this._maxDate).AddMonths(-length).FirstOfMonth;
            }

            this.viewStart = dt.Date;

            while (dt.Date > this.CultureCalendar.MinSupportedDateTime.Date && dt.DayOfWeek != firstDayOfWeek)
            {
                dt = dt.AddDays(-1);
            }

            this.realStart = dt.Date;
            return true;
        }

        /// <summary>
        /// Gets the index of the <see cref="MonthCalendarMonth"/> in the array of the specified monthYear datetime.
        /// </summary>
        /// <param name="monthYear">
        /// The date to search for.
        /// </param>
        /// <returns>
        /// The index in the array.
        /// </returns>
        private int GetIndex(DateTime monthYear)
        {
            return (from month in this.Months where month != null where month.Date == monthYear select month.Index).FirstOrDefault();
        }

        /// <summary>
        /// Gets the <see cref="MonthCalendarMonth"/> which contains the specified date.
        /// </summary>
        /// <param name="day">
        /// The day to search for.
        /// </param>
        /// <returns>
        /// An <see cref="MonthCalendarMonth"/> if day is valid; otherwise null.
        /// </returns>
        private MonthCalendarMonth GetMonth(DateTime day)
        {
            if (day == DateTime.MinValue)
            {
                return null;
            }

            return this.Months.Where(month => month != null).FirstOrDefault(month => month.ContainsDate(day));
        }

        /// <summary>Updates the shown months.</summary>
        public void UpdateMonths()
        {
            int x = this.Padding.Left, y = this.Padding.Top, index = 0;
            int calWidthDim = this.calendarDimensions.Width;
            int calHeightDim = this.calendarDimensions.Height;

            List<MonthCalendarMonth> monthList = new List<MonthCalendarMonth>();
            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.viewStart);

            if (dt.GetEndDateOfWeek(this._formatProvider).Month != dt.Month)
            {
                dt = dt.GetEndDateOfWeek(this._formatProvider).FirstOfMonth;
            }

            if (this.UseRTL)
            {
                x = this._monthWidth * (calWidthDim - 1);

                for (int i = 0; i < calHeightDim; i++)
                {
                    for (int j = calWidthDim - 1; j >= 0; j--)
                    {
                        if (dt.Date >= this._maxDate)
                        {
                            break;
                        }

                        monthList.Add(new MonthCalendarMonth(this, dt.Date) { Location = new Point(x, y), Index = index++ });

                        x -= this._monthWidth;
                        dt = dt.AddMonths(1);
                    }

                    x = this._monthWidth * (calWidthDim - 1);
                    y += this._monthHeight;
                }
            }
            else
            {
                for (int i = 0; i < calHeightDim; i++)
                {
                    for (int j = 0; j < calWidthDim; j++)
                    {
                        if (dt.Date >= this._maxDate)
                        {
                            break;
                        }

                        monthList.Add(new MonthCalendarMonth(this, dt.Date) { Location = new Point(x, y), Index = index++ });

                        x += this._monthWidth;
                        dt = dt.AddMonths(1);
                    }

                    x = 0;
                    y += this._monthHeight;
                }
            }

            this._lastVisibleDate = monthList[monthList.Count - 1].LastVisibleDate;

            this.Months = monthList.ToArray();
        }

        /// <summary>
        /// Updates the month menu.
        /// </summary>
        /// <param name="year">
        /// The year to calculate the months for.
        /// </param>
        private void UpdateMonthMenu(int year)
        {
            int i = 1;

            int monthsInYear = this.CultureCalendar.GetMonthsInYear(year);

            // set month names in menu
            foreach (ToolStripMenuItem item in this.monthMenu.Items)
            {
                if (i <= monthsInYear)
                {
                    item.Tag = i;
                    item.Text = this._formatProvider.GetMonthName(year, i);
                    bool isAvailable = (this._monthSelected.Year > this.MinDate.Year
                                        || (this._monthSelected.Year == this.MinDate.Year && i >= this.MinDate.Month))
                                       && (this._monthSelected.Year < this.MaxDate.Year
                                           || (this._monthSelected.Year == this.MaxDate.Year && i <= this.MaxDate.Month));
                    item.Available = isAvailable;
                    if (isAvailable)
                    {
                        var now = DateTime.Now;
                        bool isCurrentMonth = i == now.Month && this._monthSelected.Year == now.Year;
                        var fontStyle = isCurrentMonth ? FontStyle.Bold : FontStyle.Regular;

                        bool isSelectedMonth = i == this._monthSelected.Month;
                        if (isSelectedMonth)
                        {
                            fontStyle |= FontStyle.Underline;
                            item.Select();
                        }

                        item.ForeColor = isCurrentMonth ? Color.DodgerBlue : SystemColors.MenuText;
                        item.Font = new Font(item.Font, fontStyle);
                    }

                    i++;
                }
                else
                {
                    item.Tag = null;
                    item.Text = string.Empty;
                    item.Visible = false;
                }
            }
        }

        /// <summary>
        /// Updates the year menu.
        /// </summary>
        /// <param name="year">
        /// The year in the middle to display.
        /// </param>
        private void UpdateYearMenu(int year)
        {
            int selYear = year;
            year -= 4;

            int maxYear = this.CultureCalendar.GetYear(this._maxDate);
            int minYear = this.CultureCalendar.GetYear(this._minDate);

            if (year + 8 > maxYear)
            {
                year = maxYear - 8;
            }
            else if (year < minYear)
            {
                year = minYear;
            }

            year = Math.Max(1, year);

            foreach (ToolStripMenuItem item in this.yearMenu.Items)
            {
                item.Text = DateMethods.GetNumberString(year, this.UseNativeDigits ? this.Culture.NumberFormat.NativeDigits : null, false);
                item.Tag = year;

                bool isCurrentYear = year == this.CultureCalendar.GetYear(DateTime.Today);
                var fontStyle = isCurrentYear ? FontStyle.Bold : FontStyle.Regular;
                if (year == selYear)
                {
                    fontStyle |= FontStyle.Underline;
                    item.Select();
                }

                item.Font = new Font(item.Font, fontStyle);
                item.ForeColor = isCurrentYear ? Color.DodgerBlue : SystemColors.MenuText;
                year++;
            }
        }

        /// <summary>
        /// Handles clicks in the month menu.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void MonthClick(object sender, EventArgs e)
        {
            MonthCalendarDate currentMonthYear = new MonthCalendarDate(this.CultureCalendar, (DateTime)this.monthMenu.Tag);

            int monthClicked = (int)((ToolStripMenuItem)sender).Tag;

            if (currentMonthYear.Month != monthClicked)
            {
                MonthCalendarDate dt = new MonthCalendarDate(
                    this.CultureCalendar, 
                    new DateTime(currentMonthYear.Year, monthClicked, 1, this.CultureCalendar));
                DateTime newStartDate = dt.AddMonths(-this.GetIndex(currentMonthYear.Date)).Date;

                if (this.SetStartDate(newStartDate))
                {
                    this.UpdateMonths();
                    this.RaiseDateChanged();
                    this.Focus();
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Handles clicks in the year menu.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event data.
        /// </param>
        private void YearClick(object sender, EventArgs e)
        {
            DateTime currentMonthYear = (DateTime)this.yearMenu.Tag;

            int yearClicked = (int)((ToolStripMenuItem)sender).Tag;

            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, currentMonthYear);

            if (dt.Year != yearClicked)
            {
                MonthCalendarDate newStartDate =
                    new MonthCalendarDate(this.CultureCalendar, new DateTime(yearClicked, dt.Month, 1, this.CultureCalendar)).AddMonths(
                        -this.GetIndex(currentMonthYear));

                if (this.SetStartDate(newStartDate.Date))
                {
                    this.UpdateMonths();

                    this.RaiseDateChanged();

                    this.Focus();

                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Is called when the month menu was closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// A <see cref="ToolStripDropDownClosedEventArgs"/> that contains the event data.
        /// </param>
        private void MonthMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this._monthSelected = DateTime.MinValue;
            this._showingMenu = false;

            this.Invalidate(this.Months[this.GetIndex((DateTime)this.monthMenu.Tag)].TitleBounds);
        }

        /// <summary>
        /// Is called when the year menu was closed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// A <see cref="ToolStripDropDownClosedEventArgs"/> that contains the event data.
        /// </param>
        private void YearMenuClosed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            this._yearSelected = DateTime.MinValue;
            this._showingMenu = false;

            this.Invalidate(this.Months[this.GetIndex((DateTime)this.yearMenu.Tag)].TitleBounds);
        }

        /// <summary>Calls <see cref="OnDateSelected" />.</summary>
        private void RaiseDateSelected()
        {
            SelectionRange range = this.SelectionRange;

            DateTime selStart = range.Start;
            DateTime selEnd = range.End;

            if (selStart == DateTime.MinValue)
            {
                selStart = selEnd;
            }

            this.OnDateSelected(new DateRangeEventArgs(selStart, selEnd));
        }

        /// <summary>Calls <see cref="OnDateChanged" />.</summary>
        private void RaiseDateChanged()
        {
            this.OnDateChanged(new DateRangeEventArgs(this.realStart, this._lastVisibleDate));
        }

        /// <summary>Raises the <see cref="SelectionExtendEnd" /> event.</summary>
        private void RaiseSelectExtendEnd()
        {
            var handler = this.SelectionExtendEnd;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>Raises the <see cref="InternalDateSelected" /> event.</summary>
        private void RaiseInternalDateSelected()
        {
            if (this.InternalDateSelected != null)
            {
                this.InternalDateSelected(this, new DateEventArgs(this.selectionStart));
            }
        }

        /// <summary>
        /// Adjusts the currently displayed month by setting a new start date.
        /// </summary>
        /// <param name="up">
        /// true for scrolling upwards, false otherwise.
        /// </param>
        private void Scroll(bool up)
        {
            if (this.SetStartDate(this.GetNewScrollDate(up)))
            {
                this.UpdateMonths();
                this.RaiseDateChanged();
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets the new date for the specified scroll direction.
        /// </summary>
        /// <param name="up">
        /// true for scrolling upwards, false otherwise.
        /// </param>
        /// <returns>
        /// The new start date.
        /// </returns>
        private DateTime GetNewScrollDate(bool up)
        {
            if ((this._lastVisibleDate == this._maxDate && !up) || (this.Months[0].FirstVisibleDate == this._minDate && up))
            {
                return this.viewStart;
            }

            MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.viewStart);

            int monthsToAdd = (this.scrollChange == 0
                                   ? Math.Max((this.calendarDimensions.Width * this.calendarDimensions.Height) - 1, 1)
                                   : this.scrollChange) * (up ? -1 : 1);

            int length = this.Months == null ? Math.Max(1, this.calendarDimensions.Width * this.calendarDimensions.Height) : this.Months.Length;

            MonthCalendarDate newStartMonthDate = dt.AddMonths(monthsToAdd);
            MonthCalendarDate lastMonthDate = newStartMonthDate.AddMonths(length - 1);
            MonthCalendarDate lastMonthEndDate = lastMonthDate.AddDays(lastMonthDate.DaysInMonth - 1 - lastMonthDate.Day);

            if (newStartMonthDate.Date < this._minDate)
            {
                newStartMonthDate = new MonthCalendarDate(this.CultureCalendar, this._minDate);
            }
            else if (lastMonthEndDate.Date >= this._maxDate || lastMonthDate.Date >= this._maxDate)
            {
                MonthCalendarDate maxdt = new MonthCalendarDate(this.CultureCalendar, this._maxDate).FirstOfMonth;
                newStartMonthDate = maxdt.AddMonths(1 - length);
            }

            return newStartMonthDate.Date;
        }

        /// <summary>
        /// Gets the current month header RequestState.
        /// </summary>
        /// <param name="monthDate">
        /// The month date.
        /// </param>
        /// <returns>
        /// A <see cref="MonthCalendarHeaderState"/> value.
        /// </returns>
        private MonthCalendarHeaderState GetMonthHeaderState(DateTime monthDate)
        {
            MonthCalendarHeaderState state = MonthCalendarHeaderState.Default;

            if (this._monthSelected == monthDate)
            {
                state = MonthCalendarHeaderState.MonthNameSelected;
            }
            else if (this._yearSelected == monthDate)
            {
                state = MonthCalendarHeaderState.YearSelected;
            }
            else if (this.mouseMoveFlags.MonthName == monthDate)
            {
                state = MonthCalendarHeaderState.MonthNameActive;
            }
            else if (this.mouseMoveFlags.Year == monthDate)
            {
                state = MonthCalendarHeaderState.YearActive;
            }
            else if (this.mouseMoveFlags.HeaderDate == monthDate)
            {
                state = MonthCalendarHeaderState.Active;
            }

            return state;
        }

        /// <summary>
        /// Invalidates the region taken up by the month specified by the <paramref name="date"/>.
        /// </summary>
        /// <param name="date">
        /// The date specifying the <see cref="MonthCalendarMonth"/> to invalidate.
        /// </param>
        /// <param name="refreshInvalid">
        /// true for refreshing the whole control if invalid <paramref name="date"/> passed; otherwise false.
        /// </param>
        private void InvalidateMonth(DateTime date, bool refreshInvalid)
        {
            if (date == DateTime.MinValue)
            {
                if (refreshInvalid)
                {
                    this.Refresh();
                }

                return;
            }

            MonthCalendarMonth month = this.GetMonth(date);

            if (month != null)
            {
                this.Invalidate(month.MonthBounds);
                this.Update();
            }
            else if (date > this._lastVisibleDate)
            {
                this.Invalidate(this.Months[this.Months.Length - 1].Bounds);
                this.Update();
            }
            else if (refreshInvalid)
            {
                this.Refresh();
            }
        }

        /// <summary>
        /// Checks if the <paramref name="newSelectionDate"/> is within bounds of the <paramref name="baseDate"/>
        /// and the <see cref="MaxSelectionCount"/>.
        /// </summary>
        /// <param name="baseDate">
        /// The base date from where to check.
        /// </param>
        /// <param name="newSelectionDate">
        /// The new selection date.
        /// </param>
        /// <returns>
        /// A valid new selection date if valid parameters, otherwise <c>DateTime.MinValue</c>.
        /// </returns>
        private DateTime GetSelectionDate(DateTime baseDate, DateTime newSelectionDate)
        {
            if (this.maxSelectionCount == 0 || baseDate == DateTime.MinValue)
            {
                return newSelectionDate;
            }

            if (baseDate >= this.CultureCalendar.MinSupportedDateTime && newSelectionDate >= this.CultureCalendar.MinSupportedDateTime
                && baseDate <= this.CultureCalendar.MaxSupportedDateTime && newSelectionDate <= this.CultureCalendar.MaxSupportedDateTime)
            {
                int days = (baseDate - newSelectionDate).Days;

                if (Math.Abs(days) >= this.maxSelectionCount)
                {
                    newSelectionDate =
                        new MonthCalendarDate(this.CultureCalendar, baseDate).AddDays(
                            days < 0 ? this.maxSelectionCount - 1 : 1 - this.maxSelectionCount).Date;
                }

                return newSelectionDate;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Returns the minimum date for the control.
        /// </summary>
        /// <param name="date">
        /// The date to set as min date.
        /// </param>
        /// <returns>
        /// The min date.
        /// </returns>
        private DateTime GetMinDate(DateTime date)
        {
            DateTime dt = new DateTime(1900, 1, 1);
            DateTime minEra = this.GetEraRange().MinDate;

            // bug in JapaneseLunisolarCalendar - JapaneseLunisolarCalendar.GetYear() with date parameter
            // between 1989/1/8 and 1989/2/6 returns 0 therefore make sure, the calendar
            // can display date range if ViewStart set to min date
            if (this.cultureCalendar.GetType() == typeof(JapaneseLunisolarCalendar))
            {
                minEra = new DateTime(1989, 4, 1);
            }

            DateTime mindate = minEra < dt ? dt : minEra;
            return date < mindate ? mindate : date;
        }

        /// <summary>
        /// Returns the maximum date for the control.
        /// </summary>
        /// <param name="date">
        /// The date to set as max date.
        /// </param>
        /// <returns>
        /// The max date.
        /// </returns>
        private DateTime GetMaxDate(DateTime date)
        {
            DateTime dt = new DateTime(9998, 12, 31);
            DateTime maxEra = this.GetEraRange().MaxDate;
            DateTime maxdate = maxEra > dt ? dt : maxEra;

            return date > maxdate ? maxdate : date;
        }

        /// <summary>If changing the used calendar, then this method reassigns the selection mode to set the correct selection range.</summary>
        private void ReAssignSelectionMode()
        {
            this.SelectionRange = null;
            MonthCalendarSelectionMode selMode = this.daySelectionMode;
            this.daySelectionMode = MonthCalendarSelectionMode.Manual;
            this.SelectionMode = selMode;
        }

        /// <summary>
        /// Sets the selection range for the specified <see cref="MonthCalendarSelectionMode"/>.
        /// </summary>
        /// <param name="selMode">
        /// The <see cref="MonthCalendarSelectionMode"/> value to set the selection range for.
        /// </param>
        private void SetSelectionRange(MonthCalendarSelectionMode selMode)
        {
            switch (selMode)
            {
                case MonthCalendarSelectionMode.Day:
                    {
                        this.selectionEnd = this.selectionStart;
                        break;
                    }

                case MonthCalendarSelectionMode.WorkWeek:
                case MonthCalendarSelectionMode.FullWeek:
                    {
                        MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.selectionStart).GetFirstDayInWeek(
                            this._formatProvider);
                        this.selectionStart = dt.Date;
                        this.selectionEnd = dt.AddDays(6).Date;

                        break;
                    }

                case MonthCalendarSelectionMode.Month:
                    {
                        MonthCalendarDate dt = new MonthCalendarDate(this.CultureCalendar, this.selectionStart).FirstOfMonth;
                        this.selectionStart = dt.Date;
                        this.selectionEnd = dt.AddMonths(1).AddDays(-1).Date;

                        break;
                    }
            }
        }

        #endregion
    }
}