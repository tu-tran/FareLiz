namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Forms.VisualStyles;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>CodeProject.com "Simple pop-up control" "http://www.codeproject.com/cs/miscctrl/simplepopup.asp". Represents a pop-up window.</summary>
    [ToolboxItem(false)]
    public partial class Popup : ToolStripDropDown
    {
        #region " Constructors "

        /// <summary>
        /// Initializes a new instance of the <see cref="PopupControl.Popup"/> class.
        /// </summary>
        /// <param name="content">
        /// The content of the pop-up.
        /// </param>
        /// <remarks>
        /// Pop-up will be disposed immediately after disposion of the content control.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="content"/> is
        /// <code>
        /// null
        /// </code>
        /// .
        /// </exception>
        public Popup(Control content)
        {
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this.content = content;
            this.fade = SystemInformation.IsMenuAnimationEnabled && SystemInformation.IsMenuFadeEnabled;
            this._resizable = true;
            this.InitializeComponent();
            this.AutoSize = false;
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
            this.host = new ToolStripControlHost(content);
            this.Padding = this.Margin = this.host.Padding = this.host.Margin = Padding.Empty;
            this.MinimumSize = content.MinimumSize;
            content.MinimumSize = content.Size;
            this.MaximumSize = content.MaximumSize;
            content.MaximumSize = content.Size;
            this.Size = content.Size;
            content.Location = Point.Empty;
            this.Items.Add(this.host);
            content.Disposed += delegate
                {
                    content = null;
                    this.Dispose(true);
                };
            content.RegionChanged += delegate { this.UpdateRegion(); };
            content.Paint += delegate(object sender, PaintEventArgs e) { this.PaintSizeGrip(e); };
            this.UpdateRegion();
        }

        #endregion

        #region " Fields & Properties "

        /// <summary>The host.</summary>
        private readonly ToolStripControlHost host;

        /// <summary>The _resizable.</summary>
        private bool _resizable;

        /// <summary>The accept alt.</summary>
        private bool acceptAlt = true;

        /// <summary>The child popup.</summary>
        private Popup childPopup;

        /// <summary>The content.</summary>
        private Control content;

        /// <summary>The fade.</summary>
        private bool fade;

        /// <summary>The focus on open.</summary>
        private bool focusOnOpen = true;

        /// <summary>The owner popup.</summary>
        private Popup ownerPopup;

        /// <summary>The resizable.</summary>
        private bool resizable;

        /// <summary>Gets the content of the pop-up.</summary>
        public Control Content
        {
            get
            {
                return this.content;
            }
        }

        /// <summary>Gets a value indicating whether the <see cref="PopupControl.Popup" /> uses the fade effect.</summary>
        /// <value>
        /// <c>true</c> if pop-up uses the fade effect; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>To use the fade effect, the FocusOnOpen property also has to be set to <c>true</c>.</remarks>
        public bool UseFadeEffect
        {
            get
            {
                return this.fade;
            }

            set
            {
                if (this.fade == value)
                {
                    return;
                }

                this.fade = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether to focus the content after the pop-up has been opened.</summary>
        /// <value>
        /// <c>true</c> if the content should be focused after the pop-up has been opened; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>If the FocusOnOpen property is set to <c>false</c>, then pop-up cannot use the fade effect.</remarks>
        public bool FocusOnOpen
        {
            get
            {
                return this.focusOnOpen;
            }

            set
            {
                this.focusOnOpen = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether presing the alt key should close the pop-up.</summary>
        /// <value>
        /// <c>true</c> if presing the alt key does not close the pop-up; otherwise, <c>false</c>.
        /// </value>
        public bool AcceptAlt
        {
            get
            {
                return this.acceptAlt;
            }

            set
            {
                this.acceptAlt = value;
            }
        }

        /// <summary>Gets or sets a value indicating whether this <see cref="PopupControl.Popup" /> is resizable.</summary>
        /// <value>
        /// <c>true</c> if resizable; otherwise, <c>false</c>.
        /// </value>
        public bool Resizable
        {
            get
            {
                return this.resizable && this._resizable;
            }

            set
            {
                this.resizable = value;
            }
        }

        /// <summary>
        /// Gets or sets the size that is the lower limit that
        /// <see
        ///     cref="M:System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)" />
        /// can specify.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
        public new Size MinimumSize { get; set; }

        /// <summary>
        /// Gets or sets the size that is the upper limit that
        /// <see
        ///     cref="M:System.Windows.Forms.Control.GetPreferredSize(System.Drawing.Size)" />
        /// can specify.
        /// </summary>
        /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size" /> representing the width and height of a rectangle.</returns>
        public new Size MaximumSize { get; set; }

        /// <summary>Gets parameters of a new window.</summary>
        /// <returns>An object of type <see cref="T:System.Windows.Forms.CreateParams" /> used when creating a new window.</returns>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_NOACTIVATE;
                return cp;
            }
        }

        #endregion

        #region " Methods "

        /// <summary>The frames.</summary>
        private const int frames = 1;

        /// <summary>The totalduration.</summary>
        private const int totalduration = 0; // ML : 2007-11-05 : was 100 but caused a flicker.

        /// <summary>The frameduration.</summary>
        private const int frameduration = totalduration / frames;

        /// <summary>The last closed time stamp.</summary>
        public DateTime LastClosedTimeStamp = DateTime.Now;

        /// <summary>The resizable right.</summary>
        private bool resizableRight;

        /// <summary>The resizable top.</summary>
        private bool resizableTop;

        /// <summary>
        /// Processes a dialog box key.
        /// </summary>
        /// <param name="keyData">
        /// One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.
        /// </param>
        /// <returns>
        /// true if the key was processed by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (this.acceptAlt && ((keyData & Keys.Alt) == Keys.Alt))
            {
                return false;
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>Updates the pop-up region.</summary>
        protected void UpdateRegion()
        {
            if (this.Region != null)
            {
                this.Region.Dispose();
                this.Region = null;
            }

            if (this.content.Region != null)
            {
                this.Region = this.content.Region.Clone();
            }
        }

        /// <summary>
        /// Shows pop-up window below the specified control.
        /// </summary>
        /// <param name="control">
        /// The control below which the pop-up will be shown.
        /// </param>
        /// <remarks>
        /// When there is no space below the specified control, the pop-up control is shown above it.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control"/> is
        /// <code>
        /// null
        /// </code>
        /// .
        /// </exception>
        public void Show(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            this.SetOwnerItem(control);
            this.Show(control, control.ClientRectangle);
        }

        /// <summary>
        /// Shows pop-up window below the specified area of specified control.
        /// </summary>
        /// <param name="control">
        /// The control used to compute screen location of specified area.
        /// </param>
        /// <param name="area">
        /// The area of control below which the pop-up will be shown.
        /// </param>
        /// <remarks>
        /// When there is no space below specified area, the pop-up control is shown above it.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="control"/> is
        /// <code>
        /// null
        /// </code>
        /// .
        /// </exception>
        public void Show(Control control, Rectangle area)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            this.SetOwnerItem(control);
            this.resizableTop = this.resizableRight = false;
            var location = control.PointToScreen(new Point(area.Left, area.Top + area.Height));
            var screen = Screen.FromControl(control).WorkingArea;
            if (location.X + this.Size.Width > (screen.Left + screen.Width))
            {
                this.resizableRight = true;
                location.X = (screen.Left + screen.Width) - this.Size.Width;
            }

            if (location.Y + this.Size.Height > (screen.Top + screen.Height))
            {
                this.resizableTop = true;
                location.Y -= this.Size.Height + area.Height;
            }

            location = control.PointToClient(location);
            this.Show(control, location, ToolStripDropDownDirection.BelowRight);
        }

        /// <summary>
        /// Adjusts the size of the owner <see cref="T:System.Windows.Forms.ToolStrip"/> to accommodate the
        /// <see cref="T:System.Windows.Forms.ToolStripDropDown"/>
        /// if the owner <see cref="T:System.Windows.Forms.ToolStrip"/> is currently displayed, or clears and resets active
        /// <see cref="T:System.Windows.Forms.ToolStripDropDown"/>
        /// child controls of the
        /// <see cref="T:System.Windows.Forms.ToolStrip"/>
        /// if the <see cref="T:System.Windows.Forms.ToolStrip"/> is not currently displayed.
        /// </summary>
        /// <param name="visible">
        /// true if the owner <see cref="T:System.Windows.Forms.ToolStrip"/> is currently displayed; otherwise, false.
        /// </param>
        protected override void SetVisibleCore(bool visible)
        {
            var opacity = this.Opacity;
            if (visible && this.fade && this.focusOnOpen)
            {
                this.Opacity = 0;
            }

            base.SetVisibleCore(visible);
            if (!visible || !this.fade || !this.focusOnOpen)
            {
                return;
            }

            for (var i = 1; i <= frames; i++)
            {
                if (i > 1)
                {
                    Thread.Sleep(frameduration);
                }

                this.Opacity = opacity * i / frames;
            }

            this.Opacity = opacity;
        }

        /// <summary>
        /// The set owner item.
        /// </summary>
        /// <param name="control">
        /// The control.
        /// </param>
        private void SetOwnerItem(Control control)
        {
            if (control == null)
            {
                return;
            }

            if (control is Popup)
            {
                var popupControl = control as Popup;
                this.ownerPopup = popupControl;
                this.ownerPopup.childPopup = this;
                this.OwnerItem = popupControl.Items[0];
                return;
            }

            if (control.Parent != null)
            {
                this.SetOwnerItem(control.Parent);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnSizeChanged(EventArgs e)
        {
            this.content.MinimumSize = this.Size;
            this.content.MaximumSize = this.Size;
            this.content.Size = this.Size;
            this.content.Location = Point.Empty;
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Opening"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.
        /// </param>
        protected override void OnOpening(CancelEventArgs e)
        {
            if (this.content.IsDestructed())
            {
                e.Cancel = true;
                return;
            }

            this.UpdateRegion();
            base.OnOpening(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.ToolStripDropDown.Opened"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnOpened(EventArgs e)
        {
            if (this.ownerPopup != null)
            {
                this.ownerPopup._resizable = false;
            }

            if (this.focusOnOpen)
            {
                this.content.Focus();
            }

            base.OnOpened(e);
        }

        /// <summary>
        /// The on closed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
        {
            if (this.ownerPopup != null)
            {
                this.ownerPopup._resizable = true;
            }

            base.OnClosed(e);
        }

        /// <summary>
        /// The on visible changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            if (this.Visible == false)
            {
                this.LastClosedTimeStamp = DateTime.Now;
            }

            base.OnVisibleChanged(e);
        }

        #endregion

        #region " Resizing Support "

        /// <summary>The size grip renderer.</summary>
        private VisualStyleRenderer sizeGripRenderer;

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">
        /// The Windows <see cref="T:System.Windows.Forms.Message"/> to process.
        /// </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (this.InternalProcessResizing(ref m, false))
            {
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Processes the resizing messages.
        /// </summary>
        /// <param name="m">
        /// The message.
        /// </param>
        /// <returns>
        /// true, if the WndProc method from the base class shouldn't be invoked.
        /// </returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public bool ProcessResizing(ref Message m)
        {
            return this.InternalProcessResizing(ref m, true);
        }

        /// <summary>
        /// The internal process resizing.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <param name="contentControl">
        /// The content control.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool InternalProcessResizing(ref Message m, bool contentControl)
        {
            if (m.Msg == (int)W32_WM.WM_NCACTIVATE && m.WParam != IntPtr.Zero && this.childPopup != null && this.childPopup.Visible)
            {
                this.childPopup.Hide();
            }

            if (!this.Resizable)
            {
                return false;
            }

            if (m.Msg == (int)W32_WM.WM_NCHITTEST)
            {
                return this.OnNcHitTest(ref m, contentControl);
            }

            if (m.Msg == (int)W32_WM.WM_GETMINMAXINFO)
            {
                return this.OnGetMinMaxInfo(ref m);
            }

            return false;
        }

        /// <summary>
        /// The on get min max info.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private bool OnGetMinMaxInfo(ref Message m)
        {
            var minmax = (MINMAXINFO)Marshal.PtrToStructure(m.LParam, typeof(MINMAXINFO));
            minmax.maxTrackSize = this.MaximumSize;
            minmax.minTrackSize = this.MinimumSize;
            Marshal.StructureToPtr(minmax, m.LParam, false);
            return true;
        }

        /// <summary>
        /// The on nc hit test.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        /// <param name="contentControl">
        /// The content control.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool OnNcHitTest(ref Message m, bool contentControl)
        {
            var x = NativeMethods.LoWord(m.LParam);
            var y = NativeMethods.HiWord(m.LParam);
            var clientLocation = this.PointToClient(new Point(x, y));

            var gripBouns = new GripBounds(contentControl ? this.content.ClientRectangle : this.ClientRectangle);
            var transparent = new IntPtr(NativeMethods.HTTRANSPARENT);

            if (this.resizableTop)
            {
                if (this.resizableRight && gripBouns.TopLeft.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPLEFT;
                    return true;
                }

                if (!this.resizableRight && gripBouns.TopRight.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOPRIGHT;
                    return true;
                }

                if (gripBouns.Top.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTTOP;
                    return true;
                }
            }
            else
            {
                if (this.resizableRight && gripBouns.BottomLeft.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMLEFT;
                    return true;
                }

                if (!this.resizableRight && gripBouns.BottomRight.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                    return true;
                }

                if (gripBouns.Bottom.Contains(clientLocation))
                {
                    m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTBOTTOM;
                    return true;
                }
            }

            if (this.resizableRight && gripBouns.Left.Contains(clientLocation))
            {
                m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTLEFT;
                return true;
            }

            if (!this.resizableRight && gripBouns.Right.Contains(clientLocation))
            {
                m.Result = contentControl ? transparent : (IntPtr)NativeMethods.HTRIGHT;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Paints the size grip.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.
        /// </param>
        public void PaintSizeGrip(PaintEventArgs e)
        {
            if (e == null || e.Graphics == null || !this.resizable)
            {
                return;
            }

            var clientSize = this.content.ClientSize;
            if (Application.RenderWithVisualStyles)
            {
                if (this.sizeGripRenderer == null)
                {
                    this.sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                }

                this.sizeGripRenderer.DrawBackground(e.Graphics, new Rectangle(clientSize.Width - 0x10, clientSize.Height - 0x10, 0x10, 0x10));
            }
            else
            {
                ControlPaint.DrawSizeGrip(e.Graphics, this.content.BackColor, clientSize.Width - 0x10, clientSize.Height - 0x10, 0x10, 0x10);
            }
        }

        #endregion
    }
}