namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    partial class FlatTabControl
    {
        private SubClass scUpDown = null;

        private void FindUpDown()
        {
            bool bFound = false;

            // find the UpDown control
            IntPtr pWnd = NativeMethods.GetWindow(this.Handle, NativeMethods.GW_CHILD);

            while (pWnd != IntPtr.Zero)
            {
                //----------------------------
                // Get the window class name
                char[] className = new char[33];
                int length = NativeMethods.GetClassName(pWnd, className, 32);
                string s = new string(className, 0, length);
                //----------------------------

                if (s == "msctls_updown32")
                {
                    bFound = true;

                    if (!this._bUpDown)
                    {
                        //----------------------------
                        // Subclass it
                        this.scUpDown = new SubClass(pWnd, true);
                        this.scUpDown.SubClassedWndProc += new SubClass.SubClassWndProcEventHandler(this.scUpDown_SubClassedWndProc);
                        //----------------------------

                        this._bUpDown = true;
                    }
                    break;
                }

                pWnd = NativeMethods.GetWindow(pWnd, NativeMethods.GW_HWNDNEXT);
            }

            if ((!bFound) && (this._bUpDown))
                this._bUpDown = false;
        }

        private void UpdateUpDown()
        {
            if (this._bUpDown)
            {
                if (NativeMethods.IsWindowVisible(this.scUpDown.Handle))
                {
                    Rectangle rect = new Rectangle();
                    NativeMethods.GetClientRect(this.scUpDown.Handle, ref rect);
                    NativeMethods.InvalidateRect(this.scUpDown.Handle, ref rect, true);
                }
            }
        }

        private Rectangle GetUpDownRect()
        {
            if (!this._bUpDown)
                return Rectangle.Empty;

            Rectangle pageBound = this.GetPageBounds(this.TabCount - 1);
            Rectangle scRect = new Rectangle();
            NativeMethods.GetClientRect(this.scUpDown.Handle, ref scRect);
            Rectangle relRect = new Rectangle(this.ClientRectangle.Width - scRect.Width, pageBound.Y - scRect.Y - scRect.Height - 1, scRect.Width, scRect.Height);
            return relRect;
        }

        private void DrawUpDown(Graphics g)
        {
            if (this.TabCount < 1 || (this.leftRightImages == null) || (this.leftRightImages.Images.Count != 4))
                return;

            //----------------------------
            // calc positions
            Rectangle TabControlArea = this.ClientRectangle;
            Rectangle scRect = new Rectangle();
            NativeMethods.GetClientRect(this.scUpDown.Handle, ref scRect);

            g.FillRectangle(SystemBrushes.ControlLight, scRect);
            using (var p = PaintUtils.GetRoundedRectanglePath(scRect, 3, 1))
            {
                g.DrawPath(SystemPens.ControlDark, p);
            }

            int imgWidth = this.leftRightImages.ImageSize.Width, imgHeight = this.leftRightImages.ImageSize.Height;
            int nMiddle = (scRect.Width) / 2;
            int nTop = (scRect.Height - this.leftRightImages.ImageSize.Height) / 2;
            int nLeft = (nMiddle - this.leftRightImages.ImageSize.Width) / 2;
            //----------------------------

            //----------------------------
            // draw scroll button
            Image img = null;
            Rectangle firstTabRect = this.GetTabRect(0);
            Rectangle leftButtonRect = new Rectangle(nLeft, nTop, imgWidth, imgHeight);
            if (firstTabRect.Left < TabControlArea.Left || firstTabRect.Width < 0)
                img = this.leftRightImages.Images[1];  // Left (Enabled) image
            else
                img = this.leftRightImages.Images[3];

            if (img != null)
                using (img)
                    g.DrawImage(img, leftButtonRect);

            Rectangle lastTabRect = this.GetTabRect(this.TabCount - 1);
            Rectangle rightButtonRect = new Rectangle(nMiddle + nLeft, nTop, imgWidth, imgHeight);
            if (lastTabRect.Right > (TabControlArea.Width - scRect.Width))
                img = this.leftRightImages.Images[0];  // Right (Enabled) image
            else
                img = this.leftRightImages.Images[2];

            if (img != null)
                using (img)
                    g.DrawImage(img, rightButtonRect);
            //----------------------------
        }

        private int scUpDown_SubClassedWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)W32_WM.WM_PAINT:
                    {
                        //------------------------
                        // redraw
                        IntPtr hDC = NativeMethods.GetWindowDC(this.scUpDown.Handle);
                        using (var g = Graphics.FromHdc(hDC))
                        {
                            this.DrawUpDown(g);
                        }
                        NativeMethods.ReleaseDC(this.scUpDown.Handle, hDC);
                        //------------------------

                        // return 0 (processed)
                        m.Result = IntPtr.Zero;

                        //------------------------
                        // validate current rect
                        Rectangle rect = new Rectangle();

                        NativeMethods.GetClientRect(this.scUpDown.Handle, ref rect);
                        NativeMethods.ValidateRect(this.scUpDown.Handle, ref rect);
                        //------------------------
                    }
                    return 1;
            }

            return 0;
        }
    }
}
