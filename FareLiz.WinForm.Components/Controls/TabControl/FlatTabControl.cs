namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// Summary description for FlatTabControl.
    /// </summary>
    [ToolboxBitmap(typeof(TabControl))] //,
    public partial class FlatTabControl : TabControl
    {
        private const int _margin = 5, _overlap = 16, _radius = 16;
        private bool _bUpDown = false; // true when the button UpDown is required        

        #region Properties
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public int ActiveIndex
        {
            get { return this.GetActiveIndex(MousePosition, true); }
        }

        [Category("Appearance")]
        public new TabAlignment Alignment
        {
            get { return base.Alignment; }
            set
            {
                if (value > TabAlignment.Bottom)
                    base.Alignment = TabAlignment.Top;
                else
                    base.Alignment = value;
            }
        }

        [Category("Appearance")]
        public int IndicatorWidth
        {
            get { return this._indicatorWidth; }
            set { this._indicatorWidth = value; }
        }
        private int _indicatorWidth = 3;

        [Category("Appearance")]
        public new Point Padding
        {
            get { return base.Padding; }
            set
            {
                base.Padding = value;
                //	This line will trigger the handle to recreate, therefore invalidating the control
                if (value.X + (int)(_radius / 2) < -6)
                {
                    base.Padding = new Point(0, value.Y);
                }
                else
                {
                    base.Padding = new Point(value.X + (int)(_radius / 2) + 6, value.Y);
                }
            }
        }

        private bool _showTabBorders = true;
        [Category("Appearance"), DefaultValue(true)]
        public bool ShowTabBorders
        {
            get { return this._showTabBorders; }
            set { this._showTabBorders = value; }
        }

        private bool _useTabCloser = true;
        [Category("Appearance"), DefaultValue(true)]
        public bool UseTabCloser
        {
            get { return this._useTabCloser; }
            set { this._useTabCloser = value; }
        }

        private bool _useTabRefresher = true;
        [Category("Appearance"), DefaultValue(true)]
        public bool UseTabReloader
        {
            get { return this._useTabRefresher; }
            set { this._useTabRefresher = value; }
        }
        #endregion

        public FlatTabControl()
        {
            this.InitializeComponent();
            // double buffering
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer
                | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        }

        public int FindIndex(TabPage tabPage)
        {
            for (int i = 0; i < this.TabPages.Count; i++)
            {
                if (this.TabPages[i] == tabPage)
                    return i;
            }

            return -1;
        }

        public int GetActiveIndex(Point position, bool isRelativeToScreen)
        {
            TCHITTESTINFO hitTestInfo = new TCHITTESTINFO(isRelativeToScreen ? this.PointToClient(position) : position);
            return NativeMethods.SendMessage(this.Handle, NativeMethods.TCM_HITTEST, IntPtr.Zero, NativeMethods.ToIntPtr(hitTestInfo)).ToInt32();
        }

        private void DrawControl(Graphics g)
        {
            if (!this.Visible || !this.IsHandleCreated || this.IsDisposed || this.Disposing)
                return;

            g.SmoothingMode = SmoothingMode.HighSpeed;
            var tabControlArea = this.ClientRectangle;
            var tabArea = this.DisplayRectangle;

            //----------------------------
            // fill client area
            using (var br = new SolidBrush(this.BackColor))
                g.FillRectangle(br, tabControlArea);
            //----------------------------

            //----------------------------
            // draw border
            if (this.ShowTabBorders)
            {
                int nDelta = SystemInformation.Border3DSize.Width;
                tabArea.Inflate(nDelta, nDelta);
                g.DrawRectangle(SystemPens.ControlDark, tabArea);
            }
            //----------------------------

            //----------------------------
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
                if (i != this.SelectedIndex)
                    this.DrawTab(g, this.TabPages[i], i);

            if (this.SelectedIndex > -1)
                this.DrawTab(g, this.TabPages[this.SelectedIndex], this.SelectedIndex);

            g.Clip = rSaved;
            //----------------------------

            //----------------------------
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
            //----------------------------
        }

        internal class TabpageExCollectionEditor : CollectionEditor
        {
            public TabpageExCollectionEditor(Type type)
                : base(type)
            {
            }

            protected override Type CreateCollectionItemType()
            {
                return typeof(TabPage);
            }
        }
    }

    internal delegate void TabPageEventHandler(TabPage sender, EventArgs args);
}
