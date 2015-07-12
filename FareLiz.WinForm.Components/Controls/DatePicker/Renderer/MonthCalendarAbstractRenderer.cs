namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Renderer
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;

    /// <summary>
    /// The base renderer class for the <see cref="EnhancedMonthCalendar"/><c>.</c><see cref="EnhancedMonthCalendar.Renderer"/>.
    /// </summary>
    public abstract class MonthCalendarAbstractRenderer
    {
        #region Fields

        /// <summary>
        /// The corresponding <see cref="EnhancedMonthCalendar"/>.
        /// </summary>
        private readonly EnhancedMonthCalendar _calendar;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthCalendarAbstractRenderer"/> class.
        /// </summary>
        /// <param name="calendar">The corresponding <see cref="EnhancedMonthCalendar"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="calendar"/> is null.</exception>
        protected MonthCalendarAbstractRenderer(EnhancedMonthCalendar calendar)
        {
            if (calendar == null)
                throw new ArgumentNullException("calendar", "Parameter '_calendar' cannot be null.");

            this._calendar = calendar;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="EnhancedMonthCalendar"/> control.
        /// </summary>
        public EnhancedMonthCalendar Calendar
        {
            get { return this._calendar; }
        }
        #endregion

        #region methods

        #region static methods

        /// <summary>
        /// Fills the specified <paramref name="path"/> either with a gradient background or a solid one.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="path">The <see cref="GraphicsPath"/> to fill.</param>
        /// <param name="colorStart">The start color.</param>
        /// <param name="colorEnd">The end color if drawing a gradient background.</param>
        /// <param name="mode">The <see cref="LinearGradientMode"/> to use.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="path"/> is null.</exception>
        public static void FillBackground(
           Graphics g,
           GraphicsPath path,
           Color colorStart,
           Color colorEnd,
           LinearGradientMode? mode)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path", "parameter 'path' cannot be null.");
            }

            RectangleF rect = path.GetBounds();

            if (!CheckParams(g, path.GetBounds()) || colorStart == Color.Empty)
            {
                return;
            }

            if (colorEnd == Color.Empty)
            {
                if (colorStart != Color.Transparent)
                {
                    using (SolidBrush brush = new SolidBrush(colorStart))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
            else
            {
                rect.Height += 2;
                rect.Y--;

                using (LinearGradientBrush brush = new LinearGradientBrush(
                   rect,
                   colorStart,
                   colorEnd,
                   mode.GetValueOrDefault(LinearGradientMode.Vertical)))
                {
                    g.FillPath(brush, path);
                }
            }
        }

        /// <summary>
        /// Checks the parameters of methods.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
        /// <returns>true, if all is ok, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="g"/> is null.</exception>
        protected static bool CheckParams(Graphics g, RectangleF rect)
        {
            if (g == null)
            {
                throw new ArgumentNullException("g");
            }

            if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the string format for the specified <see cref="ContentAlignment"/> value.
        /// </summary>
        /// <param name="align">The text align value.</param>
        /// <returns>A <see cref="StringFormat"/> object.</returns>
        protected static StringFormat GetStringAlignment(ContentAlignment align)
        {
            StringFormat format = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap);

            switch (align)
            {
                case ContentAlignment.TopLeft:
                    {
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Near;

                        break;
                    }

                case ContentAlignment.TopCenter:
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Near;

                        break;
                    }

                case ContentAlignment.TopRight:
                    {
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Near;

                        break;
                    }

                case ContentAlignment.MiddleLeft:
                    {
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Center;

                        break;
                    }

                case ContentAlignment.MiddleCenter:
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        break;
                    }

                case ContentAlignment.MiddleRight:
                    {
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Center;

                        break;
                    }

                case ContentAlignment.BottomLeft:
                    {
                        format.Alignment = StringAlignment.Near;
                        format.LineAlignment = StringAlignment.Far;

                        break;
                    }

                case ContentAlignment.BottomCenter:
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Far;

                        break;
                    }

                case ContentAlignment.BottomRight:
                    {
                        format.Alignment = StringAlignment.Far;
                        format.LineAlignment = StringAlignment.Far;

                        break;
                    }
            }

            return format;
        }
        #endregion

        #region abstract methods

        /// <summary>
        /// Draws the header of a <see cref="MonthCalendarMonth"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="calMonth">The <see cref="MonthCalendarMonth"/> to draw the header for.</param>
        /// <param name="state">The <see cref="MonthCalendarHeaderState"/>.</param>
        public abstract void DrawMonthHeader(Graphics g, MonthCalendarMonth calMonth, MonthCalendarHeaderState state);

        /// <summary>
        /// Draws the background of a <see cref="MonthCalendarMonth"/>.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="bounds">The <see cref="Rectangle"/> area to draw the background on.</param>
        public abstract void DrawCalendarBackground(Graphics g, Rectangle bounds);

        /// <summary>
        /// Draws a day in the month body of the _calendar control.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="day">The <see cref="MonthCalendarDay"/> to draw.</param>
        public abstract void DrawDay(Graphics g, MonthCalendarDay day);

        /// <summary>
        /// Draws the day names.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
        public abstract void DrawDayHeader(Graphics g, Rectangle rect);

        /// <summary>
        /// Draws the footer.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="rect">The <see cref="Rectangle"/> to draw in.</param>
        /// <param name="active">true if the footer is in mouse over RequestState; otherwise false.</param>
        public abstract void DrawFooter(Graphics g, Rectangle rect, bool active);

        /// <summary>
        /// Draws a single week header element.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> object used to draw.</param>
        /// <param name="week">The <see cref="MonthCalendarWeek"/> to draw.</param>
        public abstract void DrawWeekHeaderItem(Graphics g, MonthCalendarWeek week);

        /// <summary>
        /// Draws the separator line between week header and month body.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to draw.</param>
        /// <param name="rect">The bounds of the week header.</param>
        public abstract void DrawWeekHeaderSeparator(Graphics g, Rectangle rect);

        #endregion
        #endregion
    }
}