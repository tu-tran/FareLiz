namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Helper;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.Renderer;

    /// <summary>The enhanced month calendar.</summary>
    partial class EnhancedMonthCalendar
    {
        /// <summary>Occurs when the user selects a date or a date range.</summary>
        [Category("Action")]
        [Description("Is raised, when the user selected a date or date range.")]
        public event EventHandler<DateRangeEventArgs> DateSelected;

        /// <summary>Occurs when the user changed the month or year.</summary>
        [Category("Action")]
        [Description("Is raised, when the user changed the month or year.")]
        public event EventHandler<DateRangeEventArgs> DateChanged;

        /// <summary>Is Raised when the mouse is over an date.</summary>
        [Category("Action")]
        [Description("Is raised when the mouse is over an date.")]
        public event EventHandler<ActiveDateChangedEventArgs> ActiveDateChanged;

        /// <summary>Is raised when the selection extension ended.</summary>
        [Category("Action")]
        [Description("Is raised when the selection extension ended.")]
        public event EventHandler SelectionExtendEnd;

        /// <summary>Is raised when a date was clicked.</summary>
        [Category("Action")]
        [Description("Is raised when a date in selection mode 'Day' was clicked.")]
        public event EventHandler<DateEventArgs> DateClicked;

        /// <summary>Is raises when a date was selected.</summary>
        internal event EventHandler<DateEventArgs> InternalDateSelected;

        /// <summary>
        /// Raises the <see cref="DateSelected"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DateRangeEventArgs"/> object that contains the event data.
        /// </param>
        private void OnDateSelected(DateRangeEventArgs e)
        {
            if (this.DateSelected != null)
            {
                this.DateSelected(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="DateClicked"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="DateEventArgs"/> that contains the event data.
        /// </param>
        private void OnDateClicked(DateEventArgs e)
        {
            if (this.DateClicked != null)
            {
                this.DateClicked(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="PaintEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // return if in update mode of if nothing to draw
            if (this.calendarDimensions.IsEmpty || this._inUpdate)
            {
                return;
            }

            // set text rendering hint
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            this._renderer.DrawCalendarBackground(e.Graphics, this.Bounds);

            // loop through all months to draw
            foreach (var month in this.Months)
            {
                // if month is null or not in the clip rectangle - continue
                if (month == null || !e.ClipRectangle.IntersectsWith(month.Bounds))
                {
                    continue;
                }

                var state = this.GetMonthHeaderState(month.Date);

                // draw the month header
                this._renderer.DrawMonthHeader(e.Graphics, month, state);

                // draw the day names header
                this._renderer.DrawDayHeader(e.Graphics, month.DayNamesBounds);

                // show week header ?
                if (this.showWeekHeader)
                {
                    // loop through all week header elements
                    foreach (var week in month.Weeks)
                    {
                        // if week not visible continue
                        if (!week.Visible)
                        {
                            continue;
                        }

                        // draw week header element
                        this._renderer.DrawWeekHeaderItem(e.Graphics, week);
                    }
                }

                // loop through all days in current month
                foreach (var day in month.Days)
                {
                    // if day is not visible continue
                    if (!day.Visible)
                    {
                        continue;
                    }

                    // draw the day
                    this._renderer.DrawDay(e.Graphics, day);
                }

                // draw week header separator line
                this._renderer.DrawWeekHeaderSeparator(e.Graphics, month.WeekBounds);
            }

            // if footer is shown
            if (this.showFooter)
            {
                // draw the footer
                this._renderer.DrawFooter(e.Graphics, this._footerRect, this.mouseMoveFlags.Footer);
            }

            // draw the border
            var r = this.ClientRectangle;
            r.Width--;
            r.Height--;
            e.Graphics.DrawRectangle(MonthCalendarRenderer.BorderPen, r);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseDown"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.Focus();
            this.Capture = true;

            // reset the selection range where selection started
            this._selectionStartRange = null;
            if (e.Button == MouseButtons.Left)
            {
                var hit = this.HitTest(e.Location); // perform hit test                
                this.currentMoveBounds = hit.Bounds; // set current bounds
                this.currentHitType = hit.Type; // set current hit type

                switch (hit.Type)
                {
                    case MonthCalendarHitType.Day:
                        {
                            // save old selection range
                            var oldRange = this.SelectionRange;

                            if (!this.extendSelection || this.daySelectionMode != MonthCalendarSelectionMode.Manual)
                            {
                                // clear all selection ranges
                                this.selectionRanges.Clear();
                            }

                            switch (this.daySelectionMode)
                            {
                                case MonthCalendarSelectionMode.Day:
                                    {
                                        this.OnDateClicked(new DateEventArgs(hit.Date));

                                        // only single days are selectable
                                        if (this.selectionStart != hit.Date)
                                        {
                                            this.SelectionStart = hit.Date;
                                            this.RaiseDateSelected();
                                        }

                                        break;
                                    }

                                case MonthCalendarSelectionMode.WorkWeek:
                                    {
                                        // only single work week is selectable
                                        // get first day of week
                                        var firstDay =
                                            new MonthCalendarDate(this.CultureCalendar, hit.Date).GetFirstDayInWeek(this._formatProvider).Date;

                                        // get work days
                                        var workDays = DateMethods.GetWorkDays(this.nonWorkDays);

                                        // reset selection start and end
                                        this.selectionEnd = DateTime.MinValue;
                                        this.selectionStart = DateTime.MinValue;

                                        // current range
                                        SelectionRange currentRange = null;

                                        // build selection ranges for work days
                                        for (var i = 0; i < 7; i++)
                                        {
                                            var toAdd = firstDay.AddDays(i);

                                            if (workDays.Contains(toAdd.DayOfWeek))
                                            {
                                                if (currentRange == null)
                                                {
                                                    currentRange = new SelectionRange(DateTime.MinValue, DateTime.MinValue);
                                                }

                                                if (currentRange.Start == DateTime.MinValue)
                                                {
                                                    currentRange.Start = toAdd;
                                                }
                                                else
                                                {
                                                    currentRange.End = toAdd;
                                                }
                                            }
                                            else if (currentRange != null)
                                            {
                                                this.selectionRanges.Add(currentRange);
                                                currentRange = null;
                                            }
                                        }

                                        if (this.selectionRanges.Count >= 1)
                                        {
                                            // set first selection range
                                            this.SelectionRange = this.selectionRanges[0];
                                            this.selectionRanges.RemoveAt(0);

                                            // if selection range changed, raise event
                                            if (this.SelectionRange != oldRange)
                                            {
                                                this.RaiseDateSelected();
                                            }
                                        }
                                        else
                                        {
                                            this.Refresh();
                                        }

                                        break;
                                    }

                                case MonthCalendarSelectionMode.FullWeek:
                                    {
                                        // only a full week is selectable
                                        // get selection start and end
                                        var dt = new MonthCalendarDate(this.CultureCalendar, hit.Date).GetFirstDayInWeek(this._formatProvider);
                                        this.selectionStart = dt.Date;
                                        this.selectionEnd = dt.GetEndDateOfWeek(this._formatProvider).Date;

                                        // if range changed, raise event
                                        if (this.SelectionRange != oldRange)
                                        {
                                            this.RaiseDateSelected();
                                            this.Refresh();
                                        }

                                        break;
                                    }

                                case MonthCalendarSelectionMode.Month:
                                    {
                                        // only a full month is selectable
                                        var dt = new MonthCalendarDate(this.CultureCalendar, hit.Date).FirstOfMonth;

                                        // get selection start and end
                                        this.selectionStart = dt.Date;
                                        this.selectionEnd = dt.AddMonths(1).AddDays(-1).Date;

                                        // if range changed, raise event
                                        if (this.SelectionRange != oldRange)
                                        {
                                            this.RaiseDateSelected();
                                            this.Refresh();
                                        }

                                        break;
                                    }

                                case MonthCalendarSelectionMode.Manual:
                                    {
                                        if (this.extendSelection)
                                        {
                                            var range = this.selectionRanges.Find(r => hit.Date >= r.Start && hit.Date <= r.End);
                                            if (range != null)
                                            {
                                                this.selectionRanges.Remove(range);
                                            }
                                        }

                                        // manual mode - selection ends when user is releasing the left mouse button
                                        this.selectionStarted = true;
                                        this._backupRange = this.SelectionRange;
                                        this.selectionEnd = DateTime.MinValue;
                                        this.SelectionStart = hit.Date;
                                        break;
                                    }
                            }

                            break;
                        }

                    case MonthCalendarHitType.Week:
                        {
                            this.selectionRanges.Clear();

                            if (this.maxSelectionCount > 6 || this.maxSelectionCount == 0)
                            {
                                this._backupRange = this.SelectionRange;
                                this.selectionStarted = true;
                                this.selectionEnd = new MonthCalendarDate(this.CultureCalendar, hit.Date).GetEndDateOfWeek(this._formatProvider).Date;
                                this.SelectionStart = hit.Date;
                                this._selectionStartRange = this.SelectionRange;
                            }

                            break;
                        }

                    case MonthCalendarHitType.MonthName:
                        {
                            this._monthSelected = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;

                            this.Invalidate(hit.InvalidateBounds);
                            this.Update();

                            this.monthMenu.Tag = hit.Date;
                            this.UpdateMonthMenu(this.CultureCalendar.GetYear(hit.Date));

                            this._showingMenu = true;

                            // show month menu
                            this.monthMenu.Show(this, hit.Bounds.Right, e.Location.Y);
                            break;
                        }

                    case MonthCalendarHitType.MonthYear:
                        {
                            this._yearSelected = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;

                            this.Invalidate(hit.InvalidateBounds);
                            this.Update();

                            this.UpdateYearMenu(this.CultureCalendar.GetYear(hit.Date));

                            this.yearMenu.Tag = hit.Date;

                            this._showingMenu = true;

                            // show year menu
                            this.yearMenu.Show(this, hit.Bounds.Right, e.Location.Y);

                            break;
                        }

                    case MonthCalendarHitType.Arrow:
                        {
                            // an arrow was pressed
                            // set new start date
                            if (this.SetStartDate(hit.Date))
                            {
                                // update months
                                this.UpdateMonths();

                                // raise event
                                this.RaiseDateChanged();

                                this.mouseMoveFlags.HeaderDate = this._leftArrowRect.Contains(e.Location)
                                                                     ? this.Months[0].Date
                                                                     : this.Months[this.calendarDimensions.Width - 1].Date;

                                this.Refresh();
                            }

                            break;
                        }

                    case MonthCalendarHitType.Footer:
                        {
                            // footer was pressed
                            this.selectionRanges.Clear();

                            var raiseDateChanged = false;

                            var range = this.SelectionRange;

                            // determine if date changed event has to be raised
                            if (DateTime.Today < this.Months[0].FirstVisibleDate || DateTime.Today > this._lastVisibleDate)
                            {
                                // set new start date
                                if (this.SetStartDate(DateTime.Today))
                                {
                                    // update months
                                    this.UpdateMonths();

                                    raiseDateChanged = true;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            // set new selection start and end values
                            this.selectionStart = DateTime.Today;
                            this.selectionEnd = DateTime.Today;

                            this.SetSelectionRange(this.daySelectionMode);

                            this.OnDateClicked(new DateEventArgs(DateTime.Today));

                            // raise events if necessary
                            if (range != this.SelectionRange)
                            {
                                this.RaiseDateSelected();
                            }

                            if (raiseDateChanged)
                            {
                                this.RaiseDateChanged();
                            }

                            this.Refresh();
                            break;
                        }

                    case MonthCalendarHitType.Header:
                        {
                            // header was pressed
                            this.Invalidate(hit.Bounds);
                            this.Update();
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Location == this.mouseLocation)
            {
                return;
            }

            this.mouseLocation = e.Location;

            // backup and reset mouse move flags
            this.mouseMoveFlags.BackupAndReset();

            // perform hit test
            var hit = this.HitTest(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                // if selection started - only in manual selection mode
                if (this.selectionStarted)
                {
                    // if selection started with hit type Day and mouse is over new date
                    if (hit.Type == MonthCalendarHitType.Day && this.currentHitType == MonthCalendarHitType.Day
                        && this.currentMoveBounds != hit.Bounds)
                    {
                        this.currentMoveBounds = hit.Bounds;

                        // set new selection end
                        this.SelectionEnd = hit.Date;
                    }
                    else if (hit.Type == MonthCalendarHitType.Week && this.currentHitType == MonthCalendarHitType.Week)
                    {
                        // set indicator that a week header element is selected
                        this.mouseMoveFlags.WeekHeader = true;

                        // get new end date
                        var endDate = new MonthCalendarDate(this.CultureCalendar, hit.Date).AddDays(6).Date;

                        // if new week header element
                        if (this.currentMoveBounds != hit.Bounds)
                        {
                            this.currentMoveBounds = hit.Bounds;

                            // check if selection is switched
                            if (this.selectionStart == this._selectionStartRange.End)
                            {
                                // are we after the original end date?
                                if (endDate > this.selectionStart)
                                {
                                    // set original start date
                                    this.selectionStart = this._selectionStartRange.Start;

                                    // set new end date
                                    this.SelectionEnd = endDate;
                                }
                                else
                                {
                                    // going backwards - set new "end" date - it's now the start date
                                    this.SelectionEnd = hit.Date;
                                }
                            }
                            else
                            {
                                // we are after the start date
                                if (endDate > this.selectionStart)
                                {
                                    // set end date
                                    this.SelectionEnd = endDate;
                                }
                                else
                                {
                                    // switch start and end
                                    this.selectionStart = this._selectionStartRange.End;
                                    this.SelectionEnd = hit.Date;
                                }
                            }
                        }
                    }
                }
                else
                {
                    switch (hit.Type)
                    {
                        case MonthCalendarHitType.MonthName:
                            {
                                this.mouseMoveFlags.MonthName = hit.Date;
                                this.mouseMoveFlags.HeaderDate = hit.Date;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }

                        case MonthCalendarHitType.MonthYear:
                            {
                                this.mouseMoveFlags.Year = hit.Date;
                                this.mouseMoveFlags.HeaderDate = hit.Date;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }

                        case MonthCalendarHitType.Header:
                            {
                                this.mouseMoveFlags.HeaderDate = hit.Date;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }

                        case MonthCalendarHitType.Arrow:
                            {
                                var useRTL = this.UseRTL;

                                if (this._leftArrowRect.Contains(e.Location))
                                {
                                    this.mouseMoveFlags.LeftArrow = !useRTL;
                                    this.mouseMoveFlags.RightArrow = useRTL;
                                    this.mouseMoveFlags.HeaderDate = this.Months[0].Date;
                                }
                                else
                                {
                                    this.mouseMoveFlags.LeftArrow = useRTL;
                                    this.mouseMoveFlags.RightArrow = !useRTL;
                                    this.mouseMoveFlags.HeaderDate = this.Months[this.calendarDimensions.Width - 1].Date;
                                }

                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }

                        case MonthCalendarHitType.Footer:
                            {
                                this.mouseMoveFlags.Footer = true;
                                this.Invalidate(hit.InvalidateBounds);
                                break;
                            }

                        default:
                            {
                                this.Invalidate();
                                break;
                            }
                    }
                }
            }
            else if (e.Button == MouseButtons.None)
            {
                // no mouse button is pressed
                // set flags and invalidate corresponding region
                switch (hit.Type)
                {
                    case MonthCalendarHitType.Day:
                        {
                            this.mouseMoveFlags.Day = hit.Date;
                            var bold = this.GetBoldedDates().Contains(hit.Date)
                                       || this._boldDatesCollection.Exists(d => d.Value.Date == hit.Date.Date);
                            this.OnActiveDateChanged(new ActiveDateChangedEventArgs(hit.Date, bold));
                            this.InvalidateMonth(hit.Date, true);
                            break;
                        }

                    case MonthCalendarHitType.Week:
                        {
                            this.mouseMoveFlags.WeekHeader = true;

                            break;
                        }

                    case MonthCalendarHitType.MonthName:
                        {
                            this.mouseMoveFlags.MonthName = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            break;
                        }

                    case MonthCalendarHitType.MonthYear:
                        {
                            this.mouseMoveFlags.Year = hit.Date;
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            break;
                        }

                    case MonthCalendarHitType.Header:
                        {
                            this.mouseMoveFlags.HeaderDate = hit.Date;
                            break;
                        }

                    case MonthCalendarHitType.Arrow:
                        {
                            var useRTL = this.UseRTL;

                            if (this._leftArrowRect.Contains(e.Location))
                            {
                                this.mouseMoveFlags.LeftArrow = !useRTL;
                                this.mouseMoveFlags.RightArrow = useRTL;

                                this.mouseMoveFlags.HeaderDate = this.Months[0].Date;
                            }
                            else if (this._rightArrowRect.Contains(e.Location))
                            {
                                this.mouseMoveFlags.LeftArrow = useRTL;
                                this.mouseMoveFlags.RightArrow = !useRTL;

                                this.mouseMoveFlags.HeaderDate = this.Months[this.calendarDimensions.Width - 1].Date;
                            }

                            break;
                        }

                    case MonthCalendarHitType.Footer:
                        {
                            this.mouseMoveFlags.Footer = true;
                            break;
                        }
                }

                // if left arrow RequestState changed
                if (this.mouseMoveFlags.LeftArrowChanged)
                {
                    this.Invalidate(this.UseRTL ? this._rightArrowRect : this._leftArrowRect);

                    this.Update();
                }

                // if right arrow RequestState changed
                if (this.mouseMoveFlags.RightArrowChanged)
                {
                    this.Invalidate(this.UseRTL ? this._leftArrowRect : this._rightArrowRect);

                    this.Update();
                }

                // if header RequestState changed
                if (this.mouseMoveFlags.HeaderDateChanged)
                {
                    this.Invalidate();
                }
                else if (this.mouseMoveFlags.MonthNameChanged || this.mouseMoveFlags.YearChanged)
                {
                    // if RequestState of month name or year in header changed
                    var range1 = new SelectionRange(this.mouseMoveFlags.MonthName, this.mouseMoveFlags.Backup.MonthName);

                    var range2 = new SelectionRange(this.mouseMoveFlags.Year, this.mouseMoveFlags.Backup.Year);

                    this.Invalidate(this.Months[this.GetIndex(range1.End)].TitleBounds);

                    if (range1.End != range2.End)
                    {
                        this.Invalidate(this.Months[this.GetIndex(range2.End)].TitleBounds);
                    }
                }

                // if day RequestState changed
                if (this.mouseMoveFlags.DayChanged)
                {
                    // invalidate current day
                    this.InvalidateMonth(this.mouseMoveFlags.Day, false);

                    // invalidate last day
                    this.InvalidateMonth(this.mouseMoveFlags.Backup.Day, false);
                }

                // if footer RequestState changed
                if (this.mouseMoveFlags.FooterChanged)
                {
                    this.Invalidate(this._footerRect);
                }
            }

            // if mouse is over a week header, change cursor
            if (this.mouseMoveFlags.WeekHeaderChanged)
            {
                this.Cursor = this.mouseMoveFlags.WeekHeader ? Cursors.UpArrow : Cursors.Default;
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseUp"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // if left mouse button is pressed and selection process was started
            if (e.Button == MouseButtons.Left && this.selectionStarted)
            {
                this.selectionRanges.Add(new SelectionRange(this.SelectionRange.Start, this.SelectionRange.End));

                // reset selection process
                this.selectionStarted = false;

                this.Refresh();

                // raise selected event if necessary
                if (this._backupRange.Start != this.SelectionRange.Start || this._backupRange.End != this.SelectionRange.End)
                {
                    // raise date 
                    this.RaiseDateSelected();
                }
            }

            // reset current hit type
            this.currentHitType = MonthCalendarHitType.None;

            this.Capture = false;
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseLeave"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // reset some of the mouse move flags
            this.mouseMoveFlags.LeftArrow = false;
            this.mouseMoveFlags.RightArrow = false;
            this.mouseMoveFlags.MonthName = DateTime.MinValue;
            this.mouseMoveFlags.Year = DateTime.MinValue;
            this.mouseMoveFlags.Footer = false;
            this.mouseMoveFlags.Day = DateTime.MinValue;

            if (!this._showingMenu)
            {
                this.mouseMoveFlags.HeaderDate = DateTime.MinValue;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.MouseWheel"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (!this._showingMenu)
            {
                this.Scroll(e.Delta > 0);
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.ParentChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (this.Parent != null && this.Created)
            {
                this.UpdateMonths();

                this.Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.RightToLeftChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            this._formatProvider.IsRTLLanguage = this.UseRTL;

            this.UpdateMonths();

            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.EnabledChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            this.Refresh();
        }

        /// <summary>
        /// Raises the <see cref="DateChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DateRangeEventArgs"/> object that contains the event data.
        /// </param>
        private void OnDateChanged(DateRangeEventArgs e)
        {
            if (this.DateChanged != null)
            {
                this.DateChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ActiveDateChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="ActiveDateChangedEventArgs"/> that contains the event data.
        /// </param>
        private void OnActiveDateChanged(ActiveDateChangedEventArgs e)
        {
            if (this.ActiveDateChanged != null)
            {
                this.ActiveDateChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.FontChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            this.CalculateSize(false);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="System.Windows.Forms.KeyEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                this.extendSelection = true;
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.KeyUp"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="System.Windows.Forms.KeyEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                this.extendSelection = false;

                this.RaiseSelectExtendEnd();
            }

            base.OnKeyUp(e);
        }
    }
}