namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Properties;

    /// <summary>
    /// The flat tab control.
    /// </summary>
    partial class FlatTabControl
    {
        /// <summary>
        /// The _inactive tab diff.
        /// </summary>
        private int _inactiveTabDiff = 3;

        /// <summary>
        /// The paint tab.
        /// </summary>
        /// <param name="tabPage">
        /// The tab page.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        private void PaintTab(TabPage tabPage, int index, Graphics graphics)
        {
            using (GraphicsPath tabpath = this.GetTabBorder(index))
            {
                using (Brush fillBrush = this.GetTabBackgroundBrush(index))
                {
                    // 	Paint the background
                    graphics.FillPath(fillBrush, tabpath);

                    // 	Paint a focus indication
                    if (this.Focused)
                    {
                        using (var brush = new SolidBrush(ControlPaint.Dark(tabPage.BackColor)))
                        {
                            this.DrawTabFocusIndicator(brush, tabpath, index, graphics);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The draw tab.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        /// <param name="tabPage">
        /// The tab page.
        /// </param>
        /// <param name="nIndex">
        /// The n index.
        /// </param>
        private void DrawTab(Graphics g, TabPage tabPage, int nIndex)
        {
            // ----------------------------
            // draw border
            if (this.ShowTabBorders)
            {
                using (GraphicsPath tabPageBorderPath = this.GetTabPageBorder(nIndex))
                {
                    using (var br = this.GetPageBackgroundBrush(nIndex))
                    {
                        g.FillPath(br, tabPageBorderPath);
                    }

                    this.PaintTab(tabPage, nIndex, g);

                    // Draw tab border
                    using (var borderPen = new Pen(ControlPaint.Dark(tabPage.BackColor)))
                    {
                        g.DrawPath(borderPen, tabPageBorderPath);
                    }
                }
            }

            bool bSelected = this.SelectedIndex == nIndex;

            Rectangle recBounds = this.GetTabRect(nIndex);
            RectangleF tabTextArea = this.GetTabRect(nIndex);
            var closerRect = this.GetCloserRect(nIndex);
            var refresherRect = this.GetReloaderRect(nIndex);

            if (this.UseTabCloser)
            {
                tabTextArea.Width -= closerRect.Width;
            }

            if (this.UseTabReloader)
            {
                tabTextArea.Width -= refresherRect.Width;
            }

            tabTextArea.Width -= _overlap + _margin;

            // ----------------------------
            // draw tab's icon            
            bool hasIcon = tabPage.ImageIndex >= 0 && this.ImageList != null && tabPage.ImageIndex < this.ImageList.Images.Count;
            if (hasIcon)
            {
                var tabImage = this.ImageList.Images[tabPage.ImageIndex];
                if (tabImage != null)
                {
                    using (tabImage)
                    {
                        int nLeftMargin = 14 + (bSelected ? 2 : 0);
                        const int nRightMargin = 2;
                        int nWidth = tabImage.Width - (bSelected ? 0 : 2);
                        int nHeight = tabImage.Height - (bSelected ? 0 : 2);

                        Rectangle rimage = new Rectangle(recBounds.X + nLeftMargin, recBounds.Y + 1, nWidth, nHeight);

                        // adjust rectangles
                        float nAdj = nLeftMargin + tabImage.Width + nRightMargin;

                        rimage.Y += (recBounds.Height - tabImage.Height) / 2;
                        tabTextArea.X += nAdj;
                        tabTextArea.Width -= nAdj;

                        // draw icon
                        g.DrawImage(tabImage, rimage);
                    }
                }
            }
            else
            {
                tabTextArea.Width += (this.UseTabCloser ? closerRect.Width : 0) + (this.UseTabReloader ? refresherRect.Width : 0);
            }

            // ----------------------------

            // ----------------------------
            // draw string
            using (var stringFormat = new StringFormat())
            {
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                using (var br = new SolidBrush(tabPage.ForeColor))
                {
                    using (var tabFont = new Font(this.Font, bSelected ? FontStyle.Bold : FontStyle.Regular))
                    {
                        g.DrawString(tabPage.Text, tabFont, br, tabTextArea, stringFormat);
                    }
                }
            }

            // 	Draw the closer
            this.DrawCloser(nIndex, g);
            this.DrawRefresher(nIndex, g);
        }

        /// <summary>
        /// The get tab background brush.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Brush"/>.
        /// </returns>
        private Brush GetTabBackgroundBrush(int index)
        {
            var curTab = this.TabPages[index];

            // 	Capture the colours dependant on selection RequestState of the tab
            Color brushColor = Color.Empty;

            if (this.SelectedIndex == index)
            {
                brushColor = Color.White;
            }
            else if (!curTab.Enabled)
            {
                brushColor = SystemColors.ControlDark;
            }
            else if (this.HotTrack && index == this.ActiveIndex)
            {
                var scRect = this.GetUpDownRect();
                var mousePos = this.PointToClient(MousePosition);
                bool overUpDown = scRect.Contains(mousePos);
                if (!overUpDown)
                {
                    brushColor = Color.FromArgb(40, curTab.BackColor);
                }
            }
            else
            {
                brushColor = Color.FromArgb(10, curTab.BackColor);
            }

            return new SolidBrush(brushColor);
        }

        /// <summary>
        /// The add tab border.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="tabBounds">
        /// The tab bounds.
        /// </param>
        private void AddTabBorder(GraphicsPath path, Rectangle tabBounds)
        {
            int spread;
            int eigth;
            int sixth;
            int quarter;

            if (this.Alignment <= TabAlignment.Bottom)
            {
                spread = (int)Math.Floor((decimal)tabBounds.Height * 2 / 3);
                eigth = (int)Math.Floor((decimal)tabBounds.Height * 1 / 8);
                sixth = (int)Math.Floor((decimal)tabBounds.Height * 1 / 6);
                quarter = (int)Math.Floor((decimal)tabBounds.Height * 1 / 4);
            }
            else
            {
                spread = (int)Math.Floor((decimal)tabBounds.Width * 2 / 3);
                eigth = (int)Math.Floor((decimal)tabBounds.Width * 1 / 8);
                sixth = (int)Math.Floor((decimal)tabBounds.Width * 1 / 6);
                quarter = (int)Math.Floor((decimal)tabBounds.Width * 1 / 4);
            }

            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.X, tabBounds.Bottom), new Point(tabBounds.X + sixth, tabBounds.Bottom - eigth), 
                                new Point(tabBounds.X + spread - quarter, tabBounds.Y + eigth), new Point(tabBounds.X + spread, tabBounds.Y)
                            });
                    path.AddLine(tabBounds.X + spread, tabBounds.Y, tabBounds.Right - spread, tabBounds.Y);
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.Right - spread, tabBounds.Y), new Point(tabBounds.Right - spread + quarter, tabBounds.Y + eigth), 
                                new Point(tabBounds.Right - sixth, tabBounds.Bottom - eigth), new Point(tabBounds.Right, tabBounds.Bottom)
                            });
                    break;
                case TabAlignment.Bottom:
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.Right, tabBounds.Y), new Point(tabBounds.Right - sixth, tabBounds.Y + eigth), 
                                new Point(tabBounds.Right - spread + quarter, tabBounds.Bottom - eigth), 
                                new Point(tabBounds.Right - spread, tabBounds.Bottom)
                            });
                    path.AddLine(tabBounds.Right - spread, tabBounds.Bottom, tabBounds.X + spread, tabBounds.Bottom);
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.X + spread, tabBounds.Bottom), new Point(tabBounds.X + spread - quarter, tabBounds.Bottom - eigth), 
                                new Point(tabBounds.X + sixth, tabBounds.Y + eigth), new Point(tabBounds.X, tabBounds.Y)
                            });
                    break;
                case TabAlignment.Left:
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.Right, tabBounds.Bottom), new Point(tabBounds.Right - eigth, tabBounds.Bottom - sixth), 
                                new Point(tabBounds.X + eigth, tabBounds.Bottom - spread + quarter), new Point(tabBounds.X, tabBounds.Bottom - spread)
                            });
                    path.AddLine(tabBounds.X, tabBounds.Bottom - spread, tabBounds.X, tabBounds.Y + spread);
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.X, tabBounds.Y + spread), new Point(tabBounds.X + eigth, tabBounds.Y + spread - quarter), 
                                new Point(tabBounds.Right - eigth, tabBounds.Y + sixth), new Point(tabBounds.Right, tabBounds.Y)
                            });

                    break;
                case TabAlignment.Right:
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.X, tabBounds.Y), new Point(tabBounds.X + eigth, tabBounds.Y + sixth), 
                                new Point(tabBounds.Right - eigth, tabBounds.Y + spread - quarter), new Point(tabBounds.Right, tabBounds.Y + spread)
                            });
                    path.AddLine(tabBounds.Right, tabBounds.Y + spread, tabBounds.Right, tabBounds.Bottom - spread);
                    path.AddCurve(
                        new[]
                            {
                                new Point(tabBounds.Right, tabBounds.Bottom - spread), 
                                new Point(tabBounds.Right - eigth, tabBounds.Bottom - spread + quarter), 
                                new Point(tabBounds.X + eigth, tabBounds.Bottom - sixth), new Point(tabBounds.X, tabBounds.Bottom)
                            });
                    break;
            }
        }

        /// <summary>
        /// The draw tab focus indicator.
        /// </summary>
        /// <param name="brush">
        /// The brush.
        /// </param>
        /// <param name="tabpath">
        /// The tabpath.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        private void DrawTabFocusIndicator(Brush brush, GraphicsPath tabpath, int index, Graphics graphics)
        {
            if (this.Focused && index == this.SelectedIndex)
            {
                RectangleF pathRect = tabpath.GetBounds();
                Rectangle focusRect = Rectangle.Empty;
                switch (this.Alignment)
                {
                    case TabAlignment.Top:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, (int)pathRect.Width, this.IndicatorWidth);
                        break;
                    case TabAlignment.Bottom:
                        focusRect = new Rectangle(
                            (int)pathRect.X, 
                            (int)pathRect.Bottom - this.IndicatorWidth, 
                            (int)pathRect.Width, 
                            this.IndicatorWidth);
                        break;
                    case TabAlignment.Left:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, this.IndicatorWidth, (int)pathRect.Height);
                        break;
                    case TabAlignment.Right:
                        focusRect = new Rectangle(
                            (int)pathRect.Right - this.IndicatorWidth, 
                            (int)pathRect.Y, 
                            this.IndicatorWidth, 
                            (int)pathRect.Height);
                        break;
                }

                // 	Ensure the focus stip does not go outside the tab
                using (var focusRegion = new Region(focusRect))
                {
                    focusRegion.Intersect(tabpath);
                    graphics.FillRegion(brush, focusRegion);
                }
            }
        }

        /// <summary>
        /// The get tab border.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="GraphicsPath"/>.
        /// </returns>
        private GraphicsPath GetTabBorder(int index)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle tabBounds = this.GetTabRect(index);

            this.AddTabBorder(path, tabBounds);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// The get tab rect.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        public new virtual Rectangle GetTabRect(int index)
        {
            if (index < 0)
            {
                return Rectangle.Empty;
            }

            Rectangle tabBounds = base.GetTabRect(index);
            if (this.RightToLeftLayout)
            {
                tabBounds.X = this.Width - tabBounds.Right;
            }

            // 	Expand to overlap the tabpage
            switch (this.Alignment)
            {
                case TabAlignment.Top:
                    tabBounds.Height += 2;
                    break;
                case TabAlignment.Bottom:
                    tabBounds.Height += 2;
                    tabBounds.Y -= 2;
                    break;
                case TabAlignment.Left:
                    tabBounds.Width += 2;
                    break;
                case TabAlignment.Right:
                    tabBounds.X -= 2;
                    tabBounds.Width += 2;
                    break;
            }

            // 	Create Overlap unless first tab in the row to align with tabpage
            if (_overlap > 0)
            {
                if (this.Alignment <= TabAlignment.Bottom)
                {
                    tabBounds.Width += _overlap;
                }
                else
                {
                    tabBounds.Y -= _overlap;
                    tabBounds.Height += _overlap;
                }
            }

            int selIndex = this.SelectedIndex;
            if (index != selIndex)
            {
                // Lower unselected tab, but keep the same height so that it does not shrink
                tabBounds.Y += this._inactiveTabDiff;
            }

            if (this._isDragging)
            {
                if (index == selIndex)
                {
                    // Shift the dragging tab
                    tabBounds.X += this._dragOffset;
                }
            }

            return tabBounds;
        }

        /// <summary>
        /// The get closer rect.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        private Rectangle GetCloserRect(int index)
        {
            Rectangle closerRect;
            var rect = this.GetTabRect(index);

            // 	Make it shorter or thinner to fit the height or width because of the padding added to the tab for painting
            int closerSize = rect.Height - 12;
            if (this.Alignment <= TabAlignment.Bottom)
            {
                // Top or Bottom
                int offsetX = this.Padding.X + closerSize;
                if (this.RightToLeftLayout)
                {
                    closerRect = new Rectangle(
                        rect.Left + offsetX, 
                        rect.Y + (int)Math.Floor((double)(rect.Height - closerSize) / 2), 
                        closerSize, 
                        closerSize);
                }
                else
                {
                    closerRect = new Rectangle(
                        rect.Right - offsetX, 
                        rect.Y + (int)Math.Floor((double)(rect.Height - closerSize) / 2), 
                        closerSize, 
                        closerSize);
                }
            }
            else
            {
                int offsetY = this.Padding.Y + closerSize;
                if (this.RightToLeftLayout)
                {
                    closerRect = new Rectangle(
                        rect.X + (int)Math.Floor((double)(rect.Width - closerSize) / 2), 
                        rect.Top + offsetY, 
                        closerSize, 
                        closerSize);
                }
                else
                {
                    closerRect = new Rectangle(
                        rect.X + (int)Math.Floor((double)(rect.Width - closerSize) / 2), 
                        rect.Bottom - offsetY, 
                        closerSize, 
                        closerSize);
                }
            }

            if (index != this.SelectedIndex)
            {
                int diff = this._inactiveTabDiff / 2;
                closerRect.Height -= diff;
                closerRect.Width -= diff;
                closerRect.Y -= diff / 2;
            }

            return closerRect;
        }

        /// <summary>
        /// The get closer path.
        /// </summary>
        /// <param name="closerRect">
        /// The closer rect.
        /// </param>
        /// <returns>
        /// The <see cref="GraphicsPath"/>.
        /// </returns>
        private static GraphicsPath GetCloserPath(Rectangle closerRect)
        {
            GraphicsPath closerPath = new GraphicsPath();
            int offset = 4;
            var crossRect = new Rectangle(closerRect.X + offset, closerRect.Y + offset, closerRect.Width - 2 * offset, closerRect.Height - 2 * offset);
            closerPath.AddLine(crossRect.X, crossRect.Y, crossRect.Right, crossRect.Bottom);
            closerPath.CloseFigure();
            closerPath.AddLine(crossRect.Right, crossRect.Y, crossRect.X, crossRect.Bottom);
            closerPath.CloseFigure();

            return closerPath;
        }

        /// <summary>
        /// The draw closer.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        private void DrawCloser(int index, Graphics graphics)
        {
            if (!this.UseTabCloser)
            {
                return;
            }

            Rectangle closerRect = this.GetCloserRect(index);
            var curMouse = this.PointToClient(MousePosition);
            bool mouseOverCloser = closerRect.Contains(curMouse);

            using (GraphicsPath closerPath = GetCloserPath(closerRect))
            {
                if (mouseOverCloser)
                {
                    var oldMode = graphics.SmoothingMode;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.FillEllipse(Brushes.DarkSalmon, closerRect);
                    graphics.SmoothingMode = oldMode;
                }

                Color closerColor = mouseOverCloser ? Color.White : Color.LightSlateGray;
                using (var closerPen = new Pen(closerColor, 2))
                {
                    graphics.DrawPath(closerPen, closerPath);
                }
            }
        }

        /// <summary>
        /// The get reloader rect.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Rectangle"/>.
        /// </returns>
        private Rectangle GetReloaderRect(int index)
        {
            var refresherRect = this.GetCloserRect(index);

            if (this.UseTabCloser)
            {
                refresherRect.X -= refresherRect.Width;
            }

            if (index != this.SelectedIndex)
            {
                int diff = this._inactiveTabDiff / 2;
                refresherRect.Height -= diff;
                refresherRect.Width -= diff;
                refresherRect.Y -= diff / 2;
            }

            return refresherRect;
        }

        /// <summary>
        /// The draw refresher.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="graphics">
        /// The graphics.
        /// </param>
        private void DrawRefresher(int index, Graphics graphics)
        {
            if (!this.UseTabReloader)
            {
                return;
            }

            var refresherRect = this.GetReloaderRect(index);

            var curMouse = this.PointToClient(MousePosition);
            bool mouseOverRefresher = refresherRect.Contains(curMouse);
            if (mouseOverRefresher)
            {
                var oldMode = graphics.SmoothingMode;
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.FillEllipse(Brushes.DeepSkyBlue, refresherRect);
                graphics.SmoothingMode = oldMode;
            }

            Image img = mouseOverRefresher ? Resources.Refresh_White : Resources.Refresh;

            // draw icon
            graphics.DrawImage(img, refresherRect);
        }
    }
}