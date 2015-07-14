namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    /// <summary>The sort info.</summary>
    public class SortInfo
    {
        /// <summary>The _sort ascending.</summary>
        private bool _sortAscending;

        /// <summary>The _sort column.</summary>
        private int _sortColumn = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortInfo"/> class.
        /// </summary>
        /// <param name="column">
        /// The column.
        /// </param>
        /// <param name="ascending">
        /// The ascending.
        /// </param>
        public SortInfo(int column, bool ascending)
        {
            this.SortColumn = column;
            this.SortAscending = ascending;
        }

        /// <summary>Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').</summary>
        public int SortColumn
        {
            get
            {
                return this._sortColumn;
            }

            set
            {
                this._sortColumn = value;
            }
        }

        /// <summary>Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').</summary>
        public bool SortAscending
        {
            get
            {
                return this._sortAscending;
            }

            set
            {
                this._sortAscending = value;
            }
        }
    }
}