using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.Service.LiveUpdate;
using SkyDean.FareLiz.WinForm.Components.Controls;
using SkyDean.FareLiz.WinForm.Components.Controls.ComboBox;
using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
using SkyDean.FareLiz.WinForm.Components.Dialog;
using SkyDean.FareLiz.WinForm.Components.Utils;
using SkyDean.FareLiz.WinForm.Presentation.Controllers;
using SkyDean.FareLiz.WinForm.Utils;
using SkyDean.FareLiz.Data.Monitoring;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    internal sealed partial class FlightStatisticForm : SmartForm
    {
        private const string STR_FORMAT_DATE_COMBO = "dd/MM/yyyy ddd",
                             STR_ONE_WAY = "One-way trip";

        private readonly object _lockObj = new object();
        private readonly bool _isMainForm;

        private readonly ToolStripMenuItem mnuViewThisDate = new ToolStripMenuItem("View fare history for this travel date");
        private readonly ToolStripMenuItem mnuCheckThisDate = new ToolStripMenuItem("Check latest fare for this travel date");
        private readonly ToolStripSeparator mnuSeparator = new ToolStripSeparator();

        private ExecutionParam _executionParam;
        private bool _exiting = false;
        private bool _formLoaded = false;
        private bool _firstClose = true;
        private bool _changingLocation = false;
        private bool _loadingRoutes = false;
        private bool _suppressRefreshDataView = false;
        private IList<TravelRoute> _routeData;
        private TravelRoute _activeRoute;
        private bool _lvChangeRequested;
        private DateTime _lvChangeRequestDate;

        public IFareDataProvider FareDataProvider { get { return AppContext.MonitorEnvironment.FareDataProvider; } }
        public IArchiveManager ArchiveManager { get { return AppContext.MonitorEnvironment.ArchiveManager; } }
        public IFareDatabase FareDatabase { get { return AppContext.MonitorEnvironment.FareDatabase; } }
        public ILog Logger { get { return AppContext.MonitorEnvironment.Logger; } }

        public FlightStatisticForm(IList<TravelRoute> data, ExecutionParam executionParam, bool isMainForm)
        {
            InitializeComponent();
            _isMainForm = isMainForm;
            _routeData = data;
            if (data != null && data.Count > 0)
            {
                _activeRoute = data[0];
                ReloadRoutes();
            }

            _executionParam = executionParam;
            InitializeView();
        }

        private void InitializeView()
        {
            trayIcon.Visible = _isMainForm;
            if (_isMainForm)
            {
                GUIBuilder.AttachMenuToTrayIcon(this, trayIcon, true);
                trayIcon.Icon = Icon;
            }
            else
            {
                btnExit.Text = "Close";
                btnRefresh.Enabled = chkAllDataHistory.Enabled = false;
                cbAirlines.Width += (btnGetPackage.Right - cbAirlines.Right);   // Try to fill the screen
                var gap = btnRefresh.Right - btnToCsv.Right;
                btnToCsv.Left += gap;
                btnGraph.Left += gap;
            }

            bool isDbConfigured = FareDatabase != null;
            btnRefresh.Visible = btnGetPackage.Visible = _isMainForm;
            btnGetPackage.Enabled = (isDbConfigured && FareDatabase is ISyncableDatabase);
            btnRefresh.Enabled = lvFlightData.Enabled = isDbConfigured;

            btnCheckFareFlex.Enabled = btnScheduler.Enabled = (FareDataProvider != null);

            mnuViewThisDate.Tag = mnuCheckThisDate.Tag = lvFlightData;
            lvFlightData.AppendContextMenuStrip(mnuSeparator, mnuViewThisDate, mnuCheckThisDate);
            lvFlightData.OnContextMenuStripOpening += lvFlightData_OnContextMenuStripOpening;
            mnuCheckThisDate.Click += mnuCheckThisDate_Click;
            mnuViewThisDate.Click += mnuViewThisDate_Click;

            numPriceLimit.Value = _executionParam.PriceLimit;
            numMinDuration.Value = _executionParam.MinStayDuration;
            numMaxDuration.Value = _executionParam.MaxStayDuration;

            Text = AppUtil.ProductName + " " + Text;
            progressBar.ContainerControl = this;

            mnuAbout.Text = "&About " + AppUtil.ProductName;
        }

        void mnuViewThisDate_Click(object sender, EventArgs e)
        {
            var lv = ((ToolStripItem)sender).Tag as FlightDataListView;
            if (lv == null)
                return;

            var selFlight = lv.SelectedFlight;
            if (selFlight != null)
            {
                cbDepartureDate.SelectedItem = selFlight.OutboundLeg.Departure.ToString(STR_FORMAT_DATE_COMBO);
                cbReturnDate.SelectedItem = selFlight.InboundLeg.Departure.ToString(STR_FORMAT_DATE_COMBO);
            }
        }

        void mnuCheckThisDate_Click(object sender, EventArgs e)
        {
            var lv = ((ToolStripItem)sender).Tag as FlightDataListView;
            if (lv == null)
                return;

            var selFlight = lv.SelectedFlight;
            if (selFlight != null)
            {
                if (selFlight.OutboundLeg.Departure < DateTime.Now)
                    return;

                var param = GetExecutionParam(false, false, false, OperationMode.ShowFare);
                param.DepartureDate = selFlight.OutboundLeg.Departure;
                param.ReturnDate = selFlight.InboundLeg.Departure;
                param.DepartureDateRange = param.ReturnDateRange = DateRangeDiff.Empty;
                param.MinStayDuration = param.MaxStayDuration = (int)(param.ReturnDate - param.DepartureDate).TotalDays;
                var checkFareForm = new CheckFareForm(param);
                checkFareForm.Attach(new CheckFareController());
                checkFareForm.Show();
            }
        }

        void lvFlightData_OnContextMenuStripOpening(FlightDataListView sender, MenuBuilderEventArgs e)
        {
            mnuSeparator.Available = mnuCheckThisDate.Available = mnuViewThisDate.Available = false;
            var selFlight = sender.SelectedFlight;
            if (selFlight != null)
            {
                if (selFlight.OutboundLeg.Departure.Date >= DateTime.Now.Date)
                {
                    mnuCheckThisDate.Available = true;
                    mnuCheckThisDate.Text = String.Format("Check latest fare for [{0}]", selFlight.TravelDateString);
                }
                mnuSeparator.Available = mnuViewThisDate.Available = true;
                mnuViewThisDate.Text = String.Format("View only fare for [{0}]", selFlight.TravelDateString);
            }
        }

        public void SetFilters(int minDuration, int maxDuration, int priceLimit)
        {
            numMinDuration.Value = minDuration;
            numMaxDuration.Value = maxDuration;
            numPriceLimit.Value = priceLimit;
        }

        /// <summary>
        /// Fill the data for departure/return date combobox filter
        /// </summary>
        private void FillDateComboBox()
        {
            _suppressRefreshDataView = true;
            try
            {
                cbDepartureDate.Items.Clear();
                cbReturnDate.Items.Clear();

                if (_activeRoute == null || _activeRoute.Journeys.Count < 1)
                {
                    SetStatus("There is no historical fare data for selected route");
                    lvFlightData.SetDataSourceAsync(null, false);
                    return;
                }

                // Get last selected items
                object lastDepartValue = cbDepartureDate.Items.Count > 0 ? cbDepartureDate.SelectedItem : (_executionParam.DepartureDate.IsDefined() ? _executionParam.DepartureDate.ToString(STR_FORMAT_DATE_COMBO) : null),
                       lastReturnValue = cbReturnDate.Items.Count > 0 ? cbReturnDate.SelectedItem : (_executionParam.ReturnDate.IsDefined() ? _executionParam.ReturnDate.ToString(STR_FORMAT_DATE_COMBO) : null);

                // Get the distinct list of departure and return date
                IOrderedEnumerable<DateTime> deptDateList =
                    _activeRoute.Journeys.DistinctBy(o => o.DepartureDate).Select((o) => o.DepartureDate).OrderBy((o) => o);
                IOrderedEnumerable<DateTime> retDateList =
                    _activeRoute.Journeys.DistinctBy(o => o.ReturnDate).Select((o) => o.ReturnDate).OrderBy((o) => o);

                foreach (DateTime d in deptDateList)
                    cbDepartureDate.Items.Add(d.ToString(STR_FORMAT_DATE_COMBO));

                int retCount = 0;
                foreach (DateTime d in retDateList)
                {
                    // Check if the first return date is DateTime.MinValue: If yes, add the item to the combobox
                    if (++retCount == 1 && d == DateTime.MinValue)
                        cbReturnDate.Items.Add(STR_ONE_WAY);    // Add a new item for one way trip
                    else
                        cbReturnDate.Items.Add(d.ToString(STR_FORMAT_DATE_COMBO));
                }

                cbDepartureDate.Items.Add("");  // Add an empty row to the end (this allows user to make the filter "undefined"
                cbReturnDate.Items.Add("");

                // Try to match the previously selected item with the new data source
                if (lastDepartValue != null && cbDepartureDate.Items.Contains(lastDepartValue))
                    cbDepartureDate.SelectedItem = lastDepartValue;
                else
                {
                    if (_isMainForm)
                        cbDepartureDate.SelectedIndex = 0;
                    else
                        cbDepartureDate.SelectedIndex = cbDepartureDate.Items.Count - 1;
                }

                if (lastReturnValue != null && cbReturnDate.Items.Contains(lastReturnValue))
                    cbReturnDate.SelectedItem = lastReturnValue;
                else
                {
                    if (_isMainForm)
                        cbReturnDate.SelectedIndex = 0;
                    else
                        cbReturnDate.SelectedIndex = cbReturnDate.Items.Count - 1;
                }

                // Manually trigger the event to refresh the data view
                _suppressRefreshDataView = false;
                FlightFilter_Changed(this, null);
            }
            finally
            {
                _suppressRefreshDataView = false;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _exiting = true;
            if (_isMainForm)
                Environment.Exit(0);
            else
                Dispose();
        }

        /// <summary>
        /// Bind the list view asynchronously
        /// </summary>
        private void BindListViewAsync(TravelRoute route, DateTime departureDate, DateTime returnDate, double priceLimit, bool allHistory, int amountOfPrices,
                                       int minDuration, int maxDuration, Dictionary<string, bool> opearators, bool populateData)
        {
            if (!_formLoaded)
                return;

            ThreadPool.QueueUserWorkItem(o =>
                {
                    AppUtil.NameCurrentThread(GetType().Name + "-BindListView");
                    BindListView(route, departureDate, returnDate, priceLimit, allHistory, amountOfPrices, minDuration, maxDuration, opearators, populateData);
                });
        }

        private void BindListView(TravelRoute route, DateTime departureDate, DateTime returnDate, double priceLimit, bool viewAllHistory, int amountFlightsPerData,
                                  int minDuration, int maxDuration, Dictionary<string, bool> opearators, bool populateData)
        {
            DateTime requestDate = DateTime.Now;
            if (!Monitor.TryEnter(_lockObj))   // The view is being refreshed: Send a signal
            {
                _lvChangeRequested = true;
                _lvChangeRequestDate = requestDate;
                Monitor.Enter(_lockObj);
            }

            _lvChangeRequested = false;
            try
            {
                if (requestDate < _lvChangeRequestDate) // Another request has come in: Let the later one passes through
                    return;

                var data = route.Journeys;
                if (data == null || data.Count < 1)
                    return;

                this.SafeInvoke((Action)delegate
                    {
                        progressBar.ShowInTaskbar = progressBar.Visible = true;
                        progressBar.Style = ProgressBarStyle.Marquee;
                    });

                // Get all the needed data according to the Date filter condition
                IEnumerable<Journey> journeyCollection = null;
                if (departureDate.IsDefined())
                {
                    // Known departure date
                    if (returnDate == DateTime.MaxValue)    // Unknown return date
                        journeyCollection = data.Where(o => o.DepartureDate.Date == departureDate.Date)
                                                .OrderByDescending(o => o.ReturnDate);
                    else    // Known return date or one-way trip for DateTime.MinValue
                        journeyCollection = data.Where(o => o.DepartureDate.Date == departureDate.Date && o.ReturnDate.Date == returnDate.Date);
                }
                else
                {
                    // Unknown departure date
                    if (returnDate == DateTime.MaxValue)  // Unknown return date
                        journeyCollection = data.OrderBy(o => o.DepartureDate)
                                                .ThenByDescending(o => o.ReturnDate);
                    else    // Known return date or one-way trip for DateTime.MinValue
                        journeyCollection = data.Where(o => o.ReturnDate.Date == returnDate.Date)
                                                .OrderBy(o => o.DepartureDate);
                }

                var newOperators = new List<string>();
                var existOperatorSelections = new List<CheckBoxComboBoxItem>();

                if (populateData)
                {
                    SetStatus("Loading flight data...");
                    const int loadSize = 1000;
                    var loadBatch = new List<Journey>();
                    foreach (var j in journeyCollection)
                    {
                        loadBatch.Add(j);
                        if (loadBatch.Count == loadSize)
                        {
                            FareDatabase.LoadData(loadBatch, viewAllHistory, AppContext.ProgressCallback);
                            loadBatch.Clear();
                        }
                        if (_lvChangeRequested)
                            return;
                    }

                    if (!_lvChangeRequested && loadBatch.Count > 0)
                        FareDatabase.LoadData(loadBatch, viewAllHistory, AppContext.ProgressCallback);
                }

                SetStatus("Processing data...");

                int totalJourneysCount = journeyCollection.Count();
                int totalFlightsCount = 0;
                int skippedDuration = 0, skippedPrice = 0, skippedIgnored = 0;
                var flightsData = new List<Flight>();

                this.SafeInvoke((Action)delegate
                {
                    progressBar.Style = ProgressBarStyle.Continuous;
                    progressBar.Value = 0;
                    progressBar.Maximum = totalJourneysCount;
                    existOperatorSelections.AddRange(cbAirlines.CheckBoxItems);
                });

                // Only filter based on duration if there is undefined departure or return date, and user did not choose one-way trip
                bool checkDuration = (departureDate.IsUndefined() || returnDate.IsUndefined()) && returnDate != DateTime.MinValue;
                foreach (Journey j in journeyCollection)
                {
                    this.SafeInvoke((Action)delegate { progressBar.Value++; });
                    if (_lvChangeRequested)
                        return;

                    bool skippedJourney = false;
                    if (checkDuration)
                    {
                        int stayDuration = (int)(j.ReturnDate.Date - j.DepartureDate.Date).TotalDays;
                        if (stayDuration < 0)   // One-way trip
                            stayDuration = 0;
                        skippedJourney = (stayDuration < minDuration || stayDuration > maxDuration);
                    }

                    List<Flight> journeyFlights = (skippedJourney ? null : new List<Flight>());
                    foreach (var d in j.Data)
                    {
                        // Keep counts for statistics
                        totalFlightsCount += d.Flights.Count;

                        if (skippedJourney)
                            skippedDuration += d.Flights.Count;
                        else
                        {
                            var flights = d.Flights;
                            flights.Sort(FlightPriceComparer.Instance);

                            int taking = (amountFlightsPerData > flights.Count ? flights.Count : amountFlightsPerData);
                            skippedIgnored += (flights.Count - taking);
                            for (int i = 0; i < taking; i++)
                            {
                                if (flights[i].Price > priceLimit)
                                {
                                    skippedPrice += (taking - i);
                                    break;
                                }

                                journeyFlights.Add(flights[i]);
                            }
                        }
                    }

                    if (journeyFlights == null)
                        continue;

                    // Fill the list of flight operators for filter purpose
                    foreach (Flight flight in journeyFlights)
                    {
                        string flightOperator = flight.Operator;
                        if (opearators != null)
                            if (opearators.ContainsKey(flightOperator) && !opearators[flightOperator])
                                continue;

                        flightsData.Add(flight);

                        if (!newOperators.Contains(flightOperator))
                            newOperators.Add(flightOperator);
                    }
                }

                SetStatus("Rendering view...");
                this.SafeInvoke(new Action(() =>
                    {
                        progressBar.Style = ProgressBarStyle.Marquee;
                        lvFlightData.SetDataSourceAsync(flightsData, true);

                        if (opearators == null) // Reload data - Refill list of operators
                        {
                            var newOperatorCheckList = new Dictionary<string, bool>();
                            if (opearators != null)
                                foreach (var v in opearators)
                                    newOperatorCheckList.Add(v.Key, v.Value);

                            foreach (string s in newOperators)
                            {
                                if (!newOperatorCheckList.ContainsKey(s))
                                    newOperatorCheckList.Add(s, true);
                            }
                            List<KeyValuePair<string, bool>> sortedOps =
                                newOperatorCheckList.OrderBy(p => p.Key).ToList();

                            cbAirlines.SelectedValueChanged -= FlightFilter_Changed;
                            cbAirlines.TextChanged -= FlightFilter_Changed;
                            cbAirlines.Text = null;
                            cbAirlines.Items.Clear();
                            cbAirlines.CheckBoxItems.Clear();

                            sortedOps.ForEach(p =>
                                {
                                    int pos = cbAirlines.Items.Add(p.Key);
                                    cbAirlines.CheckBoxItems[pos].Checked = p.Value;
                                });
                            cbAirlines.SelectedValueChanged += FlightFilter_Changed;
                            cbAirlines.TextChanged += FlightFilter_Changed;
                        }

                        btnToCsv.Enabled = btnGraph.Enabled = (flightsData.Count > 0);
                        progressBar.Visible = false;

                        string status;
                        if (totalFlightsCount == 0)
                            status = "There is no flight data for this date";
                        else
                        {
                            string journeyStr = totalJourneysCount > 1 ? " (" + totalJourneysCount + " dates)" : null;
                            var parts = new List<string>();
                            if (skippedDuration > 0) parts.Add(skippedDuration + " not in duration range");
                            if (skippedPrice > 0) parts.Add(skippedPrice + " too expensive");
                            if (skippedIgnored > 0) parts.Add(skippedIgnored + " ignored");
                            if (flightsData.Count > 0) parts.Add(flightsData.Count + " displayed");
                            string skipStr = (parts.Count > 0 ? " | " + String.Join(", ", parts.ToArray()) : String.Empty);
                            status = String.Format("Available flights: {0}{1}{2}", totalFlightsCount, journeyStr, skipStr);
                        }

                        numMinDuration.Enabled = numMaxDuration.Enabled = checkDuration;
                        SetStatus(status);
                    }));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                MessageBox.Show(this, ex.ToString(), "Could not display data");
                SetStatus("An internal error occured");
            }
            finally
            {
                this.SafeInvoke((Action)(() =>
                {
                    progressBar.ShowInTaskbar = progressBar.Visible = false;
                    progressBar.Value = 0;
                }));
                Monitor.Exit(_lockObj);
            }
        }

        private void FlightFilter_Changed(object sender, EventArgs e)
        {
            if (_suppressRefreshDataView)
                return;

            if (sender == null) // In case of programmatically trigger the event
            {
                FillDateComboBox();
                return;
            }

            bool reloadData = (sender == cbDepartureDate || sender == cbReturnDate || sender == this);

            FilterCondition condition = ReadFilter();

            // If users chose one way trip: Do not enable the numeric selection for stay duration
            numMinDuration.Enabled = numMaxDuration.Enabled = (condition.DepartureDate.IsUndefined() || condition.ReturnDate.IsUndefined());
            BindListViewAsync(_activeRoute, condition.DepartureDate, condition.ReturnDate, condition.MaxPrice, chkAllDataHistory.Checked, Decimal.ToInt32(numAmountOfPrices.Value),
                              condition.MinDuration, condition.MaxDuration, reloadData ? null : condition.Operators, _isMainForm);
        }

        /// <summary>
        /// Read the current filter based on the view
        /// </summary>
        private FilterCondition ReadFilter()
        {
            DateTime departureDate = String.IsNullOrEmpty(cbDepartureDate.Text)
                                         ? DateTime.MinValue
                                         : DateTime.ParseExact(cbDepartureDate.Text, STR_FORMAT_DATE_COMBO, null);
            DateTime returnDate = String.IsNullOrEmpty(cbReturnDate.Text)
                                      ? DateTime.MaxValue
                                      : (cbReturnDate.Text == STR_ONE_WAY ? DateTime.MinValue : DateTime.ParseExact(cbReturnDate.Text, STR_FORMAT_DATE_COMBO, null));

            int minDuration = Decimal.ToInt32(numMinDuration.Value);
            int maxDuration = Decimal.ToInt32(numMaxDuration.Value);
            int maxPrice = Decimal.ToInt32(numPriceLimit.Value);

            var ops = new Dictionary<string, bool>();
            cbAirlines.CheckBoxItems.ForEach(i => ops.Add(i.Text, i.Checked));
            var filter = new FilterCondition(cbDeparture.SelectedAirport, cbDestination.SelectedAirport, minDuration, maxDuration, maxPrice, departureDate, returnDate, ops);
            return filter;
        }

        private void btnGraph_Click(object sender, EventArgs e)
        {
            var data = lvFlightData.CurrentData;
            if (data == null || data.Count < 1)
                return;

            var journeys = new List<Journey>();
            // Create new data based on the visible items inside the listview
            for (int i = 0; i < data.Count; i++)
            {
                var f = data[i];
                var j = f.JourneyData.JourneyInfo;
                if (!journeys.Contains(j))
                    journeys.Add(j);
            }
            var g = new GraphForm(journeys);
            g.Show(this);
        }

        void ReloadRoutes()
        {
            if (_isMainForm && FareDatabase == null)
                SetStatus("There is no configured Fare Database! Historical fare data will be unavailable!");
            else
            {
                SetStatus("Loading journey data from database...");
                ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
                {
                    AppUtil.NameCurrentThread(GetType().Name + "-LoadJourney");
                    DoLoadRoutes();
                }));
            }
        }

        /// <summary>
        /// Do the actual database operation to load the list of routes
        /// </summary>
        private void DoLoadRoutes()
        {
            Exception err = null;
            _loadingRoutes = true;
            try
            {
                if (_isMainForm)
                    _routeData = FareDatabase.GetRoutes(false, false, false, false, AppContext.ProgressCallback);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                err = ex;
            }
            finally
            {
                List<Airport> originList = null;
                if (_routeData != null)
                {
                    originList = new List<Airport>();
                    // Get the distinct list of departures from the stored routes
                    foreach (var j in _routeData)
                    {
                        bool exist = false;
                        foreach (var dept in originList)
                            if (String.Equals(dept.IATA, j.Departure.IATA, StringComparison.OrdinalIgnoreCase))
                            {
                                exist = true;
                                break;
                            }

                        if (!exist)
                            originList.Add(j.Departure);
                    }
                    originList.Sort((x, y) => x.Name.CompareTo(y.Name));
                }

                this.SafeInvoke(new Action(() =>
                    {
                        // The location change event won't be triggered: Check if the current displayed route still exists in the database and set reference to the new object
                        cbDeparture.SelectedItem = null;
                        cbDeparture.DataSource = originList;
                        bool noRoute = (originList == null || originList.Count == 0);
                        cbDeparture.Enabled = cbDestination.Enabled = !noRoute;
                        if (noRoute)
                            cbDeparture.Text = cbDestination.Text = "There is no stored fare data in database";

                        if (err == null)
                        {
                            grpDetail.Text = String.Format("Travel Detail (data was updated on {0})", DateTime.Now);
                            FlightFilter_Changed(null, null);
                        }
                        else
                            MessageBox.Show(this, "Failed to load journey data: " + err.Message + Environment.NewLine + Environment.NewLine + "If the problem persists, this normally indicates that the database is corrupted. It is recommended to reset the database using the Configuration window!", "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }));

                _loadingRoutes = false;
            }
        }

        private void SetStatus(string text)
        {
            if (InvokeRequired)
                this.SafeInvoke(new Action(() => SetStatus(text)));
            else
                lblStatus.Text = text;
        }

        private void FlightStatisticForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If we are not exiting: Stay in the tray
            if (!_exiting && _isMainForm)
            {
                e.Cancel = true;
                if (_firstClose)
                {
                    trayIcon.ShowBalloonTip(500, "Run in tray", "Fare Monitor will continue monitoring in System tray", ToolTipIcon.Info);
                    _firstClose = false;
                }

                WindowState = FormWindowState.Minimized;
                Hide();
            }
            else
                Dispose();
        }

        private void btnCheckFareFlex_Click(object sender, EventArgs e)
        {
            var senderBtn = sender as Button;
            senderBtn.Enabled = false;
            var param = GetExecutionParam(false, false, false, OperationMode.Unspecified);
            var newForm = new CheckFareForm(param);
            newForm.Attach(new CheckFareController());
            newForm.Show();
            senderBtn.Enabled = true;
        }

        private ExecutionParam GetExecutionParam(bool autoSync, bool exitAfterDone, bool isMinimized, OperationMode opMode)
        {
            FilterCondition filter = ReadFilter();
            var result = new ExecutionParam()
            {
                ConfigHandler = _executionParam.ConfigHandler,
                AutoSync = autoSync,
                Departure = filter.Departure,
                DepartureDate = filter.DepartureDate,
                DepartureDateRange = _executionParam.DepartureDateRange,
                Destination = filter.Destination,
                ExitAfterDone = exitAfterDone,
                IsMinimized = isMinimized,
                PriceLimit = filter.MaxPrice,
                ReturnDate = filter.ReturnDate,
                ReturnDateRange = _executionParam.ReturnDateRange,
                MinStayDuration = filter.MinDuration,
                MaxStayDuration = filter.MaxDuration,
                OperationMode = opMode
            };

            if (result.Departure == null)
                result.Departure = _executionParam.Departure;
            if (result.Destination == null)
                result.Destination = _executionParam.Destination;

            return result;
        }

        protected override void WndProc(ref Message message)
        {
            if (_isMainForm && message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                ShowWindow();
            }
            base.WndProc(ref message);
        }

        public void ShowWindow()
        {
            NativeMethods.ShowToFront(Handle);
        }

        private void btnToCsv_Click(object sender, EventArgs e)
        {
            lvFlightData.ToCsv();
        }

        private static void MovePrevious(int count, params ComboBox[] itemList)
        {
            foreach (ComboBox item in itemList)
                if (item.SelectedIndex > count - 1)
                    item.SelectedIndex = item.SelectedIndex - count;
        }

        private static void MoveNext(int count, params ComboBox[] itemList)
        {
            foreach (ComboBox item in itemList)
                if (item.SelectedIndex < item.Items.Count - count)
                    item.SelectedIndex = item.SelectedIndex + count;
        }

        private void btnDeptPrev_Click(object sender, EventArgs e)
        {
            MovePrevious(1, cbDepartureDate);
        }

        private void btnDeptNext_Click(object sender, EventArgs e)
        {
            MoveNext(1, cbDepartureDate);
        }

        private void btnRetPrev_Click(object sender, EventArgs e)
        {
            MovePrevious(1, cbReturnDate);
        }

        private void btnRetNext_Click(object sender, EventArgs e)
        {
            MoveNext(1, cbReturnDate);
        }

        private void btnPrevWeek_Click(object sender, EventArgs e)
        {
            MovePrevious(7, cbDepartureDate, cbReturnDate);
        }

        private void btnNextWeek_Click(object sender, EventArgs e)
        {
            MoveNext(7, cbDepartureDate, cbReturnDate);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            ReloadRoutes();
        }

        private void btnAllDates_Click(object sender, EventArgs e)
        {
            if (cbDepartureDate.Items.Count > 0)
            {
                cbDepartureDate.SelectedIndexChanged -= FlightFilter_Changed;
                cbDepartureDate.SelectedIndex = cbDepartureDate.Items.Count - 1;
                cbDepartureDate.SelectedIndexChanged += FlightFilter_Changed;
            }
            if (cbReturnDate.Items.Count > 0)
                cbReturnDate.SelectedIndex = cbReturnDate.Items.Count - 1;
        }

        private void btnAllDepart_Click(object sender, EventArgs e)
        {
            if (cbDepartureDate.Items.Count > 0)
                cbDepartureDate.SelectedIndex = cbDepartureDate.Items.Count - 1;
        }

        private void btnAllReturn_Click(object sender, EventArgs e)
        {
            if (cbReturnDate.Items.Count > 0)
                cbReturnDate.SelectedIndex = cbReturnDate.Items.Count - 1;
        }

        private void btnScheduler_Click(object sender, EventArgs e)
        {
            var exeParam = GetExecutionParam(_executionParam.AutoSync, _executionParam.ExitAfterDone, _executionParam.IsMinimized, OperationMode.GetFareAndSave);
            new SchedulerForm(exeParam, Logger).Show(this);
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            using (var configDialog = new EnvConfiguratorDialog(AppContext.MonitorEnvironment, _executionParam))
            {
                var result = configDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    var resultEnv = configDialog.ResultEnvironment;
                    _executionParam = configDialog.ResultParam;
                    WinFormGlobalContext.GetInstance().SetEnvironment(resultEnv);
                    InitializeView();
                    ReloadRoutes();
                }
            }
        }

        private void btnGetPackage_Click(object sender, EventArgs e)
        {
            var syncDb = FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                // Get the data package and import one by one
                int count = syncDb.ReceivePackages(AppContext.ProgressCallback);
                if (count < 0)
                    count = 0;

                MessageBox.Show(this, count > 0 ? count + " packages were successfully saved to database" : "There is no new package!", "Packages Retrieval", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (count > -1) // Reload data if there is new imported packages
                    FlightFilter_Changed(null, null);
            }
        }

        private void numMinduration_ValueChanged(object sender, EventArgs e)
        {
            // Make sure Max Duration is more than Min Duration
            if (numMaxDuration.Value < numMinDuration.Value)
                numMaxDuration.Value = numMinDuration.Value;
            else
                FlightFilter_Changed(numMinDuration, EventArgs.Empty);
            numMaxDuration.Minimum = numMinDuration.Value;
        }

        private void FlightStatisticForm_Shown(object sender, EventArgs e)
        {
            // Apply the execution parameter
            if (_executionParam != null && _executionParam.IsMinimized)
            {
                WindowState = FormWindowState.Minimized;
                Hide();
                ShowInTaskbar = false;
            }

            _formLoaded = true;

            if (_executionParam != null && _executionParam.OperationMode != OperationMode.Unspecified)  // No need to reload data if we need to do something else
                return;

            // Show overlay animator on first run
            if (_isMainForm && AppContext.FirstStart)
            {
                using (var animator = new OverlayAnimation(new Dictionary<Control, string>()
                    {
                        {btnCheckFareFlex, "Welcome to " + AppUtil.ProductName + "!" + Environment.NewLine + "This main screen shows all stored fare data. And the ticket hunt can be started from here!"},
                        {btnScheduler, "You can also schedule automatic fare scan and forget about it" + Environment.NewLine + "No more looking for cheap air fares, they will come to you!"}
                    }))
                {
                    animator.ShowDialog();
                }
            }

            // If there is no preloaded data: Load data from database
            if (_routeData == null || _routeData.Count < 1)
                ThreadPool.QueueUserWorkItem(new WaitCallback(o =>
                {
                    AppUtil.NameCurrentThread(GetType().Name + "-Shown");
                    Thread.Sleep(500);
                    ReloadRoutes();
                }));
            else
            {
                FillDateComboBox();
            }
        }

        private void journey_Changed(object sender, EventArgs e)
        {
            ReloadRoutes();
        }

        private void mnuQuickStart_Click(object sender, EventArgs e)
        {
            using (var intro = new IntroForm())
                intro.ShowDialog(this);
        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm())
                about.ShowDialog(this);
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            btnHelp.ShowContextMenuStrip();
        }

        private void mnuCheckForUpdate_Click(object sender, EventArgs e)
        {
            var svc = AppContext.MonitorEnvironment.BackgroundServices.Get(typeof(LiveUpdateService)) as LiveUpdateService;
            if (svc == null)
                MessageBox.Show("Live Update is not supported");
            else
                svc.CheckForUpdates(true, false);
        }

        private void SelectedLocationChanged(object sender, EventArgs e)
        {
            // If this event was triggered due to changing of another combobox: Return
            if (_changingLocation)
                return;

            _changingLocation = true;
            try
            {
                if (_routeData == null)
                    return;

                Airport selDeparture = null;
                Airport selDestination = null;
                if (sender == cbDeparture)  // If departure location was changed
                {
                    if (cbDeparture.Items.Count < 1)
                        return;

                    if (_loadingRoutes)
                    {
                        if (_executionParam.Departure != null)
                            cbDeparture.SelectedAirportCode = _executionParam.Departure.IATA;
                        if (cbDeparture.SelectedItem == null && cbDeparture.Items.Count > 0)    // If the configured departure does not exist in database
                            cbDeparture.SelectedItem = cbDeparture.Items[0];
                    }

                    selDeparture = cbDeparture.SelectedAirport;
                    List<Airport> destList = null;
                    if (_routeData != null)
                    {
                        destList = new List<Airport>();
                        foreach (var r in _routeData)
                        {
                            if (String.Equals(r.Departure.IATA, selDeparture.IATA, StringComparison.OrdinalIgnoreCase) && !destList.Contains(r.Destination))
                                destList.Add(r.Destination);
                        }
                    }

                    cbDestination.DataSource = destList;
                    if (_loadingRoutes)
                    {
                        if (_executionParam.Destination != null)
                            cbDestination.SelectedAirport = _executionParam.Destination;
                        if (cbDestination.SelectedItem == null && cbDestination.Items.Count > 0)    // If the configured destination does not exist in database
                            cbDestination.SelectedItem = cbDestination.Items[0];                    // Select the first item available
                    }
                }
                else
                    selDeparture = cbDeparture.SelectedAirport;

                // If the destination was specified before: Try to match it with the new data
                selDestination = cbDestination.SelectedAirport;
                if (_isMainForm)
                {
                    _activeRoute = null;    // Clear the active route first
                    if (selDeparture != null && selDestination != null)
                    {
                        // Reload data only if we are running on main form
                        foreach (var r in _routeData)
                        {
                            string deptCode = r.Departure.IATA,
                                   destCode = r.Destination.IATA;
                            if (String.Equals(deptCode, selDeparture.IATA, StringComparison.OrdinalIgnoreCase)
                                && String.Equals(destCode, selDestination.IATA, StringComparison.OrdinalIgnoreCase))
                            {
                                _activeRoute = r;
                                Logger.DebugFormat("Reloading routes [{0}]-[{1}]...", deptCode, destCode);
                                FareDatabase.LoadData(_activeRoute, false, false, false, AppContext.ProgressCallback);
                                break;
                            }
                        }
                    }
                }
                FlightFilter_Changed(null, e);
            }
            finally
            {
                _changingLocation = false;
            }
        }
    }
}