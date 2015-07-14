namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Drawing;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// This is an indexer to the READONLY Size values for a column. NOTE: The Size really contains the Width and Left position, the Left is sorted
    /// in the Height property of the Size class We do this because a Rectangle is not really necessary.
    /// </summary>
    public class ColumnSizeInfoCollection
    {
        /// <summary>The _header.</summary>
        private readonly ListViewColumnHeader _header; // owning header control

        /// <summary>The _col rectangle.</summary>
        private RECT _colRectangle; // HDITEM instance

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnSizeInfoCollection"/> class. Constructor this must be given the header instance for
        /// access to the Handle property so that messages can be sent to it.
        /// </summary>
        /// <param name="header">
        /// HeaderControl
        /// </param>
        public ColumnSizeInfoCollection(ListViewColumnHeader header)
        {
            this._header = header;
        }

        /// <summary>
        /// Indexer method to get/set the Size for the column.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="Size"/>.
        /// </returns>
        public Size this[int index]
        {
            get
            {
                // if the column is valid get the rectangle
                if (index < this.Count)
                {
                    NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMRECT, index, ref this._colRectangle);
                    return new Size(this._colRectangle.right - this._colRectangle.left, this._colRectangle.left);
                }

                // return null size
                return new Size(0, 0);
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