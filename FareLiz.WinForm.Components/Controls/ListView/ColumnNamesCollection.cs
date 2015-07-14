namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>This is an indexer to the text values for each column.</summary>
    public class ColumnNamesCollection
    {
        /// <summary>The _header.</summary>
        private readonly ListViewColumnHeader _header; // owning header control

        /// <summary>The _hd item.</summary>
        private HDITEM _hdItem; // HDITEM instance

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnNamesCollection"/> class. Constructor this must be given the header instance for access
        /// to the Handle property so that messages can be sent to it.
        /// </summary>
        /// <param name="header">
        /// HeaderControl
        /// </param>
        public ColumnNamesCollection(ListViewColumnHeader header)
        {
            this._header = header;
        }

        /// <summary>
        /// Indexer method to get/set the text of the column.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[int index]
        {
            get
            {
                // set up to retrive the column header text
                this._hdItem.mask = W32_HDI.HDI_TEXT;
                this._hdItem.pszText = new string(new char[64]);
                this._hdItem.cchTextMax = this._hdItem.pszText.Length;

                // if successful the text has been retrieved and returned
                NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMW, index, ref this._hdItem);
                return this._hdItem.pszText;
            }

            set
            {
                // this must be a valid column index
                if (index < this.Count)
                {
                    // simply set the text and size in the structure and pass it on
                    this._hdItem.mask = W32_HDI.HDI_TEXT;
                    this._hdItem.pszText = value;
                    this._hdItem.cchTextMax = this._hdItem.pszText.Length;
                    NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_SETITEMA, index, ref this._hdItem);
                }
            }
        }

        /// <summary>Return the number of columns in the header.</summary>
        public int Count
        {
            get
            {
                return NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMCOUNT, 0, IntPtr.Zero);
            }
        }
    }
}