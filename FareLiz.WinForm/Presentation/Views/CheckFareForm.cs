namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data.Monitoring;
    using SkyDean.FareLiz.WinForm.Components;
    using SkyDean.FareLiz.WinForm.Components.Controls;
    using SkyDean.FareLiz.WinForm.Components.Controls.Button;
    using SkyDean.FareLiz.WinForm.Components.Controls.Custom;
    using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Components.Utils;
    using SkyDean.FareLiz.WinForm.Presentation.Controllers;
    using SkyDean.FareLiz.WinForm.Properties;
    using SkyDean.FareLiz.WinForm.Utils;

    /// <summary>Windows Form used for checking flexible fare data</summary>
    internal partial class CheckFareForm : SmartForm
    {
        /// <summary>The browser failed color.</summary>
        private readonly Color BrowserFailedColor = Color.FromArgb(255, 240, 240);

        /// <summary>The browser starting color.</summary>
        private readonly Color BrowserStartingColor = Color.FromArgb(255, 255, 245);

        /// <summary>The browser success color.</summary>
        private readonly Color BrowserSuccessColor = Color.FromArgb(237, 255, 237);

        /// <summary>The chk auto focus tab strip.</summary>
        private readonly ToolStripControl<CheckBox> chkAutoFocusTabStrip = new ToolStripControl<CheckBox>();

        /// <summary>The chk auto sync strip.</summary>
        private readonly ToolStripControl<CheckBox> chkAutoSyncStrip = new ToolStripControl<CheckBox>();

        /// <summary>The chk exit after done strip.</summary>
        private readonly ToolStripControl<CheckBox> chkExitAfterDoneStrip = new ToolStripControl<CheckBox>();

        /// <summary>The load progress.</summary>
        private readonly ToolStripControl<Windows7ProgressBar> loadProgress = new ToolStripControl<Windows7ProgressBar> { Visible = false };

        /// <summary>The notifier.</summary>
        private readonly INotifier notifier = new TaskbarTextNotifier();

        /// <summary>The spring label strip.</summary>
        private readonly ToolStripStatusLabel springLabelStrip = new ToolStripStatusLabel { Spring = true };

        /// <summary>The _controller.</summary>
        private CheckFareController _controller;

        /// <summary>The _execution param.</summary>
        private ExecutionParam _executionParam;

        /// <summary>The _first close for live monitor.</summary>
        private bool _firstCloseForLiveMonitor = true;

        /// <summary>The _is changing button state.</summary>
        private bool _isChangingButtonState;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckFareForm"/> class.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        public CheckFareForm(ExecutionParam param)
        {
            this.InitializeComponent();
            this.Text = AppUtil.ProductName + " " + this.Text;

            this.Initialize(param);
            this.trayIcon.Icon = this.Icon;
            GUIBuilder.AttachMenuToTrayIcon(this, this.trayIcon, true);
            this.fareBrowserTabs.ImageList = new ImageList { ImageSize = new Size(6, 6) }; // Get extra empty space on tab header
        }

        /// <summary>Gets the min duration.</summary>
        internal int MinDuration
        {
            get
            {
                return this.InvokeIfNeeded(() => this.numMinDuration.Enabled ? decimal.ToInt32(this.numMinDuration.Value) : int.MinValue);
            }
        }

        /// <summary>Gets the max duration.</summary>
        internal int MaxDuration
        {
            get
            {
                return this.InvokeIfNeeded(() => this.numMaxDuration.Enabled ? decimal.ToInt32(this.numMaxDuration.Value) : int.MaxValue);
            }
        }

        /// <summary>Gets a value indicating whether is round trip.</summary>
        internal bool IsRoundTrip
        {
            get
            {
                return this.InvokeIfNeeded(() => this.chkReturnDate.Checked);
            }
        }

        /// <summary>Gets the departure date range.</summary>
        internal DateRangeDiff DepartureDateRange
        {
            get
            {
                return this.InvokeIfNeeded(() => new DateRangeDiff((int)this.numDepartDateRangePlus.Value, (int)this.numDepartDateRangeMinus.Value));
            }
        }

        /// <summary>Gets the departure date.</summary>
        internal DateTime DepartureDate
        {
            get
            {
                return this.InvokeIfNeeded(() => this.departureDatePicker.Value);
            }
        }

        /// <summary>Gets the return date.</summary>
        internal DateTime ReturnDate
        {
            get
            {
                return this.InvokeIfNeeded(() => this.returnDatePicker.Value);
            }
        }

        /// <summary>Gets the departure.</summary>
        internal Airport Departure
        {
            get
            {
                return this.InvokeIfNeeded(() => this.txtDeparture.SelectedAirport);
            }
        }

        /// <summary>Gets the destination.</summary>
        internal Airport Destination
        {
            get
            {
                return this.InvokeIfNeeded(() => this.txtDestination.SelectedAirport);
            }
        }

        /// <summary>Gets a value indicating whether auto sync.</summary>
        internal bool AutoSync
        {
            get
            {
                return this.InvokeIfNeeded(() => this.chkAutoSyncStrip.ControlItem.Checked);
            }
        }

        /// <summary>Gets the notifier.</summary>
        internal INotifier Notifier
        {
            get
            {
                return this.notifier;
            }
        }

        /// <summary>
        /// The attach.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        internal void Attach(CheckFareController controller)
        {
            this._controller = controller;
            this._controller.View = this;

            this._controller.Events.MonitorStarting += this.OnMonitorStarting;

            this._controller.Events[OperationMode.ShowFare].RequestStarting += this.FareMonitor_RequestStarting;
            this._controller.Events[OperationMode.ShowFare].RequestStopping += this.FareMonitor_RequestCompleted;
            this._controller.Events[OperationMode.ShowFare].RequestCompleted += this.ShowFare_RequestCompleted;

            this._controller.Events[OperationMode.GetFareAndSave].RequestStarting += this.FareMonitor_RequestStarting;
            this._controller.Events[OperationMode.GetFareAndSave].RequestStopping += this.FareMonitor_RequestCompleted;
            this._controller.Events[OperationMode.GetFareAndSave].RequestCompleted += this.CloseAndExport_RequestCompleted;

            this._controller.Events[OperationMode.LiveMonitor].RequestStarting += this.LiveFare_RequestStarting;
            this._controller.Events[OperationMode.LiveMonitor].MonitorStopping += this.LiveFareMonitorStopping;
        }

        /// <summary>
        /// The on monitor starting.
        /// </summary>
        /// <param name="monitor">
        /// The monitor.
        /// </param>
        private void OnMonitorStarting(FareRequestMonitor monitor)
        {
            this.SetScanner(false);
            this.loadProgress.ControlItem.Value = this.loadProgress.ControlItem.Maximum = 0;
            this.loadProgress.ControlItem.ShowInTaskbar = false;
            if (monitor.Mode == OperationMode.ShowFare)
            {
                this.SetDataProcessor(false);
            }

            if (monitor.Mode == OperationMode.LiveMonitor)
            {
                this.trayIcon.Visible = true;
            }
            else
            {
                this.ClearBrowserTabs();
                this.loadProgress.ControlItem.Maximum = monitor.PendingRequests.Count;
                this.loadProgress.ControlItem.Style = monitor.PendingRequests.Count == 1 ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
                this.loadProgress.Visible = this.loadProgress.ControlItem.ShowInTaskbar = true;
            }

            this.SetScanner(true);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="param">
        /// The param.
        /// </param>
        private void Initialize(ExecutionParam param)
        {
            // Update the view based on Execution parameters
            this._executionParam = param.ReflectionDeepClone(AppContext.Logger);

            this.txtDeparture.SelectedAirport = param.Departure;
            this.txtDestination.SelectedAirport = param.Destination;
            if (param.MinStayDuration == int.MinValue || param.MinStayDuration == int.MaxValue)
            {
                param.MinStayDuration = Settings.Default.DefaultDurationMin;
            }

            if (param.MaxStayDuration == int.MinValue || param.MaxStayDuration == int.MaxValue)
            {
                param.MaxStayDuration = Settings.Default.DefaultDurationMax;
            }

            if (param.DepartureDate.IsUndefined() || param.DepartureDate < DateTime.Now)
            {
                param.DepartureDate = DateTime.Now.AddDays(1);
            }

            if (param.ReturnDate.IsUndefined() || param.ReturnDate < param.DepartureDate)
            {
                param.ReturnDate = param.DepartureDate.AddDays(param.MaxStayDuration);
            }

            this.departureDatePicker.MinDate = this.returnDatePicker.MinDate = DateTime.Now.Date;
            this.departureDatePicker.Value = param.DepartureDate;
            this.returnDatePicker.Value = param.ReturnDate;
            this.numDepartDateRangePlus.Value = param.DepartureDateRange.Plus;
            this.numDepartDateRangeMinus.Value = param.DepartureDateRange.Minus;
            this.numReturnDateRangePlus.Value = param.ReturnDateRange.Plus;
            this.numReturnDateRangeMinus.Value = param.ReturnDateRange.Minus;
            this.numMinDuration.Value = param.MinStayDuration;
            this.numMaxDuration.Value = param.MaxStayDuration;
            this.numPriceLimit.Value = param.PriceLimit > 0 ? param.PriceLimit : 1;

            this.loadProgress.ControlItem.ContainerControl = this;
            this.loadProgress.ControlItem.ShowInTaskbar = false;
            this.statusStrip.Items.Add(this.loadProgress);

            this.statusStrip.Items.Add(this.springLabelStrip);

            this.chkAutoFocusTabStrip.Control.Text = "Auto-focus completed tab";
            this.chkAutoFocusTabStrip.Alignment = ToolStripItemAlignment.Right;
            this.statusStrip.Items.Add(this.chkAutoFocusTabStrip);

            this.chkAutoSyncStrip.Control.Text = "Auto-sync exported data";
            this.chkAutoSyncStrip.Alignment = ToolStripItemAlignment.Right;
            this.chkAutoSyncStrip.ControlItem.DataBindings.Clear();
            this.chkAutoSyncStrip.ControlItem.DataBindings.Add("Checked", this._executionParam, "AutoSync");
            this.statusStrip.Items.Add(this.chkAutoSyncStrip);

            this.chkExitAfterDoneStrip.ControlItem.Text = "Exit after done";
            this.chkExitAfterDoneStrip.ControlItem.DataBindings.Clear();
            this.chkExitAfterDoneStrip.ControlItem.DataBindings.Add("Checked", this._executionParam, "ExitAfterDone");
            this.chkExitAfterDoneStrip.Alignment = ToolStripItemAlignment.Right;
            this.statusStrip.Items.Add(this.chkExitAfterDoneStrip);
            foreach (ToolStripItem item in this.statusStrip.Items)
            {
                item.Margin = new Padding(3, 1, 3, 1);
            }

            this.UpdateViewForDuration();
            this.ResizeStatusStrip();
        }

        /// <summary>The update view for duration.</summary>
        private void UpdateViewForDuration()
        {
            bool skipDurationConstraint = this.numDepartDateRangePlus.Value == 0 && this.numDepartDateRangeMinus.Value == 0
                                          && this.numReturnDateRangePlus.Value == 0 && this.numReturnDateRangeMinus.Value == 0;
            this.numMinDuration.Enabled = this.numMaxDuration.Enabled = !skipDurationConstraint;
        }

        /// <summary>
        /// The set scanner.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        internal void SetScanner(bool enabled)
        {
            this.btnShowFare.Enabled = this.btnLiveMonitor.Enabled = this.btnGetFareAndSave.Enabled = enabled;
        }

        /// <summary>
        /// The set data processor.
        /// </summary>
        /// <param name="enabled">
        /// The enabled.
        /// </param>
        internal void SetDataProcessor(bool enabled)
        {
            this.btnSummary.Enabled = this.btnSave.Enabled = this.btnUploadPackages.Enabled = enabled;
        }

        /// <summary>The clear browser tabs.</summary>
        internal void ClearBrowserTabs()
        {
            this.fareBrowserTabs.SuspendLayout();
            foreach (TabPage t in this.fareBrowserTabs.TabPages)
            {
                this.fareBrowserTabs.TabPages.Remove(t);
                t.Dispose();
            }

            this.fareBrowserTabs.ResumeLayout();
        }

        /// <summary>The extract tab data.</summary>
        /// <returns>The <see cref="IList" />.</returns>
        private IList<TravelRoute> ExtractTabData()
        {
            AppContext.Logger.Debug("Generating journey from all tabs...");
            var result = new List<TravelRoute>();

            foreach (TabPage tab in this.fareBrowserTabs.TabPages)
            {
                try
                {
                    var request = tab.Tag as FareMonitorRequest;
                    if (request != null && request.BrowserControl != null)
                    {
                        TravelRoute route = request.BrowserControl.LastRetrievedRoute;
                        if (route != null)
                        {
                            TravelRoute existRoute = null;
                            foreach (var r in result)
                            {
                                if (r.IsSameRoute(route))
                                {
                                    existRoute = r;
                                    break;
                                }
                            }

                            if (existRoute == null)
                            {
                                existRoute = new TravelRoute(route);
                                result.Add(route);
                            }

                            existRoute.AddJourney(route.Journeys, false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppContext.Logger.ErrorFormat("Failed to get data from tab: " + ex);
                }
            }

            return result;
        }

        /// <summary>
        /// The set status.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="image">
        /// The image.
        /// </param>
        private void SetStatus(string text, Image image)
        {
            if (this.InvokeRequired)
            {
                this.SafeInvoke(new MethodInvoker(() => this.SetStatus(text, image)));
            }
            else
            {
                this.lblStatus.Text = text;
                this.lblStatus.Image = image;
                this.ResizeStatusStrip();
            }
        }

        /// <summary>
        /// The set status.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        internal void SetStatus(FareMonitorRequest request)
        {
            string action = request.BrowserControl.RequestState == DataRequestState.Pending ? "Getting" : "Waiting";
            bool oneWayTrip = request.ReturnDate.IsUndefined();
            string trip = string.Format(
                "{0} trip {1}{2}", 
                oneWayTrip ? "one-way" : "round", 
                request.DepartureDate.ToShortDayAndDateString(), 
                oneWayTrip ? string.Empty : " - " + request.ReturnDate.ToShortDayAndDateString());
            var img = request.BrowserControl.RequestState == DataRequestState.Pending ? Resources.Loading : null;
            string message = string.Format(
                "{0} fare data for {1} ({2}/{3})...", 
                action, 
                trip, 
                this.loadProgress.ControlItem.Value, 
                this.loadProgress.ControlItem.Maximum);
            AppContext.Logger.Debug(message);
            this.SetStatus(message, img);
        }

        /// <summary>The resize status strip.</summary>
        private void ResizeStatusStrip()
        {
            int minusWidth = (this.statusStrip.SizingGrip ? this.statusStrip.SizeGripBounds.Width : 0) + 5 + 2 * SystemInformation.BorderSize.Width
                             + this.loadProgress.Margin.Left + this.loadProgress.Margin.Right;
            foreach (ToolStripItem item in this.statusStrip.Items)
            {
                if (item != this.loadProgress && item != this.springLabelStrip && item.Visible)
                {
                    minusWidth += item.Width + item.Margin.Left + item.Margin.Right;
                }
            }

            int newWidth = this.statusStrip.Width - minusWidth;
            if (newWidth > 0)
            {
                this.loadProgress.Size = new Size(newWidth, this.statusStrip.Height / 2);
            }
        }

        /// <summary>The check progress.</summary>
        private void CheckProgress()
        {
            if (this.loadProgress.ControlItem.Value == this.loadProgress.ControlItem.Maximum)
            {
                if (this._executionParam.ExitAfterDone)
                {
                    Environment.Exit(0);
                }

                this.loadProgress.ControlItem.Value = this.loadProgress.ControlItem.Maximum = 0;
                this.loadProgress.Visible = false;
                this.loadProgress.ControlItem.ShowInTaskbar = false;
                this.SetStatus("Idle", null);
            }
        }

        /// <summary>The increase progress.</summary>
        private void IncreaseProgress()
        {
            if (this.loadProgress.ControlItem.Value < this.loadProgress.ControlItem.Maximum)
            {
                this.loadProgress.ControlItem.Value++;
            }
        }

        /// <summary>
        /// The check fare form_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckFareForm_Load(object sender, EventArgs e)
        {
            // Click the appropriate button after everything is fully loaded
            if (this._executionParam.OperationMode != OperationMode.Unspecified)
            {
                var opMode = this._executionParam.OperationMode;
                if (opMode == OperationMode.ShowFare)
                {
                    this.btnShowFare.PerformClick();
                }
                else if (opMode == OperationMode.GetFareAndSave)
                {
                    this.btnGetFareAndSave.PerformClick();
                }
                else if (opMode == OperationMode.LiveMonitor)
                {
                    this.btnLiveMonitor.PerformClick();
                }

                this.WindowState = this._executionParam.IsMinimized ? FormWindowState.Minimized : FormWindowState.Normal;
                this.ShowInTaskbar = this.WindowState != FormWindowState.Minimized;
            }
        }

        /// <summary>
        /// The btn save_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            this.btnSave.Enabled = false;
            try
            {
                var db = AppContext.MonitorEnvironment.FareDatabase;
                AppContext.Logger.Info("Export current fare data on all tabs");
                var tabRoutes = this.ExtractTabData();
                db.AddData(tabRoutes, AppContext.ProgressCallback);
            }
            finally
            {
                this.btnSave.Enabled = true;
            }
        }

        /// <summary>
        /// The btn upload packages_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnUploadPackages_Click(object sender, EventArgs e)
        {
            var syncDb = AppContext.MonitorEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb == null)
            {
                MessageBox.Show(
                    this, 
                    "You have not selected a database type which supports data synchronization", 
                    "Unsupported Operation", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Asterisk);
            }
            else
            {
                var journeys = this.ExtractTabData();
                if (journeys.Count == 0)
                {
                    MessageBox.Show(
                        this, 
                        "There is no journey available for synchronizing! Get fare data first!", 
                        "Unsupported Operation", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Asterisk);
                }
                else
                {
                    string pkgId = null;
                    ProgressDialog.ExecuteTask(
                        null, 
                        "Data Package Sending", 
                        "Please wait while the new data packages are being sent...", 
                        this.GetType().Name + "UploadPackages", 
                        ProgressBarStyle.Marquee, 
                        AppContext.Logger, 
                        callback =>
                            {
                                var progress = callback as ProgressDialog;
                                progress.Begin();
                                pkgId = syncDb.SendData(journeys, AppContext.ProgressCallback);

                                if (pkgId != null)
                                {
                                    AppContext.ProgressCallback.Inform(
                                        this, 
                                        "A new package with ID " + pkgId + " has been successfully sent using the configured synchronizer", 
                                        "Package Sending", 
                                        NotificationType.Exclamation);
                                }
                            });
                }
            }
        }

        /// <summary>
        /// The start monitor button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// </exception>
        private void StartMonitorButton_Click(object sender, EventArgs e)
        {
            var btn = sender as SwitchButton;
            if (btn != null)
            {
                if (btn.IsSecondState)
                {
                    // The monitor is actually running
                    var existMon = ((Control)sender).Tag as FareRequestMonitor;
                    if (existMon != null)
                    {
                        this._controller.Stop(existMon);
                    }

                    if (sender == this.btnLiveMonitor)
                    {
                        if (this.trayIcon.Icon != null)
                        {
                            this.trayIcon.Visible = false;
                        }

                        if (!this.Visible)
                        {
                            this.Show();
                        }
                    }
                }
                else
                {
                    btn.Enabled = false;
                    try
                    {
                        OperationMode mode;
                        if (sender == this.btnShowFare)
                        {
                            mode = OperationMode.ShowFare;
                        }
                        else if (sender == this.btnGetFareAndSave)
                        {
                            mode = OperationMode.GetFareAndSave;
                        }
                        else if (sender == this.btnLiveMonitor)
                        {
                            mode = OperationMode.LiveMonitor;
                        }
                        else
                        {
                            throw new NotImplementedException("Unsupported fare monitor mode!");
                        }

                        var newMon = this._controller.Monitor(mode);
                        if (newMon != null)
                        {
                            if (mode == OperationMode.LiveMonitor)
                            {
                                this.notifier.Show(
                                    "Live Fare Monitor", 
                                    "Live Fare Monitor will run in background" + Environment.NewLine
                                    + "You will receive a notification whenever the flight fare has been changed", 
                                    7000, 
                                    NotificationType.Success, 
                                    false);
                            }

                            btn.IsSecondState = true;
                        }

                        btn.Tag = newMon;
                    }
                    finally
                    {
                        btn.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// The btn summary_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnSummary_Click(object sender, EventArgs e)
        {
            string err = null;
            IList<TravelRoute> tabData = null;
            var tabPagesCount = this.fareBrowserTabs.TabPages.Count;

            if (tabPagesCount < 1)
            {
                err = "There is no loaded flight data. Select to show the fare first before viewing the summary!";
            }
            else
            {
                tabData = this.ExtractTabData();

                // Remove empty data set
                for (int i = 0; i < tabData.Count; i++)
                {
                    var route = tabData[i];
                    if (route.Journeys.Count < 1)
                    {
                        tabData.RemoveAt(i--);
                    }
                }

                // Set error message if there is not sufficient data for summary view
                if (tabData != null)
                {
                    if (tabData.Count < 1)
                    {
                        err = "There is no available data. Please wait until the fare browser has finished loading data!";
                    }
                    else if (tabData.Count == 1)
                    {
                        if (tabPagesCount == 1)
                        {
                            err =
                                "There is only one loaded journey. The summary view is only helpful when you have loaded multiple journeys. Try changing the travel date offsets and try again!";
                        }
                        else if (tabData[0].Journeys.Count == 1)
                        {
                            err =
                                "Only one journey has completed loading. The summary view is only helpful when you have multiple loaded journeys. Please wait until the application has finished loading another journey and try again!";
                        }
                    }
                }
            }

            if (err != null)
            {
                MessageBox.Show(this, err, "There is not enough data", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            var newParam = this._executionParam.ReflectionDeepClone(AppContext.Logger);
            int minStay = 0, maxStay = 0;
            double maxPrice = 0;
            foreach (var r in tabData)
            {
                foreach (var j in r.Journeys)
                {
                    if (j.StayDuration < minStay)
                    {
                        minStay = j.StayDuration;
                    }
                    else if (j.StayDuration > maxStay)
                    {
                        maxStay = j.StayDuration;
                    }

                    var jPrice = j.Data.Max(d => d.Flights.Max(f => f.Fares.Max(p => p.Price)));
                    if (jPrice > maxPrice)
                    {
                        maxPrice = jPrice;
                    }
                }
            }

            var priceLim = (int)Math.Ceiling(maxPrice);
            newParam.MinStayDuration = minStay;
            newParam.MaxStayDuration = maxStay;
            newParam.PriceLimit = priceLim;
            var summaryForm = new FlightStatisticForm(tabData, newParam, false);
            summaryForm.Show(this);
        }

        /// <summary>
        /// The date picker_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void datePicker_ValueChanged(object sender, CheckDateEventArgs e)
        {
            if (sender == this.departureDatePicker)
            {
                this.returnDatePicker.MinDate = this.departureDatePicker.Value;
            }

            if (this.departureDatePicker.Value > this.returnDatePicker.Value)
            {
                this.returnDatePicker.Value = this.departureDatePicker.Value.AddDays(7);
            }
        }

        /// <summary>
        /// The check fare form_ resize.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckFareForm_Resize(object sender, EventArgs e)
        {
            this.ResizeStatusStrip();
        }

        /// <summary>
        /// The lbl status_ text changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lblStatus_TextChanged(object sender, EventArgs e)
        {
            this.ResizeStatusStrip();
        }

        /// <summary>
        /// The notify icon_ mouse double click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// The btn live fare data_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnLiveFareData_Click(object sender, EventArgs e)
        {
            this._controller.ShowLiveFare();
        }

        /// <summary>
        /// The check fare form_ form closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckFareForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.btnLiveMonitor.IsSecondState)
            {
                // If LiveMonitor is Running
                e.Cancel = true;
                if (this._firstCloseForLiveMonitor)
                {
                    this.trayIcon.ShowBalloonTip(500, "Run in tray", "Fare Monitor will continue monitoring in System tray", ToolTipIcon.Info);
                    this._firstCloseForLiveMonitor = false;
                }

                this.Hide();
            }
            else
            {
                if (this.loadProgress.Visible)
                {
                    // Something is on-going
                    var closeDialog = MessageBox.Show(
                        this, 
                        "Fare scanning is in progress. Are you sure you want to close this window and abort the active operations?", 
                        "Operations in progress", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);
                    if (closeDialog == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                this._controller.ClearMonitors();
                this.Dispose();
            }
        }

        /// <summary>
        /// The close and export_ request completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void CloseAndExport_RequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.SafeInvoke(
                new Action(
                    () =>
                        {
                            this.FareMonitor_RequestCompleted(sender, args);
                            var browserControl = args.Request.BrowserControl as WebFareBrowserControl;
                            var tabPageObj = browserControl.Parent as TabPage;
                            if (tabPageObj != null)
                            {
                                this.fareBrowserTabs.TabPages.Remove(tabPageObj);
                                tabPageObj.Dispose();
                            }

                            if (sender.State == MonitorState.Stopped && this.chkExitAfterDoneStrip.ControlItem.Checked)
                            {
                                Environment.Exit(0);
                            }
                        }));
        }

        /// <summary>
        /// The show fare_ request completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void ShowFare_RequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.InvokeActionIfNeeded(
                new Action(
                    () =>
                        {
                            var fareBrowserObj = args.Request.BrowserControl as WebFareBrowserControl;
                            if (fareBrowserObj.IsDestructed() || fareBrowserObj.LastRequestInitiatedDate != args.RequestInitiatedDate
                                || fareBrowserObj.RequestState == DataRequestState.Pending)
                            {
                                return;
                            }

                            var browserTabPage = fareBrowserObj.Parent as TabPage;
                            if (browserTabPage == null)
                            {
                                return;
                            }

                            if (fareBrowserObj.RequestState == DataRequestState.Pending || fareBrowserObj.RequestState == DataRequestState.Requested)
                            {
                                browserTabPage.BackColor = this.BrowserStartingColor;
                            }
                            else if (fareBrowserObj.RequestState == DataRequestState.Ok)
                            {
                                browserTabPage.BackColor = this.BrowserSuccessColor;
                            }
                            else
                            {
                                browserTabPage.BackColor = this.BrowserFailedColor;
                            }

                            if (this.chkAutoFocusTabStrip.ControlItem.Checked)
                            {
                                this.fareBrowserTabs.SelectedTab = browserTabPage;
                            }

                            this.FareMonitor_RequestCompleted(sender, args);
                        }));
        }

        /// <summary>
        /// The fare monitor_ request completed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void FareMonitor_RequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            // This method should be triggered both before stopping the request or after the request is completed
            var browser = args.Request.BrowserControl; // The browser might be nulled
            this.InvokeActionIfNeeded(
                new Action(
                    () =>
                        {
                            SwitchButton senderBtn = null;
                            var monType = sender.GetType();
                            var resultRoute = browser == null ? null : browser.LastRetrievedRoute;

                            if (monType == typeof(FareRequestMonitor))
                            {
                                // Browser can be nulled if the request was aborted before it is event started
                                if (browser != null && resultRoute != null && !this.btnSummary.Enabled)
                                {
                                    this.SetDataProcessor(true);
                                }

                                senderBtn = this.btnShowFare;
                            }
                            else if (monType == typeof(FareExportMonitor))
                            {
                                senderBtn = this.btnGetFareAndSave;
                            }

                            this.IncreaseProgress();
                            this.CheckProgress();

                            if (!this.loadProgress.Visible && senderBtn != null)
                            {
                                var senderMonitor = senderBtn.Tag as FareRequestMonitor;
                                if (senderMonitor == sender && senderBtn.IsSecondState)
                                {
                                    senderBtn.IsSecondState = false; // Reset the button after everything is done!
                                }
                            }
                        }));
        }

        /// <summary>
        /// The live fare monitor stopping.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void LiveFareMonitorStopping(FareRequestMonitor sender)
        {
            this.btnLiveMonitor.IsSecondState = false;
        }

        /// <summary>
        /// The live fare_ request starting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void LiveFare_RequestStarting(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            args.Request.Initialize();
            var monitor = sender as LiveFareMonitor;
            monitor.PriceLimit = (double)this.numPriceLimit.Value;
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
            Environment.Exit(0);
        }

        /// <summary>
        /// The process cmd key.
        /// </summary>
        /// <param name="msg">
        /// The msg.
        /// </param>
        /// <param name="keyData">
        /// The key data.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Close window if user press Esc and Live Monitor is running
            if (this.btnLiveMonitor.IsSecondState && keyData == Keys.Escape)
            {
                this.Close();
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        /// <summary>
        /// The btn no depart range_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnNoDepartRange_Click(object sender, EventArgs e)
        {
            this.numDepartDateRangeMinus.Value = this.numDepartDateRangePlus.Value = 0;
        }

        /// <summary>
        /// The btn no return range_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnNoReturnRange_Click(object sender, EventArgs e)
        {
            this.numReturnDateRangeMinus.Value = this.numReturnDateRangePlus.Value = 0;
        }

        /// <summary>
        /// The fare browser tabs_ tab page refreshing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void fareBrowserTabs_TabPageRefreshing(object sender, TabControlEventArgs e)
        {
            var browserTabPage = e.TabPage;
            var request = browserTabPage.Tag as FareMonitorRequest;
            if (request != null)
            {
                request.Start();
                browserTabPage.BackColor = this.BrowserStartingColor;
                browserTabPage.ImageIndex = 0;
            }
        }

        /// <summary>
        /// The fare browser tabs_ tab page closing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void fareBrowserTabs_TabPageClosing(object sender, TabControlCancelEventArgs e)
        {
            var tagPage = e.TabPage;
            var request = tagPage.Tag as FareMonitorRequest;
            if (request != null)
            {
                request.Stop();
                request.OwnerMonitor.FinalizeRequest(request);
                if (!(request.BrowserControl.RequestState > DataRequestState.Requested) && this.loadProgress.ControlItem.Maximum > 0)
                {
                    // If the request was not completed: Increase the progress
                    this.IncreaseProgress();
                    this.CheckProgress();
                }

                if (this.fareBrowserTabs.TabPages.Count == 1)
                {
                    // This is the last tab: Disable unneeded buttons
                    this.SetDataProcessor(false);
                }
            }
        }

        /// <summary>
        /// The chk return date_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void chkReturnDate_CheckedChanged(object sender, EventArgs e)
        {
            this.lblDuration.Enabled =
                this.numMinDuration.Enabled =
                this.numMaxDuration.Enabled =
                this.returnDatePicker.Enabled =
                this.numReturnDateRangeMinus.Enabled =
                this.numReturnDateRangePlus.Enabled = this.btnNoReturnRange.Enabled = this.chkReturnDate.Checked;
        }

        /// <summary>
        /// The get fare buttons_ state changing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void GetFareButtons_StateChanging(object sender, CancelEventArgs e)
        {
            if (this._isChangingButtonState)
            {
                return;
            }

            this._isChangingButtonState = true;
            var btn = sender as SwitchButton;
            if (btn != null)
            {
                if (btn == this.btnShowFare)
                {
                    this.btnGetFareAndSave.Enabled = btn.IsSecondState;
                }
                else if (btn == this.btnGetFareAndSave)
                {
                    this.btnShowFare.Enabled = btn.IsSecondState;
                }
            }

            this._isChangingButtonState = false;
        }

        /// <summary>
        /// The date range_ value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DateRange_ValueChanged(object sender, EventArgs e)
        {
            this.UpdateViewForDuration();
        }

        /// <summary>
        /// The fare monitor_ request starting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private void FareMonitor_RequestStarting(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.InvokeActionIfNeeded(
                new Action(
                    () =>
                        {
                            FareMonitorRequest request = args.Request;
                            request.Initialize(); // Initialize request here: So that the control is created on the UI thread
                            var browserTabPage =
                                new TabPage(
                                    request.DepartureDate.ToString(NamingRule.DATE_FORMAT)
                                    + (request.IsRoundTrip ? " - " + request.ReturnDate.ToString(NamingRule.DATE_FORMAT) : string.Empty));

                            var browser = request.BrowserControl as Control;
                            if (browser != null)
                            {
                                browser.Dock = DockStyle.Fill;
                                browserTabPage.Controls.Add(browser);
                                browserTabPage.BackColor = this.BrowserStartingColor;
                                browserTabPage.ImageIndex = 0;
                                browserTabPage.Tag = request;
                                browserTabPage.ToolTipText = request.Detail;
                                this.fareBrowserTabs.TabPages.Add(browserTabPage);
                            }

                            this.SetStatus(request);
                        }));
        }

        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="icon">
        /// The icon.
        /// </param>
        internal void Show(string message, string title, MessageBoxIcon icon)
        {
            this.InvokeActionIfNeeded(new Action(() => MessageBox.Show(this, message, title, MessageBoxButtons.OK, icon)));
        }
    }
}