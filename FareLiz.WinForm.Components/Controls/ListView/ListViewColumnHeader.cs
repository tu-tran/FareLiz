namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    public class ListViewColumnHeader : NativeWindow
    {
        private EnhancedListView _ownerListView; // owning listview control        
        private HDITEM hdr_hditem; // instance container

        /// <summary>
        /// Indexer to the alignment by column.
        /// </summary>
        public readonly ColumnAlignmentCollection Alignment;

        /// <summary>
        /// Indexer to the data types by column.
        /// </summary>
        public readonly ColumnDataTypeCollection DataType;

        /// <summary>
        /// Indexer to the column filter text.
        /// </summary>
        public readonly ColumnFilterCollection Filters;

        /// <summary>
        /// Indexer to the column name text.
        /// </summary>
        public readonly ColumnNamesCollection Names;

        /// <summary>
        /// Indexer to the column size (Width and Left as Height).
        /// </summary>
        public readonly ColumnSizeInfoCollection SizeInfo;

        private bool hdr_filter; // show filterbar in header
        /// <summary>
        /// When the Filtered property changes update the window style.
        /// </summary>
        public bool Filtered
        {
            get { return this.hdr_filter; }
            set
            {
                if (this.hdr_filter != value)
                {
                    this.hdr_filter = value;
                    this.ChangeFiltered();
                }
            }
        }

        private readonly SortInfo _sortInfo = new SortInfo(-1, true);
        public SortInfo SortInfo
        {
            get { return this._sortInfo; }
        }

        /// <summary>
        /// Class constructor.  Initializes various elements
        /// </summary>
        public ListViewColumnHeader(bool filtered)
        {
            // create the collection properties
            this.DataType = new ColumnDataTypeCollection(this);
            this.Filters = new ColumnFilterCollection(this);
            this.Names = new ColumnNamesCollection(this);
            this.SizeInfo = new ColumnSizeInfoCollection(this);
            this.Alignment = new ColumnAlignmentCollection(this);
            this.Filtered = filtered;
        }

        public void Attach(EnhancedListView listView)
        {
            this._ownerListView = listView;
            var headerHandle = NativeMethods.GetDlgItem(this._ownerListView.Handle, 0);

            if ((int)headerHandle != 0)
            {
                if ((int)this.Handle != 0)
                    this.ReleaseHandle();
                this.AssignHandle(headerHandle); // set the handle to this control.  the first dialog item for a listview is this header control...
            }
        }

        public void ClearAllFilters()
        {
            for (int i = 0; i < this.Filters.Count; i++)
                this.Filters[i] = "";
        }

        /// <summary>
        ///     Set the window style to reflect the value of the Filtered
        ///     property.  This is done when the Handle or property change.
        /// </summary>
        private void ChangeFiltered()
        {
            // we need to set a new style value for this control to turn
            // on/off the HDS_FILTERBAR flag.  First get the style itself.
            const int HDS_FILTR = (int)W32_HDS.HDS_FILTERBAR;
            int style = NativeMethods.GetWindowLong(this.Handle, W32_GWL.GWL_STYLE);

            // now that we have the flag see if it is not what is desired
            if (((style & HDS_FILTR) != 0) != this.hdr_filter)
            {
                // set/reset the flag for the filterbar
                if (this.hdr_filter) style |= HDS_FILTR;
                else style ^= HDS_FILTR;
                NativeMethods.SetWindowLong(this.Handle, W32_GWL.GWL_STYLE, style);

                // now we have to resize this control.  we do this by sending
                // a set item message to column 0 to change it's size.  this
                // is a kludge but the invalidate and others just don't work.
                this.hdr_hditem.mask = W32_HDI.HDI_HEIGHT;
                NativeMethods.SendMessage(this.Handle, W32_HDM.HDM_GETITEMW, 0, ref this.hdr_hditem);
                this.hdr_hditem.cxy += (this.hdr_filter) ? 1 : -1;
                NativeMethods.SendMessage(this.Handle, W32_HDM.HDM_SETITEMW, 0, ref this.hdr_hditem);
            }

            // it can't hurt to set the filter timeout limit to .5 seconds
            //NativeMethods.SendMessage(Handle, W32_HDM.HDM_SETFILTERCHANGETIMEOUT, 0, 500);

            // it is necessary to clear all filters which will cause a 
            // notification be sent to the ListView with -1 column.
            NativeMethods.SendMessage(this.Handle, W32_HDM.HDM_CLEARFILTER, -1, IntPtr.Zero);
        }

        protected override void OnHandleChange()
        {
            base.OnHandleChange();
            // reset the filter settings if there is a handle
            if (this.Handle != (IntPtr)0)
                this.ChangeFiltered();

        }
    }
}
