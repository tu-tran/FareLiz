namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses;
    using SkyDean.FareLiz.WinForm.Components.Properties;

    /// <summary>
    /// The enhanced date picker.
    /// </summary>
    partial class EnhancedDatePicker
    {
        /// <summary>
        /// The value changed.
        /// </summary>
        [Category("Action")]
        [Description("Is Raised when the date value changed.")]
        public event EventHandler<CheckDateEventArgs> ValueChanged;

        /// <summary>Is raised when the mouse is over an date.</summary>
        [Category("Action")]
        [Description("Is raised when the mouse is over an date.")]
        public event EventHandler<ActiveDateChangedEventArgs> ActiveDateChanged;

        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">
        /// The event args.
        /// </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rect = this.ClientRectangle;
            e.Graphics.Clear(this.Enabled ? this.BackColor : SystemColors.Window);
            var mouseLoc = this.PointToClient(MousePosition);
            bool mouseOverControl = rect.Contains(mouseLoc);

            using (var borderPen = new Pen(mouseOverControl ? SystemColors.HotTrack : SystemColors.ActiveBorder))
            {
                e.Graphics.DrawRectangle(borderPen, rect);
            }

            var borderWidth = SystemInformation.BorderSize.Width;
            var borderHeight = SystemInformation.BorderSize.Height;

            rect.X = rect.Right - DropDownButtonWidth + borderWidth;
            rect.Width = DropDownButtonWidth - 2 * borderWidth;
            rect.Y += borderHeight;
            rect.Height = rect.Bottom - 2 * borderHeight;
            this._buttonBounds = rect;

            var isMouseOverDropDownButton = rect.Contains(mouseLoc);
            if (isMouseOverDropDownButton)
            {
                // Draw background only if the mouse if over
                ButtonRenderer.DrawButton(
                    e.Graphics, 
                    rect, 
                    this._buttonState == ComboButtonState.Pressed ? PushButtonState.Pressed : PushButtonState.Hot);
            }

            e.Graphics.DrawImage(Resources.CalendarToggle, new Point(rect.X + 2, rect.Y + 2));

            var arrowHeight = this.Height / 6;
            var arrowWidth = arrowHeight * 0.8;
            var arrawLoc = new Point((int)(rect.Right - arrowWidth * 4), rect.Height / 2);

            // if the width is odd - favor pushing it over one pixel right.
            var arrow = new[]
                            {
                                new Point((int)(arrawLoc.X - arrowWidth), arrawLoc.Y - 1), new Point((int)(arrawLoc.X + arrowWidth + 1), arrawLoc.Y - 1), 
                                new Point(arrawLoc.X, arrawLoc.Y + arrowHeight)
                            };

            if (this.Enabled)
            {
                e.Graphics.FillPolygon(SystemBrushes.ControlText, arrow);
            }
            else
            {
                e.Graphics.FillPolygon(SystemBrushes.ButtonShadow, arrow);
            }
        }

        /// <summary>
        /// Raises the mouse enter event.
        /// </summary>
        /// <param name="e">
        /// The event args.
        /// </param>
        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this._isDropped)
            {
                this._buttonState = ComboButtonState.Hot;
                this.Refresh();
            }

            base.OnMouseEnter(e);
        }

        /// <summary>
        /// Raises the mouse leave event.
        /// </summary>
        /// <param name="e">
        /// The event args.
        /// </param>
        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this._isDropped)
            {
                this._buttonState = ComboButtonState.Normal;
                this.Invalidate();
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// Raises the mouse down event.
        /// </summary>
        /// <param name="e">
        /// The event args.
        /// </param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();

            if (e.Button == MouseButtons.Left && this._buttonBounds.Contains(e.Location))
            {
                this.SwitchPickerState();
                this.Refresh();
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// Raises the <see cref="Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="MouseEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.None && !this._isDropped)
            {
                ComboButtonState st = this._buttonState;

                this._buttonState = this._buttonBounds.Contains(e.Location) ? ComboButtonState.Hot : ComboButtonState.Normal;

                if (st != this._buttonState)
                {
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Control.LostFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnLostFocus(EventArgs e)
        {
            if (!this._isDropped)
            {
                this._buttonState = ComboButtonState.Normal;

                this.Invalidate();
            }

            if (!this.Focused)
            {
                base.OnLostFocus(e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Control.GotFocus"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="Control.FontChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            this.dateTextBox.Font = this.Font;

            this.Height = Math.Max(22, this.MeasureControlSize());
        }

        /// <summary>
        /// Raises the <see cref="Control.ForeColorChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            if (this.dateTextBox != null)
            {
                this.dateTextBox.ForeColor = this.ForeColor;
            }

            base.OnForeColorChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="Control.EnabledChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            this.Invalidate();
        }

        /// <summary>
        /// Raises the <see cref="Control.RightToLeftChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            this.dateTextBox.RightToLeft = this.RightToLeft;
            this.dateTextBox.Refresh();
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="EventArgs"/> that contains the event data.
        /// </param>
        private void OnValueChanged(CheckDateEventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(this, e);
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
    }
}