namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data.Monitoring;
    using SkyDean.FareLiz.Service.LiveUpdate;
    using SkyDean.FareLiz.WinForm.Components.Controls;
    using SkyDean.FareLiz.WinForm.Components.Controls.ComboBox;
    using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Components.Utils;
    using SkyDean.FareLiz.WinForm.Presentation.Controllers;
    using SkyDean.FareLiz.WinForm.Utils;

    /// <summary>The flight statistic form.</summary>
    internal sealed partial class FlightStatisticForm : SmartForm
    {
        /// <summary>The st r_ forma t_ dat e_ combo.</summary>
        private const string STR_FORMAT_DATE_COMBO = "dd/MM/yyyy ddd";

        /// <summary>The st r_ on e_ way.</summary>
        private const string STR_ONE_WAY = "One-way trip";

        /// <summary>The _is main form.</summary>
        private readonly bool _isMainForm;

        /// <summary>The _lock obj.</summary>
        private readonly object _lockObj = new object();

        /// <summary>The mnu check this date.</summary>
        private readonly ToolStripMenuItem mnuCheckThisDate = new ToolStripMenuItem("Check latest fare for this travel date");

        /// <summary>The mnu separator.</summary>
        private readonly ToolStripSeparator mnuSeparator = new ToolStripSeparator();

        /// <summary>The mnu view this date.</summary>
        private readonly ToolStripMenuItem mnuViewThisDate = new ToolStripMenuItem("View fare history for this travel date");

        /// <summary>The _active route.</summary>
        private TravelRoute _activeRoute;

        /// <summary>The _changing location.</summary>
        private bool _changingLocation;

        /// <summary>The _execution param.</summary>
        private ExecutionParam _executionParam;

        /// <summary>The _exiting.</summary>
        private bool _exiting;

        /// <summary>The _first close.</summary>
        private bool _firstClose = true;

        /// <summary>The _form loaded.</summary>
        private bool _formLoaded;

        /// <summary>The _loading routes.</summary>
        private bool _loadingRoutes;

        /// <summary>The _lv change request date.</summary>
        private DateTime _lvChangeRequestDate;

        /// <summary>The _lv change requested.</summary>
        private bool _lvChangeRequested;

        /// <summary>The _route data.</summary>
        private IList<TravelRoute> _routeData;

        /// <summary>The _suppress refresh data view.</summary>
        private bool _suppressRefreshDataView;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightStatisticForm"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="executionParam">
        /// The execution param.
        /// </param>
        /// <param name="isMainForm">
        /// The is main form.
        /// </param>
        public FlightStatisticForm(IList<TravelRoute> data, ExecutionParam executionParam, bool isMainForm)
        {
            this.InitializeComponent();
            this._isMainForm = isMainForm;
            this._routeData = data;
            if (data != null && data.Count > 0)
            {
                this._activeRoute = data[0];
                this.ReloadRoutes();
            }

            this._executionParam = executionParam;
            this.InitializeView();
        }

        /// <summary>Gets the fare data provider.</summary>
        public IFareDataProvider FareDataProvider
        {
            get
            {
                return AppContext.MonitorEnvironment.FareDataProvider;
            }
        }

        /// <summary>Gets the archive manager.</summary>
        public IArchiveManager ArchiveManager
        {
            get
            {
                return AppContext.MonitorEnvironment.ArchiveManager;
            }
        }

        /// <summary>Gets the fare database.</summary>
        public IFareDatabase FareDatabase
        {
            get
            {
                return AppContext.MonitorEnvironment.FareDatabase;
            }
        }

        /// <summary>Gets the logger.</summary>
        public ILogger Logger
        {
            get
            {
                return AppContext.MonitorEnvironment.Logger;
            }
        }

        /// <summary>The initialize view.</summary>
        private void InitializeView()
        {
            this.trayIcon.Visible = this._isMainForm;
            if (this._isMainForm)
            {
                GUIBuilder.AttachMenuToTrayIcon(this, this.trayIcon, true);
                this.trayIcon.Icon = this.Icon;
            }
            else
            {
                this.btnExit.Text = "Close";
                this.btnRefresh.Enabled = this.chkAllDataHistory.Enabled = false;
                this.cbAirlines.Width += this.btnGetPackage.Right - this.cbAirlines.Right; // Try to fill the screen
                var gap = this.btnRefresh.Right - this.btnToCsv.Right;
                this.btnToCsv.Left += gap;
                this.btnGraph.Left += gap;
            }

            bool isDbConfigured = this.FareDatabase != null;
            this.btnRefresh.Visible = this.btnGetPackage.Visible = this._isMainForm;
            this.btnGetPackage.Enabled = isDbConfigured && this.FareDatabase is ISyncableDatabase;
            this.btnRefresh.Enabled = this.lvFlightData.Enabled = isDbConfigured;

            this.btnCheckFareFlex.Enabled = this.btnScheduler.Enabled = this.FareDataProvider != null;

            this.mnuViewThisDate.Tag = this.mnuCheckThisDate.Tag = this.lvFlightData;
            this.lvFlightData.AppendContextMenuStrip(this.mnuSeparator, this.mnuViewThisDate, this.mnuCheckThisDate);
            this.lvFlightData.OnContextMenuStripOpening += this.lvFlightData_OnContextMenuStripOpening;
            this.mnuCheckThisDate.Click += this.mnuCheckThisDate_Click;
            this.mnuViewThisDate.Click += this.mnuViewThisDate_Click;

            this.numPriceLimit.Value = this._executionParam.PriceLimit;
            this.numMinDuration.Value = this._executionParam.MinStayDuration;
            this.numMaxDuration.Value = this._executionParam.MaxStayDuration;

            this.Text = AppUtil.ProductName + " " + this.Text;
            this.progressBar.ContainerControl = this;

            this.mnuAbout.Text = "&About " + AppUtil.ProductName;
        }

        /// <summary>
        /// The mnu view this date_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuViewThisDate_Click(object sender, EventArgs e)
        {
            var lv = ((ToolStripItem)sender).Tag as FlightDataListView;
            if (lv == null)
            {
                return;
            }

            var selFlight = lv.SelectedFlight;
            if (selFlight != null)
            {
                this.cbDepartureDate.SelectedItem = selFlight.OutboundLeg.Departure.ToString(STR_FORMAT_DATE_COMBO);
                this.cbReturnDate.SelectedItem = selFlight.InboundLeg.Departure.ToString(STR_FORMAT_DATE_COMBO);
            }
        }

        /// <summary>
        /// The mnu check this date_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuCheckThisDate_Click(object sender, EventArgs e)
        {
            var lv = ((ToolStripItem)sender).Tag as FlightDataListView;
            if (lv == null)
            {
                return;
            }

            var selFlight = lv.SelectedFlight;
            if (selFlight != null)
            {
                if (selFlight.OutboundLeg.Departure < DateTime.Now)
                {
                    return;
                }

                var param = this.GetExecutionParam(false, false, false, OperationMode.ShowFare);
                param.DepartureDate = selFlight.OutboundLeg.Departure;
                param.ReturnDate = selFlight.InboundLeg.Departure;
                param.DepartureDateRange = param.ReturnDateRange = DateRangeDiff.Empty;
                param.MinStayDuration = param.MaxStayDuration = (int)(param.ReturnDate - param.DepartureDate).TotalDays;
                var checkFareForm = new CheckFareForm(param);
                checkFareForm.Attach(new CheckFareController());
                checkFareForm.Show();
            }
        }

        /// <summary>
        /// The lv flight data_ on context menu strip opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lvFlightData_OnContextMenuStripOpening(FlightDataListView sender, MenuBuilderEventArgs e)
        {
            this.mnuSeparator.Available = this.mnuCheckThisDate.Available = this.mnuViewThisDate.Available = false;
            var selFlight = sender.SelectedFlight;
            if (selFlight != null)
            {
                if (selFlight.OutboundLeg.Departure.Date >= DateTime.Now.Date)
                {
                    this.mnuCheckThisDate.Available = true;
                    this.mnuCheckThisDate.Text = string.Format("Check latest fare for [{0}]", selFlight.TravelDateString);
                }

                this.mnuSeparator.Available = this.mnuViewThisDate.Available = true;
                this.mnuViewThisDate.Text = string.Format("View only fare for [{0}]", selFlight.TravelDateString);
            }
        }

        /// <summary>
        /// The set filters.
        /// </summary>
        /// <param name="minDuration">
        /// The min duration.
        /// </param>
        /// <param name="maxDuration">
        /// The max duration.
        /// </param>
        /// <param name="priceLimit">
        /// The price limit.
        /// </param>
        public void SetFilters(int minDuration, int maxDuration, int priceLimit)
        {
            this.numMinDuration.Value = minDuration;
            this.numMaxDuration.Value = maxDuration;
            this.numPriceLimit.Value = priceLimit;
        }

        /// <summary>Fill the data for departure/return date combobox filter</summary>
        private void FillDateComboBox()
        {
            this._suppressRefreshDataView = true;
            try
            {
                this.cbDepartureDate.Items.Clear();
                this.cbReturnDate.Items.Clear();

                if (this._activeRoute == null || this._activeRoute.Journeys.Count < 1)
                {
                    this.SetStatus("There is no historical fare data for selected route");
                    this.lvFlightData.SetDataSourceAsync(null, false);
                    return;
                }

                // Get last selected items
                object lastDepartValue = this.cbDepartureDate.Items.Count > 0
                                             ? this.cbDepartureDate.SelectedItem
                                             : (this._executionParam.DepartureDate.IsDefined()
                                                    ? this._executionParam.DepartureDate.ToString(STR_FORMAT_DATE_COMBO)
                                                    : null), 
                       lastReturnValue = this.cbReturnDate.Items.Count > 0
                                             ? this.cbReturnDate.SelectedItem
                                             : (this._executionParam.ReturnDate.IsDefined()
                                                    ? this._executionParam.ReturnDate.ToString(STR_FORMAT_DATE_COMBO)
                                                    : null);

                // Get the distinct list of departure and return date
                IOrderedEnumerable<DateTime> deptDateList =
                    this._activeRoute.Journeys.DistinctBy(o => o.DepartureDate).Select(o => o.DepartureDate).OrderBy(o => o);
                IOrderedEnumerable<DateTime> retDateList =
                    this._activeRoute.Journeys.DistinctBy(o => o.ReturnDate).Select(o => o.ReturnDate).OrderBy(o => o);

                foreach (DateTime d in deptDateList)
                {
                    this.cbDepartureDate.Items.Add(d.ToString(STR_FORMAT_DATE_COMBO));
                }

                int retCount = 0;
                foreach (DateTime d in retDateList)
                {
                    // Check if the first return date is DateTime.MinValue: If yes, add the item to the combobox
                    if (++retCount == 1 && d == DateTime.MinValue)
                    {
                        this.cbReturnDate.Items.Add(STR_ONE_WAY); // Add a new item for one way trip
                    }
                    else
                    {
                        this.cbReturnDate.Items.Add(d.ToString(STR_FORMAT_DATE_COMBO));
                    }
                }

                this.cbDepartureDate.Items.Add(string.Empty); // Add an empty row to the end (this allows user to make the filter "undefined"
                this.cbReturnDate.Items.Add(string.Empty);

                // Try to match the previously selected item with the new data source
                if (lastDepartValue != null && this.cbDepartureDate.Items.Contains(lastDepartValue))
                {
                    this.cbDepartureDate.SelectedItem = lastDepartValue;
                }
                else
                {
                    if (this._isMainForm)
                    {
                        this.cbDepartureDate.SelectedIndex = 0;
                    }
                    else
                    {
                        this.cbDepartureDate.SelectedIndex = this.cbDepartureDate.Items.Count - 1;
                    }
                }

                if (lastReturnValue != null && this.cbReturnDate.Items.Contains(lastReturnValue))
                {
                    this.cbReturnDate.SelectedItem = lastReturnValue;
                }
                else
                {
                    if (this._isMainForm)
                    {
                        this.cbReturnDate.SelectedIndex = 0;
                    }
                    else
                    {
                        this.cbReturnDate.SelectedIndex = this.cbReturnDate.Items.Count - 1;
                    }
                }

                // Manually trigger the event to refresh the data view
                this._suppressRefreshDataView = false;
                this.FlightFilter_Changed(this, null);
            }
            finally
            {
                this._suppressRefreshDataView = false;
            }
        }

        /// <summary>
        /// The btn exit_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this._exiting = true;
            if (this._isMainForm)
            {
                Environment.Exit(0);
            }
            else
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// Bind the list view asynchronously
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="departureDate">
        /// The departure Date.
        /// </param>
        /// <param name="returnDate">
        /// The return Date.
        /// </param>
        /// <param name="priceLimit">
        /// The price Limit.
        /// </param>
        /// <param name="allHistory">
        /// The all History.
        /// </param>
        /// <param name="amountOfPrices">
        /// The amount Of Prices.
        /// </param>
        /// <param name="minDuration">
        /// The min Duration.
        /// </param>
        /// <param name="maxDuration">
        /// The max Duration.
        /// </param>
        /// <param name="opearators">
        /// The opearators.
        /// </param>
        /// <param name="populateData">
        /// The populate Data.
        /// </param>
        private void BindListViewAsync(
            TravelRoute route, 
            DateTime departureDate, 
            DateTime returnDate, 
            double priceLimit, 
            bool allHistory, 
            int amountOfPrices, 
            int minDuration, 
            int maxDuration, 
            Dictionary<string, bool> opearators, 
            bool populateData)
        {
            if (!this._formLoaded)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(
                o =>
                    {
                        AppUtil.NameCurrentThread(this.GetType().Name + "-BindListView");
                        this.BindListView(
                            route, 
                            departureDate, 
                            returnDate, 
                            priceLimit, 
                            allHistory, 
                            amountOfPrices, 
                            minDuration, 
                            maxDuration, 
                            opearators, 
                            populateData);
                    });
        }

        /// <summary>
        /// The bind list view.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="departureDate">
        /// The departure date.
        /// </param>
        /// <param name="returnDate">
        /// The return date.
        /// </param>
        /// <param name="priceLimit">
        /// The price limit.
        /// </param>
        /// <param name="viewAllHistory">
        /// The view all history.
        /// </param>
        /// <param name="amountFlightsPerData">
        /// The amount flights per data.
        /// </param>
        /// <param name="minDuration">
        /// The min duration.
        /// </param>
        /// <param name="maxDuration">
        /// The max duration.
        /// </param>
        /// <param name="opearators">
        /// The opearators.
        /// </param>
        /// <param name="populateData">
        /// The populate data.
        /// </param>
        private void BindListView(
            TravelRoute route, 
            DateTime departureDate, 
            DateTime returnDate, 
            double priceLimit, 
            bool viewAllHistory, 
            int amountFlightsPerData, 
            int minDuration, 
            int maxDuration, 
            Dictionary<string, bool> opearators, 
            bool populateData)
        {
            DateTime requestDate = DateTime.Now;
            if (!Monitor.TryEnter(this._lockObj))
            {
                // The view is being refreshed: Send a signal
                this._lvChangeRequested = true;
                this._lvChangeRequestDate = requestDate;
                Monitor.Enter(this._lockObj);
            }

            this._lvChangeRequested = false;
            try
            {
                if (requestDate < this._lvChangeRequestDate)
                {
                    // Another request has come in: Let the later one passes through
                    return;
                }

                var data = route.Journeys;
                if (data == null || data.Count < 1)
                {
                    return;
                }

                this.SafeInvoke(
                    (Action)delegate
                        {
                            progressBar.ShowInTaskbar = progressBar.Visible = true;
                            progressBar.Style = ProgressBarStyle.Marquee;
                        });

                // Get all the needed data according to the Date filter condition
                IEnumerable<Journey> journeyCollection = null;
                if (departureDate.IsDefined())
                {
                    // Known departure date
                    if (returnDate == DateTime.MaxValue)
                    {
                        // Unknown return date
                        journeyCollection = data.Where(o => o.DepartureDate.Date == departureDate.Date).OrderByDescending(o => o.ReturnDate);
                    }
                    else
                    {
                        // Known return date or one-way trip for DateTime.MinValue
                        journeyCollection = data.Where(o => o.DepartureDate.Date == departureDate.Date && o.ReturnDate.Date == returnDate.Date);
                    }
                }
                else
                {
                    // Unknown departure date
                    if (returnDate == DateTime.MaxValue)
                    {
                        // Unknown return date
                        journeyCollection = data.OrderBy(o => o.DepartureDate).ThenByDescending(o => o.ReturnDate);
                    }
                    else
                    {
                        // Known return date or one-way trip for DateTime.MinValue
                        journeyCollection = data.Where(o => o.ReturnDate.Date == returnDate.Date).OrderBy(o => o.DepartureDate);
                    }
                }

                var newOperators = new List<string>();
                var existOperatorSelections = new List<CheckBoxComboBoxItem>();

                if (populateData)
                {
                    this.SetStatus("Loading flight data...");
                    const int loadSize = 1000;
                    var loadBatch = new List<Journey>();
                    foreach (var j in journeyCollection)
                    {
                        loadBatch.Add(j);
                        if (loadBatch.Count == loadSize)
                        {
                            this.FareDatabase.LoadData(loadBatch, viewAllHistory, AppContext.ProgressCallback);
                            loadBatch.Clear();
                        }

                        if (this._lvChangeRequested)
                        {
                            return;
                        }
                    }

                    if (!this._lvChangeRequested && loadBatch.Count > 0)
                    {
                        this.FareDatabase.LoadData(loadBatch, viewAllHistory, AppContext.ProgressCallback);
                    }
                }

                this.SetStatus("Processing data...");

                int totalJourneysCount = journeyCollection.Count();
                int totalFlightsCount = 0;
                int skippedDuration = 0, skippedPrice = 0, skippedIgnored = 0;
                var flightsData = new List<Flight>();

                this.SafeInvoke(
                    (Action)delegate
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
                    if (this._lvChangeRequested)
                    {
                        return;
                    }

                    bool skippedJourney = false;
                    if (checkDuration)
                    {
                        int stayDuration = (int)(j.ReturnDate.Date - j.DepartureDate.Date).TotalDays;
                        if (stayDuration < 0)
                        {
                            // One-way trip
                            stayDuration = 0;
                        }

                        skippedJourney = stayDuration < minDuration || stayDuration > maxDuration;
                    }

                    List<Flight> journeyFlights = skippedJourney ? null : new List<Flight>();
                    foreach (var d in j.Data)
                    {
                        // Keep counts for statistics
                        totalFlightsCount += d.Flights.Count;

                        if (skippedJourney)
                        {
                            skippedDuration += d.Flights.Count;
                        }
                        else
                        {
                            var flights = d.Flights;
                            flights.Sort(FlightPriceComparer.Instance);

                            int taking = amountFlightsPerData > flights.Count ? flights.Count : amountFlightsPerData;
                            skippedIgnored += flights.Count - taking;
                            for (int i = 0; i < taking; i++)
                            {
                                if (flights[i].Price > priceLimit)
                                {
                                    skippedPrice += taking - i;
                                    break;
                                }

                                journeyFlights.Add(flights[i]);
                            }
                        }
                    }

                    if (journeyFlights == null)
                    {
                        continue;
                    }

                    // Fill the list of flight operators for filter purpose
                    foreach (Flight flight in journeyFlights)
                    {
                        string flightOperator = flight.Operator;
                        if (opearators != null)
                        {
                            if (opearators.ContainsKey(flightOperator) && !opearators[flightOperator])
                            {
                                continue;
                            }
                        }

                        flightsData.Add(flight);

                        if (!newOperators.Contains(flightOperator))
                        {
                            newOperators.Add(flightOperator);
                        }
                    }
                }

                this.SetStatus("Rendering view...");
                this.SafeInvoke(
                    new Action(
                        () =>
                            {
                                this.progressBar.Style = ProgressBarStyle.Marquee;
                                this.lvFlightData.SetDataSourceAsync(flightsData, true);

                                if (opearators == null)
                                {
                                    // Reload data - Refill list of operators
                                    var newOperatorCheckList = new Dictionary<string, bool>();
                                    if (opearators != null)
                                    {
                                        foreach (var v in opearators)
                                        {
                                            newOperatorCheckList.Add(v.Key, v.Value);
                                        }
                                    }

                                    foreach (string s in newOperators)
                                    {
                                        if (!newOperatorCheckList.ContainsKey(s))
                                        {
                                            newOperatorCheckList.Add(s, true);
                                        }
                                    }

                                    List<KeyValuePair<string, bool>> sortedOps = newOperatorCheckList.OrderBy(p => p.Key).ToList();

                                    this.cbAirlines.SelectedValueChanged -= this.FlightFilter_Changed;
                                    this.cbAirlines.TextChanged -= this.FlightFilter_Changed;
                                    this.cbAirlines.Text = null;
                                    this.cbAirlines.Items.Clear();
                                    this.cbAirlines.CheckBoxItems.Clear();

                                    sortedOps.ForEach(
                                        p =>
                                            {
                                                int pos = this.cbAirlines.Items.Add(p.Key);
                                                this.cbAirlines.CheckBoxItems[pos].Checked = p.Value;
                                            });
                                    this.cbAirlines.SelectedValueChanged += this.FlightFilter_Changed;
                                    this.cbAirlines.TextChanged += this.FlightFilter_Changed;
                                }

                                this.btnToCsv.Enabled = this.btnGraph.Enabled = flightsData.Count > 0;
                                this.progressBar.Visible = false;

                                string status;
                                if (totalFlightsCount == 0)
                                {
                                    status = "There is no flight data for this date";
                                }
                                else
                                {
                                    string journeyStr = totalJourneysCount > 1 ? " (" + totalJourneysCount + " dates)" : null;
                                    var parts = new List<string>();
                                    if (skippedDuration > 0)
                                    {
                                        parts.Add(skippedDuration + " not in duration range");
                                    }

                                    if (skippedPrice > 0)
                                    {
                                        parts.Add(skippedPrice + " too expensive");
                                    }

                                    if (skippedIgnored > 0)
                                    {
                                        parts.Add(skippedIgnored + " ignored");
                                    }

                                    if (flightsData.Count > 0)
                                    {
                                        parts.Add(flightsData.Count + " displayed");
                                    }

                                    string skipStr = parts.Count > 0 ? " | " + string.Join(", ", parts.ToArray()) : string.Empty;
                                    status = string.Format("Available flights: {0}{1}{2}", totalFlightsCount, journeyStr, skipStr);
                                }

                                this.numMinDuration.Enabled = this.numMaxDuration.Enabled = checkDuration;
                                this.SetStatus(status);
                            }));
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex);
                MessageBox.Show(this, ex.ToString(), "Could not display data");
                this.SetStatus("An internal error occured");
            }
            finally
            {
                this.SafeInvoke(
                    (Action)(() =>
                        {
                            this.progressBar.ShowInTaskbar = this.progressBar.Visible = false;
                            this.progressBar.Value = 0;
                        }));
                Monitor.Exit(this._lockObj);
            }
        }

        /// <summary>
        /// The flight filter_ changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlightFilter_Changed(object sender, EventArgs e)
        {
            if (this._suppressRefreshDataView)
            {
                return;
            }

            if (sender == null)
            {
                // In case of programmatically trigger the event
                this.FillDateComboBox();
                return;
            }

            bool reloadData = sender == this.cbDepartureDate || sender == this.cbReturnDate || sender == this;

            FilterCondition condition = this.ReadFilter();

            // If users chose one way trip: Do not enable the numeric selection for stay duration
            this.numMinDuration.Enabled = this.numMaxDuration.Enabled = condition.DepartureDate.IsUndefined() || condition.ReturnDate.IsUndefined();
            this.BindListViewAsync(
                this._activeRoute, 
                condition.DepartureDate, 
                condition.ReturnDate, 
                condition.MaxPrice, 
                this.chkAllDataHistory.Checked, 
                decimal.ToInt32(this.numAmountOfPrices.Value), 
                condition.MinDuration, 
                condition.MaxDuration, 
                reloadData ? null : condition.Operators, 
                this._isMainForm);
        }

        /// <summary>Read the current filter based on the view</summary>
        /// <returns>The <see cref="FilterCondition" />.</returns>
        private FilterCondition ReadFilter()
        {
            DateTime departureDate = string.IsNullOrEmpty(this.cbDepartureDate.Text)
                                         ? DateTime.MinValue
                                         : DateTime.ParseExact(this.cbDepartureDate.Text, STR_FORMAT_DATE_COMBO, null);
            DateTime returnDate = string.IsNullOrEmpty(this.cbReturnDate.Text)
                                      ? DateTime.MaxValue
                                      : (this.cbReturnDate.Text == STR_ONE_WAY
                                             ? DateTime.MinValue
                                             : DateTime.ParseExact(this.cbReturnDate.Text, STR_FORMAT_DATE_COMBO, null));

            int minDuration = decimal.ToInt32(this.numMinDuration.Value);
            int maxDuration = decimal.ToInt32(this.numMaxDuration.Value);
            int maxPrice = decimal.ToInt32(this.numPriceLimit.Value);

            var ops = new Dictionary<string, bool>();
            this.cbAirlines.CheckBoxItems.ForEach(i => ops.Add(i.Text, i.Checked));
            var filter = new FilterCondition(
                this.cbDeparture.SelectedAirport, 
                this.cbDestination.SelectedAirport, 
                minDuration, 
                maxDuration, 
                maxPrice, 
                departureDate, 
                returnDate, 
                ops);
            return filter;
        }

        /// <summary>
        /// The btn graph_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnGraph_Click(object sender, EventArgs e)
        {
            var data = this.lvFlightData.CurrentData;
            if (data == null || data.Count < 1)
            {
                return;
            }

            var journeys = new List<Journey>();

            // Create new data based on the visible items inside the listview
            for (int i = 0; i < data.Count; i++)
            {
                var f = data[i];
                var j = f.JourneyData.JourneyInfo;
                if (!journeys.Contains(j))
                {
                    journeys.Add(j);
                }
            }

            var g = new GraphForm(journeys);
            g.Show(this);
        }

        /// <summary>The reload routes.</summary>
        private void ReloadRoutes()
        {
            if (this._isMainForm && this.FareDatabase == null)
            {
                this.SetStatus("There is no configured Fare Database! Historical fare data will be unavailable!");
            }
            else
            {
                this.SetStatus("Loading journey data from database...");
                ThreadPool.QueueUserWorkItem(
                    o =>
                        {
                            AppUtil.NameCurrentThread(this.GetType().Name + "-LoadJourney");
                            this.DoLoadRoutes();
                        });
            }
        }

        /// <summary>Do the actual database operation to load the list of routes</summary>
        private void DoLoadRoutes()
        {
            Exception err = null;
            this._loadingRoutes = true;
            try
            {
                if (this._isMainForm)
                {
                    this._routeData = this.FareDatabase.GetRoutes(false, false, false, false, AppContext.ProgressCallback);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error(ex);
                err = ex;
            }
            finally
            {
                List<Airport> originList = null;
                if (this._routeData != null)
                {
                    originList = new List<Airport>();

                    // Get the distinct list of departures from the stored routes
                    foreach (var j in this._routeData)
                    {
                        bool exist = false;
                        foreach (var dept in originList)
                        {
                            if (string.Equals(dept.IATA, j.Departure.IATA, StringComparison.OrdinalIgnoreCase))
                            {
                                exist = true;
                                break;
                            }
                        }

                        if (!exist)
                        {
                            originList.Add(j.Departure);
                        }
                    }

                    originList.Sort((x, y) => x.Name.CompareTo(y.Name));
                }

                this.SafeInvoke(
                    new Action(
                        () =>
                            {
                                // The location change event won't be triggered: Check if the current displayed route still exists in the database and set reference to the new object
                                this.cbDeparture.SelectedItem = null;
                                this.cbDeparture.DataSource = originList;
                                bool noRoute = originList == null || originList.Count == 0;
                                this.cbDeparture.Enabled = this.cbDestination.Enabled = !noRoute;
                                if (noRoute)
                                {
                                    this.cbDeparture.Text = this.cbDestination.Text = "There is no stored fare data in database";
                                }

                                if (err == null)
                                {
                                    this.grpDetail.Text = string.Format("Travel Detail (data was updated on {0})", DateTime.Now);
                                    this.FlightFilter_Changed(null, null);
                                }
                                else
                                {
                                    MessageBox.Show(
                                        this, 
                                        "Failed to load journey data: " + err.Message + Environment.NewLine + Environment.NewLine
                                        + "If the problem persists, this normally indicates that the database is corrupted. It is recommended to reset the database using the Configuration window!", 
                                        "An error occured", 
                                        MessageBoxButtons.OK, 
                                        MessageBoxIcon.Error);
                                }
                            }));

                this._loadingRoutes = false;
            }
        }

        /// <summary>
        /// The set status.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        private void SetStatus(string text)
        {
            if (this.InvokeRequired)
            {
                this.SafeInvoke(new Action(() => this.SetStatus(text)));
            }
            else
            {
                this.lblStatus.Text = text;
            }
        }

        /// <summary>
        /// The flight statistic form_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlightStatisticForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // If we are not exiting: Stay in the tray
            if (!this._exiting && this._isMainForm)
            {
                e.Cancel = true;
                if (this._firstClose)
                {
                    this.trayIcon.ShowBalloonTip(500, "Run in tray", "Fare Monitor will continue monitoring in System tray", ToolTipIcon.Info);
                    this._firstClose = false;
                }

                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            else
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// The btn check fare flex_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnCheckFareFlex_Click(object sender, EventArgs e)
        {
            var senderBtn = sender as Button;
            senderBtn.Enabled = false;
            var param = this.GetExecutionParam(false, false, false, OperationMode.Unspecified);
            var newForm = new CheckFareForm(param);
            newForm.Attach(new CheckFareController());
            newForm.Show();
            senderBtn.Enabled = true;
        }

        /// <summary>
        /// The get execution param.
        /// </summary>
        /// <param name="autoSync">
        /// The auto sync.
        /// </param>
        /// <param name="exitAfterDone">
        /// The exit after done.
        /// </param>
        /// <param name="isMinimized">
        /// The is minimized.
        /// </param>
        /// <param name="opMode">
        /// The op mode.
        /// </param>
        /// <returns>
        /// The <see cref="ExecutionParam"/>.
        /// </returns>
        private ExecutionParam GetExecutionParam(bool autoSync, bool exitAfterDone, bool isMinimized, OperationMode opMode)
        {
            FilterCondition filter = this.ReadFilter();
            var result = new ExecutionParam
                             {
                                 ConfigHandler = this._executionParam.ConfigHandler, 
                                 AutoSync = autoSync, 
                                 Departure = filter.Departure, 
                                 DepartureDate = filter.DepartureDate, 
                                 DepartureDateRange = this._executionParam.DepartureDateRange, 
                                 Destination = filter.Destination, 
                                 ExitAfterDone = exitAfterDone, 
                                 IsMinimized = isMinimized, 
                                 PriceLimit = filter.MaxPrice, 
                                 ReturnDate = filter.ReturnDate, 
                                 ReturnDateRange = this._executionParam.ReturnDateRange, 
                                 MinStayDuration = filter.MinDuration, 
                                 MaxStayDuration = filter.MaxDuration, 
                                 OperationMode = opMode
                             };

            if (result.Departure == null)
            {
                result.Departure = this._executionParam.Departure;
            }

            if (result.Destination == null)
            {
                result.Destination = this._executionParam.Destination;
            }

            return result;
        }

        /// <summary>
        /// The wnd proc.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        protected override void WndProc(ref Message message)
        {
            if (this._isMainForm && message.Msg == SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                this.ShowWindow();
            }

            base.WndProc(ref message);
        }

        /// <summary>The show window.</summary>
        public void ShowWindow()
        {
            NativeMethods.ShowToFront(this.Handle);
        }

        /// <summary>
        /// The btn to csv_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnToCsv_Click(object sender, EventArgs e)
        {
            this.lvFlightData.ToCsv();
        }

        /// <summary>
        /// The move previous.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="itemList">
        /// The item list.
        /// </param>
        private static void MovePrevious(int count, params ComboBox[] itemList)
        {
            foreach (ComboBox item in itemList)
            {
                if (item.SelectedIndex > count - 1)
                {
                    item.SelectedIndex = item.SelectedIndex - count;
                }
            }
        }

        /// <summary>
        /// The move next.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <param name="itemList">
        /// The item list.
        /// </param>
        private static void MoveNext(int count, params ComboBox[] itemList)
        {
            foreach (ComboBox item in itemList)
            {
                if (item.SelectedIndex < item.Items.Count - count)
                {
                    item.SelectedIndex = item.SelectedIndex + count;
                }
            }
        }

        /// <summary>
        /// The btn dept prev_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDeptPrev_Click(object sender, EventArgs e)
        {
            MovePrevious(1, this.cbDepartureDate);
        }

        /// <summary>
        /// The btn dept next_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnDeptNext_Click(object sender, EventArgs e)
        {
            MoveNext(1, this.cbDepartureDate);
        }

        /// <summary>
        /// The btn ret prev_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRetPrev_Click(object sender, EventArgs e)
        {
            MovePrevious(1, this.cbReturnDate);
        }

        /// <summary>
        /// The btn ret next_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRetNext_Click(object sender, EventArgs e)
        {
            MoveNext(1, this.cbReturnDate);
        }

        /// <summary>
        /// The btn prev week_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnPrevWeek_Click(object sender, EventArgs e)
        {
            MovePrevious(7, this.cbDepartureDate, this.cbReturnDate);
        }

        /// <summary>
        /// The btn next week_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnNextWeek_Click(object sender, EventArgs e)
        {
            MoveNext(7, this.cbDepartureDate, this.cbReturnDate);
        }

        /// <summary>
        /// The btn refresh_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.ReloadRoutes();
        }

        /// <summary>
        /// The btn all dates_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnAllDates_Click(object sender, EventArgs e)
        {
            if (this.cbDepartureDate.Items.Count > 0)
            {
                this.cbDepartureDate.SelectedIndexChanged -= this.FlightFilter_Changed;
                this.cbDepartureDate.SelectedIndex = this.cbDepartureDate.Items.Count - 1;
                this.cbDepartureDate.SelectedIndexChanged += this.FlightFilter_Changed;
            }

            if (this.cbReturnDate.Items.Count > 0)
            {
                this.cbReturnDate.SelectedIndex = this.cbReturnDate.Items.Count - 1;
            }
        }

        /// <summary>
        /// The btn all depart_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnAllDepart_Click(object sender, EventArgs e)
        {
            if (this.cbDepartureDate.Items.Count > 0)
            {
                this.cbDepartureDate.SelectedIndex = this.cbDepartureDate.Items.Count - 1;
            }
        }

        /// <summary>
        /// The btn all return_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnAllReturn_Click(object sender, EventArgs e)
        {
            if (this.cbReturnDate.Items.Count > 0)
            {
                this.cbReturnDate.SelectedIndex = this.cbReturnDate.Items.Count - 1;
            }
        }

        /// <summary>
        /// The btn scheduler_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnScheduler_Click(object sender, EventArgs e)
        {
            var exeParam = this.GetExecutionParam(
                this._executionParam.AutoSync, 
                this._executionParam.ExitAfterDone, 
                this._executionParam.IsMinimized, 
                OperationMode.GetFareAndSave);
            new SchedulerForm(exeParam, this.Logger).Show(this);
        }

        /// <summary>
        /// The btn config_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnConfig_Click(object sender, EventArgs e)
        {
            using (var configDialog = new EnvConfiguratorDialog(AppContext.MonitorEnvironment, this._executionParam))
            {
                var result = configDialog.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    var resultEnv = configDialog.ResultEnvironment;
                    this._executionParam = configDialog.ResultParam;
                    WinFormGlobalContext.GetInstance().SetEnvironment(resultEnv);
                    this.InitializeView();
                    this.ReloadRoutes();
                }
            }
        }

        /// <summary>
        /// The btn get package_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnGetPackage_Click(object sender, EventArgs e)
        {
            var syncDb = this.FareDatabase as ISyncableDatabase;
            if (syncDb != null)
            {
                // Get the data package and import one by one
                int count = syncDb.ReceivePackages(AppContext.ProgressCallback);
                if (count < 0)
                {
                    count = 0;
                }

                MessageBox.Show(
                    this, 
                    count > 0 ? count + " packages were successfully saved to database" : "There is no new package!", 
                    "Packages Retrieval", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Information);
                if (count > -1)
                {
                    // Reload data if there is new imported packages
                    this.FlightFilter_Changed(null, null);
                }
            }
        }

        /// <summary>
        /// The num minduration_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void numMinduration_ValueChanged(object sender, EventArgs e)
        {
            // Make sure Max Duration is more than Min Duration
            if (this.numMaxDuration.Value < this.numMinDuration.Value)
            {
                this.numMaxDuration.Value = this.numMinDuration.Value;
            }
            else
            {
                this.FlightFilter_Changed(this.numMinDuration, EventArgs.Empty);
            }

            this.numMaxDuration.Minimum = this.numMinDuration.Value;
        }

        /// <summary>
        /// The flight statistic form_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void FlightStatisticForm_Shown(object sender, EventArgs e)
        {
            // Apply the execution parameter
            if (this._executionParam != null && this._executionParam.IsMinimized)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
                this.ShowInTaskbar = false;
            }

            this._formLoaded = true;

            if (this._executionParam != null && this._executionParam.OperationMode != OperationMode.Unspecified)
            {
                // No need to reload data if we need to do something else
                return;
            }

            // Show overlay animator on first run
            if (this._isMainForm && AppContext.FirstStart)
            {
                using (
                    var animator =
                        new OverlayAnimation(
                            new Dictionary<Control, string>
                                {
                                    {
                                        this.btnCheckFareFlex, 
                                        "Welcome to " + AppUtil.ProductName + "!" + Environment.NewLine
                                        + "This main screen shows all stored fare data. And the ticket hunt can be started from here!"
                                    }, 
                                    {
                                        this.btnScheduler, 
                                        "You can also schedule automatic fare scan and forget about it"
                                        + Environment.NewLine
                                        + "No more looking for cheap air fares, they will come to you!"
                                    }
                                }))
                {
                    animator.ShowDialog();
                }
            }

            // If there is no preloaded data: Load data from database
            if (this._routeData == null || this._routeData.Count < 1)
            {
                ThreadPool.QueueUserWorkItem(
                    o =>
                        {
                            AppUtil.NameCurrentThread(this.GetType().Name + "-Shown");
                            Thread.Sleep(500);
                            this.ReloadRoutes();
                        });
            }
            else
            {
                this.FillDateComboBox();
            }
        }

        /// <summary>
        /// The journey_ changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void journey_Changed(object sender, EventArgs e)
        {
            this.ReloadRoutes();
        }

        /// <summary>
        /// The mnu quick start_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuQuickStart_Click(object sender, EventArgs e)
        {
            using (var intro = new IntroForm()) intro.ShowDialog(this);
        }

        /// <summary>
        /// The mnu about_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuAbout_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm()) about.ShowDialog(this);
        }

        /// <summary>
        /// The btn help_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnHelp_Click(object sender, EventArgs e)
        {
            this.btnHelp.ShowContextMenuStrip();
        }

        /// <summary>
        /// The mnu check for update_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void mnuCheckForUpdate_Click(object sender, EventArgs e)
        {
            var svc = AppContext.MonitorEnvironment.BackgroundServices.Get(typeof(LiveUpdateService)) as LiveUpdateService;
            if (svc == null)
            {
                MessageBox.Show("Live Update is not supported");
            }
            else
            {
                svc.CheckForUpdates(true, false);
            }
        }

        /// <summary>
        /// The selected location changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SelectedLocationChanged(object sender, EventArgs e)
        {
            // If this event was triggered due to changing of another combobox: Return
            if (this._changingLocation)
            {
                return;
            }

            this._changingLocation = true;
            try
            {
                if (this._routeData == null)
                {
                    return;
                }

                Airport selDeparture = null;
                Airport selDestination = null;
                if (sender == this.cbDeparture)
                {
                    // If departure location was changed
                    if (this.cbDeparture.Items.Count < 1)
                    {
                        return;
                    }

                    if (this._loadingRoutes)
                    {
                        if (this._executionParam.Departure != null)
                        {
                            this.cbDeparture.SelectedAirportCode = this._executionParam.Departure.IATA;
                        }

                        if (this.cbDeparture.SelectedItem == null && this.cbDeparture.Items.Count > 0)
                        {
                            // If the configured departure does not exist in database
                            this.cbDeparture.SelectedItem = this.cbDeparture.Items[0];
                        }
                    }

                    selDeparture = this.cbDeparture.SelectedAirport;
                    List<Airport> destList = null;
                    if (this._routeData != null)
                    {
                        destList = new List<Airport>();
                        foreach (var r in this._routeData)
                        {
                            if (string.Equals(r.Departure.IATA, selDeparture.IATA, StringComparison.OrdinalIgnoreCase)
                                && !destList.Contains(r.Destination))
                            {
                                destList.Add(r.Destination);
                            }
                        }
                    }

                    this.cbDestination.DataSource = destList;
                    if (this._loadingRoutes)
                    {
                        if (this._executionParam.Destination != null)
                        {
                            this.cbDestination.SelectedAirport = this._executionParam.Destination;
                        }

                        if (this.cbDestination.SelectedItem == null && this.cbDestination.Items.Count > 0)
                        {
                            // If the configured destination does not exist in database
                            this.cbDestination.SelectedItem = this.cbDestination.Items[0]; // Select the first item available
                        }
                    }
                }
                else
                {
                    selDeparture = this.cbDeparture.SelectedAirport;
                }

                // If the destination was specified before: Try to match it with the new data
                selDestination = this.cbDestination.SelectedAirport;
                if (this._isMainForm)
                {
                    this._activeRoute = null; // Clear the active route first
                    if (selDeparture != null && selDestination != null)
                    {
                        // Reload data only if we are running on main form
                        foreach (var r in this._routeData)
                        {
                            string deptCode = r.Departure.IATA, destCode = r.Destination.IATA;
                            if (string.Equals(deptCode, selDeparture.IATA, StringComparison.OrdinalIgnoreCase)
                                && string.Equals(destCode, selDestination.IATA, StringComparison.OrdinalIgnoreCase))
                            {
                                this._activeRoute = r;
                                this.Logger.DebugFormat("Reloading routes [{0}]-[{1}]...", deptCode, destCode);
                                this.FareDatabase.LoadData(this._activeRoute, false, false, false, AppContext.ProgressCallback);
                                break;
                            }
                        }
                    }
                }

                this.FlightFilter_Changed(null, e);
            }
            finally
            {
                this._changingLocation = false;
            }
        }
    }
}