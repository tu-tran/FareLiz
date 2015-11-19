namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// The flat tab control.
    /// </summary>
    partial class FlatTabControl
    {
        /// <summary>
        /// The mouse drag threshold.
        /// </summary>
        private const int MouseDragThreshold = 5;

        /// <summary>
        /// The _drag compensate.
        /// </summary>
        private int _dragCompensate;

        /// <summary>
        /// The _drag offset.
        /// </summary>
        private int _dragOffset;

        /// <summary>
        /// The _drag start location.
        /// </summary>
        private Point _dragStartLocation = Point.Empty;

        /// <summary>
        /// The _drag tab rect.
        /// </summary>
        private Rectangle _dragTabRect = Rectangle.Empty;

        /// <summary>
        /// The _dropping index.
        /// </summary>
        private int _droppingIndex = -1;

        /// <summary>
        /// The _is dragging.
        /// </summary>
        private bool _isDragging;

        /// <summary>
        /// The _last active index.
        /// </summary>
        private int _lastActiveIndex = -1; // Keep track of last active index in order to reduce redundant poainting        

        /// <summary>
        /// The _old scroll value.
        /// </summary>
        private int _oldScrollValue;

        /// <summary>
        /// The tab page closing.
        /// </summary>
        [Category("Action")]
        public event TabControlCancelEventHandler TabPageClosing;

        /// <summary>
        /// The tab page reloading.
        /// </summary>
        [Category("Action")]
        public event TabControlEventHandler TabPageReloading;

        /// <summary>
        /// The tab scrolling.
        /// </summary>
        [Category("Action")]
        public event ScrollEventHandler TabScrolling;

        /// <summary>
        /// The on create control.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.FindUpDown();
        }

        /// <summary>
        /// The on mouse click.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (!this.DesignMode)
            {
                if (!this._isDragging)
                {
                    int index = this.ActiveIndex;
                    if (index < 0)
                    {
                        return;
                    }

                    var mouseLoc = this.PointToClient(MousePosition);

                    if (this.UseTabReloader && this.TabPageReloading != null && this.GetReloaderRect(index).Contains(mouseLoc))
                    {
                        this.ReloadTab(index);
                        return;
                    }

                    // Tab Closer should be handled at last since it may remove the tab!
                    if (this.UseTabCloser && this.GetCloserRect(index).Contains(mouseLoc))
                    {
                        this.CloseTab(index);
                        return;
                    }

                    if (e.Button == MouseButtons.Right)
                    {
                        this.tabPageContextMenu.Tag = index; // Keep track of the affected tab
                        this.tabPageContextMenu.Show(this, e.Location);
                    }
                }

                // 	Fire the base event
                base.OnMouseClick(e);
            }
        }

        /// <summary>
        /// The on mouse down.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.AllowDrop && e.Button == MouseButtons.Left)
            {
                this._dragStartLocation = e.Location;
                this._dragOffset = this._dragCompensate = 0;
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// The on mouse move.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.AllowDrop && this.Alignment <= TabAlignment.Bottom && e.Button == MouseButtons.Left)
            {
                if (this.SelectedIndex != -1)
                {
                    // Measure how far has the mouse dragged
                    this._dragOffset = e.Location.X - this._dragStartLocation.X;

                    if (this._isDragging)
                    {
                        var mouseLoc = e.Location;
                        bool dragRight = this._dragOffset > 0; // Mouse move to the right
                        Point testPoint = dragRight
                                              ? new Point(this._dragTabRect.Right, this._dragTabRect.Height / 2)
                                              : new Point(this._dragTabRect.Left, this._dragTabRect.Height / 2);

                        this._dragOffset += this._dragCompensate;
                        this._dragTabRect = this.GetTabRect(this.SelectedIndex);

                        // Debug.WriteLine("Dragging tab " + SelectedIndex + " to the " + (dragRight ? "right" : "left") + " by " + _dragOffset + "px at location: " + _dragTabRect);
                        int index = this.GetActiveIndex(testPoint, false);
                        if (index == this.SelectedIndex)
                        {
                            this._droppingIndex = -1;
                        }
                        else
                        {
                            this._droppingIndex = index;
                        }

                        if (index != -1)
                        {
                            bool canSwitch = (dragRight && index > this.SelectedIndex) || (!dragRight && index < this.SelectedIndex);
                            if (canSwitch)
                            {
                                var toRect = this.GetTabRect(index);
                                Rectangle testRect;
                                var minSlideDistance = (int)(toRect.Width * 0.35);

                                if (dragRight)
                                {
                                    // Dragging to the right?
                                    testRect = new Rectangle(toRect.X + minSlideDistance, toRect.Y, minSlideDistance, toRect.Height);
                                }
                                else
                                {
                                    testRect = new Rectangle(toRect.X, toRect.Y, toRect.Width - minSlideDistance, toRect.Height);
                                }

                                if (testRect.Contains(testPoint))
                                {
                                    // The edge of dragging edge moved fare enough?
                                    // Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "Switching tab {0} and {1}", SelectedIndex, index));
                                    // Swap tab
                                    var fromTab = this.TabPages[this.SelectedIndex];
                                    var toTab = this.TabPages[index];
                                    int newOffset = this._dragOffset;
                                    this._dragOffset = 0;
                                    var fromTabOriginalRect = this.GetTabRect(this.SelectedIndex);
                                    int offsetDiff = fromTabOriginalRect.X - toRect.X;
                                    this._dragOffset = newOffset + offsetDiff;
                                    this.SuspendLayout();
                                    this.TabPages[this.SelectedIndex] = toTab;
                                    this.TabPages[index] = fromTab;
                                    this.SelectedIndex = index;
                                    this._dragTabRect = this.GetTabRect(this.SelectedIndex);
                                    this._droppingIndex = -1;
                                    this._dragStartLocation = e.Location;
                                    this._dragCompensate = this._dragOffset; // So that the next calculation will persist the same drag offset
                                    this.ResumeLayout();
                                }
                            }

                            this.Cursor = Cursors.Default;
                        }
                        else
                        {
                            this.Cursor = Cursors.No;
                        }
                    }
                    else
                    {
                        if (Math.Abs(this._dragOffset) >= MouseDragThreshold)
                        {
                            this._isDragging = true;
                            this._dragTabRect = this.GetTabRect(this.SelectedIndex);
                        }
                    }
                }
            }

            bool needRefresh = this._isDragging;
            if (!needRefresh)
            {
                var index = this.ActiveIndex;
                if (this._lastActiveIndex == index)
                {
                    var mouseLoc = new Point(e.X, e.Y);

                    // Check if button is being hovered
                    if ((this.UseTabReloader && this.GetReloaderRect(index).Contains(mouseLoc))
                        || (this.UseTabCloser && this.GetCloserRect(index).Contains(mouseLoc)))
                    {
                        needRefresh = true;
                    }
                }
                else
                {
                    needRefresh = true;
                    this._lastActiveIndex = index;
                }
            }

            if (needRefresh)
            {
                this.Invalidate();
            }
        }

        /// <summary>
        /// The on mouse up.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.CancelDrag();
        }

        /// <summary>
        /// The on mouse leave.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.CancelDrag();
            this.Invalidate();
        }

        /// <summary>
        /// The on paint.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.DrawControl(e.Graphics);
        }

        /// <summary>
        /// The on h scroll.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected virtual void OnHScroll(ScrollEventArgs e)
        {
            // 	repaint the moved tabs
            this.Invalidate();

            // 	Raise the event
            if (this.TabScrolling != null)
            {
                this.TabScrolling(this, e);
            }

            if (e.Type == ScrollEventType.EndScroll)
            {
                this._oldScrollValue = e.NewValue;
            }
        }

        /// <summary>
        /// The on deselecting.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnDeselecting(TabControlCancelEventArgs e)
        {
            var mouseLoc = this.PointToClient(MousePosition);
            var index = this.ActiveIndex;
            if (index != e.TabPageIndex)
            {
                if ((this.UseTabReloader && this.TabPageReloading != null && this.GetReloaderRect(index).Contains(mouseLoc))
                    || (this.UseTabCloser && this.GetCloserRect(index).Contains(mouseLoc)))
                {
                    e.Cancel = true;
                }
            }

            // Tab Closer should be handled at last since it may remove the tab!
            base.OnDeselecting(e);
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == (int)W32_WM.WM_HSCROLL)
            {
                // 	Raise the scroll event when the scroller is scrolled
                this.OnHScroll(
                    new ScrollEventArgs(
                        (ScrollEventType)NativeMethods.LoWord(m.WParam), 
                        this._oldScrollValue, 
                        NativeMethods.HiWord(m.WParam), 
                        ScrollOrientation.HorizontalScroll));
            }
        }

        /// <summary>
        /// The close tab.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        private void CloseTab(int index)
        {
            // 	If we are clicking on a closer then remove the tab instead of raising the standard mouse click event. But raise the tab closing event first
            TabPage tab = this.TabPages[index];
            if (this.TabPageClosing != null)
            {
                var args = new TabControlCancelEventArgs(tab, index, false, TabControlAction.Deselecting);
                this.TabPageClosing(this, args);
                if (args.Cancel)
                {
                    return;
                }
            }

            this.TabPages.Remove(tab);
            tab.Dispose();
        }

        /// <summary>
        /// The flat tab control_ control added.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlatTabControl_ControlAdded(object sender, ControlEventArgs e)
        {
            this.FindUpDown();
            this.UpdateUpDown();
        }

        /// <summary>
        /// The flat tab control_ control removed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlatTabControl_ControlRemoved(object sender, ControlEventArgs e)
        {
            this.FindUpDown();
            this.UpdateUpDown();
        }

        /// <summary>
        /// The flat tab control_ key down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlatTabControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F4)
            {
                // Close the tab
                var selTabIdx = this.SelectedIndex;
                if (selTabIdx > -1)
                {
                    this.CloseTab(selTabIdx);
                }
            }
        }

        /// <summary>
        /// The flat tab control_ selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlatTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateUpDown();
            this.Invalidate(); // we need to update border and background colors
        }

        /// <summary>
        /// The cancel drag.
        /// </summary>
        private void CancelDrag()
        {
            this._dragTabRect = Rectangle.Empty;
            this._dragStartLocation = Point.Empty;
            this._dragOffset = 0;
            this._droppingIndex = -1;
            this.Cursor = Cursors.Default;
            if (this._isDragging)
            {
                this._isDragging = false;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The reload tab.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        private void ReloadTab(int index)
        {
            if (this.TabPageReloading != null)
            {
                TabPage tab = this.TabPages[index];
                var args = new TabControlEventArgs(tab, index, TabControlAction.Selecting);
                this.TabPageReloading(this, args);
            }
        }

        /// <summary>
        /// The tab page context menu_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TabPageContextMenu_Opening(object sender, CancelEventArgs e)
        {
            int activeIdx = (int)this.tabPageContextMenu.Tag;
            int count = this.TabPages.Count;
            this.mnuCloseAllTabs.Available = this.UseTabCloser && count > 1;
            this.mnuCloseLeftTabs.Available = this.UseTabCloser && count > 1 && activeIdx > 0;
            this.mnuCloseRightTabs.Available = this.UseTabCloser && count > 1 && activeIdx < this.TabPages.Count - 1;
            this.mnuReload.Available = this.UseTabReloader && this.TabPageReloading != null;
        }

        /// <summary>
        /// The mnu close tab_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuCloseTab_Click(object sender, EventArgs e)
        {
            int activeIdx = (int)this.tabPageContextMenu.Tag;
            this.CloseTab(activeIdx);
        }

        /// <summary>
        /// The mnu close all tabs_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuCloseAllTabs_Click(object sender, EventArgs e)
        {
            int activeIdx = (int)this.tabPageContextMenu.Tag;
            this.SuspendLayout();
            try
            {
                int totalCount = this.TabPages.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    this.CloseTab(0);
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// The mnu close left tabs_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuCloseLeftTabs_Click(object sender, EventArgs e)
        {
            int activeIdx = (int)this.tabPageContextMenu.Tag;
            this.SuspendLayout();
            try
            {
                for (int i = 0; i < activeIdx; i++)
                {
                    this.CloseTab(0);
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// The mnu close right tabs_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuCloseRightTabs_Click(object sender, EventArgs e)
        {
            int activeIdx = (int)this.tabPageContextMenu.Tag;
            this.SuspendLayout();
            try
            {
                for (int i = activeIdx + 1; i < this.TabPages.Count;)
                {
                    this.CloseTab(i);
                }
            }
            finally
            {
                this.ResumeLayout();
            }
        }

        /// <summary>
        /// The mnu reload_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuReload_Click(object sender, EventArgs e)
        {
            int activeIdx = (int)this.tabPageContextMenu.Tag;
            this.ReloadTab(activeIdx);
        }
    }
}