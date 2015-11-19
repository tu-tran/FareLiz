namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>Summary description for FlatTabControl.</summary>
    [ToolboxBitmap(typeof(TabControl))] // ,
    public partial class FlatTabControl : TabControl
    {
        /// <summary>
        /// The _margin.
        /// </summary>
        private const int _margin = 5;

        /// <summary>
        /// The _overlap.
        /// </summary>
        private const int _overlap = 16;

        /// <summary>
        /// The _radius.
        /// </summary>
        private const int _radius = 16;

        /// <summary>
        /// The _b up down.
        /// </summary>
        private bool _bUpDown; // true when the button UpDown is required        

        /// <summary>
        /// Initializes a new instance of the <see cref="FlatTabControl"/> class.
        /// </summary>
        public FlatTabControl()
        {
            this.InitializeComponent();

            // double buffering
            this.SetStyle(
                ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw
                | ControlStyles.SupportsTransparentBackColor, 
                true);
        }

        /// <summary>
        /// The find index.
        /// </summary>
        /// <param name="tabPage">
        /// The tab page.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int FindIndex(TabPage tabPage)
        {
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                if (this.TabPages[i] == tabPage)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// The get active index.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="isRelativeToScreen">
        /// The is relative to screen.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetActiveIndex(Point position, bool isRelativeToScreen)
        {
            TCHITTESTINFO hitTestInfo = new TCHITTESTINFO(isRelativeToScreen ? this.PointToClient(position) : position);
            return NativeMethods.SendMessage(this.Handle, NativeMethods.TCM_HITTEST, IntPtr.Zero, NativeMethods.ToIntPtr(hitTestInfo)).ToInt32();
        }

        /// <summary>
        /// The draw control.
        /// </summary>
        /// <param name="g">
        /// The g.
        /// </param>
        private void DrawControl(Graphics g)
        {
            if (!this.Visible || !this.IsHandleCreated || this.IsDisposed || this.Disposing)
            {
                return;
            }

            g.SmoothingMode = SmoothingMode.HighSpeed;
            var tabControlArea = this.ClientRectangle;
            var tabArea = this.DisplayRectangle;

            // ----------------------------
            // fill client area
            using (var br = new SolidBrush(this.BackColor)) g.FillRectangle(br, tabControlArea);

            // ----------------------------

            // ----------------------------
            // draw border
            if (this.ShowTabBorders)
            {
                int nDelta = SystemInformation.Border3DSize.Width;
                tabArea.Inflate(nDelta, nDelta);
                g.DrawRectangle(SystemPens.ControlDark, tabArea);
            }

            // ----------------------------

            // ----------------------------
            // clip region for drawing tabs
            Region rSaved = g.Clip;
            int nWidth = tabArea.Width + _margin;
            if (this._bUpDown)
            {
                // exclude updown control for painting
                if (NativeMethods.IsWindowVisible(this.scUpDown.Handle))
                {
                    Rectangle rupdown = new Rectangle();
                    NativeMethods.GetWindowRect(this.scUpDown.Handle, ref rupdown);
                    Rectangle rupdown2 = this.RectangleToClient(rupdown);
                    nWidth = rupdown2.X;
                }
            }

            var rReg = new Rectangle(tabArea.Left, tabControlArea.Top, nWidth - _margin, tabControlArea.Height);
            g.SetClip(rReg);

            // draw tabs
            for (int i = this.TabCount - 1; i >= 0; i--)
            {
                if (i != this.SelectedIndex)
                {
                    this.DrawTab(g, this.TabPages[i], i);
                }
            }

            if (this.SelectedIndex > -1)
            {
                this.DrawTab(g, this.TabPages[this.SelectedIndex], this.SelectedIndex);
            }

            g.Clip = rSaved;

            // ----------------------------

            // ----------------------------
            // draw background to cover flat border areas
            if (this.ShowTabBorders)
            {
                if (this.SelectedTab != null)
                {
                    TabPage tabPage = this.SelectedTab;
                    Color color = tabPage.BackColor;
                    using (Pen border = new Pen(color))
                    {
                        tabArea.Offset(1, 1);
                        tabArea.Width -= 2;
                        tabArea.Height -= 2;

                        g.DrawRectangle(border, tabArea);
                        tabArea.Width -= 1;
                        tabArea.Height -= 1;
                        g.DrawRectangle(border, tabArea);
                    }
                }
            }

            // ----------------------------
        }

        /// <summary>
        /// The tabpage ex collection editor.
        /// </summary>
        internal class TabpageExCollectionEditor : CollectionEditor
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TabpageExCollectionEditor"/> class.
            /// </summary>
            /// <param name="type">
            /// The type.
            /// </param>
            public TabpageExCollectionEditor(Type type)
                : base(type)
            {
            }

            /// <summary>
            /// The create collection item type.
            /// </summary>
            /// <returns>
            /// The <see cref="Type"/>.
            /// </returns>
            protected override Type CreateCollectionItemType()
            {
                return typeof(TabPage);
            }
        }

        #region Properties

        /// <summary>
        /// Gets the active index.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ActiveIndex
        {
            get
            {
                return this.GetActiveIndex(MousePosition, true);
            }
        }

        /// <summary>
        /// Gets or sets the alignment.
        /// </summary>
        [Category("Appearance")]
        public new TabAlignment Alignment
        {
            get
            {
                return base.Alignment;
            }

            set
            {
                if (value > TabAlignment.Bottom)
                {
                    base.Alignment = TabAlignment.Top;
                }
                else
                {
                    base.Alignment = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the indicator width.
        /// </summary>
        [Category("Appearance")]
        public int IndicatorWidth
        {
            get
            {
                return this._indicatorWidth;
            }

            set
            {
                this._indicatorWidth = value;
            }
        }

        /// <summary>
        /// The _indicator width.
        /// </summary>
        private int _indicatorWidth = 3;

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        [Category("Appearance")]
        public new Point Padding
        {
            get
            {
                return base.Padding;
            }

            set
            {
                base.Padding = value;

                // 	This line will trigger the handle to recreate, therefore invalidating the control
                if (value.X + _radius / 2 < -6)
                {
                    base.Padding = new Point(0, value.Y);
                }
                else
                {
                    base.Padding = new Point(value.X + _radius / 2 + 6, value.Y);
                }
            }
        }

        /// <summary>
        /// The _show tab borders.
        /// </summary>
        private bool _showTabBorders = true;

        /// <summary>
        /// Gets or sets a value indicating whether show tab borders.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowTabBorders
        {
            get
            {
                return this._showTabBorders;
            }

            set
            {
                this._showTabBorders = value;
            }
        }

        /// <summary>
        /// The _use tab closer.
        /// </summary>
        private bool _useTabCloser = true;

        /// <summary>
        /// Gets or sets a value indicating whether use tab closer.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool UseTabCloser
        {
            get
            {
                return this._useTabCloser;
            }

            set
            {
                this._useTabCloser = value;
            }
        }

        /// <summary>
        /// The _use tab refresher.
        /// </summary>
        private bool _useTabRefresher = true;

        /// <summary>
        /// Gets or sets a value indicating whether use tab reloader.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool UseTabReloader
        {
            get
            {
                return this._useTabRefresher;
            }

            set
            {
                this._useTabRefresher = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// The tab page event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    internal delegate void TabPageEventHandler(TabPage sender, EventArgs args);
}