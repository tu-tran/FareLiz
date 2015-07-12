namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System.Collections;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;

    public abstract class ListViewColumnSorter : IComparer
    {
        public SortInfo SortInfo { get; set; }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int col = this.SortInfo.SortColumn;
            if (col < 0)
                return 0;

            // Cast the objects to be compared to ListViewItem objects
            var listviewX = (ListViewItem)x;
            var listviewY = (ListViewItem)y;

            int compareResult = this.CompareItem(listviewX.SubItems[col], listviewY.SubItems[col]);
            return this.SortInfo.SortAscending ? compareResult : -compareResult;
        }

        protected abstract int CompareItem(ListViewItem.ListViewSubItem x, ListViewItem.ListViewSubItem y);
    }

    public class ListViewTextColumnSorter : ListViewColumnSorter
    {
        protected override int CompareItem(ListViewItem.ListViewSubItem x, ListViewItem.ListViewSubItem y)
        {
            int compareResult = StringLogicalComparer.Compare(x.Text, y.Text);
            return compareResult;
        }
    }
}
