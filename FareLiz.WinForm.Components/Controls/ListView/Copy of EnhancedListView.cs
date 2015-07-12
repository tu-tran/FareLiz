using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SkyDean.FareLiz.Core.Utils;

namespace SkyDean.FareLiz.WinForm.Controls
{
    /// <summary>
    /// Extended ListView control with sorting/filtering capabilities and support for Virtual Mode data handling
    /// </summary>
    [ToolboxBitmap(typeof(ListView))]
    [DefaultEvent("ColumnClick")]
    public partial class EnhancedListView : ListView
    {
        private int _filterButtonColumn; // Filter button column
        private readonly ListViewColumnHeader _header = new ListViewColumnHeader(true);
        private readonly List<ListViewItem> _filteredItems = new List<ListViewItem>(); // Filtered out items
        private readonly List<ListViewFilter> _activeFilters = new List<ListViewFilter>(); // Array of active LVFFliters        
        private readonly Dictionary<int, RadioToolStripMenuItem> _groupStrip = new Dictionary<int, RadioToolStripMenuItem>();

        public ListViewMethod VirtualModeSort;
        public ListViewFilterMethod VirtualModeFilter;

        [Description("In Details mode, the header will display filter controls."), Category("Behavior"), DefaultValue(false)]
        public bool Filtered
        {
            get { return _header.Filtered; }
            set
            {
                if (_header.Filtered != value)
                {
                    _header.Filtered = value;
                    Refresh();
                }
            }
        }

        private bool _isFilterIgnoreCase = true; // Ignore case on filter
        [Description("Treat filter strings case-insensitive for comparison."), Category("Behavior"), DefaultValue(true)]
        public bool FilterIgnoreCase
        {
            get { return _isFilterIgnoreCase; }
            set
            {
                if (_isFilterIgnoreCase != value)
                {
                    _isFilterIgnoreCase = value;
                    ApplyFilters();
                }
            }
        }

        private bool _doubleBuffering;
        public bool DoubleBuffering
        {
            get { return _doubleBuffering; }
            set
            {
                if (_doubleBuffering != value)
                {
                    this.SetDoubleBuffering(value);
                    _doubleBuffering = value;
                }
            }
        }

        int _groupingCol = -1;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public int GroupColumnIndex
        {
            get { return _groupingCol; }
            set
            {
                if (value >= Columns.Count)
                    throw new ArgumentException("Invalid grouping column index: " + value);

                _groupingCol = value;
                if (value > -1)
                    AutoGroup();
            }
        }

        private ListViewColumnSorter _sorter;
        public new ListViewColumnSorter ListViewItemSorter
        {
            get { return _sorter; }
            set
            {
                base.ListViewItemSorter = _sorter = value;
                if (_sorter != null)
                    _sorter.SortInfo = _header.SortInfo;
            }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool ShowGroups
        {
            get { return GroupColumnIndex > -1; }
        }

        private bool _collapsibleGroup = false;
        public bool CollapsibleGroup
        { get { return _collapsibleGroup; } set { SetGroupState(value); _collapsibleGroup = value; } }

        /// <summary>
        /// Get the first selected item index. Return -1 if there is no selected item
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public ListViewItem FirstSelectedItem
        {
            get
            {
                int selIndex = SelectedIndices.Count > 0 ? SelectedIndices[0] : -1;
                return selIndex > -1 ? Items[selIndex] : null;
            }
        }

        public EnhancedListView(ListViewColumnSorter sorter)
        {
            InitializeComponent();
            ListViewItemSorter = sorter;
            KeyDown += EnhancedListView_KeyDown;
        }

        public EnhancedListView()
            : this(null)
        {
        }

        public void AddRange(ListViewItem[] items)
        {
            var oldSorter = _sorter;
            _sorter = null;
            base.Items.AddRange(items);
            _sorter = oldSorter;
            if (_sorter != null && _sorter.SortInfo.SortColumn >= 0)
                Sort();
        }

        /// <summary>
        /// Attach the menu items for AutoResize / Grouping / Filtering for the list view
        /// </summary>
        /// <param name="targetMenuStrip"></param>
        public void AttachMenuStrip(ToolStripDropDown targetMenuStrip)
        {
            var mnuAutoSize = new ToolStripMenuItem("Auto resize columns");
            mnuAutoSize.ShortcutKeys = Keys.F8;
            mnuAutoSize.Click += ((o, e) => AutoResize());
            targetMenuStrip.Items.Add(mnuAutoSize);

            // Attach the grouping menu strip
            var mnuGroupBy = new ToolStripMenuItem("Group by");
            var mnuUngroup = new RadioToolStripMenuItem("Ungroup", -1);
            _groupStrip.Clear();    // Update the list of menustrip so that we can make it checked when setting group index
            var mnuGroupBySeparator = new ToolStripSeparator();
            targetMenuStrip.Items.Add(mnuGroupBySeparator);
            targetMenuStrip.Items.Add(mnuGroupBy);
            mnuUngroup.Click += GroupByMenuItem_Click;
            mnuGroupBy.DropDownItems.Add(mnuUngroup);
            mnuGroupBy.DropDownItems.Add(new ToolStripSeparator());
            for (int i = 0; i < Columns.Count; i++)
            {
                var newItem = new RadioToolStripMenuItem(Columns[i].Text, i);
                newItem.Click += GroupByMenuItem_Click;
                mnuGroupBy.DropDownItems.Add(newItem);
                _groupStrip.Add(i, newItem);
            }

            var mnuFilterData = new ToolStripMenuItem("Filters");
            mnuFilterData.DropDownOpening += FilterDataMenuStrip_DropDownOpening;
            var mnuFilterSeparator = new ToolStripSeparator();
            if (targetMenuStrip.Items.Count > 0)
                targetMenuStrip.Items.Add(mnuFilterSeparator);
            targetMenuStrip.Items.Add(mnuFilterData);

            targetMenuStrip.Opening += ((s, e) => mnuGroupBySeparator.Available = mnuGroupBy.Available = mnuFilterData.Available = mnuFilterSeparator.Available = (Items.Count > 0 && Columns.Count > 0 && !VirtualMode));
        }

        /// <summary>
        /// Auto resize columns depending on the length of the column header and items
        /// </summary>
        public void AutoResize()
        {
            BeginUpdate();
            try
            {
                var colSize = new int[Columns.Count];
                // Measure the column headers' width
                for (int i = 0; i < Columns.Count; i++)
                {
                    colSize[i] = TextRenderer.MeasureText(Columns[i].Text, Font).Width;
                }

                // Measure the item width
                var topItem = TopItem;
                var totalHeight = Bounds.Height;
                var totalItems = Items.Count;
                int startIdx, endIdx;

                if (VirtualMode)
                {
                    var itemsCount = (int)Math.Ceiling((double)totalHeight / topItem.Bounds.Height) - 1; // Get visible items count (minus the header)
                    startIdx = topItem.Index;
                    endIdx = startIdx + itemsCount;
                }
                else
                {
                    startIdx = 0;
                    endIdx = totalItems;
                }

                if (endIdx > totalItems)
                    endIdx = totalItems;

                for (int c = 0; c < Columns.Count; c++)
                {
                    for (int i = startIdx; i < endIdx; i++)
                    {
                        var item = Items[i];
                        int cellWidth = TextRenderer.MeasureText(item.SubItems[c].Text, item.SubItems[c].Font).Width;
                        if (cellWidth > colSize[c])
                            colSize[c] = cellWidth;
                    }

                    Columns[c].Width = colSize[c] + 5;  // Set the width plus some padding
                }
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Group the items according to the GroupIndexColumn
        /// </summary>
        public void AutoGroup()
        {
            if (GroupColumnIndex < 0)
                throw new ArgumentException("GroupColumnIndex was not set properly");

            if (VirtualMode)
                throw new InvalidOperationException("Could not group items in Virtual Mode");

            if (Items.Count > 0)
            {
                BeginUpdate();
                try
                {
                    Groups.Clear();

                    for (int i = 0; i < Items.Count; i++)
                    {
                        var item = Items[i];
                        string groupName = item.SubItems[GroupColumnIndex].Text;
                        ListViewGroup group = null;
                        foreach (ListViewGroup g in Groups)
                        {
                            if (g.Header == groupName || g.Name == groupName)
                            {
                                group = g;
                                break;
                            }
                        }

                        if (group == null)
                        {
                            group = new ListViewGroup(groupName);
                            Groups.Add(group);
                        }

                        item.Group = group;
                    }
                }
                finally
                {
                    EndUpdate();
                }
            }
        }

        /// <summary>
        /// Get the index of the column header
        /// </summary>
        public int GetColumn(ColumnHeader col)
        {
            for (int i = 0; i < Columns.Count; i++)
                if (Columns[i] == col)
                    return i;

            return -1;
        }

        public string GetItemText(ListViewItem i, int c)
        {
            // ensure we have an item to work with here...
            if (i != null)
            {
                // if column 0 return the text
                if (c == 0) return i.Text;

                // not 0, ensure that the subitem is valid and exists
                if ((c < i.SubItems.Count) && (i.SubItems[c] != null))
                    return i.SubItems[c].Text;
            }

            // not valid item/subitem return empty string
            return "";
        }

        /// <summary>
        /// Resize columns so that all of them have a uniform width
        /// </summary>
        public void ResizeColumnsEvenly()
        {
            if (Columns.Count < 1)
                return;

            BeginUpdate();
            try
            {
                int colSize = Bounds.Width / Columns.Count;
                foreach (ColumnHeader col in Columns)
                    col.Width = colSize;
            }
            finally
            {
                EndUpdate();
            }
        }

        /// <summary>
        /// Set selected column as the base column for grouping. This method does not refresh the view (Call AutoGroup() to refresh the view with the new group)
        /// </summary>
        /// <param name="col"></param>
        public void SetGroupColumn(ColumnHeader col)
        {
            var colIndex = GetColumn(col);

            if (colIndex > -1)
                SetGroupColumn(colIndex);
            else
                throw new ArgumentException("Could not set group column since the column does not belong to the ListView");
        }

        /// <summary>
        /// Set the column to be used for grouping
        /// </summary>
        public void SetGroupColumn(int index)
        {
            GroupColumnIndex = index;
            AutoGroup();
            RadioToolStripMenuItem item;
            if (_groupStrip.TryGetValue(index, out item))
                item.Checked = true;
        }

        /// <summary>
        /// Sort ListVIew Items
        /// </summary>
        public new void Sort()
        {
            if (_header.SortInfo.SortColumn < 0)
                return;

            if (VirtualMode)
            {
                if (VirtualModeSort == null)
                    return;

                VirtualModeSort(this);
                Invalidate();
            }
            else
                base.Sort();
            SetSortIcon(_header.SortInfo.SortColumn, _header.SortInfo.SortAscending);
        }

        /// <summary>
        /// Export all list view items to CSV file
        /// </summary>
        public void ToCsv(string dataFilePath)
        {
            using (var w = new StreamWriter(dataFilePath, false, Encoding.Default))
            {
                foreach (ColumnHeader col in Columns)
                {
                    w.Write("\"" + col.Text + "\",");
                }
                w.Write(Environment.NewLine);

                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
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
        /// <returns>Change occured after the filtering</returns>
        public static bool DoFilter<T>(List<T> activeData, List<T> filteredData, List<ListViewFilter> filters, ListViewFilterResultMethod<T> filterMethod)
        {
            var holdingData = new List<T>();
            bool isChanged = false;
            if (filters.Count > 0)
            {
                for (int i = 0; i < activeData.Count; ++i)
                {
                    T curItem = activeData[i];
                    foreach (ListViewFilter f in filters)
                    {
                        if (!filterMethod(curItem, f))    // Check if the item passes the filter
                        {
                            holdingData.Add(curItem);
                            activeData.RemoveAt(i--);
                            isChanged = true;
                            break;
                        }
                    }
                }

                for (int i = 0; i < filteredData.Count; ++i)
                {
                    T curItem = filteredData[i];
                    bool r = true;

                    foreach (ListViewFilter f in filters)
                    {
                        if (!filterMethod(curItem, f))    // Check if the item passes the filter
                        {
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
                isChanged = (filteredData.Count > 0);
                foreach (T item in filteredData)
                    activeData.Add(item);
                filteredData.Clear();
            }

            return isChanged;
        }

        private void SetGroupState(bool collapsible)
        {
            if (!SupportCollapsibleGroups)
                return;

            foreach (ListViewGroup lvg in Groups)
                SetGroupState(lvg, collapsible);
        }

        //Only Vista and forward allows collaps of ListViewGroups
        private bool SupportCollapsibleGroups { get { return Environment.OSVersion.Version.Major < 6; } }

        private void SetGroupState(ListViewGroup lstvwgrp, bool collapsible)
        {
            if (!SupportCollapsibleGroups)
                return;

            if (lstvwgrp == null || lstvwgrp.ListView == null)
                return;

            if (lstvwgrp.ListView.InvokeRequired)
                lstvwgrp.ListView.Invoke(new CallBackSetGroupState(SetGroupState), lstvwgrp, collapsible);
            else
            {
                int? GrpId = GetGroupID(lstvwgrp);
                int gIndex = lstvwgrp.ListView.Groups.IndexOf(lstvwgrp);
                LVGROUP group = new LVGROUP();
                group.CbSize = Marshal.SizeOf(group);
                group.State = state;
                group.Mask = ListViewGroupMask.State;
                if (GrpId != null)
                {
                    group.IGroupId = GrpId.Value;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, GrpId.Value, group);
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, GrpId.Value, group);
                }
                else
                {
                    group.IGroupId = gIndex;
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                    SendMessage(lstvwgrp.ListView.Handle, LVM_SETGROUPINFO, gIndex, group);
                }
                lstvwgrp.ListView.Refresh();
            }
        }

        /// <summary>
        ///     Create a new filter entry for the given column.  This
        ///     will replace/add a LVFFilter instance for the column
        ///     to the itm_filtrs array for use by FilterUpdate().
        /// </summary>
        /// <param name="column">Column number</param>
        private void BuildFilter(int column)
        {
            // if this column exists in the filters array remove it
            foreach (ListViewFilter f in _activeFilters)
                if (f.Column == column)
                {
                    _activeFilters.Remove(f);
                    break;
                }

            // get the filter text for this column from the header
            string filterString = _header.Filters[column].Trim();

            // if there is any size to it then create a new filter
            if (filterString.Length > 0)
            {
                // create a new filter object for this column
                var f = new ListViewFilter
                {
                    Column = column,
                    FilterString = filterString,
                    Compare = FilterComparison.Equal,
                    Type = _header.DataType[column]
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
                _activeFilters.Add(f);
            }
        }

        /// <summary>
        /// Filter the list view items
        /// </summary>
        private void ApplyFilters()
        {
            if (VirtualMode)
            {
                if (VirtualModeFilter == null)
                    return;

                VirtualModeFilter(this, _activeFilters);
                Invalidate();
            }
            else
            {
                BeginUpdate();

                try
                {
                    var itemList = new List<ListViewItem>();
                    for (int i = 0; i < Items.Count; i++)
                        itemList.Add(Items[i]);

                    bool isChanged = DoFilter(itemList, _filteredItems, _activeFilters, FilterItem);
                    if (isChanged)
                    {
                        Items.Clear();
                        AddRange(itemList.ToArray());
                        Sort();  // resort the items if the item content has changed
                    }
                }
                finally
                {
                    // ensure that updates are re-enabled
                    EndUpdate();
                }
            }
        }

        /// <summary>
        /// Apply a filter to a list view item
        /// </summary>
        private bool FilterItem(ListViewItem i, ListViewFilter f)
        {
            // get the result of the data type comparison
            int r = (_isFilterIgnoreCase)
                        ? CompareData(GetItemText(i, f.Column).ToLower(CultureInfo.InvariantCulture), f.FilterString.ToLower(CultureInfo.InvariantCulture), f.Type)
                        : CompareData(GetItemText(i, f.Column), f.FilterString, f.Type);

            // compare the result against the filter comparison type
            switch (f.Compare)
            {
                case FilterComparison.Equal:
                    return (r == 0);
                case FilterComparison.NotEqual:
                    return (r != 0);
                case FilterComparison.Greater:
                    return (r > 0);
                case FilterComparison.GreaterEqual:
                    return (r >= 0);
                case FilterComparison.Less:
                    return (r < 0);
                case FilterComparison.LessEqual:
                    return (r <= 0);
                default:
                    return (r == 0);
            }
        }

        /// <summary>
        /// Compare two strings and return a -/=/+ result
        /// </summary>
        /// <returns>Less, Equal, Greater as -1,0,1</returns>
        public int CompareData(string xText, string yText, FilterDataType type)
        {
            switch (type)
            {
                case FilterDataType.String:
                    bool match = xText.IsMatch(yText);  // Match string with wildcards
                    if (match)
                        return 0;
                    return String.Compare(xText, yText, StringComparison.Ordinal);

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
                    throw new ArgumentException("Unsupported filter data type: " + type.ToString());
            }
        }
    }
}
