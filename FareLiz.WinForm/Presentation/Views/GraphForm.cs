namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Controls.Graph;
    using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>The graph form.</summary>
    internal partial class GraphForm : SmartForm
    {
        /// <summary>The _graph data.</summary>
        private readonly Dictionary<Journey, PointPairList> _graphData = new Dictionary<Journey, PointPairList>();

        /// <summary>The _hidden curves.</summary>
        private readonly CurveList _hiddenCurves = new CurveList();

        /// <summary>The _journey data.</summary>
        private readonly IEnumerable<Journey> _journeyData;

        /// <summary>The _suppress draw.</summary>
        private bool _suppressDraw;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphForm"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public GraphForm(IEnumerable<Journey> data)
        {
            this.InitializeComponent();

            this.Text = AppUtil.ProductName + " " + this.Text;
            this.splitContainer1.FixedPanel = FixedPanel.Panel1;
            this._journeyData = data.OrderBy(j => j.DepartureDate).ThenBy(j => j.ReturnDate);
            this.lvDataDate.ListViewItemSorter = new DateComparer();
            this.lvTravelPeriod.ListViewItemSorter = new TravelPeriodCompare();
            this.lvDataDate.ItemCheck += this.lvFilter_ItemCheck;
            this.lvTravelPeriod.ItemCheck += this.lvFilter_ItemCheck;
        }

        /// <summary>The initialize view.</summary>
        private void InitializeView()
        {
            GraphPane graphPane = this.graph.GraphPane;

            // Set the title and axis labels            
            graphPane.Title.FontSpec.Family = this.Font.FontFamily.Name;
            graphPane.Title.FontSpec.Size = 14;
            graphPane.Title.Text = "Fare Statistics";

            graphPane.XAxis.Scale.FontSpec.Size = 12;
            graphPane.XAxis.Title.FontSpec.Size = 14;
            graphPane.XAxis.Title.Text = "Data Date";
            graphPane.XAxis.Type = AxisType.Date;

            graphPane.YAxis.Scale.FontSpec.Size = 12;
            graphPane.YAxis.Title.FontSpec.Size = 14;
            graphPane.YAxis.Title.Text = "Price";

            graphPane.Legend.Border.IsVisible = false;
            graphPane.Legend.Position = LegendPos.Bottom;
            graphPane.Legend.FontSpec.Size = 13;

            graphPane.IsFontsScaled = false;
            this.PopulateData();
        }

        /// <summary>The populate data.</summary>
        private void PopulateData()
        {
            Dictionary<string, object> travelPeriod = new Dictionary<string, object>();
            Dictionary<string, object> dataDate = new Dictionary<string, object>();

            foreach (var j in this._journeyData)
            {
                var travelDate = new DatePeriod(j.DepartureDate, j.ReturnDate);
                travelPeriod.Add(j.DepartureDate.ToString("ddd dd/MM/yyyy") + " - " + j.ReturnDate.ToString("ddd dd/MM/yyyy"), travelDate);
                var points = new PointPairList();

                var sortedData = j.Data.OrderBy(d => d.DataDate);
                foreach (var d in sortedData)
                {
                    var cheapestFlight = d.Flights.OrderBy(f => f.Price).FirstOrDefault();
                    if (cheapestFlight != null)
                    {
                        var str = d.DataDate.ToLongDateString();
                        if (!dataDate.ContainsKey(str))
                        {
                            dataDate.Add(str, d.DataDate.Date);
                        }

                        var x = (double)new XDate(d.DataDate);
                        var newPoint = new PointPair(x, cheapestFlight.Price) { Tag = cheapestFlight };
                        points.Add(newPoint);
                    }
                }

                this._graphData.Add(j, points);
            }

            // Render listview
            this.PopulateListView(travelPeriod, this.lvTravelPeriod, false);
            this.PopulateListView(dataDate, this.lvDataDate, true);
        }

        /// <summary>The render graph.</summary>
        private void RenderGraph()
        {
            GraphPane graphPane = this.graph.GraphPane;
            Random rand = new Random(DateTime.Now.Millisecond);
            var usedColors = new HashSet<string>();
            int count = 0;

            foreach (var pair in this._graphData)
            {
                Journey j = pair.Key;
                string period = StringUtil.GetPeriodString(j.DepartureDate, j.ReturnDate);
                string colorHex;

                do
                {
                    colorHex = string.Format("#{0:X6}", rand.Next(0x1000000) & 0x5F5F5F);
                }
                while (usedColors.Contains(colorHex));

                usedColors.Add(colorHex);
                Color curveColor = ColorTranslator.FromHtml(colorHex);
                LineItem curve = new LineItem(period, pair.Value, curveColor, SymbolType.Square);
                var curveTarget = count++ == 0 ? graphPane.CurveList : this._hiddenCurves;
                curveTarget.Add(curve);

                curve.Tag = new DatePeriod(j.DepartureDate, j.ReturnDate);
                curve.Line.Width = 2;

                foreach (ListViewItem item in this.lvTravelPeriod.Items)
                {
                    var travelDate = item.Tag as DatePeriod;
                    if (travelDate != null && travelDate.StartDate == j.DepartureDate && travelDate.EndDate == j.ReturnDate)
                    {
                        item.ForeColor = curveColor;
                        break;
                    }
                }
            }

            graphPane.AxisChange();
        }

        /// <summary>
        /// The populate list view.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="checkAll">
        /// The check all.
        /// </param>
        private void PopulateListView(Dictionary<string, object> data, ListView listView, bool checkAll)
        {
            var newItems = new ListViewItem[data.Count];
            int i = 0;
            foreach (var pair in data)
            {
                newItems[i] = new ListViewItem(pair.Key) { Checked = i++ == 0 || checkAll, Tag = pair.Value };
            }

            listView.Items.AddRange(newItems);
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /// <summary>
        /// The set all items checked.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="isChecked">
        /// The is checked.
        /// </param>
        private void SetAllItemsChecked(ListView listView, bool isChecked)
        {
            this.lblStatus.Text = "Processing data...";
            this.graph.SuspendLayout();
            this._suppressDraw = true;
            try
            {
                foreach (ListViewItem item in listView.Items)
                {
                    item.Checked = isChecked;
                }

                this.lblStatus.Text = "Idle";
            }
            catch (Exception ex)
            {
                this.lblStatus.Text = ex.Message;
            }
            finally
            {
                this._suppressDraw = false;
                this.RefreshGraph();
            }
        }

        /// <summary>
        /// The update graph.
        /// </summary>
        /// <param name="isAdding">
        /// The is adding.
        /// </param>
        /// <param name="tagData">
        /// The tag data.
        /// </param>
        private void UpdateGraph(bool isAdding, object tagData)
        {
            try
            {
                if (tagData is DateTime)
                {
                    // Change data date
                    var date = (DateTime)tagData;

                    foreach (var pair in this._graphData)
                    {
                        var pointList = pair.Value;
                        if (isAdding)
                        {
                            var journey = pair.Key;
                            foreach (var d in journey.Data)
                            {
                                if (d.DataDate.Date == date && d.Flights.Count > 0)
                                {
                                    Flight cheapestFlight = d.Flights[0];
                                    double cheapestPrice = cheapestFlight.Price;
                                    foreach (var f in d.Flights)
                                    {
                                        if (f.Price < cheapestPrice)
                                        {
                                            cheapestFlight = f;
                                            cheapestPrice = f.Price;
                                        }
                                    }

                                    var newPoint = new PointPair(new XDate(d.DataDate), cheapestPrice) { Tag = cheapestFlight };
                                    pointList.Add(newPoint);
                                }
                            }

                            pointList.Sort(SortType.XValues);
                        }
                        else
                        {
                            for (int i = 0; i < pointList.Count; i++)
                            {
                                var point = pointList[i];
                                if (new XDate(point.X).DateTime.Date == date.Date)
                                {
                                    pointList.RemoveAt(i--);
                                }
                            }
                        }
                    }
                }
                else if (tagData is DatePeriod)
                {
                    // Change travel period
                    var date = (DatePeriod)tagData;
                    foreach (var pair in this._graphData)
                    {
                        var pointList = pair.Value;
                        var targetCurveList = isAdding ? this._hiddenCurves : this.graph.GraphPane.CurveList;
                        for (int i = 0; i < targetCurveList.Count; i++)
                        {
                            var curve = targetCurveList[i];
                            DatePeriod tagPeriod = curve.Tag as DatePeriod;
                            if (tagPeriod.StartDate == date.StartDate && tagPeriod.EndDate == date.EndDate)
                            {
                                if (isAdding)
                                {
                                    this.graph.GraphPane.CurveList.Add(curve);
                                    this._hiddenCurves.RemoveAt(i);
                                }
                                else
                                {
                                    this._hiddenCurves.Add(curve);
                                    this.graph.GraphPane.CurveList.RemoveAt(i);
                                }

                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (!this._suppressDraw)
                {
                    this.RefreshGraph();
                }
            }
        }

        /// <summary>The refresh graph.</summary>
        private void RefreshGraph()
        {
            this.graph.RestoreScale(this.graph.GraphPane);
            this.graph.Invalidate();
        }

        /// <summary>
        /// The btn no travel period_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnNoTravelPeriod_Click(object sender, EventArgs e)
        {
            this.SetAllItemsChecked(this.lvTravelPeriod, false);
        }

        /// <summary>
        /// The btn all travel period_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnAllTravelPeriod_Click(object sender, EventArgs e)
        {
            this.SetAllItemsChecked(this.lvTravelPeriod, true);
        }

        /// <summary>
        /// The btn no data date_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnNoDataDate_Click(object sender, EventArgs e)
        {
            this.SetAllItemsChecked(this.lvDataDate, false);
        }

        /// <summary>
        /// The btn all data date_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnAllDataDate_Click(object sender, EventArgs e)
        {
            this.SetAllItemsChecked(this.lvDataDate, true);
        }

        /// <summary>
        /// The lv filter_ item check.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lvFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool added = e.NewValue == CheckState.Checked;
            object tag = ((ListView)sender).Items[e.Index].Tag;

            this.UpdateGraph(added, tag);
        }

        /// <summary>
        /// The graph_ point value event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="pane">
        /// The pane.
        /// </param>
        /// <param name="curve">
        /// The curve.
        /// </param>
        /// <param name="iPt">
        /// The i pt.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string graph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair point = curve[iPt];
            var flight = point.Tag as Flight;
            if (flight != null)
            {
                return flight.SummaryString;
            }

            return string.Empty;
        }

        /// <summary>
        /// The graph form_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GraphForm_Shown(object sender, EventArgs e)
        {
            this.InitializeView();
            this.RenderGraph();
        }
    }

    /// <summary>The date comparer.</summary>
    internal class DateComparer : ListViewColumnSorter
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
            DateTime dateX = DateTime.Parse(x.Text);
            DateTime dateY = DateTime.Parse(y.Text);
            return dateX.CompareTo(dateY);
        }
    }

    /// <summary>The travel period compare.</summary>
    internal class TravelPeriodCompare : ListViewColumnSorter
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
            DatePeriod dateX = DatePeriod.Parse(x.Text), dateY = DatePeriod.Parse(y.Text);
            return dateX.CompareTo(dateY);
        }
    }
}