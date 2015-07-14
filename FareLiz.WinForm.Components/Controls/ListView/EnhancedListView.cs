namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Controls.Custom;

    /// <summary>Extended ListView control with sorting/filtering capabilities and support for Virtual Mode data handling</summary>
    [ToolboxBitmap(typeof(ListView))]
    [DefaultEvent("ColumnClick")]
    public partial class EnhancedListView : ListView
    {
        /// <summary>The _active filters.</summary>
        private readonly List<ListViewFilter> _activeFilters = new List<ListViewFilter>();

        // Array of active LVFFliters        

        /// <summary>The _filtered items.</summary>
        private readonly List<ListViewItem> _filteredItems = new List<ListViewItem>(); // Filtered out items

        /// <summary>The _group strip.</summary>
        private readonly Dictionary<int, RadioToolStripMenuItem> _groupStrip = new Dictionary<int, RadioToolStripMenuItem>();

        /// <summary>The _header.</summary>
        private readonly ListViewColumnHeader _header = new ListViewColumnHeader(true);

        /// <summary>The _items.</summary>
        private readonly ObservableListViewItemCollection _items;

        /// <summary>The _collapsible group state.</summary>
        private ListViewGroupState _collapsibleGroupState;

        /// <summary>The _double buffering.</summary>
        private bool _doubleBuffering;

        /// <summary>The _filter button column.</summary>
        private int _filterButtonColumn; // Filter button column

        /// <summary>The _grouping col.</summary>
        private int _groupingCol = -1;

        /// <summary>The _is filter ignore case.</summary>
        private bool _isFilterIgnoreCase = true; // Ignore case on filter

        /// <summary>The _sorter.</summary>
        private ListViewColumnSorter _sorter;

        /// <summary>The virtual mode filter.</summary>
        public ListViewFilterMethod VirtualModeFilter;

        /// <summary>The virtual mode sort.</summary>
        public ListViewMethod VirtualModeSort;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnhancedListView"/> class.
        /// </summary>
        /// <param name="sorter">
        /// The sorter.
        /// </param>
        public EnhancedListView(ListViewColumnSorter sorter)
        {
            this.InitializeComponent();
            this.ListViewItemSorter = sorter;
            this._items = new ObservableListViewItemCollection(this);
        }

        /// <summary>Initializes a new instance of the <see cref="EnhancedListView" /> class.</summary>
        public EnhancedListView()
            : this(new ListViewTextColumnSorter())
        {
        }

        /// <summary>Gets the items.</summary>
        public new ObservableListViewItemCollection Items
        {
            get
            {
                return this._items;
            }
        }

        /// <summary>Gets or sets a value indicating whether filtered.</summary>
        [Description("In Details mode, the header will display filter controls.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool Filtered
        {
            get
            {
                return this._header.Filtered;
            }

            set
            {
                if (this._header.Filtered != value)
                {
                    this._header.Filtered = value;
                    this.Refresh();
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether filter ignore case.</summary>
        [Description("Treat filter strings case-insensitive for comparison.")]
        [Category("Behavior")]
        [DefaultValue(true)]
        public bool FilterIgnoreCase
        {
            get
            {
                return this._isFilterIgnoreCase;
            }

            set
            {
                if (this._isFilterIgnoreCase != value)
                {
                    this._isFilterIgnoreCase = value;
                    this.ApplyFilters();
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether double buffering.</summary>
        public bool DoubleBuffering
        {
            get
            {
                return this._doubleBuffering;
            }

            set
            {
                if (this._doubleBuffering != value)
                {
                    this.SetDoubleBuffering(value);
                    this._doubleBuffering = value;
                }
            }
        }

        /// <summary>Gets or sets the group column index.</summary>
        /// <exception cref="ArgumentException"></exception>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int GroupColumnIndex
        {
            get
            {
                return this._groupingCol;
            }

            set
            {
                if (value >= this.Columns.Count)
                {
                    throw new ArgumentException("Invalid grouping column index: " + value);
                }

                this._groupingCol = value;
                if (value > -1)
                {
                    this.AutoGroup();
                }
            }
        }

        /// <summary>Gets or sets the list view item sorter.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ListViewColumnSorter ListViewItemSorter
        {
            get
            {
                return this._sorter;
            }

            set
            {
                base.ListViewItemSorter = this._sorter = value;
                if (this._sorter != null)
                {
                    this._sorter.SortInfo = this._header.SortInfo;
                }
            }
        }

        /// <summary>Gets a value indicating whether show groups.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool ShowGroups
        {
            get
            {
                return this.GroupColumnIndex > -1;
            }
        }

        /// <summary>Gets or sets the collapsible group state.</summary>
        public ListViewGroupState CollapsibleGroupState
        {
            get
            {
                return this._collapsibleGroupState;
            }

            set
            {
                this.SetGroupState(value);
                this._collapsibleGroupState = value;
            }
        }

        /// <summary>Get the first selected item index. Return -1 if there is no selected item</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ListViewItem FirstSelectedItem
        {
            get
            {
                var selIndex = this.SelectedIndices.Count > 0 ? this.SelectedIndices[0] : -1;
                return selIndex > -1 ? this.Items[selIndex] : null;
            }
        }

        /// <summary>
        /// The add range.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AddRange(ListViewItem[] items)
        {
            var oldSorter = this._sorter;
            this._sorter = null;
            base.Items.AddRange(items);
            this._sorter = oldSorter;
            if (this._sorter != null && this._sorter.SortInfo.SortColumn >= 0)
            {
                this.Sort();
            }
        }

        /// <summary>
        /// Attach the menu items for AutoResize / Grouping / Filtering for the list view
        /// </summary>
        /// <param name="targetMenuStrip">
        /// </param>
        public void AttachMenuStrip(ToolStripDropDown targetMenuStrip)
        {
            var mnuAutoSize = new ToolStripMenuItem("Auto resize columns");
            mnuAutoSize.ShortcutKeys = Keys.F8;
            mnuAutoSize.Click += (o, e) => this.AutoResize();
            targetMenuStrip.Items.Add(mnuAutoSize);

            // Attach the grouping menu strip
            var mnuGroupBy = new ToolStripMenuItem("Group by");
            var mnuUngroup = new RadioToolStripMenuItem("Ungroup", -1);
            this._groupStrip.Clear();

            // Update the list of menustrip so that we can make it checked when setting group index
            var mnuGroupBySeparator = new ToolStripSeparator();
            targetMenuStrip.Items.Add(mnuGroupBySeparator);
            targetMenuStrip.Items.Add(mnuGroupBy);
            mnuUngroup.Click += this.GroupByMenuItem_Click;
            mnuGroupBy.DropDownItems.Add(mnuUngroup);
            mnuGroupBy.DropDownItems.Add(new ToolStripSeparator());
            for (var i = 0; i < this.Columns.Count; i++)
            {
                var newItem = new RadioToolStripMenuItem(this.Columns[i].Text, i);
                newItem.Click += this.GroupByMenuItem_Click;
                mnuGroupBy.DropDownItems.Add(newItem);
                this._groupStrip.Add(i, newItem);
            }

            var mnuFilterData = new ToolStripMenuItem("Filters");
            mnuFilterData.DropDownOpening += this.FilterDataMenuStrip_DropDownOpening;
            var mnuFilterSeparator = new ToolStripSeparator();
            if (targetMenuStrip.Items.Count > 0)
            {
                targetMenuStrip.Items.Add(mnuFilterSeparator);
            }

            targetMenuStrip.Items.Add(mnuFilterData);

            targetMenuStrip.Opening +=
                (s, e) =>
                mnuGroupBySeparator.Available =
                mnuGroupBy.Available =
                mnuFilterData.Available = mnuFilterSeparator.Available = this.Items.Count > 0 && this.Columns.Count > 0 && !this.VirtualMode;
        }

        /// <summary>Auto resize columns depending on the length of the column header and items</summary>
        public void AutoResize()
        {
            this.BeginUpdate();
            try
            {
                var colSize = new int[this.Columns.Count];

                // Measure the column headers' width
                for (var i = 0; i < this.Columns.Count; i++)
                {
                    colSize[i] = TextRenderer.MeasureText(this.Columns[i].Text, this.Font).Width;
                }

                // Measure the item width
                var topItem = this.TopItem;
                var totalHeight = this.Bounds.Height;
                var totalItems = this.Items.Count;
                int startIdx, endIdx;

                if (this.VirtualMode)
                {
                    var itemsCount = (int)Math.Ceiling((double)totalHeight / topItem.Bounds.Height) - 1;

                    // Get visible items count (minus the header)
                    startIdx = topItem.Index;
                    endIdx = startIdx + itemsCount;
                }
                else
                {
                    startIdx = 0;
                    endIdx = totalItems;
                }

                if (endIdx > totalItems)
                {
                    endIdx = totalItems;
                }

                for (var c = 0; c < this.Columns.Count; c++)
                {
                    for (var i = startIdx; i < endIdx; i++)
                    {
                        var item = this.Items[i];
                        var cellWidth = TextRenderer.MeasureText(item.SubItems[c].Text, item.SubItems[c].Font).Width;
                        if (cellWidth > colSize[c])
                        {
                            colSize[c] = cellWidth;
                        }
                    }

                    this.Columns[c].Width = colSize[c] + 5; // Set the width plus some padding
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        /// <summary>
        /// Get the index of the column header
        /// </summary>
        /// <param name="col">
        /// The col.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetColumn(ColumnHeader col)
        {
            for (var i = 0; i < this.Columns.Count; i++)
            {
                if (this.Columns[i] == col)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// The get item text.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetItemText(ListViewItem i, int c)
        {
            // ensure we have an item to work with here...
            if (i != null)
            {
                // if column 0 return the text
                if (c == 0)
                {
                    return i.Text;
                }

                // not 0, ensure that the subitem is valid and exists
                if ((c < i.SubItems.Count) && (i.SubItems[c] != null))
                {
                    return i.SubItems[c].Text;
                }
            }

            // not valid item/subitem return empty string
            return string.Empty;
        }

        /// <summary>Resize columns so that all of them have a uniform width</summary>
        public void ResizeColumnsEvenly()
        {
            if (this.Columns.Count < 1)
            {
                return;
            }

            this.BeginUpdate();
            try
            {
                var colSize = this.Bounds.Width / this.Columns.Count;
                foreach (ColumnHeader col in this.Columns)
                {
                    col.Width = colSize;
                }
            }
            finally
            {
                this.EndUpdate();
            }
        }

        /// <summary>Sort ListVIew Items</summary>
        public new void Sort()
        {
            if (this._header.SortInfo.SortColumn < 0)
            {
                return;
            }

            if (this.VirtualMode)
            {
                if (this.VirtualModeSort == null)
                {
                    return;
                }

                this.VirtualModeSort(this);
                this.Invalidate();
            }
            else
            {
                base.Sort();
            }

            this.SetSortIcon(this._header.SortInfo.SortColumn, this._header.SortInfo.SortAscending);
        }

        /// <summary>
        /// Export all list view items to CSV file
        /// </summary>
        /// <param name="dataFilePath">
        /// The data File Path.
        /// </param>
        public void ToCsv(string dataFilePath)
        {
            using (var w = new StreamWriter(dataFilePath, false, Encoding.Default))
            {
                foreach (ColumnHeader col in this.Columns)
                {
                    w.Write("\"" + col.Text + "\",");
                }

                w.Write(Environment.NewLine);

                for (var i = 0; i < this.Items.Count; i++)
                {
                    var item = this.Items[i];
                    foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                    {
                        w.Write("\"" + subItem.Text + "\",");
                    }

                    w.Write(Environment.NewLine);
                }
            }
        }

        /// <summary>
        /// Do filtering of item and store the items in corresponding list
        /// </summary>
        /// <param name="activeData">
        /// The active Data.
        /// </param>
        /// <param name="filteredData">
        /// The filtered Data.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        /// <param name="filterMethod">
        /// The filter Method.
        /// </param>
        /// <returns>
        /// Change occured after the filtering
        /// </returns>
        public static bool DoFilter<T>(List<T> activeData, 
                                       List<T> filteredData, 
                                       List<ListViewFilter> filters, 
                                       ListViewFilterResultMethod<T> filterMethod)
        {
            var holdingData = new List<T>();
            var isChanged = false;
            if (filters.Count > 0)
            {
                for (var i = 0; i < activeData.Count; ++i)
                {
                    var curItem = activeData[i];
                    foreach (var f in filters)
                    {
                        if (!filterMethod(curItem, f))
                        {
                            // Check if the item passes the filter
                            holdingData.Add(curItem);
                            activeData.RemoveAt(i--);
                            isChanged = true;
                            break;
                        }
                    }
                }

                for (var i = 0; i < filteredData.Count; ++i)
                {
                    var curItem = filteredData[i];
                    var r = true;

                    foreach (var f in filters)
                    {
                        if (!filterMethod(curItem, f))
                        {
                            // Check if the item passes the filter
                            r = false;
                            break;
                        }
                    }

                    if (r)
                    {
                        activeData.Add(curItem);
                        filteredData.RemoveAt(i--);
                        isChanged = true;
                    }
                }

                filteredData.AddRange(holdingData);
            }
            else
            {
                isChanged = filteredData.Count > 0;
                foreach (var item in filteredData)
                {
                    activeData.Add(item);
                }

                filteredData.Clear();
            }

            return isChanged;
        }

        /// <summary>
        /// Create a new filter entry for the given column.  This will replace/add a LVFFilter instance for the column to the itm_filtrs array for use
        /// by FilterUpdate().
        /// </summary>
        /// <param name="column">
        /// Column number
        /// </param>
        private void BuildFilter(int column)
        {
            // if this column exists in the filters array remove it
            foreach (var f in this._activeFilters)
            {
                if (f.Column == column)
                {
                    this._activeFilters.Remove(f);
                    break;
                }
            }

            // get the filter text for this column from the header
            var filterString = this._header.Filters[column].Trim();

            // if there is any size to it then create a new filter
            if (filterString.Length > 0)
            {
                // create a new filter object for this column
                var f = new ListViewFilter
                            {
                                Column = column, 
                                FilterString = filterString, 
                                Compare = FilterComparison.Equal, 
                                Type = this._header.DataType[column]
                            };

                // check the first characters of the string to see
                // if this is not a default equality comparison
                switch (filterString[0])
                {
                    case '=':
                        f.FilterString = filterString.Remove(0, 1);
                        break;
                    case '!':
                        f.Compare = FilterComparison.NotEqual;
                        f.FilterString = filterString.Remove(0, 1);
                        break;
                    case '>':
                        if ((filterString.Length > 1) && (filterString[1] == '='))
                        {
                            f.Compare = FilterComparison.GreaterEqual;
                            f.FilterString = filterString.Remove(0, 2);
                        }
                        else
                        {
                            f.Compare = FilterComparison.Greater;
                            f.FilterString = filterString.Remove(0, 1);
                        }

                        break;
                    case '<':
                        if ((filterString.Length > 1) && (filterString[1] == '='))
                        {
                            f.Compare = FilterComparison.LessEqual;
                            f.FilterString = filterString.Remove(0, 2);
                        }
                        else
                        {
                            f.Compare = FilterComparison.Less;
                            f.FilterString = filterString.Remove(0, 1);
                        }

                        break;
                }

                // add this to the array of filters
                this._activeFilters.Add(f);
            }
        }

        /// <summary>Filter the list view items</summary>
        private void ApplyFilters()
        {
            if (this.VirtualMode)
            {
                if (this.VirtualModeFilter == null)
                {
                    return;
                }

                this.VirtualModeFilter(this, this._activeFilters);
                this.Invalidate();
            }
            else
            {
                this.BeginUpdate();

                try
                {
                    var itemList = new List<ListViewItem>();
                    for (var i = 0; i < this.Items.Count; i++)
                    {
                        itemList.Add(this.Items[i]);
                    }

                    var isChanged = DoFilter(itemList, this._filteredItems, this._activeFilters, this.FilterItem);
                    if (isChanged)
                    {
                        this.Items.Clear();
                        this.AddRange(itemList.ToArray());
                        this.Sort(); // resort the items if the item content has changed
                    }
                }
                finally
                {
                    // ensure that updates are re-enabled
                    this.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Apply a filter to a list view item
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="f">
        /// The f.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterItem(ListViewItem i, ListViewFilter f)
        {
            // get the result of the data type comparison
            var r = this._isFilterIgnoreCase
                        ? this.CompareData(
                            this.GetItemText(i, f.Column).ToLower(CultureInfo.InvariantCulture), 
                            f.FilterString.ToLower(CultureInfo.InvariantCulture), 
                            f.Type)
                        : this.CompareData(this.GetItemText(i, f.Column), f.FilterString, f.Type);

            // compare the result against the filter comparison type
            switch (f.Compare)
            {
                case FilterComparison.Equal:
                    return r == 0;
                case FilterComparison.NotEqual:
                    return r != 0;
                case FilterComparison.Greater:
                    return r > 0;
                case FilterComparison.GreaterEqual:
                    return r >= 0;
                case FilterComparison.Less:
                    return r < 0;
                case FilterComparison.LessEqual:
                    return r <= 0;
                default:
                    return r == 0;
            }
        }

        /// <summary>
        /// Compare two strings and return a -/=/+ result
        /// </summary>
        /// <param name="xText">
        /// The x Text.
        /// </param>
        /// <param name="yText">
        /// The y Text.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// Less, Equal, Greater as -1,0,1
        /// </returns>
        private int CompareData(string xText, string yText, FilterDataType type)
        {
            switch (type)
            {
                case FilterDataType.String:
                    var match = xText.IsMatch(yText); // Match string with wildcards
                    if (match)
                    {
                        return 0;
                    }

                    return string.Compare(xText, yText, StringComparison.Ordinal);

                case FilterDataType.Number:
                    float xFloat, yFloat;
                    float.TryParse(xText, out xFloat);
                    float.TryParse(yText, out yFloat);
                    return xFloat.CompareTo(yFloat);

                case FilterDataType.Date:
                    DateTime xDate, yDate;
                    DateTime.TryParse(xText, out xDate);
                    DateTime.TryParse(yText, out yDate);
                    return DateTime.Compare(xDate, yDate);

                default:
                    throw new ArgumentException("Unsupported filter data type: " + type);
            }
        }

        /// <summary>
        /// The on items added.
        /// </summary>
        /// <param name="addedItems">
        /// The added items.
        /// </param>
        private void OnItemsAdded(List<ListViewItem> addedItems)
        {
            if (this.ItemsAdded != null)
            {
                this.ItemsAdded(this, addedItems);
            }
        }

        /// <summary>
        /// The on item removed.
        /// </summary>
        /// <param name="removedItem">
        /// The removed item.
        /// </param>
        private void OnItemRemoved(ListViewItem removedItem)
        {
            if (this.ItemRemoved != null)
            {
                this.ItemRemoved(this, removedItem);
            }
        }
    }
}