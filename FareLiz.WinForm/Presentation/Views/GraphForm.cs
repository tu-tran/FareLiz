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

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    internal partial class GraphForm : SmartForm
    {
        readonly IEnumerable<Journey> _journeyData;
        readonly Dictionary<Journey, PointPairList> _graphData = new Dictionary<Journey, PointPairList>();
        readonly CurveList _hiddenCurves = new CurveList();
        private bool _suppressDraw = false;

        public GraphForm(IEnumerable<Journey> data)
        {
            InitializeComponent();

            Text = AppUtil.ProductName + " " + Text;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            _journeyData = data.OrderBy(j => j.DepartureDate).ThenBy(j => j.ReturnDate);
            lvDataDate.ListViewItemSorter = new DateComparer();
            lvTravelPeriod.ListViewItemSorter = new TravelPeriodCompare();
            lvDataDate.ItemCheck += lvFilter_ItemCheck;
            lvTravelPeriod.ItemCheck += lvFilter_ItemCheck;
        }

        private void InitializeView()
        {
            GraphPane graphPane = graph.GraphPane;

            // Set the title and axis labels            
            graphPane.Title.FontSpec.Family = Font.FontFamily.Name;
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
            PopulateData();
        }

        void PopulateData()
        {
            Dictionary<string, object> travelPeriod = new Dictionary<string, object>();
            Dictionary<string, object> dataDate = new Dictionary<string, object>();

            foreach (var j in _journeyData)
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
                            dataDate.Add(str, d.DataDate.Date);
                        var x = (double)new XDate(d.DataDate);
                        var newPoint = new PointPair(x, cheapestFlight.Price) { Tag = cheapestFlight };
                        points.Add(newPoint);
                    }
                }

                _graphData.Add(j, points);
            }

            // Render listview
            PopulateListView(travelPeriod, lvTravelPeriod, false);
            PopulateListView(dataDate, lvDataDate, true);
        }

        private void RenderGraph()
        {
            GraphPane graphPane = graph.GraphPane;
            Random rand = new Random(DateTime.Now.Millisecond);
            var usedColors = new HashSet<string>();
            int count = 0;

            foreach (var pair in _graphData)
            {
                Journey j = pair.Key;
                string period = StringUtil.GetPeriodString(j.DepartureDate, j.ReturnDate);
                string colorHex;

                do
                {
                    colorHex = String.Format("#{0:X6}", rand.Next(0x1000000) & 0x5F5F5F);
                }
                while (usedColors.Contains(colorHex));

                usedColors.Add(colorHex);
                Color curveColor = ColorTranslator.FromHtml(colorHex);
                LineItem curve = new LineItem(period, pair.Value, curveColor, SymbolType.Square);
                var curveTarget = (count++ == 0 ? graphPane.CurveList : _hiddenCurves);
                curveTarget.Add(curve);

                curve.Tag = new DatePeriod(j.DepartureDate, j.ReturnDate);
                curve.Line.Width = 2;

                foreach (ListViewItem item in lvTravelPeriod.Items)
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

        private void PopulateListView(Dictionary<string, object> data, ListView listView, bool checkAll)
        {
            var newItems = new ListViewItem[data.Count];
            int i = 0;
            foreach (var pair in data)
                newItems[i] = new ListViewItem(pair.Key) { Checked = (i++ == 0 || checkAll), Tag = pair.Value };

            listView.Items.AddRange(newItems);
            listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void SetAllItemsChecked(ListView listView, bool isChecked)
        {
            lblStatus.Text = "Processing data...";
            graph.SuspendLayout();
            _suppressDraw = true;
            try
            {
                foreach (ListViewItem item in listView.Items)
                    item.Checked = isChecked;
                lblStatus.Text = "Idle";
            }
            catch (Exception ex)
            {
                lblStatus.Text = ex.Message;
            }
            finally
            {
                _suppressDraw = false;
                RefreshGraph();
            }
        }

        private void UpdateGraph(bool isAdding, object tagData)
        {
            try
            {
                if (tagData is DateTime)    // Change data date
                {
                    var date = (DateTime)tagData;

                    foreach (var pair in _graphData)
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
                                        if (f.Price < cheapestPrice)
                                        {
                                            cheapestFlight = f;
                                            cheapestPrice = f.Price;
                                        }

                                    var newPoint = new PointPair((double)new XDate(d.DataDate), cheapestPrice) { Tag = cheapestFlight };
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
                                    pointList.RemoveAt(i--);
                            }
                        }
                    }
                }
                else if (tagData is DatePeriod) // Change travel period
                {
                    var date = (DatePeriod)tagData;
                    foreach (var pair in _graphData)
                    {
                        var pointList = pair.Value;
                        var targetCurveList = (isAdding ? _hiddenCurves : graph.GraphPane.CurveList);
                        for (int i = 0; i < targetCurveList.Count; i++)
                        {
                            var curve = targetCurveList[i];
                            DatePeriod tagPeriod = curve.Tag as DatePeriod;
                            if (tagPeriod.StartDate == date.StartDate && tagPeriod.EndDate == date.EndDate)
                            {
                                if (isAdding)
                                {
                                    graph.GraphPane.CurveList.Add(curve);
                                    _hiddenCurves.RemoveAt(i);
                                }
                                else
                                {
                                    _hiddenCurves.Add(curve);
                                    graph.GraphPane.CurveList.RemoveAt(i);
                                }

                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (!_suppressDraw)
                {
                    RefreshGraph();
                }
            }
        }

        private void RefreshGraph()
        {
            graph.RestoreScale(graph.GraphPane);
            graph.Invalidate();
        }

        private void btnNoTravelPeriod_Click(object sender, EventArgs e)
        {
            SetAllItemsChecked(lvTravelPeriod, false);
        }

        private void btnAllTravelPeriod_Click(object sender, EventArgs e)
        {
            SetAllItemsChecked(lvTravelPeriod, true);
        }

        private void btnNoDataDate_Click(object sender, EventArgs e)
        {
            SetAllItemsChecked(lvDataDate, false);
        }

        private void btnAllDataDate_Click(object sender, EventArgs e)
        {
            SetAllItemsChecked(lvDataDate, true);
        }

        private void lvFilter_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            bool added = (e.NewValue == CheckState.Checked);
            object tag = ((ListView)sender).Items[e.Index].Tag;

            UpdateGraph(added, tag);
        }

        private string graph_PointValueEvent(ZedGraphControl sender, GraphPane pane, CurveItem curve, int iPt)
        {
            PointPair point = curve[iPt];
            var flight = point.Tag as Flight;
            if (flight != null)
                return flight.SummaryString;

            return String.Empty;
        }

        private void GraphForm_Shown(object sender, EventArgs e)
        {
            InitializeView();
            RenderGraph();
        }
    }

    internal class DateComparer : ListViewColumnSorter
    {
        internal DateComparer() { }
        protected override int CompareItem(ListViewItem.ListViewSubItem x, ListViewItem.ListViewSubItem y)
        {
            DateTime dateX = DateTime.Parse(x.Text);
            DateTime dateY = DateTime.Parse(y.Text);
            return dateX.CompareTo(dateY);
        }
    }

    internal class TravelPeriodCompare : ListViewColumnSorter
    {
        internal TravelPeriodCompare() { }
        protected override int CompareItem(ListViewItem.ListViewSubItem x, ListViewItem.ListViewSubItem y)
        {
            DatePeriod dateX = DatePeriod.Parse(x.Text), dateY = DatePeriod.Parse(y.Text);
            return dateX.CompareTo(dateY);
        }
    }
}