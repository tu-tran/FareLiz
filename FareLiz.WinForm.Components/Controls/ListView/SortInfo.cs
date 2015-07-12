namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    public class SortInfo
    {
        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>        
        public int SortColumn
        {
            set { this._sortColumn = value; }
            get { return this._sortColumn; }
        }
        private int _sortColumn = -1;

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>        
        public bool SortAscending
        {
            set { this._sortAscending = value; }
            get { return this._sortAscending; }
        }
        private bool _sortAscending;

        public SortInfo(int column, bool ascending)
        {
            this.SortColumn = column;
            this.SortAscending = ascending;
        }
    }
}
