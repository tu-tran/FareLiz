namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Controls.Custom;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The enhanced list view.</summary>
    public partial class EnhancedListView
    {
        /// <summary>The items added.</summary>
        public event ListViewItemsDelegate ItemsAdded;

        /// <summary>The item removed.</summary>
        public event ListViewItemDelegate ItemRemoved;

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)W32_WM.WM_NOTIFY)
            {
                // notify messages can come from the header and are used for column sorting and item filtering if wanted
                var h1 = (NMHEADER)m.GetLParam(typeof(NMHEADER));

                // get the notify message header from this message LParam
                if ((this._header != null) && (h1.hdr.hwndFrom == this._header.Handle))
                {
                    // process messages ONLY from our header control
                    this.NotifyHeaderMessage(h1);
                }
            }
            else if (m.Msg == (int)W32_WM.WM_LBUTTONUP)
            {
                this.DefWndProc(ref m);
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// The on handle destroyed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this._header != null)
            {
                this._header.ReleaseHandle();
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// The on handle created.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnHandleCreated(EventArgs e)
        {
            if (this._header != null)
            {
                this._header.ReleaseHandle();
                this._header.Attach(this); // Subclass for the header control
            }

            base.OnHandleCreated(e);
        }

        /// <summary>
        /// When user click on a Filter menu item
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FilterMenuItem_Click(object sender, EventArgs e)
        {
            var mnu = sender as ToolStripMenuItem;
            if (mnu == null)
            {
                return;
            }

            var index = (int)mnu.Tag;
            if (index == -1)
            {
                this._header.ClearAllFilters();
            }
            else
            {
                var selItem = this.FirstSelectedItem;
                if (selItem != null)
                {
                    this._header.Filters[index] = selItem.SubItems[index].Text;
                }
            }
        }

        /// <summary>
        /// Populate the filter context menu strip (depending on the available columns and selected item)
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event Args.
        /// </param>
        private void FilterDataMenuStrip_DropDownOpening(object sender, EventArgs eventArgs)
        {
            var mnu = sender as ToolStripDropDownItem;
            if (mnu != null)
            {
                mnu.DropDownItems.Clear();
                var mnuClearAllFilters = new RadioToolStripMenuItem("Clear all filters", -1);
                mnuClearAllFilters.Click += this.FilterMenuItem_Click;
                mnu.DropDownItems.Add(mnuClearAllFilters);

                var selItem = this.FirstSelectedItem;
                if (selItem != null)
                {
                    mnu.DropDownItems.Add(new ToolStripSeparator());
                    for (var i = 0; i < selItem.SubItems.Count; i++)
                    {
                        var subItem = selItem.SubItems[i];
                        var newItem = new RadioToolStripMenuItem(this.Columns[i].Text + " = " + subItem.Text, i);
                        newItem.Click += this.FilterMenuItem_Click;
                        mnu.DropDownItems.Add(newItem);
                    }
                }
            }
        }

        /// <summary>
        /// When user click on a Group By menu item
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GroupByMenuItem_Click(object sender, EventArgs e)
        {
            var mnu = sender as ToolStripMenuItem;
            if (mnu == null)
            {
                return;
            }

            var index = (int)mnu.Tag;
            this.GroupColumnIndex = index;
            if (base.ShowGroups = index > -1)
            {
                this.AutoGroup();
            }
        }

        /// <summary>
        /// The notify header message.
        /// </summary>
        /// <param name="h">
        /// The h.
        /// </param>
        private void NotifyHeaderMessage(NMHEADER h)
        {
            // process only specific header notification messages
            switch (h.hdr.code)
            {
                // a header column was clicked, do the sort
                case (int)W32_HDN.HDN_ITEMCLICKA:
                case (int)W32_HDN.HDN_ITEMCLICKW:
                    var col = h.iItem;
                    if (this._header.SortInfo.SortColumn == col)
                    {
                        // Sort on same column, just the opposite direction.
                        this._header.SortInfo.SortAscending = !this._header.SortInfo.SortAscending;
                    }
                    else
                    {
                        this._header.SortInfo.SortAscending = true;
                        this._header.SortInfo.SortColumn = col;
                    }

                    this.Sort();
                    break;

                // a filter button was clicked display the popup menu
                // to handle setting filter options for the column
                case (int)W32_HDN.HDN_FILTERBTNCLICK:
                    this._filterButtonColumn = h.iItem;
                    this.mnuFilterButton.Show(this, this.PointToClient(MousePosition));
                    break;

                // a filter content changed, update the items collection
                case (int)W32_HDN.HDN_FILTERCHANGE:

                    // if this is for item -1 then this is a clear all filters
                    if (h.iItem < 0)
                    {
                        this._activeFilters.Clear();
                    }

                    // if we are filtered this is a real filter data change
                    else if (this.Filtered)
                    {
                        this.BuildFilter(h.iItem);
                    }

                    // update the items array with new filters applied
                    this.ApplyFilters();
                    if (this.GroupColumnIndex > -1)
                    {
                        this.AutoGroup();
                    }

                    break;
            }
        }

        /// <summary>
        /// Set the sort icon for the column depending on the sort type
        /// </summary>
        /// <param name="columnIndex">
        /// The column Index.
        /// </param>
        /// <param name="ascending">
        /// The ascending.
        /// </param>
        private void SetSortIcon(int columnIndex, bool ascending)
        {
            var columnHeader = NativeMethods.SendMessage(this.Handle, (int)W32_LVM.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            for (var columnNumber = 0; columnNumber <= this.Columns.Count - 1; columnNumber++)
            {
                var columnPtr = new IntPtr(columnNumber);
                var item = new HDITEM { mask = W32_HDI.HDI_FORMAT };

                if (NativeMethods.SendMessage(columnHeader, (int)W32_HDM.HDM_GETITEMW, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                if (columnNumber == columnIndex)
                {
                    if (ascending)
                    {
                        item.fmt &= ~W32_HDF.HDF_SORTDOWN;
                        item.fmt |= W32_HDF.HDF_SORTUP;
                    }
                    else
                    {
                        item.fmt &= ~W32_HDF.HDF_SORTUP;
                        item.fmt |= W32_HDF.HDF_SORTDOWN;
                    }
                }
                else
                {
                    item.fmt &= ~W32_HDF.HDF_SORTDOWN & ~W32_HDF.HDF_SORTUP;
                }

                if (NativeMethods.SendMessage(columnHeader, (int)W32_HDM.HDM_SETITEMW, columnPtr, ref item) == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
            }
        }

        /// <summary>
        /// Update the menu when users click on the Filter button on the column header
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FilterButtonMenu_Popup(object sender, EventArgs e)
        {
            // also set the current alignment
            switch (this._header.Alignment[this._filterButtonColumn])
            {
                case HorizontalAlignment.Center:
                    this.mnuAlignLeft.Checked = this.mnuAlignRight.Checked = !(this.mnuAlignCenter.Checked = true);
                    break;
                case HorizontalAlignment.Right:
                    this.mnuAlignLeft.Checked = this.mnuAlignCenter.Checked = !(this.mnuAlignRight.Checked = true);
                    break;
                default:
                    this.mnuAlignCenter.Checked = this.mnuAlignRight.Checked = !(this.mnuAlignLeft.Checked = true);
                    break;
            }

            this.mnuIgnoreCase.Checked = this._isFilterIgnoreCase;
        }

        /// <summary>
        /// MenuItem click event.  Perform the requested menu action and always force a filter rebuild since all these change the filtering properties
        /// in some manner.  The sender identifies the specific menuitem clicked.
        /// </summary>
        /// <param name="sender">
        /// MenuItem
        /// </param>
        /// <param name="e">
        /// Event
        /// </param>
        private void FilterButtonMenuItem_Click(object sender, EventArgs e)
        {
            // 'Clear filter', set the Header.Filter for the column
            if (sender == this.mnuClearFilter)
            {
                this._header.Filters[this._filterButtonColumn] = string.Empty;
            }
            else if (sender == this.mnuClearAllFilter)
            {
                this._header.ClearAllFilters();
            }

            // 'Ignore case', toggle the flag
            else if (sender == this.mnuIgnoreCase)
            {
                this.FilterIgnoreCase = !this.FilterIgnoreCase;
            }

            // 'Left', set the alignment
            else if (sender == this.mnuAlignLeft)
            {
                this._header.Alignment[this._filterButtonColumn] = HorizontalAlignment.Left;
            }

            // 'Right', set the alignment
            else if (sender == this.mnuAlignRight)
            {
                this._header.Alignment[this._filterButtonColumn] = HorizontalAlignment.Right;
            }

            // 'Center', set the alignment
            else if (sender == this.mnuAlignCenter)
            {
                this._header.Alignment[this._filterButtonColumn] = HorizontalAlignment.Center;
            }

            // unknown, ignore this type
            else
            {
                return;
            }

            // force a filter build on the specific column
            this.BuildFilter(this._filterButtonColumn);

            // follow with a filter update
            this.ApplyFilters();

            // set focus to ourself after menu action otherwise
            // focus is left in the filter edit itself...
            this.Focus();

            // if this was an alignment change then we need to invalidate
            if (((MenuItem)sender).Parent == this.mnuAlignment)
            {
                this.Invalidate();
            }
        }
    }
}