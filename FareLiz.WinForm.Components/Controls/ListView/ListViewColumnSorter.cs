namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System.Collections;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;

    /// <summary>The list view column sorter.</summary>
    public abstract class ListViewColumnSorter : IComparer
    {
        /// <summary>Gets or sets the sort info.</summary>
        public SortInfo SortInfo { get; set; }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">
        /// First object to be compared
        /// </param>
        /// <param name="y">
        /// Second object to be compared
        /// </param>
        /// <returns>
        /// The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'
        /// </returns>
        public int Compare(object x, object y)
        {
            var col = this.SortInfo.SortColumn;
            if (col < 0)
            {
                return 0;
            }

            // Cast the objects to be compared to ListViewItem objects
            var listviewX = (ListViewItem)x;
            var listviewY = (ListViewItem)y;

            var compareResult = this.CompareItem(listviewX.SubItems[col], listviewY.SubItems[col]);
            return this.SortInfo.SortAscending ? compareResult : -compareResult;
        }

        /// <summary>
        /// The compare item.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected abstract int CompareItem(ListViewItem.ListViewSubItem x, ListViewItem.ListViewSubItem y);
    }

    /// <summary>The list view text column sorter.</summary>
    public class ListViewTextColumnSorter : ListViewColumnSorter
    {
        /// <summary>
        /// The compare item.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected override int CompareItem(ListViewItem.ListViewSubItem x, ListViewItem.ListViewSubItem y)
        {
            var compareResult = StringLogicalComparer.Compare(x.Text, y.Text);
            return compareResult;
        }
    }
}