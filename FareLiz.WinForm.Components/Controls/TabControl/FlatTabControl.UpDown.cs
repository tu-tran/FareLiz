namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The flat tab control.</summary>
    partial class FlatTabControl
    {
        /// <summary>The sc up down.</summary>
        private SubClass scUpDown;

        /// <summary>The find up down.</summary>
        private void FindUpDown()
        {
            var bFound = false;

            // find the UpDown control
            var pWnd = NativeMethods.GetWindow(this.Handle, NativeMethods.GW_CHILD);

            while (pWnd != IntPtr.Zero)
            {
                // ----------------------------
                // Get the window class name
                var className = new char[33];
                var length = NativeMethods.GetClassName(pWnd, className, 32);
                var s = new string(className, 0, length);

                // ----------------------------
                if (s == "msctls_updown32")
                {
                    bFound = true;

                    if (!this._bUpDown)
                    {
                        // ----------------------------
                        // Subclass it
                        this.scUpDown = new SubClass(pWnd, true);
                        this.scUpDown.SubClassedWndProc += this.scUpDown_SubClassedWndProc;

                        // ----------------------------
                        this._bUpDown = true;
                    }

                    break;
                }

                pWnd = NativeMethods.GetWindow(pWnd, NativeMethods.GW_HWNDNEXT);
            }

            if ((!bFound) && this._bUpDown)
            {
                this._bUpDown = false;
            }
        }

        /// <summary>The update up down.</summary>
        private void UpdateUpDown()
        {
            if (this._bUpDown)
            {
                if (NativeMethods.IsWindowVisible(this.scUpDown.Handle))
                {
                    var rect = new Rectangle();
                    NativeMethods.GetClientRect(this.scUpDown.Handle, ref rect);
                    NativeMethods.InvalidateRect(this.scUpDown.Handle, ref rect, true);
                }
            }
        }

        /// <summary>The get up down rect.</summary>
        /// <returns>The <see cref="Rectangle" />.</returns>
        private Rectangle GetUpDownRect()
        {
            if (!this._bUpDown)
            {
                return Rectangle.Empty;
            }

            var pageBound = this.GetPageBounds(this.TabCount - 1);
            var scRect = new Rectangle();
            NativeMethods.GetClientRect(this.scUpDown.Handle, ref scRect);
            var relRect = new Rectangle(
                this.ClientRectangle.Width - scRect.Width, 
                pageBound.Y - scRect.Y - scRect.Height - 1, 
                scRect.Width, 
                scRect.Height);
            return relRect;
        }

        /// <summary>
        /// The draw up down.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        private void DrawUpDown(Graphics g)
        {
            if (this.TabCount < 1 || (this.leftRightImages == null) || (this.leftRightImages.Images.Count != 4))
            {
                return;
            }

            // ----------------------------
            // calc positions
            var TabControlArea = this.ClientRectangle;
            var scRect = new Rectangle();
            NativeMethods.GetClientRect(this.scUpDown.Handle, ref scRect);

            g.FillRectangle(SystemBrushes.ControlLight, scRect);
            using (var p = PaintUtils.GetRoundedRectanglePath(scRect, 3, 1))
            {
                g.DrawPath(SystemPens.ControlDark, p);
            }

            int imgWidth = this.leftRightImages.ImageSize.Width, imgHeight = this.leftRightImages.ImageSize.Height;
            var nMiddle = scRect.Width / 2;
            var nTop = (scRect.Height - this.leftRightImages.ImageSize.Height) / 2;
            var nLeft = (nMiddle - this.leftRightImages.ImageSize.Width) / 2;

            // ----------------------------

            // ----------------------------
            // draw scroll button
            Image img = null;
            var firstTabRect = this.GetTabRect(0);
            var leftButtonRect = new Rectangle(nLeft, nTop, imgWidth, imgHeight);
            if (firstTabRect.Left < TabControlArea.Left || firstTabRect.Width < 0)
            {
                img = this.leftRightImages.Images[1]; // Left (Enabled) image
            }
            else
            {
                img = this.leftRightImages.Images[3];
            }

            if (img != null)
            {
                using (img) g.DrawImage(img, leftButtonRect);
            }

            var lastTabRect = this.GetTabRect(this.TabCount - 1);
            var rightButtonRect = new Rectangle(nMiddle + nLeft, nTop, imgWidth, imgHeight);
            if (lastTabRect.Right > (TabControlArea.Width - scRect.Width))
            {
                img = this.leftRightImages.Images[0]; // Right (Enabled) image
            }
            else
            {
                img = this.leftRightImages.Images[2];
            }

            if (img != null)
            {
                using (img) g.DrawImage(img, rightButtonRect);
            }

            // ----------------------------
        }

        /// <summary>
        /// The sc up down_ sub classed wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int scUpDown_SubClassedWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)W32_WM.WM_PAINT:
                    {
                        // ------------------------
                        // redraw
                        var hDC = NativeMethods.GetWindowDC(this.scUpDown.Handle);
                        using (var g = Graphics.FromHdc(hDC))
                        {
                            this.DrawUpDown(g);
                        }

                        NativeMethods.ReleaseDC(this.scUpDown.Handle, hDC);

                        // ------------------------

                        // return 0 (processed)
                        m.Result = IntPtr.Zero;

                        // ------------------------
                        // validate current rect
                        var rect = new Rectangle();

                        NativeMethods.GetClientRect(this.scUpDown.Handle, ref rect);
                        NativeMethods.ValidateRect(this.scUpDown.Handle, ref rect);

                        // ------------------------
                    }

                    return 1;
            }

            return 0;
        }
    }
}