using SkyDean.FareLiz.Data.Csv;

namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    public enum CsvDataType { CsvFile, CsvString }
    public class CsvListView : EnhancedListView
    {
        private readonly List<string[]> _dataLines = new List<string[]>();
        private readonly List<string[]> _filteredLines = new List<string[]>();
        private readonly CsvListViewColumnSorter _sorter = new CsvListViewColumnSorter();

        public CsvListView()
        {
            this.ListViewItemSorter = this._sorter;
            this.VirtualMode = true;
            this.RetrieveVirtualItem += this.CsvListView_RetrieveVirtualItem;
            this.VirtualModeFilter = this.CsvListViewVirtualModeFilter;
            this.VirtualModeSort = this.CsvListVIewVirtualModeSort;
            this.View = View.Details;
        }

        private void CsvListVIewVirtualModeSort(EnhancedListView listView)
        {
            this._dataLines.Sort(this._sorter);
        }

        private void CsvListViewVirtualModeFilter(EnhancedListView listView, List<ListViewFilter> filters)
        {
            if (DoFilter<string[]>(this._dataLines, this._filteredLines, filters, this.FilterLine))
            {
                this.VirtualListSize = this._dataLines.Count;
            }
        }

        private bool FilterLine(string[] data, ListViewFilter filter)
        {
            int index = filter.Column;
            return data[index].IsMatch(filter.FilterString);
        }

        public void BindData(string data, CsvDataType type)
        {
            if (String.IsNullOrEmpty(data) || (type == CsvDataType.CsvFile && !File.Exists(data)))
                throw new ArgumentException("Invalid input data for CsvListView. Check that the data is not empty or the file exists");

            this.Clear();
            this._dataLines.Clear();
            this._filteredLines.Clear();
            this.VirtualListSize = 0;

            TextReader stream = null;
            if (type == CsvDataType.CsvFile)
                stream = new StreamReader(data);
            else
                stream = new StringReader(data);

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
                            item[i] = reader[i];
                        this._dataLines.Add(item);
                    }
                }
            }

            this.VirtualListSize = this._dataLines.Count;
            this.Invalidate();
        }

        private void CsvListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var itemIdx = e.ItemIndex;
            e.Item = new ListViewItem(this._dataLines[itemIdx]);
        }

        private class CsvListViewColumnSorter : ListViewTextColumnSorter, IComparer<string[]>
        {
            public int Compare(string[] x, string[] y)
            {
                string xText = x[this.SortInfo.SortColumn],
                       yText = y[this.SortInfo.SortColumn];
                int result = StringLogicalComparer.Compare(xText, yText);
                return this.SortInfo.SortAscending ? result : -result;
            }
        }
    }
}
