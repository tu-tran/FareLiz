namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Controls.Custom;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The flight data list view.</summary>
    public partial class FlightDataListView : UserControl
    {
        /// <summary>The _filtered flights data.</summary>
        private readonly List<Flight> _filteredFlightsData = new List<Flight>();

        /// <summary>The _lv flight item adaptor.</summary>
        private readonly FlightDataListViewItemAdaptor _lvFlightItemAdaptor = new FlightDataListViewItemAdaptor();

        /// <summary>The _sorter.</summary>
        private readonly FlightDataColumnSorter _sorter = new FlightDataColumnSorter();

        /// <summary>The _sync object.</summary>
        private readonly object _syncObject = new object();

        /// <summary>The _flights data.</summary>
        private List<Flight> _flightsData = new List<Flight>();

        /// <summary>The _lv change request date.</summary>
        private DateTime _lvChangeRequestDate = DateTime.MinValue;

        /// <summary>The _lv change requested.</summary>
        private volatile bool _lvChangeRequested;

        /// <summary>Initializes a new instance of the <see cref="FlightDataListView" /> class.</summary>
        public FlightDataListView()
        {
            this.InitializeComponent();
            this.VirtualModeThreshold = 3000;
            this.lvFlightData.ListViewItemSorter = this._sorter;
            this.lvFlightData.VirtualModeSort = this.FlightDataVirtualModeSort;
            this.lvFlightData.VirtualModeFilter = this.FlightDataVirtualModeFilter;
            this.lvFlightData.AttachMenuStrip(this.lvFlightDataContextMenuStrip);
            this.lvFlightData.SetGroupColumn(this.colPeriod);
        }

        /// <summary>The minimum amount of items count before VirtualMode is used</summary>
        public int VirtualModeThreshold { get; set; }

        /// <summary>Gets the current data.</summary>
        public List<Flight> CurrentData
        {
            get
            {
                return this._flightsData;
            }
        }

        /// <summary>Gets the currency provider.</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICurrencyProvider CurrencyProvider
        {
            get
            {
                return AppContext.MonitorEnvironment.CurrencyProvider;
            }
        }

        /// <summary>Gets or sets the active currency.</summary>
        public string ActiveCurrency { get; set; }

        /// <summary>Gets the selected flight.</summary>
        public Flight SelectedFlight
        {
            get
            {
                if (this.lvFlightData.SelectedIndices.Count == 1)
                {
                    var selItem = this.lvFlightData.Items[this.lvFlightData.SelectedIndices[0]];
                    return selItem.Tag as Flight;
                }

                return null;
            }
        }

        /// <summary>The on context menu strip opening.</summary>
        public event ContextMenuStripEventHandler OnContextMenuStripOpening;

        /// <summary>
        /// The append context menu strip.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        public void AppendContextMenuStrip(params ToolStripItem[] items)
        {
            if (items != null)
            {
                this.lvFlightDataContextMenuStrip.Items.AddRange(items);
            }
        }

        /// <summary>The clear data.</summary>
        public void ClearData()
        {
            this.lvFlightData.VirtualListSize = 0;
            this.lvFlightData.Items.Clear();
            this.lvFlightData.Groups.Clear();
        }

        /// <summary>
        /// Thread-safe setting water mark for the listview
        /// </summary>
        /// <param name="text">
        /// </param>
        public void SetWatermark(string text)
        {
            this.lblWaterMark.InvokeActionIfNeeded(
                (MethodInvoker)delegate
                    {
                        this.lblWaterMark.Visible = !string.IsNullOrEmpty(text);
                        this.lblWaterMark.Text = text;
                    });
        }

        /// <summary>
        /// Set the data source for the list view and render the items. This method returns immediately
        /// </summary>
        /// <param name="flights">
        /// The flights.
        /// </param>
        /// <param name="autoResize">
        /// The auto Resize.
        /// </param>
        public void SetDataSourceAsync(List<Flight> flights, bool autoResize)
        {
            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    AppUtil.NameCurrentThread(
                        (this.ParentForm == null ? string.Empty : this.ParentForm.GetType().Name + "-") + "FlightLV-SetDataSourceAsync");
                    this.SetDataSource(flights, autoResize);
                });
        }

        /// <summary>
        /// The set data source.
        /// </summary>
        /// <param name="flights">
        /// The flights.
        /// </param>
        /// <param name="autoResize">
        /// The auto resize.
        /// </param>
        public void SetDataSource(List<Flight> flights, bool autoResize)
        {
            var requestDate = this._lvChangeRequestDate = DateTime.Now;

            Debug.WriteLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "[{0}] Entering lock for FlightDataListView [{1}]",
                    Thread.CurrentThread.ManagedThreadId,
                    this.Name));
            if (!Monitor.TryEnter(this._syncObject))
            {
                Debug.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "[{0}] Could not enter lock for FlightDataListView [{1}]... Try again",
                        Thread.CurrentThread.ManagedThreadId,
                        this.Name));
                this._lvChangeRequested = true;
                Monitor.Enter(this._syncObject);
            }

            Debug.WriteLine(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "[{0}] Succesfully obtain lock for FlightDataListView [{1}]",
                    Thread.CurrentThread.ManagedThreadId,
                    this.Name));
            this._lvChangeRequested = false;

            try
            {
                if (requestDate < this._lvChangeRequestDate || this.IsUnusable())
                {
                    // Another request has come in: Let the later one passes through, or return if control is disposed
                    return;
                }

                var totalFlightsCount = flights == null ? 0 : flights.Count;
                var needVirtualMode = totalFlightsCount > this.VirtualModeThreshold;

                ListViewItem[] lvItems = null;
                if (!needVirtualMode)
                {
                    lvItems = new ListViewItem[totalFlightsCount];
                    for (var i = 0; i < totalFlightsCount; i++)
                    {
                        var f = flights[i];
                        lvItems[i] = this._lvFlightItemAdaptor.GetListViewItem(f, this.ActiveCurrency);
                    }
                }

                if (this.IsUnusable() || this._lvChangeRequested)
                {
                    return;
                }

                // Render the view
                this.BeginSafeInvoke(
                    (Action)delegate
                        {
                            this.ClearData();

                            if (totalFlightsCount > 0)
                            {
                                this.lvFlightData.VirtualMode = needVirtualMode;
                                if (needVirtualMode)
                                {
                                    this.lvFlightData.VirtualListSize = this._flightsData.Count;
                                }

                                if (!needVirtualMode)
                                {
                                    this.lvFlightData.BeginUpdate();
                                    try
                                    {
                                        this.lvFlightData.AddRange(lvItems);
                                        if (this.IsUnusable() || this._lvChangeRequested)
                                        {
                                            return;
                                        }

                                        if (this.lvFlightData.GroupColumnIndex > -1)
                                        {
                                            this.lvFlightData.AutoGroup();
                                        }
                                    }
                                    finally
                                    {
                                        this.lvFlightData.EndUpdate();
                                    }
                                }

                                if (autoResize && this._flightsData != null && this._flightsData.Count > 0)
                                {
                                    this.lvFlightData.AutoResize();
                                }
                            }
                        });

                this._flightsData = flights;
            }
            catch (Exception ex)
            {
                if (this.IsUnusable() || this._lvChangeRequested)
                {
                    return;
                }

                Console.Error.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                Monitor.Exit(this._syncObject);
                Debug.WriteLine(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "[{0}] Released lock for FlightDataListView [{1}]",
                        Thread.CurrentThread.ManagedThreadId,
                        this.Name));
            }
        }

        /// <summary>
        /// The flight data virtual mode sort.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        private void FlightDataVirtualModeSort(EnhancedListView listView)
        {
            this._flightsData.Sort(this._sorter);
        }

        /// <summary>
        /// The flight data virtual mode filter.
        /// </summary>
        /// <param name="listView">
        /// The list view.
        /// </param>
        /// <param name="filters">
        /// The filters.
        /// </param>
        private void FlightDataVirtualModeFilter(EnhancedListView listView, List<ListViewFilter> filters)
        {
            if (EnhancedListView.DoFilter(this._flightsData, this._filteredFlightsData, filters, this.FilterFlight))
            {
                listView.VirtualListSize = this._flightsData.Count;
                listView.Sort();
            }
        }

        /// <summary>
        /// The filter flight.
        /// </summary>
        /// <param name="flight">
        /// The flight.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool FilterFlight(Flight flight, ListViewFilter filter)
        {
            var data = this._lvFlightItemAdaptor.GetPresentationStrings(flight, this.ActiveCurrency);
            if (!data[filter.Column].IsMatch(filter.FilterString))
            {
                return false;
            }

            return true;
        }

        /// <summary>The to csv.</summary>
        public void ToCsv()
        {
            var dlg = new SaveFileDialog
                          {
                              Filter = "CSV Document|*.csv|Text Document|*.txt|All files|*",
                              Title = "Export Flight AirportData",
                              FileName = "FareData"
                          };

            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                this.lvFlightData.ToCsv(dlg.FileName);
            }
        }

        /// <summary>
        /// The mnu refresh tool strip menu item_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuRefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RefreshData();
        }

        /// <summary>
        /// The lv flight data_ retrieve virtual item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lvFlightData_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < this._flightsData.Count)
            {
                e.Item = this._lvFlightItemAdaptor.GetListViewItem(this._flightsData[e.ItemIndex], this.ActiveCurrency);
            }
        }

        /// <summary>
        /// The lv flight data context menu strip_ opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lvFlightDataContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var selFlight = this.SelectedFlight;
            this.mnuBookTrip.Available = this.CanBuy(selFlight);

            if (this.mnuBookTrip.Available)
            {
                this.mnuBookTrip.ToolTipText = selFlight.SummaryString;
            }

            if (this.OnContextMenuStripOpening != null)
            {
                var args = new MenuBuilderEventArgs(selFlight);
                this.OnContextMenuStripOpening(this, args);
                e.Cancel = args.Cancel;
            }
        }

        /// <summary>
        /// The mnu change currency_ drop down opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuChangeCurrency_DropDownOpening(object sender, EventArgs e)
        {
            this.UpdateCurrencyMenuStrip();
            this.mnuCurrencySeparator.Available = this.mnuChangeCurrency.DropDownItems.Count > 2;
        }

        /// <summary>The update currency menu strip.</summary>
        public void UpdateCurrencyMenuStrip()
        {
            if (this.mnuChangeCurrency.DropDownItems.Count > 2)
            {
                for (var i = 2; i < this.mnuChangeCurrency.DropDownItems.Count; i++)
                {
                    this.mnuChangeCurrency.DropDownItems.RemoveAt(i--);
                }
            }

            var currencyProvider = this.CurrencyProvider;
            if (currencyProvider == null)
            {
                return;
            }

            var allCurrencies = currencyProvider.GetCurrencies();
            var sel = currencyProvider.AllowedCurrencies;
            var selCurrency = this.mnuUseDataCurrency;

            foreach (var c in allCurrencies)
            {
                if (sel == null || sel.Contains(c.Key))
                {
                    var newItem =
                        new RadioToolStripMenuItem(
                            string.Format(CultureInfo.InvariantCulture, "{0} - {1} {2}", c.Key, c.Value.FullName, c.Value.Symbol),
                            c.Key);
                    newItem.Click += this.changeCurrency_Click;
                    this.mnuChangeCurrency.DropDownItems.Add(newItem);
                    if (c.Key == this.ActiveCurrency)
                    {
                        selCurrency = newItem;
                    }
                }
            }

            selCurrency.Checked = true;
        }

        /// <summary>
        /// The change currency_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void changeCurrency_Click(object sender, EventArgs e)
        {
            var mnuItem = sender as ToolStripMenuItem;
            if (mnuItem != null && mnuItem.Checked)
            {
                var selected = (string)mnuItem.Tag;
                if (this.ActiveCurrency != selected)
                {
                    this.ActiveCurrency = selected;
                    this.RefreshData();
                }
            }
        }

        /// <summary>The refresh data.</summary>
        public void RefreshData()
        {
            this.SetDataSourceAsync(this._flightsData, true);
        }

        /// <summary>
        /// The buy ticket_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void BuyTicket_Click(object sender, EventArgs e)
        {
            this.BuySelectedFlight();
        }

        /// <summary>
        /// The can buy.
        /// </summary>
        /// <param name="flight">
        /// The flight.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CanBuy(Flight flight)
        {
            var result = flight != null && flight.TravelAgency != null && !string.IsNullOrEmpty(flight.TravelAgency.Url)
                         && !(flight.OutboundLeg != null && flight.OutboundLeg.Departure.Date <= DateTime.Now.Date);
            return result;
        }

        /// <summary>The buy selected flight.</summary>
        private void BuySelectedFlight()
        {
            var selFlight = this.SelectedFlight;
            if (!this.CanBuy(selFlight))
            {
                return;
            }

            var url = selFlight.TravelAgency.Url;
            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    BrowserUtils.Open(url);
                }
                catch (Exception ex)
                {
                    var error = "Failed to launch web browser for ticket purchase: " + ex.Message;
                    AppContext.Logger.Error(error);
                    MessageBox.Show(error, "Ticket Purchase", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}