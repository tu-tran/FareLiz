namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data.Csv;

    /// <summary>
    /// The csv data type.
    /// </summary>
    public enum CsvDataType
    {
        /// <summary>
        /// The csv file.
        /// </summary>
        CsvFile, 

        /// <summary>
        /// The csv string.
        /// </summary>
        CsvString
    }

    /// <summary>
    /// The csv list view.
    /// </summary>
    public class CsvListView : EnhancedListView
    {
        /// <summary>
        /// The _data lines.
        /// </summary>
        private readonly List<string[]> _dataLines = new List<string[]>();

        /// <summary>
        /// The _filtered lines.
        /// </summary>
        private readonly List<string[]> _filteredLines = new List<string[]>();

        /// <summary>
        /// The _sorter.
        /// </summary>
        private readonly CsvListViewColumnSorter _sorter = new CsvListViewColumnSorter();

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvListView"/> class.
        /// </summary>
        public CsvListView()
        {
            this.ListViewItemSorter = this._sorter;
            this.VirtualMode = true;
            this.RetrieveVirtualItem += this.CsvListView_RetrieveVirtualItem;
            this.VirtualModeFilter = this.CsvListViewVirtualModeFilter;
            this.VirtualModeSort = this.CsvListVIewVirtualModeSort;
            this.View = View.Details;
        }

        /// <summary>
        /// The csv list v iew virtual mode sort.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        private void CsvListVIewVirtualModeSort(EnhancedListView listView)
        {
            this._dataLines.Sort(this._sorter);
        }

        /// <summary>
        /// The csv list view virtual mode filter.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        private void CsvListViewVirtualModeFilter(EnhancedListView listView, List<ListViewFilter> filters)
        {
            if (DoFilter(this._dataLines, this._filteredLines, filters, this.FilterLine))
            {
                this.VirtualListSize = this._dataLines.Count;
            }
        }

        /// <summary>
        /// The filter line.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterLine(string[] data, ListViewFilter filter)
        {
            int index = filter.Column;
            return data[index].IsMatch(filter.FilterString);
        }

        /// <summary>
        /// The bind data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void BindData(string data, CsvDataType type)
        {
            if (string.IsNullOrEmpty(data) || (type == CsvDataType.CsvFile && !File.Exists(data)))
            {
                throw new ArgumentException("Invalid input data for CsvListView. Check that the data is not empty or the file exists");
            }

            this.Clear();
            this._dataLines.Clear();
            this._filteredLines.Clear();
            this.VirtualListSize = 0;

            TextReader stream = null;
            if (type == CsvDataType.CsvFile)
            {
                stream = new StreamReader(data);
            }
            else
            {
                stream = new StringReader(data);
            }

            using (stream)
            {
                using (var reader = new CsvReader(stream, true) { DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine })
                {
                    var headers = reader.GetFieldHeaders();
                    int fieldCount = headers.Length;
                    var colWidth = this.ClientSize.Width / fieldCount;

                    foreach (var header in headers)
                    {
                        var newCol = this.Columns.Add(header);
                        newCol.Width = colWidth;
                    }

                    while (reader.ReadNextRecord())
                    {
                        var item = new string[fieldCount];
                        for (int i = 0; i < fieldCount; i++)
                        {
                            item[i] = reader[i];
                        }

                        this._dataLines.Add(item);
                    }
                }
            }

            this.VirtualListSize = this._dataLines.Count;
            this.Invalidate();
        }

        /// <summary>
        /// The csv list view_ retrieve virtual item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CsvListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var itemIdx = e.ItemIndex;
            e.Item = new ListViewItem(this._dataLines[itemIdx]);
        }

        /// <summary>
        /// The csv list view column sorter.
        /// </summary>
        private class CsvListViewColumnSorter : ListViewTextColumnSorter, IComparer<string[]>
        {
            /// <summary>
            /// The compare.
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
            public int Compare(string[] x, string[] y)
            {
                string xText = x[this.SortInfo.SortColumn], yText = y[this.SortInfo.SortColumn];
                int result = StringLogicalComparer.Compare(xText, yText);
                return this.SortInfo.SortAscending ? result : -result;
            }
        }
    }
}