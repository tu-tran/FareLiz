namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Runtime.InteropServices;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    /// This is an indexer to the filter values for each column.  This is the most complex access to the HDITEM data since the filter data must be get/set
    /// via the HDTEXTFILTER structure referenced in the HDITEM structure.  It is simple to Marshall this, but figuring that out in the first place took alot
    /// of effort.
    /// </summary>
    public class ColumnFilterCollection
    {
        /// <summary>
        /// The _header.
        /// </summary>
        private readonly ListViewColumnHeader _header; // owning header control

        /// <summary>
        /// The _hditem.
        /// </summary>
        private HDITEM _hditem; // HDITEM instance

        /// <summary>
        /// The _hd text filter.
        /// </summary>
        private HDTEXTFILTER _hdTextFilter; // HDTEXTFILTER instance

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnFilterCollection"/> class. 
        /// Constructor this must be given the header instance for access to the Handle property so that messages can be sent to it.
        /// </summary>
        /// <param name="header">
        /// HeaderControl
        /// </param>
        public ColumnFilterCollection(ListViewColumnHeader header)
        {
            this._header = header;
        }

        /// <summary>
        /// Indexer method to get/set the filter text for the column.
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
                // if the column is invalid return nothing
                if (index >= this.Count || index < 0)
                {
                    return string.Empty;
                }

                // this is tricky since it involves marshalling pointers
                // to structures that are used as a reference in another
                // structure.  first initialize the receiving HDTEXTFILTER
                this._hdTextFilter.pszText = new string(new char[64]);
                this._hdTextFilter.cchTextMax = this._hdTextFilter.pszText.Length;

                // set the HDITEM up to request the current filter content
                this._hditem.mask = W32_HDI.HDI_FILTER;
                this._hditem.type = (uint)W32_HDFT.HDFT_ISSTRING;

                // marshall memory big enough to contain a HDTEXTFILTER 
                this._hditem.pvFilter = Marshal.AllocCoTaskMem(Marshal.SizeOf(this._hdTextFilter));

                // now copy the HDTEXTFILTER structure to the marshalled memory
                Marshal.StructureToPtr(this._hdTextFilter, this._hditem.pvFilter, false);

                NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMW, index, ref this._hditem);

                    // First check if the header fileter is empty or not
                this._hdTextFilter = (HDTEXTFILTER)Marshal.PtrToStructure(this._hditem.pvFilter, typeof(HDTEXTFILTER));

                    // un-marshall the memory back into the HDTEXTFILTER structure                
                if (!string.IsNullOrEmpty(this._hdTextFilter.pszText))
                {
                    NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_GETITEMA, index, ref this._hditem);
                    this._hdTextFilter = (HDTEXTFILTER)Marshal.PtrToStructure(this._hditem.pvFilter, typeof(HDTEXTFILTER));
                }

                Marshal.FreeCoTaskMem(this._hditem.pvFilter); // remember to free the marshalled IntPtr memory...

                // return the string in the text filter area
                return this._hdTextFilter.pszText;
            }

            set
            {
                // ensure that the column exists before attempting this
                if (index < this.Count)
                {
                    // this is just like the get{} except we don't have to
                    // return anything and the message is HDM_SETITEMA. we
                    // use the non-unicode methods for both the get and set.
                    // reference the get{} method for marshalling details.
                    this._hdTextFilter.pszText = value;
                    this._hdTextFilter.cchTextMax = 64;
                    this._hditem.mask = W32_HDI.HDI_FILTER;
                    this._hditem.type = (uint)W32_HDFT.HDFT_ISSTRING;
                    this._hditem.pvFilter = Marshal.AllocCoTaskMem(Marshal.SizeOf(this._hdTextFilter));
                    Marshal.StructureToPtr(this._hdTextFilter, this._hditem.pvFilter, false);
                    NativeMethods.SendMessage(this._header.Handle, W32_HDM.HDM_SETITEMA, index, ref this._hditem);
                    Marshal.FreeCoTaskMem(this._hditem.pvFilter);
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