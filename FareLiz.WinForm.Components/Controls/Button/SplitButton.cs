//Get the latest version of SplitButton at: http://wyday.com/splitbutton/

namespace SkyDean.FareLiz.WinForm.Components.Controls.Button
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using ContentAlignment = System.Drawing.ContentAlignment;

    public class SplitButton : ImageButton
    {
        private const int SplitSectionWidth = 18;

        private static readonly int BorderSize = SystemInformation.Border3DSize.Width * 2;
        private PushButtonState _state;
        private Rectangle dropDownRectangle;
        private bool isMouseEntered;

        private bool isSplitMenuVisible;


        private ContextMenu m_SplitMenu;
        private ContextMenuStrip m_SplitMenuStrip;
        private bool showSplit;
        private bool skipNextOpen;

        private TextFormatFlags textFormatFlags = TextFormatFlags.Default;

        public SplitButton()
        {
            this.AutoSize = true;
        }

        #region Properties

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return this.SplitMenuStrip; }
            set { this.SplitMenuStrip = value; }
        }

        [DefaultValue(null)]
        public ContextMenu SplitMenu
        {
            get { return this.m_SplitMenu; }
            set
            {
                //remove the event handlers for the old SplitMenu
                if (this.m_SplitMenu != null)
                {
                    this.m_SplitMenu.Popup -= this.SplitMenu_Popup;
                }

                //add the event handlers for the new SplitMenu
                if (value != null)
                {
                    this.ShowSplit = true;
                    value.Popup += this.SplitMenu_Popup;
                }
                else
                    this.ShowSplit = false;

                this.m_SplitMenu = value;
            }
        }

        [DefaultValue(null)]
        public ContextMenuStrip SplitMenuStrip
        {
            get { return this.m_SplitMenuStrip; }
            set
            {
                //remove the event handlers for the old SplitMenuStrip
                if (this.m_SplitMenuStrip != null)
                {
                    this.m_SplitMenuStrip.Closing -= this.SplitMenuStrip_Closing;
                    this.m_SplitMenuStrip.Opening -= this.SplitMenuStrip_Opening;
                }

                //add the event handlers for the new SplitMenuStrip
                if (value != null)
                {
                    this.ShowSplit = true;
                    value.Closing += this.SplitMenuStrip_Closing;
                    value.Opening += this.SplitMenuStrip_Opening;
                }
                else
                    this.ShowSplit = false;


                this.m_SplitMenuStrip = value;
            }
        }

        [DefaultValue(true)]
        public bool ShowSplit
        {
            set
            {
                if (value != this.showSplit)
                {
                    this.showSplit = value;
                    this.Invalidate();

                    if (this.Parent != null)
                        this.Parent.PerformLayout();
                }
            }
        }

        private PushButtonState State
        {
            get { return this._state; }
            set
            {
                if (!this._state.Equals(value))
                {
                    this._state = value;
                    this.Invalidate();
                }
            }
        }

        #endregion Properties

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData.Equals(Keys.Down) && this.showSplit)
                return true;

            return base.IsInputKey(keyData);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (!this.showSplit)
            {
                base.OnGotFocus(e);
                return;
            }

            if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
            {
                this.State = PushButtonState.Default;
            }
        }

        protected override void OnKeyDown(KeyEventArgs kevent)
        {
            if (this.showSplit)
            {
                if (kevent.KeyCode.Equals(Keys.Down) && !this.isSplitMenuVisible)
                {
                    this.ShowContextMenuStrip();
                }

                else if (kevent.KeyCode.Equals(Keys.Space) && kevent.Modifiers == Keys.None)
                {
                    this.State = PushButtonState.Pressed;
                }
            }

            base.OnKeyDown(kevent);
        }

        protected override void OnKeyUp(KeyEventArgs kevent)
        {
            if (kevent.KeyCode.Equals(Keys.Space))
            {
                if (MouseButtons == MouseButtons.None)
                {
                    this.State = PushButtonState.Normal;
                }
            }
            else if (kevent.KeyCode.Equals(Keys.Apps))
            {
                if (MouseButtons == MouseButtons.None && !this.isSplitMenuVisible)
                {
                    this.ShowContextMenuStrip();
                }
            }

            base.OnKeyUp(kevent);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            this.State = this.Enabled ? PushButtonState.Normal : PushButtonState.Disabled;

            base.OnEnabledChanged(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!this.showSplit)
            {
                base.OnLostFocus(e);
                return;
            }

            if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
            {
                this.State = PushButtonState.Normal;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (!this.showSplit)
            {
                base.OnMouseEnter(e);
                return;
            }

            this.isMouseEntered = true;

            if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
            {
                this.State = PushButtonState.Hot;
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!this.showSplit)
            {
                base.OnMouseLeave(e);
                return;
            }

            this.isMouseEntered = false;

            if (!this.State.Equals(PushButtonState.Pressed) && !this.State.Equals(PushButtonState.Disabled))
            {
                this.State = this.Focused ? PushButtonState.Default : PushButtonState.Normal;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.showSplit)
            {
                base.OnMouseDown(e);
                return;
            }

            //handle ContextMenu re-clicking the drop-down region to close the menu
            if (this.m_SplitMenu != null && e.Button == MouseButtons.Left && !this.isMouseEntered)
                this.skipNextOpen = true;

            if (this.dropDownRectangle.Contains(e.Location) && !this.isSplitMenuVisible && e.Button == MouseButtons.Left)
            {
                this.ShowContextMenuStrip();
            }
            else
            {
                this.State = PushButtonState.Pressed;
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (!this.showSplit)
            {
                base.OnMouseUp(mevent);
                return;
            }

            // if the right button was released inside the button
            if (mevent.Button == MouseButtons.Right && this.ClientRectangle.Contains(mevent.Location) && !this.isSplitMenuVisible)
            {
                this.ShowContextMenuStrip();
            }
            else if (this.m_SplitMenuStrip == null && this.m_SplitMenu == null || !this.isSplitMenuVisible)
            {
                this.SetButtonDrawState();

                if (this.ClientRectangle.Contains(mevent.Location) && !this.dropDownRectangle.Contains(mevent.Location))
                {
                    this.OnClick(new EventArgs());
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (!this.showSplit)
                return;

            Graphics g = pevent.Graphics;
            Rectangle bounds = this.ClientRectangle;

            // draw the button background as according to the current RequestState.
            if (this.State != PushButtonState.Pressed && this.IsDefault && !Application.RenderWithVisualStyles)
            {
                Rectangle backgroundBounds = bounds;
                backgroundBounds.Inflate(-1, -1);
                ButtonRenderer.DrawButton(g, backgroundBounds, this.State);

                // button renderer doesnt draw the black frame when themes are off
                g.DrawRectangle(SystemPens.WindowFrame, 0, 0, bounds.Width - 1, bounds.Height - 1);
            }
            else
            {
                ButtonRenderer.DrawButton(g, bounds, this.State);
            }

            // calculate the current dropdown rectangle.
            this.dropDownRectangle = new Rectangle(bounds.Right - SplitSectionWidth, 0, SplitSectionWidth, bounds.Height);

            int internalBorder = BorderSize;
            var focusRect =
                new Rectangle(internalBorder - 1,
                              internalBorder - 1,
                              bounds.Width - this.dropDownRectangle.Width - internalBorder,
                              bounds.Height - (internalBorder * 2) + 2);

            bool drawSplitLine = (this.State == PushButtonState.Hot || this.State == PushButtonState.Pressed ||
                                  !Application.RenderWithVisualStyles);


            if (this.RightToLeft == RightToLeft.Yes)
            {
                this.dropDownRectangle.X = bounds.Left + 1;
                focusRect.X = this.dropDownRectangle.Right;

                if (drawSplitLine)
                {
                    // draw two lines at the edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Left + SplitSectionWidth, BorderSize,
                               bounds.Left + SplitSectionWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Left + SplitSectionWidth + 1, BorderSize,
                               bounds.Left + SplitSectionWidth + 1, bounds.Bottom - BorderSize);
                }
            }
            else
            {
                if (drawSplitLine)
                {
                    // draw two lines at the edge of the dropdown button
                    g.DrawLine(SystemPens.ButtonShadow, bounds.Right - SplitSectionWidth, BorderSize,
                               bounds.Right - SplitSectionWidth, bounds.Bottom - BorderSize);
                    g.DrawLine(SystemPens.ButtonFace, bounds.Right - SplitSectionWidth - 1, BorderSize,
                               bounds.Right - SplitSectionWidth - 1, bounds.Bottom - BorderSize);
                }
            }

            // Draw an arrow in the correct location
            this.PaintArrow(g, this.dropDownRectangle);

            //paint the image and text in the "button" part of the splitButton
            this.PaintTextandImage(g, new Rectangle(0, 0, this.ClientRectangle.Width - SplitSectionWidth, this.ClientRectangle.Height));

            // draw the focus rectangle.
            if (this.State != PushButtonState.Pressed && this.Focused && this.ShowFocusCues)
            {
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }

        private void PaintTextandImage(Graphics g, Rectangle bounds)
        {
            // Figure out where our text and image should go
            Rectangle text_rectangle;
            Rectangle image_rectangle;

            this.CalculateButtonTextAndImageLayout(ref bounds, out text_rectangle, out image_rectangle);

            //draw the image
            if (this.Image != null)
            {
                if (this.Enabled)
                    g.DrawImage(this.Image, image_rectangle.X, image_rectangle.Y, this.Image.Width, this.Image.Height);
                else
                    ControlPaint.DrawImageDisabled(g, this.Image, image_rectangle.X, image_rectangle.Y, this.BackColor);
            }

            // If we dont' use mnemonic, set formatFlag to NoPrefix as this will show ampersand.
            if (!this.UseMnemonic)
                this.textFormatFlags = this.textFormatFlags | TextFormatFlags.NoPrefix;
            else if (!this.ShowKeyboardCues)
                this.textFormatFlags = this.textFormatFlags | TextFormatFlags.HidePrefix;

            //draw the text
            if (!string.IsNullOrEmpty(this.Text))
            {
                if (this.Enabled)
                    TextRenderer.DrawText(g, this.Text, this.Font, text_rectangle, this.ForeColor, this.textFormatFlags);
                else
                    ControlPaint.DrawStringDisabled(g, this.Text, this.Font, this.BackColor, text_rectangle, this.textFormatFlags);
            }
        }

        private void PaintArrow(Graphics g, Rectangle dropDownRect)
        {
            var middle = new Point(Convert.ToInt32(dropDownRect.Left + dropDownRect.Width / 2),
                                   Convert.ToInt32(dropDownRect.Top + dropDownRect.Height / 2));

            //if the width is odd - favor pushing it over one pixel right.
            middle.X += (dropDownRect.Width % 2);

            var arrow = new[]
                {
                    new Point(middle.X - 2, middle.Y - 1), new Point(middle.X + 3, middle.Y - 1),
                    new Point(middle.X, middle.Y + 2)
                };

            if (this.Enabled)
                g.FillPolygon(SystemBrushes.ControlText, arrow);
            else
                g.FillPolygon(SystemBrushes.ButtonShadow, arrow);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);

            //autosize correctly for splitbuttons
            if (this.showSplit)
            {
                if (this.AutoSize)
                    return this.CalculateButtonAutoSize();

                if (!string.IsNullOrEmpty(this.Text) &&
                    TextRenderer.MeasureText(this.Text, this.Font).Width + SplitSectionWidth > preferredSize.Width)
                    return preferredSize + new Size(SplitSectionWidth + BorderSize * 2, 0);
            }

            return preferredSize;
        }

        private Size CalculateButtonAutoSize()
        {
            Size ret_size = Size.Empty;
            Size text_size = TextRenderer.MeasureText(this.Text, this.Font);
            Size image_size = this.Image == null ? Size.Empty : this.Image.Size;

            // Pad the text size
            if (this.Text.Length != 0)
            {
                text_size.Height += 4;
                text_size.Width += 4;
            }

            switch (this.TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    ret_size.Height = Math.Max(this.Text.Length == 0 ? 0 : text_size.Height, image_size.Height);
                    ret_size.Width = Math.Max(text_size.Width, image_size.Width);
                    break;
                case TextImageRelation.ImageAboveText:
                case TextImageRelation.TextAboveImage:
                    ret_size.Height = text_size.Height + image_size.Height;
                    ret_size.Width = Math.Max(text_size.Width, image_size.Width);
                    break;
                case TextImageRelation.ImageBeforeText:
                case TextImageRelation.TextBeforeImage:
                    ret_size.Height = Math.Max(text_size.Height, image_size.Height);
                    ret_size.Width = text_size.Width + image_size.Width;
                    break;
            }

            // Pad the result
            ret_size.Height += (this.Padding.Vertical + 6);
            ret_size.Width += (this.Padding.Horizontal + 6);

            //pad the splitButton arrow region
            if (this.showSplit)
                ret_size.Width += SplitSectionWidth;

            return ret_size;
        }

        public void ShowContextMenuStrip()
        {
            if (this.skipNextOpen)
            {
                // we were called because we're closing the context menu strip
                // when clicking the dropdown button.
                this.skipNextOpen = false;
                return;
            }

            this.State = PushButtonState.Pressed;

            if (this.m_SplitMenu != null)
            {
                this.m_SplitMenu.Show(this, new Point(0, this.Height));
            }
            else if (this.m_SplitMenuStrip != null)
            {
                this.m_SplitMenuStrip.Show(this, new Point(0, this.Height), ToolStripDropDownDirection.BelowRight);
            }
        }

        private void SplitMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            this.isSplitMenuVisible = true;
        }

        private void SplitMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            this.isSplitMenuVisible = false;

            this.SetButtonDrawState();

            if (e.CloseReason == ToolStripDropDownCloseReason.AppClicked)
            {
                this.skipNextOpen = (this.dropDownRectangle.Contains(this.PointToClient(Cursor.Position))) &&
                               MouseButtons == MouseButtons.Left;
            }
        }


        private void SplitMenu_Popup(object sender, EventArgs e)
        {
            this.isSplitMenuVisible = true;
        }

        protected override void WndProc(ref Message m)
        {
            //0x0212 == WM_EXITMENULOOP
            if (m.Msg == 0x0212)
            {
                //this message is only sent when a ContextMenu is closed (not a ContextMenuStrip)
                this.isSplitMenuVisible = false;
                this.SetButtonDrawState();
            }

            base.WndProc(ref m);
        }

        private void SetButtonDrawState()
        {
            if (this.Bounds.Contains(this.Parent.PointToClient(Cursor.Position)))
            {
                this.State = PushButtonState.Hot;
            }
            else if (this.Focused)
            {
                this.State = PushButtonState.Default;
            }
            else if (!this.Enabled)
            {
                this.State = PushButtonState.Disabled;
            }
            else
            {
                this.State = PushButtonState.Normal;
            }
        }

        #region Button Layout Calculations

        //The following layout functions were taken from Mono's Windows.Forms 
        //implementation, specifically "ThemeWin32Classic.cs", 
        //then modified to fit the context of this splitButton

        private void CalculateButtonTextAndImageLayout(ref Rectangle content_rect, out Rectangle textRectangle,
                                                       out Rectangle imageRectangle)
        {
            Size text_size = TextRenderer.MeasureText(this.Text, this.Font, content_rect.Size, this.textFormatFlags);
            Size image_size = this.Image == null ? Size.Empty : this.Image.Size;

            textRectangle = Rectangle.Empty;
            imageRectangle = Rectangle.Empty;

            switch (this.TextImageRelation)
            {
                case TextImageRelation.Overlay:
                    // Overlay is easy, text always goes here
                    textRectangle = OverlayObjectRect(ref content_rect, ref text_size, this.TextAlign);
                    // Rectangle.Inflate(content_rect, -4, -4);

                    //Offset on Windows 98 style when button is pressed
                    if (this._state == PushButtonState.Pressed && !Application.RenderWithVisualStyles)
                        textRectangle.Offset(1, 1);

                    // Image is dependent on ImageAlign
                    if (this.Image != null)
                        imageRectangle = OverlayObjectRect(ref content_rect, ref image_size, this.ImageAlign);

                    break;
                case TextImageRelation.ImageAboveText:
                    content_rect.Inflate(-4, -4);
                    this.LayoutTextAboveOrBelowImage(content_rect, false, text_size, image_size, out textRectangle,
                                                out imageRectangle);
                    break;
                case TextImageRelation.TextAboveImage:
                    content_rect.Inflate(-4, -4);
                    this.LayoutTextAboveOrBelowImage(content_rect, true, text_size, image_size, out textRectangle,
                                                out imageRectangle);
                    break;
                case TextImageRelation.ImageBeforeText:
                    content_rect.Inflate(-4, -4);
                    this.LayoutTextBeforeOrAfterImage(content_rect, false, text_size, image_size, out textRectangle,
                                                 out imageRectangle);
                    break;
                case TextImageRelation.TextBeforeImage:
                    content_rect.Inflate(-4, -4);
                    this.LayoutTextBeforeOrAfterImage(content_rect, true, text_size, image_size, out textRectangle,
                                                 out imageRectangle);
                    break;
            }
        }

        private static Rectangle OverlayObjectRect(ref Rectangle container, ref Size sizeOfObject,
                                                   ContentAlignment alignment)
        {
            int x, y;

            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                    x = 4;
                    y = 4;
                    break;
                case ContentAlignment.TopCenter:
                    x = (container.Width - sizeOfObject.Width) / 2;
                    y = 4;
                    break;
                case ContentAlignment.TopRight:
                    x = container.Width - sizeOfObject.Width - 4;
                    y = 4;
                    break;
                case ContentAlignment.MiddleLeft:
                    x = 4;
                    y = (container.Height - sizeOfObject.Height) / 2;
                    break;
                case ContentAlignment.MiddleCenter:
                    x = (container.Width - sizeOfObject.Width) / 2;
                    y = (container.Height - sizeOfObject.Height) / 2;
                    break;
                case ContentAlignment.MiddleRight:
                    x = container.Width - sizeOfObject.Width - 4;
                    y = (container.Height - sizeOfObject.Height) / 2;
                    break;
                case ContentAlignment.BottomLeft:
                    x = 4;
                    y = container.Height - sizeOfObject.Height - 4;
                    break;
                case ContentAlignment.BottomCenter:
                    x = (container.Width - sizeOfObject.Width) / 2;
                    y = container.Height - sizeOfObject.Height - 4;
                    break;
                case ContentAlignment.BottomRight:
                    x = container.Width - sizeOfObject.Width - 4;
                    y = container.Height - sizeOfObject.Height - 4;
                    break;
                default:
                    x = 4;
                    y = 4;
                    break;
            }

            return new Rectangle(x, y, sizeOfObject.Width, sizeOfObject.Height);
        }

        private void LayoutTextBeforeOrAfterImage(Rectangle totalArea, bool textFirst, Size textSize, Size imageSize,
                                                  out Rectangle textRect, out Rectangle imageRect)
        {
            int element_spacing = 0; // Spacing between the Text and the Image
            int total_width = textSize.Width + element_spacing + imageSize.Width;

            if (!textFirst)
                element_spacing += 2;

            // If the text is too big, chop it down to the size we have available to it
            if (total_width > totalArea.Width)
            {
                textSize.Width = totalArea.Width - element_spacing - imageSize.Width;
                total_width = totalArea.Width;
            }

            int excess_width = totalArea.Width - total_width;
            int offset = 0;

            Rectangle final_text_rect;
            Rectangle final_image_rect;

            HorizontalAlignment h_text = GetHorizontalAlignment(this.TextAlign);
            HorizontalAlignment h_image = GetHorizontalAlignment(this.ImageAlign);

            if (h_image == HorizontalAlignment.Left)
                offset = 0;
            else if (h_image == HorizontalAlignment.Right && h_text == HorizontalAlignment.Right)
                offset = excess_width;
            else if (h_image == HorizontalAlignment.Center &&
                     (h_text == HorizontalAlignment.Left || h_text == HorizontalAlignment.Center))
                offset += excess_width / 3;
            else
                offset += 2 * (excess_width / 3);

            if (textFirst)
            {
                final_text_rect = new Rectangle(totalArea.Left + offset,
                                                AlignInRectangle(totalArea, textSize, this.TextAlign).Top, textSize.Width,
                                                textSize.Height);
                final_image_rect = new Rectangle(final_text_rect.Right + element_spacing,
                                                 AlignInRectangle(totalArea, imageSize, this.ImageAlign).Top, imageSize.Width,
                                                 imageSize.Height);
            }
            else
            {
                final_image_rect = new Rectangle(totalArea.Left + offset,
                                                 AlignInRectangle(totalArea, imageSize, this.ImageAlign).Top, imageSize.Width,
                                                 imageSize.Height);
                final_text_rect = new Rectangle(final_image_rect.Right + element_spacing,
                                                AlignInRectangle(totalArea, textSize, this.TextAlign).Top, textSize.Width,
                                                textSize.Height);
            }

            textRect = final_text_rect;
            imageRect = final_image_rect;
        }

        private void LayoutTextAboveOrBelowImage(Rectangle totalArea, bool textFirst, Size textSize, Size imageSize,
                                                 out Rectangle textRect, out Rectangle imageRect)
        {
            int element_spacing = 0; // Spacing between the Text and the Image
            int total_height = textSize.Height + element_spacing + imageSize.Height;

            if (textFirst)
                element_spacing += 2;

            if (textSize.Width > totalArea.Width)
                textSize.Width = totalArea.Width;

            // If the there isn't enough room and we're text first, cut out the image
            if (total_height > totalArea.Height && textFirst)
            {
                imageSize = Size.Empty;
                total_height = totalArea.Height;
            }

            int excess_height = totalArea.Height - total_height;
            int offset = 0;

            Rectangle final_text_rect;
            Rectangle final_image_rect;

            VerticalAlignment v_text = GetVerticalAlignment(this.TextAlign);
            VerticalAlignment v_image = GetVerticalAlignment(this.ImageAlign);

            if (v_image == VerticalAlignment.Top)
                offset = 0;
            else if (v_image == VerticalAlignment.Bottom && v_text == VerticalAlignment.Bottom)
                offset = excess_height;
            else if (v_image == VerticalAlignment.Center &&
                     (v_text == VerticalAlignment.Top || v_text == VerticalAlignment.Center))
                offset += excess_height / 3;
            else
                offset += 2 * (excess_height / 3);

            if (textFirst)
            {
                final_text_rect = new Rectangle(AlignInRectangle(totalArea, textSize, this.TextAlign).Left,
                                                totalArea.Top + offset, textSize.Width, textSize.Height);
                final_image_rect = new Rectangle(AlignInRectangle(totalArea, imageSize, this.ImageAlign).Left,
                                                 final_text_rect.Bottom + element_spacing, imageSize.Width,
                                                 imageSize.Height);
            }
            else
            {
                final_image_rect = new Rectangle(AlignInRectangle(totalArea, imageSize, this.ImageAlign).Left,
                                                 totalArea.Top + offset, imageSize.Width, imageSize.Height);
                final_text_rect = new Rectangle(AlignInRectangle(totalArea, textSize, this.TextAlign).Left,
                                                final_image_rect.Bottom + element_spacing, textSize.Width,
                                                textSize.Height);

                if (final_text_rect.Bottom > totalArea.Bottom)
                    final_text_rect.Y = totalArea.Top;
            }

            textRect = final_text_rect;
            imageRect = final_image_rect;
        }

        private static HorizontalAlignment GetHorizontalAlignment(ContentAlignment align)
        {
            switch (align)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                    return HorizontalAlignment.Left;
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                    return HorizontalAlignment.Center;
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                    return HorizontalAlignment.Right;
            }

            return HorizontalAlignment.Left;
        }

        private static VerticalAlignment GetVerticalAlignment(ContentAlignment align)
        {
            switch (align)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    return VerticalAlignment.Top;
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    return VerticalAlignment.Center;
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    return VerticalAlignment.Bottom;
            }

            return VerticalAlignment.Top;
        }

        internal static Rectangle AlignInRectangle(Rectangle outer, Size inner, ContentAlignment align)
        {
            int x = 0;
            int y = 0;

            if (align == ContentAlignment.BottomLeft || align == ContentAlignment.MiddleLeft ||
                align == ContentAlignment.TopLeft)
                x = outer.X;
            else if (align == ContentAlignment.BottomCenter || align == ContentAlignment.MiddleCenter ||
                     align == ContentAlignment.TopCenter)
                x = Math.Max(outer.X + ((outer.Width - inner.Width) / 2), outer.Left);
            else if (align == ContentAlignment.BottomRight || align == ContentAlignment.MiddleRight ||
                     align == ContentAlignment.TopRight)
                x = outer.Right - inner.Width;
            if (align == ContentAlignment.TopCenter || align == ContentAlignment.TopLeft ||
                align == ContentAlignment.TopRight)
                y = outer.Y;
            else if (align == ContentAlignment.MiddleCenter || align == ContentAlignment.MiddleLeft ||
                     align == ContentAlignment.MiddleRight)
                y = outer.Y + (outer.Height - inner.Height) / 2;
            else if (align == ContentAlignment.BottomCenter || align == ContentAlignment.BottomRight ||
                     align == ContentAlignment.BottomLeft)
                y = outer.Bottom - inner.Height;

            return new Rectangle(x, y, Math.Min(inner.Width, outer.Width), Math.Min(inner.Height, outer.Height));
        }

        #endregion Button Layout Calculations
    }
}