using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
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

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    /// <summary>
    /// Windows Form used for checking flexible fare data
    /// </summary>
    internal partial class CheckFareForm : SmartForm
    {
        private readonly INotifier notifier = new TaskbarTextNotifier();
        private readonly ToolStripControl<Windows7ProgressBar> loadProgress = new ToolStripControl<Windows7ProgressBar> { Visible = false };
        private readonly ToolStripStatusLabel springLabelStrip = new ToolStripStatusLabel { Spring = true };
        private readonly ToolStripControl<CheckBox> chkAutoFocusTabStrip = new ToolStripControl<CheckBox>();
        private readonly ToolStripControl<CheckBox> chkAutoSyncStrip = new ToolStripControl<CheckBox>();
        private readonly ToolStripControl<CheckBox> chkExitAfterDoneStrip = new ToolStripControl<CheckBox>();

        private bool _isChangingButtonState = false;

        private readonly Color BrowserStartingColor = Color.FromArgb(255, 255, 245),
                               BrowserSuccessColor = Color.FromArgb(237, 255, 237),
                               BrowserFailedColor = Color.FromArgb(255, 240, 240);

        private bool _firstCloseForLiveMonitor = true;
        private CheckFareController _controller;
        private ExecutionParam _executionParam;

        internal int MinDuration
        {
            get
            {
                return this.InvokeIfNeeded(() =>
                    numMinDuration.Enabled ? Decimal.ToInt32(numMinDuration.Value) : int.MinValue);
            }
        }

        internal int MaxDuration
        {
            get
            {
                return this.InvokeIfNeeded(() =>
                    numMaxDuration.Enabled ? Decimal.ToInt32(numMaxDuration.Value) : int.MaxValue);
            }
        }

        internal bool IsRoundTrip
        {
            get { return this.InvokeIfNeeded(() => chkReturnDate.Checked); }
        }

        internal DateRangeDiff DepartureDateRange
        {
            get
            {
                return this.InvokeIfNeeded(() =>
                    new DateRangeDiff((int)numDepartDateRangePlus.Value, (int)numDepartDateRangeMinus.Value));
            }
        }

        internal DateTime DepartureDate
        {
            get { return this.InvokeIfNeeded(() => departureDatePicker.Value); }
        }

        internal DateTime ReturnDate
        {
            get { return this.InvokeIfNeeded(() => departureDatePicker.Value); }
        }

        internal Airport Departure
        {
            get { return this.InvokeIfNeeded(() => txtDeparture.SelectedAirport); }
        }
        internal Airport Destination
        {
            get { return this.InvokeIfNeeded(() => txtDestination.SelectedAirport); }
        }
        internal bool AutoSync
        {
            get { return this.InvokeIfNeeded(() => chkAutoSyncStrip.ControlItem.Checked); }
        }

        internal INotifier Notifier { get { return notifier; } }

        public CheckFareForm(ExecutionParam param)
        {
            InitializeComponent();
            Text = AppUtil.ProductName + " " + Text;

            Initialize(param);
            trayIcon.Icon = Icon;
            GUIBuilder.AttachMenuToTrayIcon(this, trayIcon, true);
            fareBrowserTabs.ImageList = new ImageList { ImageSize = new Size(6, 6) }; // Get extra empty space on tab header
        }

        internal void Attach(CheckFareController controller)
        {
            _controller = controller;
            _controller.View = this;

            _controller.Events.MonitorStarting += OnMonitorStarting;

            _controller.Events[OperationMode.ShowFare].RequestStarting += FareMonitor_RequestStarting;
            _controller.Events[OperationMode.ShowFare].RequestStopping += FareMonitor_RequestCompleted;
            _controller.Events[OperationMode.ShowFare].RequestCompleted += ShowFare_RequestCompleted;

            _controller.Events[OperationMode.GetFareAndSave].RequestStarting += FareMonitor_RequestStarting;
            _controller.Events[OperationMode.GetFareAndSave].RequestStopping += FareMonitor_RequestCompleted;
            _controller.Events[OperationMode.GetFareAndSave].RequestCompleted += CloseAndExport_RequestCompleted;

            _controller.Events[OperationMode.LiveMonitor].RequestStarting += LiveFare_RequestStarting;
            _controller.Events[OperationMode.LiveMonitor].MonitorStopping += LiveFareMonitorStopping;
        }

        private void OnMonitorStarting(FareRequestMonitor monitor)
        {
            SetScanner(false);
            loadProgress.ControlItem.Value = loadProgress.ControlItem.Maximum = 0;
            loadProgress.ControlItem.ShowInTaskbar = false;
            if (monitor.Mode == OperationMode.ShowFare)
                SetDataProcessor(false);

            if (monitor.Mode == OperationMode.LiveMonitor)
            {
                trayIcon.Visible = true;
            }
            else
            {
                ClearBrowserTabs();
                loadProgress.ControlItem.Maximum = monitor.PendingRequests.Count;
                loadProgress.ControlItem.Style = (monitor.PendingRequests.Count == 1 ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous);
                loadProgress.Visible = loadProgress.ControlItem.ShowInTaskbar = true;
            }

            this.SetScanner(true);
        }

        private void Initialize(ExecutionParam param)
        {
            // Update the view based on Execution parameters
            _executionParam = param.ReflectionDeepClone(AppContext.Logger);

            txtDeparture.SelectedAirport = param.Departure;
            txtDestination.SelectedAirport = param.Destination;
            if (param.MinStayDuration == Int32.MinValue || param.MinStayDuration == Int32.MaxValue)
                param.MinStayDuration = Settings.Default.DefaultDurationMin;
            if (param.MaxStayDuration == Int32.MinValue || param.MaxStayDuration == Int32.MaxValue)
                param.MaxStayDuration = Settings.Default.DefaultDurationMax;
            if (param.DepartureDate.IsUndefined() || param.DepartureDate < DateTime.Now)
                param.DepartureDate = DateTime.Now.AddDays(1);
            if (param.ReturnDate.IsUndefined() || param.ReturnDate < param.DepartureDate)
                param.ReturnDate = param.DepartureDate.AddDays(param.MaxStayDuration);
            departureDatePicker.MinDate = returnDatePicker.MinDate = DateTime.Now.Date;
            departureDatePicker.Value = param.DepartureDate;
            returnDatePicker.Value = param.ReturnDate;
            numDepartDateRangePlus.Value = param.DepartureDateRange.Plus;
            numDepartDateRangeMinus.Value = param.DepartureDateRange.Minus;
            numReturnDateRangePlus.Value = param.ReturnDateRange.Plus;
            numReturnDateRangeMinus.Value = param.ReturnDateRange.Minus;
            numMinDuration.Value = param.MinStayDuration;
            numMaxDuration.Value = param.MaxStayDuration;
            numPriceLimit.Value = param.PriceLimit > 0 ? param.PriceLimit : 1;

            loadProgress.ControlItem.ContainerControl = this;
            loadProgress.ControlItem.ShowInTaskbar = false;
            statusStrip.Items.Add(loadProgress);

            statusStrip.Items.Add(springLabelStrip);

            chkAutoFocusTabStrip.Control.Text = "Auto-focus completed tab";
            chkAutoFocusTabStrip.Alignment = ToolStripItemAlignment.Right;
            statusStrip.Items.Add(chkAutoFocusTabStrip);

            chkAutoSyncStrip.Control.Text = "Auto-sync exported data";
            chkAutoSyncStrip.Alignment = ToolStripItemAlignment.Right;
            chkAutoSyncStrip.ControlItem.DataBindings.Clear();
            chkAutoSyncStrip.ControlItem.DataBindings.Add("Checked", _executionParam, "AutoSync");
            statusStrip.Items.Add(chkAutoSyncStrip);

            chkExitAfterDoneStrip.ControlItem.Text = "Exit after done";
            chkExitAfterDoneStrip.ControlItem.DataBindings.Clear();
            chkExitAfterDoneStrip.ControlItem.DataBindings.Add("Checked", _executionParam, "ExitAfterDone");
            chkExitAfterDoneStrip.Alignment = ToolStripItemAlignment.Right;
            statusStrip.Items.Add(chkExitAfterDoneStrip);
            foreach (ToolStripItem item in statusStrip.Items)
                item.Margin = new Padding(3, 1, 3, 1);

            UpdateViewForDuration();
            ResizeStatusStrip();
        }

        private void UpdateViewForDuration()
        {
            bool skipDurationConstraint = numDepartDateRangePlus.Value == 0 && numDepartDateRangeMinus.Value == 0
                                          && numReturnDateRangePlus.Value == 0 && numReturnDateRangeMinus.Value == 0;
            numMinDuration.Enabled = numMaxDuration.Enabled = !skipDurationConstraint;
        }

        internal void SetScanner(bool enabled)
        {
            btnShowFare.Enabled = btnLiveMonitor.Enabled = btnGetFareAndSave.Enabled = enabled;
        }

        internal void SetDataProcessor(bool enabled)
        {
            btnSummary.Enabled = btnSave.Enabled = btnUploadPackages.Enabled = enabled;
        }

        internal void ClearBrowserTabs()
        {
            fareBrowserTabs.SuspendLayout();
            foreach (TabPage t in fareBrowserTabs.TabPages)
            {
                fareBrowserTabs.TabPages.Remove(t);
                t.Dispose();
            }
            fareBrowserTabs.ResumeLayout();
        }

        private IList<TravelRoute> ExtractTabData()
        {
            AppContext.Logger.Debug("Generating journey from all tabs...");
            var result = new List<TravelRoute>();

            foreach (TabPage tab in fareBrowserTabs.TabPages)
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

        private void SetStatus(string text, Image image)
        {
            if (InvokeRequired)
                this.SafeInvoke(new MethodInvoker(() => SetStatus(text, image)));
            else
            {
                lblStatus.Text = text;
                lblStatus.Image = image;
                ResizeStatusStrip();
            }
        }

        internal void SetStatus(FareMonitorRequest request)
        {
            string action = (request.BrowserControl.RequestState == DataRequestState.Pending ? "Getting" : "Waiting");
            bool oneWayTrip = request.ReturnDate.IsUndefined();
            string trip = String.Format("{0} trip {1}{2}", oneWayTrip ? "one-way" : "round", request.DepartureDate.ToShortDayAndDateString(), oneWayTrip ? "" : " - " + request.ReturnDate.ToShortDayAndDateString());
            var img = (request.BrowserControl.RequestState == DataRequestState.Pending ? Properties.Resources.Loading : null);
            string message = String.Format("{0} fare data for {1} ({2}/{3})...",
                                           action,
                                           trip,
                                           loadProgress.ControlItem.Value,
                                           loadProgress.ControlItem.Maximum);
            AppContext.Logger.Debug(message);
            SetStatus(message, img);
        }

        private void ResizeStatusStrip()
        {
            int minusWidth = (statusStrip.SizingGrip ? statusStrip.SizeGripBounds.Width : 0) + 5 + 2 * SystemInformation.BorderSize.Width + loadProgress.Margin.Left + loadProgress.Margin.Right;
            foreach (ToolStripItem item in statusStrip.Items)
                if (item != loadProgress && item != springLabelStrip && item.Visible)
                    minusWidth += item.Width + item.Margin.Left + item.Margin.Right;

            int newWidth = statusStrip.Width - minusWidth;
            if (newWidth > 0)
                loadProgress.Size = new Size(newWidth, statusStrip.Height / 2);
        }

        private void CheckProgress()
        {
            if (loadProgress.ControlItem.Value == loadProgress.ControlItem.Maximum)
            {
                if (_executionParam.ExitAfterDone)
                    Environment.Exit(0);

                loadProgress.ControlItem.Value = loadProgress.ControlItem.Maximum = 0;
                loadProgress.Visible = false;
                loadProgress.ControlItem.ShowInTaskbar = false;
                SetStatus("Idle", null);
            }
        }

        private void IncreaseProgress()
        {
            if (loadProgress.ControlItem.Value < loadProgress.ControlItem.Maximum)
                loadProgress.ControlItem.Value++;
        }

        private void CheckFareForm_Load(object sender, EventArgs e)
        {
            // Click the appropriate button after everything is fully loaded
            if (_executionParam.OperationMode != OperationMode.Unspecified)
            {
                var opMode = _executionParam.OperationMode;
                if (opMode == OperationMode.ShowFare)
                    btnShowFare.PerformClick();
                else if (opMode == OperationMode.GetFareAndSave)
                    btnGetFareAndSave.PerformClick();
                else if (opMode == OperationMode.LiveMonitor)
                    btnLiveMonitor.PerformClick();

                WindowState = _executionParam.IsMinimized ? FormWindowState.Minimized : FormWindowState.Normal;
                ShowInTaskbar = (WindowState != FormWindowState.Minimized);
            }
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            try
            {
                var db = AppContext.MonitorEnvironment.FareDatabase;
                AppContext.Logger.Info("Export current fare data on all tabs");
                var tabRoutes = ExtractTabData();
                db.AddData(tabRoutes, AppContext.ProgressCallback);
            }
            finally
            {
                btnSave.Enabled = true;
            }
        }

        private void btnUploadPackages_Click(object sender, EventArgs e)
        {
            var syncDb = AppContext.MonitorEnvironment.FareDatabase as ISyncableDatabase;
            if (syncDb == null)
                MessageBox.Show(this, "You have not selected a database type which supports data synchronization", "Unsupported Operation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            else
            {
                var journeys = ExtractTabData();
                if (journeys.Count == 0)
                    MessageBox.Show(this, "There is no journey available for synchronizing! Get fare data first!", "Unsupported Operation", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                else
                {
                    string pkgId = null;
                    ProgressDialog.ExecuteTask(null, "Data Package Sending", "Please wait while the new data packages are being sent...", GetType().Name + "UploadPackages", ProgressBarStyle.Marquee, AppContext.Logger, callback =>
                        {
                            var progress = callback as ProgressDialog;
                            progress.Begin();
                            pkgId = syncDb.SendData(journeys, AppContext.ProgressCallback);

                            if (pkgId != null)
                                AppContext.ProgressCallback.Inform(this, "A new package with ID " + pkgId + " has been successfully sent using the configured synchronizer", "Package Sending", NotificationType.Exclamation);
                        });
                }
            }
        }

        private void StartMonitorButton_Click(object sender, EventArgs e)
        {
            var btn = sender as SwitchButton;
            if (btn != null)
            {
                if (btn.IsSecondState) // The monitor is actually running
                {
                    var existMon = ((Control)sender).Tag as FareRequestMonitor;
                    if (existMon != null)
                    {
                        _controller.Stop(existMon);
                    }

                    if (sender == btnLiveMonitor)
                    {
                        if (trayIcon.Icon != null)
                            trayIcon.Visible = false;

                        if (!Visible)
                            Show();
                    }
                }
                else
                {
                    btn.Enabled = false;
                    try
                    {

                        OperationMode mode;
                        if (sender == btnShowFare)
                            mode = OperationMode.ShowFare;
                        else if (sender == btnGetFareAndSave)
                            mode = OperationMode.GetFareAndSave;
                        else if (sender == btnLiveMonitor)
                            mode = OperationMode.LiveMonitor;
                        else
                            throw new NotImplementedException("Unsupported fare monitor mode!");

                        var newMon = _controller.Monitor(mode);
                        if (newMon != null)
                        {
                            if (mode == OperationMode.LiveMonitor)
                                notifier.Show("Live Fare Monitor", "Live Fare Monitor will run in background" + Environment.NewLine + "You will receive a notification whenever the flight fare has been changed",
                                              7000, NotificationType.Success, false);
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

        void btnSummary_Click(object sender, EventArgs e)
        {
            string err = null;
            IList<TravelRoute> tabData = null;
            var tabPagesCount = fareBrowserTabs.TabPages.Count;

            if (tabPagesCount < 1)
                err = "There is no loaded flight data. Select to show the fare first before viewing the summary!";
            else
            {
                tabData = ExtractTabData();
                // Remove empty data set
                for (int i = 0; i < tabData.Count; i++)
                {
                    var route = tabData[i];
                    if (route.Journeys.Count < 1)
                        tabData.RemoveAt(i--);
                }

                // Set error message if there is not sufficient data for summary view
                if (tabData != null)
                {
                    if (tabData.Count < 1)
                        err = "There is no available data. Please wait until the fare browser has finished loading data!";
                    else if (tabData.Count == 1)
                    {
                        if (tabPagesCount == 1)
                            err = "There is only one loaded journey. The summary view is only helpful when you have loaded multiple journeys. Try changing the travel date offsets and try again!";
                        else if (tabData[0].Journeys.Count == 1)
                            err = "Only one journey has completed loading. The summary view is only helpful when you have multiple loaded journeys. Please wait until the application has finished loading another journey and try again!";
                    }
                }
            }

            if (err != null)
            {
                MessageBox.Show(this, err, "There is not enough data", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }

            var newParam = _executionParam.ReflectionDeepClone(AppContext.Logger);
            int minStay = 0, maxStay = 0;
            double maxPrice = 0;
            foreach (var r in tabData)
                foreach (var j in r.Journeys)
                {
                    if (j.StayDuration < minStay)
                        minStay = j.StayDuration;
                    else if (j.StayDuration > maxStay)
                        maxStay = j.StayDuration;
                    var jPrice = j.Data.Max(d => d.Flights.Max(f => f.Price));
                    if (jPrice > maxPrice)
                        maxPrice = jPrice;
                }

            var priceLim = (int)Math.Ceiling(maxPrice);
            newParam.MinStayDuration = minStay;
            newParam.MaxStayDuration = maxStay;
            newParam.PriceLimit = priceLim;
            var summaryForm = new FlightStatisticForm(tabData, newParam, false);
            summaryForm.Show(this);
        }

        private void datePicker_ValueChanged(object sender, CheckDateEventArgs e)
        {
            if (sender == departureDatePicker)
                returnDatePicker.MinDate = departureDatePicker.Value;

            if (departureDatePicker.Value > returnDatePicker.Value)
                returnDatePicker.Value = departureDatePicker.Value.AddDays(7);
        }

        private void CheckFareForm_Resize(object sender, EventArgs e)
        {
            ResizeStatusStrip();
        }

        private void lblStatus_TextChanged(object sender, EventArgs e)
        {
            ResizeStatusStrip();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
        }

        private void btnLiveFareData_Click(object sender, EventArgs e)
        {
            _controller.ShowLiveFare();
        }

        private void CheckFareForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnLiveMonitor.IsSecondState) // If LiveMonitor is Running
            {
                e.Cancel = true;
                if (_firstCloseForLiveMonitor)
                {
                    trayIcon.ShowBalloonTip(500, "Run in tray", "Fare Monitor will continue monitoring in System tray", ToolTipIcon.Info);
                    _firstCloseForLiveMonitor = false;
                }

                Hide();
            }
            else
            {
                if (loadProgress.Visible)   // Something is on-going
                {
                    var closeDialog = MessageBox.Show(this, "Fare scanning is in progress. Are you sure you want to close this window and abort the active operations?", "Operations in progress", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (closeDialog == DialogResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                _controller.ClearMonitors();
                Dispose();
            }
        }

        void CloseAndExport_RequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.SafeInvoke(new Action(() =>
            {
                FareMonitor_RequestCompleted(sender, args);
                var browserControl = args.Request.BrowserControl as WebFareBrowserControl;
                var tabPageObj = browserControl.Parent as TabPage;
                if (tabPageObj != null)
                {
                    fareBrowserTabs.TabPages.Remove(tabPageObj);
                    tabPageObj.Dispose();
                }

                if (sender.State == MonitorState.Stopped && chkExitAfterDoneStrip.ControlItem.Checked)
                    Environment.Exit(0);
            }));
        }

        void ShowFare_RequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.InvokeActionIfNeeded(new Action(() =>
            {
                var fareBrowserObj = args.Request.BrowserControl as WebFareBrowserControl;
                if (fareBrowserObj.IsDestructed()
                    || fareBrowserObj.LastRequestInitiatedDate != args.RequestInitiatedDate ||
                    fareBrowserObj.RequestState == DataRequestState.Pending)
                    return;

                var browserTabPage = fareBrowserObj.Parent as TabPage;
                if (browserTabPage == null)
                    return;

                if (fareBrowserObj.RequestState == DataRequestState.Pending ||
                    fareBrowserObj.RequestState == DataRequestState.Requested)
                    browserTabPage.BackColor = BrowserStartingColor;
                else if (fareBrowserObj.RequestState == DataRequestState.Ok)
                    browserTabPage.BackColor = BrowserSuccessColor;
                else
                    browserTabPage.BackColor = BrowserFailedColor;

                if (chkAutoFocusTabStrip.ControlItem.Checked)
                    fareBrowserTabs.SelectedTab = browserTabPage;

                FareMonitor_RequestCompleted(sender, args);
            }));
        }

        void FareMonitor_RequestCompleted(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            // This method should be triggered both before stopping the request or after the request is completed
            var browser = args.Request.BrowserControl;  // The browser might be nulled
            this.InvokeActionIfNeeded(new Action(() =>
            {
                SwitchButton senderBtn = null;
                var monType = sender.GetType();
                var resultRoute = (browser == null ? null : browser.LastRetrievedRoute);

                if (monType == typeof(FareRequestMonitor))
                {
                    // Browser can be nulled if the request was aborted before it is event started
                    if (browser != null && resultRoute != null && !btnSummary.Enabled)
                        SetDataProcessor(true);
                    senderBtn = btnShowFare;
                }
                else if (monType == typeof(FareExportMonitor))
                    senderBtn = btnGetFareAndSave;

                IncreaseProgress();
                CheckProgress();

                if (!loadProgress.Visible && senderBtn != null)
                {
                    var senderMonitor = senderBtn.Tag as FareRequestMonitor;
                    if (senderMonitor == sender && senderBtn.IsSecondState)
                        senderBtn.IsSecondState = false;    // Reset the button after everything is done!
                }
            }));
        }

        void LiveFareMonitorStopping(FareRequestMonitor sender)
        {
            btnLiveMonitor.IsSecondState = false;
        }

        void LiveFare_RequestStarting(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            args.Request.Initialize();
            var monitor = sender as LiveFareMonitor;
            monitor.PriceLimit = (double)numPriceLimit.Value;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Close window if user press Esc and Live Monitor is running
            if (btnLiveMonitor.IsSecondState && keyData == Keys.Escape)
                Close();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnNoDepartRange_Click(object sender, EventArgs e)
        {
            numDepartDateRangeMinus.Value = numDepartDateRangePlus.Value = 0;
        }

        private void btnNoReturnRange_Click(object sender, EventArgs e)
        {
            numReturnDateRangeMinus.Value = numReturnDateRangePlus.Value = 0;
        }

        private void fareBrowserTabs_TabPageRefreshing(object sender, TabControlEventArgs e)
        {
            var browserTabPage = e.TabPage;
            var request = browserTabPage.Tag as FareMonitorRequest;
            if (request != null)
            {
                request.Start();
                browserTabPage.BackColor = BrowserStartingColor;
                browserTabPage.ImageIndex = 0;
            }
        }

        private void fareBrowserTabs_TabPageClosing(object sender, TabControlCancelEventArgs e)
        {
            var tagPage = e.TabPage;
            var request = tagPage.Tag as FareMonitorRequest;
            if (request != null)
            {
                request.Stop();
                request.OwnerMonitor.FinalizeRequest(request);
                if (!(request.BrowserControl.RequestState > DataRequestState.Requested) && loadProgress.ControlItem.Maximum > 0) // If the request was not completed: Increase the progress
                {
                    IncreaseProgress();
                    CheckProgress();
                }

                if (fareBrowserTabs.TabPages.Count == 1)    // This is the last tab: Disable unneeded buttons
                    SetDataProcessor(false);
            }
        }

        private void chkReturnDate_CheckedChanged(object sender, EventArgs e)
        {
            lblDuration.Enabled = numMinDuration.Enabled = numMaxDuration.Enabled =
                returnDatePicker.Enabled = numReturnDateRangeMinus.Enabled = numReturnDateRangePlus.Enabled = lblReturnPM.Enabled = btnNoReturnRange.Enabled
                = chkReturnDate.Checked;
        }

        private void GetFareButtons_StateChanging(object sender, CancelEventArgs e)
        {
            if (_isChangingButtonState)
                return;

            _isChangingButtonState = true;
            var btn = sender as SwitchButton;
            if (btn != null)
            {
                if (btn == btnShowFare)
                    btnGetFareAndSave.Enabled = btn.IsSecondState;
                else if (btn == btnGetFareAndSave)
                    btnShowFare.Enabled = btn.IsSecondState;
            }
            _isChangingButtonState = false;
        }

        private void DateRange_ValueChanged(object sender, EventArgs e)
        {
            UpdateViewForDuration();
        }

        void FareMonitor_RequestStarting(FareRequestMonitor sender, FareBrowserRequestArg args)
        {
            this.InvokeActionIfNeeded(new Action(() =>
            {
                FareMonitorRequest request = args.Request;
                request.Initialize(); // Initialize request here: So that the control is created on the UI thread
                var browserTabPage = new TabPage(request.DepartureDate.ToString(NamingRule.DATE_FORMAT)
                                                 +
                                                 (request.IsRoundTrip
                                                     ? " - " + request.ReturnDate.ToString(NamingRule.DATE_FORMAT)
                                                     : String.Empty));

                var browser = request.BrowserControl as Control;
                if (browser != null)
                {
                    browser.Dock = DockStyle.Fill;
                    browserTabPage.Controls.Add(browser);
                    browserTabPage.BackColor = BrowserStartingColor;
                    browserTabPage.ImageIndex = 0;
                    browserTabPage.Tag = request;
                    browserTabPage.ToolTipText = request.Detail;
                    fareBrowserTabs.TabPages.Add(browserTabPage);
                }
                SetStatus(request);
            }));
        }

        internal void Show(string message, string title, MessageBoxIcon icon)
        {
            this.InvokeActionIfNeeded(new Action(() => MessageBox.Show(this, message, title, MessageBoxButtons.OK, icon)));
        }
    }
}