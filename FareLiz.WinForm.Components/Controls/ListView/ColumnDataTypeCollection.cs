namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// This is an indexer to the LVDataType values for each column.
    /// The data is stored in the lParam value of the column.
    /// </summary>
    public class ColumnDataTypeCollection
    {
        private readonly ListViewColumnHeader _header; // owning header control
        private HDITEM _hdItem; // HDITEM instance

        /// <summary>
        /// Constructor this must be given the header instance for access to the Handle property so that messages can be sent to it.
        /// </summary>
        /// <param name="header">HeaderControl</param>
        public ColumnDataTypeCollection(ListViewColumnHeader header)
        {
            this._header = header;
        }

        /// <summary>
        /// Indexer method to get/set the LVDataType for the column.
        /// </summary>
        public FilterDataType this[int index]
        {
            get
            {
                // the lparam of the column header contains the datatype
                this._hdItem.mask = W32_HDI.HDI_LPARAM;
                this._hdItem.lParam = (int)FilterDataType.String;

                // if it is valid the lparam is updated and then returned
                NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMW, index, ref this._hdItem);
                return (FilterDataType)this._hdItem.lParam;
            }
            set
            {
                // ensure that this is a valid column
                if (index < this.Count)
                {
                    // simply set the new LVDataType in the lparam and pass it on
                    this._hdItem.mask = W32_HDI.HDI_LPARAM;
                    this._hdItem.lParam = (int)value;
                    NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_SETITEMW, index, ref this._hdItem);
                }
            }
        }

        /// <summary>
        /// Return the number of columns in the header.
        /// </summary>
        public int Count
        {
            get { return NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMCOUNT, 0, IntPtr.Zero); }
        }
    }
}
