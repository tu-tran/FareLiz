namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The column alignment collection.</summary>
    public class ColumnAlignmentCollection
    {
        /// <summary>The _header.</summary>
        private readonly ListViewColumnHeader _header; // owning header control

        /// <summary>The _hd item.</summary>
        private HDITEM _hdItem; // HDITEM instance

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAlignmentCollection"/> class. Constructor this must be given the header instance for
        /// access to the Handle property so that messages can be sent to it.
        /// </summary>
        /// <param name="header">
        /// HeaderControl
        /// </param>
        public ColumnAlignmentCollection(ListViewColumnHeader header)
        {
            this._header = header;
        }

        /// <summary>
        /// Indexer method to get/set the Alignment for the column.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="HorizontalAlignment"/>.
        /// </returns>
        public HorizontalAlignment this[int index]
        {
            get
            {
                // ensure that this is a valid column
                if (index >= this.Count)
                {
                    return HorizontalAlignment.Left;
                }

                // get the current format for the column
                this._hdItem.mask = W32_HDI.HDI_FORMAT;
                NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMW, index, ref this._hdItem);

                // return the current setting
                if ((this._hdItem.fmt & W32_HDF.HDF_CENTER) != 0)
                {
                    return HorizontalAlignment.Center;
                }

                if ((this._hdItem.fmt & W32_HDF.HDF_RIGHT) != 0)
                {
                    return HorizontalAlignment.Right;
                }

                return HorizontalAlignment.Left;
            }

            set
            {
                // ensure that this is a valid column
                if (index < this.Count)
                {
                    // get the current format for the column
                    this._hdItem.mask = W32_HDI.HDI_FORMAT;
                    NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMW, index, ref this._hdItem);

                    // turn off any existing alignment values
                    this._hdItem.fmt &= W32_HDF.HDF_NOJUSTIFY;

                    // turn on the correct alignment
                    switch (value)
                    {
                        case HorizontalAlignment.Center:
                            this._hdItem.fmt |= W32_HDF.HDF_CENTER;
                            break;
                        case HorizontalAlignment.Right:
                            this._hdItem.fmt |= W32_HDF.HDF_RIGHT;
                            break;
                        default:
                            this._hdItem.fmt |= W32_HDF.HDF_LEFT;
                            break;
                    }

                    // now update the column format
                    NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_SETITEMW, index, ref this._hdItem);
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