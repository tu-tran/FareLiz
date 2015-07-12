namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Helper;

    /// <summary>
    /// The Calendar control renderer.
    /// </summary>
    public class MonthCalendarRenderer : MonthCalendarAbstractRenderer
    {
        private readonly EnhancedMonthCalendar _calendar;

        public static readonly Brush SelectedDayBackgroundBrush = Brushes.OrangeRed;
        public static readonly Color SelectedDayTextColor = Color.White;
        public static readonly Brush HoverDayBackgroundBrush = Brushes.Teal;
        public static readonly Color HoverDayTextColor = Color.White;

        public static readonly Pen BorderPen = Pens.Silver;
        public static readonly Brush BorderBrush = Brushes.Silver;
        public static readonly Brush ActiveTextBrush = Brushes.Teal;
        public static readonly Brush InactiveTextBrush = Brushes.Black;
        public static readonly Brush SelectedTextBrush = Brushes.OrangeRed;
        public static readonly Brush DayHeaderTextBrush = Brushes.Teal;
        public static readonly Brush BackgroundBrush = Brushes.White;

        public static readonly Color InactiveTextColor = Color.Black;
        public static readonly Color ActiveTextColor = Color.Teal;
        public static readonly Color SelectedTextColor = Color.White;
        public static readonly Color TrailingDayColor = Color.Gainsboro;

        public static readonly Brush InactiveArrowBrush = Brushes.Black;
        public static readonly Brush ActiveArrowBrush = Brushes.Teal;

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthCalendarRenderer"/> class.
        /// </summary>
        /// <param name="calendar">The corresponding <see cref="EnhancedMonthCalendar"/>.</param>
        public MonthCalendarRenderer(EnhancedMonthCalendar calendar)
            : base(calendar)
        {
            this._calendar = calendar;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the corresponding <see cref="EnhancedMonthCalendar"/>.
        /// </summary>
        public EnhancedMonthCalendar EnhancedMonthCalendar
        {
            get { return this._calendar; }
        }

        #endregion

        #region methods

        public override void DrawMonthHeader(Graphics g, MonthCalendarMonth calMonth, MonthCalendarHeaderState state)
        {
            if (calMonth == null || !CheckParams(g, calMonth.TitleBounds))
                return;

            // get title bounds
            Rectangle rect = calMonth.TitleBounds;

            MonthCalendarDate date = new MonthCalendarDate(this._calendar.CultureCalendar, calMonth.Date);
            MonthCalendarDate firstVisible = new MonthCalendarDate(this._calendar.CultureCalendar, calMonth.FirstVisibleDate);

            string month;
            int year;

            // gets the month name for the month the MonthCalendarMonth represents and the year string
            if (firstVisible.Era != date.Era)
            {
                month = this._calendar.FormatProvider.GetMonthName(firstVisible.Year, firstVisible.Month);
                year = firstVisible.Year;
            }
            else
            {
                month = this._calendar.FormatProvider.GetMonthName(date.Year, date.Month);
                year = date.Year;
            }

            string yearString = this._calendar.UseNativeDigits
               ? DateMethods.GetNativeNumberString(year, this._calendar.Culture.NumberFormat.NativeDigits, false)
               : year.ToString(CultureInfo.CurrentUICulture);

            // get used font
            Font headerFont = this._calendar.HeaderFont;

            // create bold font
            Font boldFont = new Font(headerFont.FontFamily, headerFont.SizeInPoints, FontStyle.Bold);

            // measure sizes
            SizeF monthSize = g.MeasureString(month, boldFont);

            SizeF yearSize = g.MeasureString(yearString, boldFont);

            float maxHeight = Math.Max(monthSize.Height, yearSize.Height);

            // calculates the width and the starting position of the arrows
            int width = (int)monthSize.Width + (int)yearSize.Width + 7;
            int arrowLeftX = rect.X + 6;
            int arrowRightX = rect.Right - 6;
            int arrowY = rect.Y + (rect.Height / 2) - 4;

            int x = Math.Max(0, rect.X + (rect.Width / 2) + 1 - (width / 2));
            int y = Math.Max(
               0,
               rect.Y + (rect.Height / 2) + 1 - (((int)maxHeight + 1) / 2));

            // set the title month name bounds
            calMonth.TitleMonthBounds = new Rectangle(
               x,
               y,
               (int)monthSize.Width + 1,
               (int)maxHeight + 1);

            // set the title year bounds
            calMonth.TitleYearBounds = new Rectangle(
               x + calMonth.TitleMonthBounds.Width + 7,
               y,
               (int)yearSize.Width + 1,
               (int)maxHeight + 1);

            // generate points for the left and right arrow
            Point[] arrowLeft = new[]
         {
            new Point(arrowLeftX, arrowY + 4),
            new Point(arrowLeftX + 4, arrowY),
            new Point(arrowLeftX + 4, arrowY + 8),
            new Point(arrowLeftX, arrowY + 4)
         };

            Point[] arrowRight = new[]
         {
            new Point(arrowRightX, arrowY + 4),
            new Point(arrowRightX - 4, arrowY),
            new Point(arrowRightX - 4, arrowY + 8),
            new Point(arrowRightX, arrowY + 4)
         };

            // get brushes for normal, mouse over and selected RequestState
            // get title month name and year bounds
            Rectangle monthRect = calMonth.TitleMonthBounds;
            Rectangle yearRect = calMonth.TitleYearBounds;

            // set used fonts
            Font monthFont = headerFont;
            Font yearFont = headerFont;

            // set used brushes
            Brush monthBrush = InactiveTextBrush, yearBrush = InactiveTextBrush;

            // adjust brush and font if year selected
            if (state == MonthCalendarHeaderState.YearSelected)
            {
                yearBrush = SelectedTextBrush;
                yearFont = boldFont;
                yearRect.Width += 4;
            }
            else if (state == MonthCalendarHeaderState.YearActive)
            {
                // adjust brush if mouse over year
                yearBrush = ActiveTextBrush;
            }

            // adjust brush and font if month name is selected
            if (state == MonthCalendarHeaderState.MonthNameSelected)
            {
                monthBrush = SelectedTextBrush;
                monthFont = boldFont;
                monthRect.Width += 4;
            }
            else if (state == MonthCalendarHeaderState.MonthNameActive)
            {
                // adjust brush if mouse over month name
                monthBrush = ActiveTextBrush;
            }

            // draws the month name and year string
            g.DrawString(month, monthFont, monthBrush, monthRect);
            g.DrawString(yearString, yearFont, yearBrush, yearRect);

            boldFont.Dispose();

            // if left arrow has to be drawn
            if (calMonth.DrawLeftButton)
            {
                // get arrow color
                var arrowBrush = this._calendar.LeftButtonState == ButtonState.Normal ? InactiveArrowBrush : ActiveArrowBrush;
                // set left arrow rect
                this._calendar.SetLeftArrowRect(new Rectangle(rect.X, rect.Y, 15, rect.Height));
                // draw left arrow
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(arrowLeft);
                    g.FillPath(arrowBrush, path);
                }
            }

            // if right arrow has to be drawn
            if (calMonth.DrawRightButton)
            {
                Brush arrowBrush = this._calendar.RightButtonState == ButtonState.Normal ? InactiveArrowBrush : ActiveArrowBrush;
                // set right arrow rect
                this._calendar.SetRightArrowRect(new Rectangle(rect.Right - 15, rect.Y, 15, rect.Height));
                // draw arrow
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLines(arrowRight);
                    g.FillPath(arrowBrush, path);
                }
            }
        }

        public override void DrawCalendarBackground(Graphics g, Rectangle bounds)
        {
            g.FillRectangle(BackgroundBrush, bounds);
        }

        public override void DrawDay(Graphics g, MonthCalendarDay day)
        {
            if (!CheckParams(g, day.Bounds))
                return;

            // if today, draw border
            if (day.Date == DateTime.Today)
                this.DrawTodayLegend(g, day.Bounds);

            // get the bounds of the day
            Rectangle rect = new Rectangle(day.Bounds.X + 2, day.Bounds.Y, day.Bounds.Width - 4, day.Bounds.Height); ;

            var boldDate = this._calendar.BoldedDatesCollection.Find(d => d.Value.Date == day.Date.Date);

            // if day is selected or in mouse over RequestState
            if (day.Selected)
                g.FillRectangle(SelectedDayBackgroundBrush, rect);
            else if (day.MouseOver)
                g.FillRectangle(HoverDayBackgroundBrush, rect);
            else if (!boldDate.IsEmpty && boldDate.Category.BackColorStart != Color.Empty && boldDate.Category.BackColorStart != Color.Transparent)
                g.DrawRectangle(BorderPen, rect);

            // get bolded dates
            List<DateTime> boldedDates = this._calendar.GetBoldedDates();
            bool bold = boldedDates.Contains(day.Date) || !boldDate.IsEmpty;

            // draw the day
            using (StringFormat format = GetStringAlignment(this._calendar.DayTextAlignment))
            {
                Color textColor = bold ? (boldDate.IsEmpty || boldDate.Category.ForeColor == Color.Empty || boldDate.Category.ForeColor == Color.Transparent ? InactiveTextColor : boldDate.Category.ForeColor)
                   : (day.Selected ? SelectedDayTextColor
                   : (day.MouseOver ? HoverDayTextColor
                   : (day.TrailingDate ? TrailingDayColor
                   : InactiveTextColor)));

                using (SolidBrush brush = new SolidBrush(textColor))
                {
                    using (Font font = new Font(this._calendar.Font.FontFamily, this._calendar.Font.SizeInPoints, FontStyle.Bold))
                    {
                        // adjust width
                        Rectangle textRect = day.Bounds;
                        textRect.Width -= 2;

                        // determine if to use bold font
                        bool useBoldFont = day.Selected || bold;

                        var calDate = new MonthCalendarDate(this._calendar.CultureCalendar, day.Date);

                        string dayString = this._calendar.UseNativeDigits
                                              ? DateMethods.GetNativeNumberString(calDate.Day, this._calendar.Culture.NumberFormat.NativeDigits, false)
                                              : calDate.Day.ToString(this._calendar.Culture);

                        if (this._calendar.Enabled)
                            g.DrawString(dayString, (useBoldFont ? font : this._calendar.Font), brush, textRect, format);
                        else
                            ControlPaint.DrawStringDisabled(g, dayString, (useBoldFont ? font : this._calendar.Font), Color.Transparent, textRect, format);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the day header.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
        public override void DrawDayHeader(Graphics g, Rectangle rect)
        {
            // get day width
            int dayWidth = this._calendar.DaySize.Width;

            if (!CheckParams(g, rect) || dayWidth <= 0)
                return;

            // get abbreviated day names
            List<string> names = new List<string>(DateMethods.GetDayNames(this._calendar.FormatProvider, this._calendar.UseShortestDayNames ? 2 : 1));

            // if in RTL mode, reverse order
            if (this._calendar.UseRTL)
                names.Reverse();

            // get bounds for a single element
            Rectangle dayRect = rect;
            dayRect.Width = dayWidth;

            // draw day names
            using (StringFormat format =
               new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap)
               {
                   Alignment = StringAlignment.Center,
                   LineAlignment = StringAlignment.Center
               })
            {
                names.ForEach(day =>
                   {
                       if (this._calendar.Enabled)
                           g.DrawString(day, this._calendar.DayHeaderFont, DayHeaderTextBrush, dayRect, format);
                       else
                           ControlPaint.DrawStringDisabled(g, day, this._calendar.DayHeaderFont, Color.Transparent, dayRect, format);
                       dayRect.X += dayWidth;
                   });
            }

            // draw separator line
            g.DrawLine(BorderPen, rect.X, rect.Bottom - 1, rect.Right - 1, rect.Bottom - 1);
        }

        /// <summary>
        /// Draws the footer.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
        /// <param name="active">true if the footer is in mouse over RequestState; otherwise false.</param>
        public override void DrawFooter(Graphics g, Rectangle rect, bool active)
        {
            if (!CheckParams(g, rect))
                return;

            string dateString = "Today: " + new MonthCalendarDate(this._calendar.CultureCalendar, DateTime.Today).ToString(
               null,
               null,
               this._calendar.FormatProvider,
               this._calendar.UseNativeDigits ? this._calendar.Culture.NumberFormat.NativeDigits : null);

            // get date size
            SizeF dateSize = g.MeasureString(dateString, this._calendar.FooterFont);

            // get today rectangle and adjust position
            int todayX = (this._calendar.UseRTL ? rect.Right - 20 : rect.X + 2);
            int todayY = rect.Y + ((rect.Height - 10) / 2);
            Rectangle todayRect = new Rectangle(todayX, todayY, 20, 10);

            // draw the today rectangle
            this.DrawTodayLegend(g, todayRect);

            // get top position to draw the text at
            int y = rect.Y + (rect.Height / 2) - ((int)dateSize.Height / 2);

            Rectangle dateRect;

            // if in RTL mode
            if (this._calendar.UseRTL)
            {
                // get date bounds
                dateRect = new Rectangle(rect.X + 1, y, todayRect.Left - rect.X, (int)dateSize.Height + 1);
            }
            else
            {
                // get date bounds
                dateRect = new Rectangle(todayRect.Right + 2, y, rect.Width - todayRect.Width, (int)dateSize.Height + 1);
            }

            // draw date string
            using (StringFormat format = GetStringAlignment(this._calendar.UseRTL ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft))
            {
                var brush = active ? ActiveTextBrush : InactiveTextBrush;
                g.DrawString(dateString, this._calendar.FooterFont, brush, dateRect, format);
            }
        }

        /// <summary>
        /// Draws a single week header element.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="week">The <see cref="MonthCalendarWeek"/> to draw.</param>
        public override void DrawWeekHeaderItem(Graphics g, MonthCalendarWeek week)
        {
            if (!CheckParams(g, week.Bounds))
                return;

            var weekString = this._calendar.UseNativeDigits
               ? DateMethods.GetNativeNumberString(week.WeekNumber, this._calendar.Culture.NumberFormat.NativeDigits, false)
               : week.WeekNumber.ToString(CultureInfo.CurrentUICulture);

            // draw week header element
            using (StringFormat format = GetStringAlignment(this._calendar.DayTextAlignment))
            {
                // set alignment
                format.Alignment = StringAlignment.Center;

                // draw string
                if (this._calendar.Enabled)
                    g.DrawString(weekString, this._calendar.Font, MonthCalendarRenderer.BorderBrush, week.Bounds, format);
                else
                    ControlPaint.DrawStringDisabled(g, weekString, this._calendar.Font, Color.Transparent, week.Bounds, format);
            }
        }

        /// <summary>
        /// Draws the separator line between week header and month body.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to draw.</param>
        /// <param name="rect">The bounds of the week header.</param>
        public override void DrawWeekHeaderSeparator(Graphics g, Rectangle rect)
        {
            if (!CheckParams(g, rect) || !this._calendar.ShowWeekHeader)
                return;

            // draw separator line
            if (this._calendar.UseRTL)
                g.DrawLine(BorderPen, rect.X, rect.Y - 1, rect.X, rect.Bottom - 1);
            else
                g.DrawLine(BorderPen, rect.Right - 1, rect.Y - 1, rect.Right - 1, rect.Bottom - 1);
        }

        private void DrawTodayLegend(Graphics g, Rectangle bounds)
        {
            g.DrawRectangle(BorderPen, bounds);
        }
        #endregion
    }
}