using SkyDean.FareLiz.WinForm.Components.Controls.Button;
using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker;
using SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses;
using SkyDean.FareLiz.WinForm.Components.Controls.TabControl;
using SkyDean.FareLiz.WinForm.Components.Controls.TextBox;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    partial class CheckFareForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckFareForm));
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.txtDeparture = new SkyDean.FareLiz.WinForm.Components.Controls.TextBox.AirportTextBox();
            this.numMaxDuration = new System.Windows.Forms.NumericUpDown();
            this.txtDestination = new SkyDean.FareLiz.WinForm.Components.Controls.TextBox.AirportTextBox();
            this.numMinDuration = new System.Windows.Forms.NumericUpDown();
            this.btnExit = new SkyDean.FareLiz.WinForm.Components.Controls.Button.ImageButton();
            this.btnNoReturnRange = new System.Windows.Forms.Button();
            this.btnNoDepartRange = new System.Windows.Forms.Button();
            this.lblDuration = new System.Windows.Forms.Label();
            this.btnUploadPackages = new SkyDean.FareLiz.WinForm.Components.Controls.Button.ImageButton();
            this.btnGetFareAndSave = new SkyDean.FareLiz.WinForm.Components.Controls.Button.SwitchButton();
            this.numReturnDateRangeMinus = new System.Windows.Forms.NumericUpDown();
            this.numDepartDateRangeMinus = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new SkyDean.FareLiz.WinForm.Components.Controls.Button.ImageButton();
            this.numReturnDateRangePlus = new System.Windows.Forms.NumericUpDown();
            this.btnSummary = new System.Windows.Forms.Button();
            this.numDepartDateRangePlus = new System.Windows.Forms.NumericUpDown();
            this.returnDatePicker = new SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker.EnhancedDatePicker();
            this.departureDatePicker = new SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker.EnhancedDatePicker();
            this.btnShowFare = new SkyDean.FareLiz.WinForm.Components.Controls.Button.SwitchButton();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblDeparture = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblReturnPM = new System.Windows.Forms.Label();
            this.lblTravelDate = new System.Windows.Forms.Label();
            this.chkReturnDate = new System.Windows.Forms.CheckBox();
            this.numPriceLimit = new System.Windows.Forms.NumericUpDown();
            this.lblPriceLimit = new System.Windows.Forms.Label();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnLiveFareData = new SkyDean.FareLiz.WinForm.Components.Controls.Button.ImageButton();
            this.btnLiveMonitor = new SkyDean.FareLiz.WinForm.Components.Controls.Button.SwitchButton();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnLiveMonitorImageList = new System.Windows.Forms.ImageList(this.components);
            this.grpLiveMonitor = new System.Windows.Forms.GroupBox();
            this.fareBrowserTabs = new SkyDean.FareLiz.WinForm.Components.Controls.TabControl.FlatTabControl();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReturnDateRangeMinus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepartDateRangeMinus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReturnDateRangePlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepartDateRangePlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPriceLimit)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.grpLiveMonitor.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDetail
            // 
            this.grpDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetail.Controls.Add(this.txtDeparture);
            this.grpDetail.Controls.Add(this.numMaxDuration);
            this.grpDetail.Controls.Add(this.txtDestination);
            this.grpDetail.Controls.Add(this.numMinDuration);
            this.grpDetail.Controls.Add(this.btnExit);
            this.grpDetail.Controls.Add(this.btnNoReturnRange);
            this.grpDetail.Controls.Add(this.btnNoDepartRange);
            this.grpDetail.Controls.Add(this.lblDuration);
            this.grpDetail.Controls.Add(this.btnUploadPackages);
            this.grpDetail.Controls.Add(this.btnGetFareAndSave);
            this.grpDetail.Controls.Add(this.numReturnDateRangeMinus);
            this.grpDetail.Controls.Add(this.numDepartDateRangeMinus);
            this.grpDetail.Controls.Add(this.btnSave);
            this.grpDetail.Controls.Add(this.numReturnDateRangePlus);
            this.grpDetail.Controls.Add(this.btnSummary);
            this.grpDetail.Controls.Add(this.numDepartDateRangePlus);
            this.grpDetail.Controls.Add(this.returnDatePicker);
            this.grpDetail.Controls.Add(this.departureDatePicker);
            this.grpDetail.Controls.Add(this.btnShowFare);
            this.grpDetail.Controls.Add(this.lblDestination);
            this.grpDetail.Controls.Add(this.lblDeparture);
            this.grpDetail.Controls.Add(this.label5);
            this.grpDetail.Controls.Add(this.lblReturnPM);
            this.grpDetail.Controls.Add(this.lblTravelDate);
            this.grpDetail.Controls.Add(this.chkReturnDate);
            this.grpDetail.Location = new System.Drawing.Point(12, 12);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(764, 112);
            this.grpDetail.TabIndex = 4;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Journey Information";
            // 
            // txtDeparture
            // 
            this.txtDeparture.AlwaysShowSuggest = false;
            this.txtDeparture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeparture.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDeparture.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDeparture.CaseSensitive = false;
            this.txtDeparture.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDeparture.Location = new System.Drawing.Point(77, 21);
            this.txtDeparture.MinTypedCharacters = 1;
            this.txtDeparture.Name = "txtDeparture";
            this.txtDeparture.Size = new System.Drawing.Size(283, 20);
            this.txtDeparture.TabIndex = 1;
            this.txtDeparture.VisibleSuggestItems = 10;
            // 
            // numMaxDuration
            // 
            this.numMaxDuration.Location = new System.Drawing.Point(133, 83);
            this.numMaxDuration.Name = "numMaxDuration";
            this.numMaxDuration.Size = new System.Drawing.Size(40, 20);
            this.numMaxDuration.TabIndex = 14;
            this.numMaxDuration.Value = new decimal(new int[] {
            42,
            0,
            0,
            0});
            // 
            // txtDestination
            // 
            this.txtDestination.AlwaysShowSuggest = false;
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDestination.CaseSensitive = false;
            this.txtDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestination.Location = new System.Drawing.Point(77, 50);
            this.txtDestination.MinTypedCharacters = 1;
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(283, 20);
            this.txtDestination.TabIndex = 2;
            this.txtDestination.VisibleSuggestItems = 10;
            // 
            // numMinDuration
            // 
            this.numMinDuration.Location = new System.Drawing.Point(77, 83);
            this.numMinDuration.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numMinDuration.Name = "numMinDuration";
            this.numMinDuration.Size = new System.Drawing.Size(40, 20);
            this.numMinDuration.TabIndex = 13;
            this.numMinDuration.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Exit;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(693, 78);
            this.btnExit.Name = "btnExit";
            this.btnExit.Padding = new System.Windows.Forms.Padding(3, 0, 5, 0);
            this.btnExit.Size = new System.Drawing.Size(59, 26);
            this.btnExit.TabIndex = 21;
            this.btnExit.Text = "E&xit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnNoReturnRange
            // 
            this.btnNoReturnRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNoReturnRange.FlatAppearance.BorderSize = 0;
            this.btnNoReturnRange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNoReturnRange.Location = new System.Drawing.Point(730, 49);
            this.btnNoReturnRange.Name = "btnNoReturnRange";
            this.btnNoReturnRange.Size = new System.Drawing.Size(20, 20);
            this.btnNoReturnRange.TabIndex = 10;
            this.btnNoReturnRange.Text = "-";
            this.toolTip.SetToolTip(this.btnNoReturnRange, "Shift journey 1 week later");
            this.btnNoReturnRange.UseVisualStyleBackColor = true;
            this.btnNoReturnRange.Click += new System.EventHandler(this.btnNoReturnRange_Click);
            // 
            // btnNoDepartRange
            // 
            this.btnNoDepartRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNoDepartRange.FlatAppearance.BorderSize = 0;
            this.btnNoDepartRange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNoDepartRange.Location = new System.Drawing.Point(730, 24);
            this.btnNoDepartRange.Name = "btnNoDepartRange";
            this.btnNoDepartRange.Size = new System.Drawing.Size(20, 20);
            this.btnNoDepartRange.TabIndex = 7;
            this.btnNoDepartRange.Text = "-";
            this.toolTip.SetToolTip(this.btnNoDepartRange, "Shift journey 1 week later");
            this.btnNoDepartRange.UseVisualStyleBackColor = true;
            this.btnNoDepartRange.Click += new System.EventHandler(this.btnNoDepartRange_Click);
            // 
            // lblDuration
            // 
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(26, 85);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(177, 13);
            this.lblDuration.TabIndex = 31;
            this.lblDuration.Text = "Duration:                 -                 days";
            // 
            // btnUploadPackages
            // 
            this.btnUploadPackages.Enabled = false;
            this.btnUploadPackages.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Upload;
            this.btnUploadPackages.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUploadPackages.Location = new System.Drawing.Point(380, 78);
            this.btnUploadPackages.Name = "btnUploadPackages";
            this.btnUploadPackages.Padding = new System.Windows.Forms.Padding(3, 0, 7, 0);
            this.btnUploadPackages.Size = new System.Drawing.Size(80, 26);
            this.btnUploadPackages.TabIndex = 20;
            this.btnUploadPackages.Text = "Upload";
            this.btnUploadPackages.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnUploadPackages, "Send the fare data which is shown on screen using the configured data synchronize" +
        "r");
            this.btnUploadPackages.UseVisualStyleBackColor = true;
            this.btnUploadPackages.Click += new System.EventHandler(this.btnUploadPackages_Click);
            // 
            // btnGetFareAndSave
            // 
            this.btnGetFareAndSave.AutoSwitchStateOnClick = false;
            this.btnGetFareAndSave.FirstStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.Export;
            this.btnGetFareAndSave.FirstStateText = "Get Fare && Save";
            this.btnGetFareAndSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGetFareAndSave.IsSecondState = false;
            this.btnGetFareAndSave.Location = new System.Drawing.Point(561, 78);
            this.btnGetFareAndSave.Name = "btnGetFareAndSave";
            this.btnGetFareAndSave.Padding = new System.Windows.Forms.Padding(5, 0, 4, 0);
            this.btnGetFareAndSave.SecondStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.Stop;
            this.btnGetFareAndSave.SecondStateText = "Stop";
            this.btnGetFareAndSave.Size = new System.Drawing.Size(120, 26);
            this.btnGetFareAndSave.TabIndex = 19;
            this.btnGetFareAndSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnGetFareAndSave, "Get latest fare data and export them (fare data won\'t be shown on screen)");
            this.btnGetFareAndSave.UseVisualStyleBackColor = true;
            this.btnGetFareAndSave.StateChanging += new System.ComponentModel.CancelEventHandler(this.GetFareButtons_StateChanging);
            this.btnGetFareAndSave.Click += new System.EventHandler(this.StartMonitorButton_Click);
            // 
            // numReturnDateRangeMinus
            // 
            this.numReturnDateRangeMinus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numReturnDateRangeMinus.Location = new System.Drawing.Point(695, 49);
            this.numReturnDateRangeMinus.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numReturnDateRangeMinus.Name = "numReturnDateRangeMinus";
            this.numReturnDateRangeMinus.Size = new System.Drawing.Size(36, 20);
            this.numReturnDateRangeMinus.TabIndex = 9;
            this.numReturnDateRangeMinus.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numReturnDateRangeMinus.ValueChanged += new System.EventHandler(this.DateRange_ValueChanged);
            // 
            // numDepartDateRangeMinus
            // 
            this.numDepartDateRangeMinus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDepartDateRangeMinus.Location = new System.Drawing.Point(695, 24);
            this.numDepartDateRangeMinus.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numDepartDateRangeMinus.Name = "numDepartDateRangeMinus";
            this.numDepartDateRangeMinus.Size = new System.Drawing.Size(36, 20);
            this.numDepartDateRangeMinus.TabIndex = 6;
            this.numDepartDateRangeMinus.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numDepartDateRangeMinus.ValueChanged += new System.EventHandler(this.DateRange_ValueChanged);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(309, 78);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnSave.Size = new System.Drawing.Size(70, 26);
            this.btnSave.TabIndex = 18;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnSave, "Export all fare data which is shown on screen");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // numReturnDateRangePlus
            // 
            this.numReturnDateRangePlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numReturnDateRangePlus.Location = new System.Drawing.Point(644, 49);
            this.numReturnDateRangePlus.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numReturnDateRangePlus.Name = "numReturnDateRangePlus";
            this.numReturnDateRangePlus.Size = new System.Drawing.Size(36, 20);
            this.numReturnDateRangePlus.TabIndex = 8;
            this.numReturnDateRangePlus.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numReturnDateRangePlus.ValueChanged += new System.EventHandler(this.DateRange_ValueChanged);
            // 
            // btnSummary
            // 
            this.btnSummary.Enabled = false;
            this.btnSummary.Location = new System.Drawing.Point(464, 78);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(79, 26);
            this.btnSummary.TabIndex = 17;
            this.btnSummary.Text = "Summary";
            this.toolTip.SetToolTip(this.btnSummary, "View summary of all visible fare data which is shown on screen");
            this.btnSummary.UseVisualStyleBackColor = true;
            this.btnSummary.Click += new System.EventHandler(this.btnSummary_Click);
            // 
            // numDepartDateRangePlus
            // 
            this.numDepartDateRangePlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numDepartDateRangePlus.Location = new System.Drawing.Point(644, 24);
            this.numDepartDateRangePlus.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numDepartDateRangePlus.Name = "numDepartDateRangePlus";
            this.numDepartDateRangePlus.Size = new System.Drawing.Size(36, 20);
            this.numDepartDateRangePlus.TabIndex = 5;
            this.numDepartDateRangePlus.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numDepartDateRangePlus.ValueChanged += new System.EventHandler(this.DateRange_ValueChanged);
            // 
            // returnDatePicker
            // 
            this.returnDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.returnDatePicker.BackColor = System.Drawing.SystemColors.Window;
            this.returnDatePicker.Culture = new System.Globalization.CultureInfo("en-US");
            this.returnDatePicker.CultureCalendar = ((System.Globalization.Calendar)(resources.GetObject("returnDatePicker.CultureCalendar")));
            this.returnDatePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnDatePicker.InvalidForeColor = System.Drawing.SystemColors.ControlText;
            this.returnDatePicker.Location = new System.Drawing.Point(452, 49);
            this.returnDatePicker.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.returnDatePicker.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.returnDatePicker.Name = "returnDatePicker";
            this.returnDatePicker.PickerBoldedDates = ((System.Collections.Generic.List<System.DateTime>)(resources.GetObject("returnDatePicker.PickerBoldedDates")));
            this.returnDatePicker.PickerDayFont = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.returnDatePicker.PickerDayHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnDatePicker.PickerFooterFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.returnDatePicker.PickerHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.returnDatePicker.Size = new System.Drawing.Size(171, 22);
            this.returnDatePicker.TabIndex = 4;
            this.returnDatePicker.Value = new System.DateTime(2013, 7, 2, 0, 0, 0, 0);
            this.returnDatePicker.ValueChanged += new System.EventHandler<SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses.CheckDateEventArgs>(this.datePicker_ValueChanged);
            // 
            // departureDatePicker
            // 
            this.departureDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.departureDatePicker.BackColor = System.Drawing.SystemColors.Window;
            this.departureDatePicker.Culture = new System.Globalization.CultureInfo("en-US");
            this.departureDatePicker.CultureCalendar = ((System.Globalization.Calendar)(resources.GetObject("departureDatePicker.CultureCalendar")));
            this.departureDatePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.departureDatePicker.InvalidForeColor = System.Drawing.SystemColors.ControlText;
            this.departureDatePicker.Location = new System.Drawing.Point(452, 23);
            this.departureDatePicker.MaxDate = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this.departureDatePicker.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.departureDatePicker.Name = "departureDatePicker";
            this.departureDatePicker.PickerBoldedDates = ((System.Collections.Generic.List<System.DateTime>)(resources.GetObject("departureDatePicker.PickerBoldedDates")));
            this.departureDatePicker.PickerDayFont = new System.Drawing.Font("Segoe UI", 9F);
            this.departureDatePicker.PickerDayHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.departureDatePicker.PickerFooterFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.departureDatePicker.PickerHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.departureDatePicker.Size = new System.Drawing.Size(171, 22);
            this.departureDatePicker.TabIndex = 3;
            this.departureDatePicker.Value = new System.DateTime(2013, 7, 2, 0, 0, 0, 0);
            this.departureDatePicker.ValueChanged += new System.EventHandler<SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.EventClasses.CheckDateEventArgs>(this.datePicker_ValueChanged);
            // 
            // btnShowFare
            // 
            this.btnShowFare.AutoSwitchStateOnClick = false;
            this.btnShowFare.FirstStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.Search;
            this.btnShowFare.FirstStateText = "Get Fare";
            this.btnShowFare.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnShowFare.IsSecondState = false;
            this.btnShowFare.Location = new System.Drawing.Point(213, 78);
            this.btnShowFare.Name = "btnShowFare";
            this.btnShowFare.Padding = new System.Windows.Forms.Padding(5, 0, 9, 0);
            this.btnShowFare.SecondStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.Stop;
            this.btnShowFare.SecondStateText = "Stop";
            this.btnShowFare.Size = new System.Drawing.Size(95, 26);
            this.btnShowFare.TabIndex = 16;
            this.btnShowFare.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnShowFare, "Show the fare data matching selected filters");
            this.btnShowFare.UseVisualStyleBackColor = true;
            this.btnShowFare.StateChanging += new System.ComponentModel.CancelEventHandler(this.GetFareButtons_StateChanging);
            this.btnShowFare.Click += new System.EventHandler(this.StartMonitorButton_Click);
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(12, 53);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(63, 13);
            this.lblDestination.TabIndex = 7;
            this.lblDestination.Text = "Destination:";
            // 
            // lblDeparture
            // 
            this.lblDeparture.AutoSize = true;
            this.lblDeparture.Location = new System.Drawing.Point(18, 24);
            this.lblDeparture.Name = "lblDeparture";
            this.lblDeparture.Size = new System.Drawing.Size(57, 13);
            this.lblDeparture.TabIndex = 5;
            this.lblDeparture.Text = "Departure:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(629, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "+                -";
            // 
            // lblReturnPM
            // 
            this.lblReturnPM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblReturnPM.AutoSize = true;
            this.lblReturnPM.Location = new System.Drawing.Point(629, 51);
            this.lblReturnPM.Name = "lblReturnPM";
            this.lblReturnPM.Size = new System.Drawing.Size(64, 13);
            this.lblReturnPM.TabIndex = 19;
            this.lblReturnPM.Text = "+                -";
            // 
            // lblTravelDate
            // 
            this.lblTravelDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTravelDate.AutoSize = true;
            this.lblTravelDate.Location = new System.Drawing.Point(369, 27);
            this.lblTravelDate.Name = "lblTravelDate";
            this.lblTravelDate.Size = new System.Drawing.Size(80, 13);
            this.lblTravelDate.TabIndex = 10;
            this.lblTravelDate.Text = "Departure Date";
            // 
            // chkReturnDate
            // 
            this.chkReturnDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkReturnDate.AutoSize = true;
            this.chkReturnDate.Checked = true;
            this.chkReturnDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReturnDate.Location = new System.Drawing.Point(368, 51);
            this.chkReturnDate.Name = "chkReturnDate";
            this.chkReturnDate.Size = new System.Drawing.Size(84, 17);
            this.chkReturnDate.TabIndex = 65;
            this.chkReturnDate.Text = "Return Date";
            this.chkReturnDate.UseVisualStyleBackColor = true;
            this.chkReturnDate.CheckedChanged += new System.EventHandler(this.chkReturnDate_CheckedChanged);
            // 
            // numPriceLimit
            // 
            this.numPriceLimit.Location = new System.Drawing.Point(42, 20);
            this.numPriceLimit.Maximum = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numPriceLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPriceLimit.Name = "numPriceLimit";
            this.numPriceLimit.Size = new System.Drawing.Size(61, 20);
            this.numPriceLimit.TabIndex = 15;
            this.numPriceLimit.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblPriceLimit
            // 
            this.lblPriceLimit.AutoSize = true;
            this.lblPriceLimit.Location = new System.Drawing.Point(12, 21);
            this.lblPriceLimit.Name = "lblPriceLimit";
            this.lblPriceLimit.Size = new System.Drawing.Size(103, 13);
            this.lblPriceLimit.TabIndex = 34;
            this.lblPriceLimit.Text = "Limit:                      €";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 525);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(929, 22);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(26, 17);
            this.lblStatus.Text = "Idle";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.TextChanged += new System.EventHandler(this.lblStatus_TextChanged);
            // 
            // btnLiveFareData
            // 
            this.btnLiveFareData.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.DataBrowser;
            this.btnLiveFareData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLiveFareData.Location = new System.Drawing.Point(7, 78);
            this.btnLiveFareData.Name = "btnLiveFareData";
            this.btnLiveFareData.Padding = new System.Windows.Forms.Padding(5, 0, 10, 0);
            this.btnLiveFareData.Size = new System.Drawing.Size(124, 25);
            this.btnLiveFareData.TabIndex = 12;
            this.btnLiveFareData.Text = "Historical Data";
            this.btnLiveFareData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnLiveFareData, "View all historical fare data");
            this.btnLiveFareData.UseVisualStyleBackColor = true;
            this.btnLiveFareData.Click += new System.EventHandler(this.btnLiveFareData_Click);
            // 
            // btnLiveMonitor
            // 
            this.btnLiveMonitor.AutoSwitchStateOnClick = false;
            this.btnLiveMonitor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLiveMonitor.FirstStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.LiveMonitor;
            this.btnLiveMonitor.FirstStateText = "Live Monitor";
            this.btnLiveMonitor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLiveMonitor.IsSecondState = false;
            this.btnLiveMonitor.Location = new System.Drawing.Point(7, 45);
            this.btnLiveMonitor.Name = "btnLiveMonitor";
            this.btnLiveMonitor.Padding = new System.Windows.Forms.Padding(5, 0, 12, 0);
            this.btnLiveMonitor.SecondStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.Stop;
            this.btnLiveMonitor.SecondStateText = "Stop Live Monitor";
            this.btnLiveMonitor.Size = new System.Drawing.Size(124, 27);
            this.btnLiveMonitor.TabIndex = 11;
            this.btnLiveMonitor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnLiveMonitor, "Live monitor of air fare in background");
            this.btnLiveMonitor.UseVisualStyleBackColor = true;
            this.btnLiveMonitor.Click += new System.EventHandler(this.StartMonitorButton_Click);
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.BalloonTipTitle = "Fare Price was changed";
            this.trayIcon.Text = "SkyDean Flight Fare Monitor - Live Monitor";
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // btnLiveMonitorImageList
            // 
            this.btnLiveMonitorImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.btnLiveMonitorImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.btnLiveMonitorImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // grpLiveMonitor
            // 
            this.grpLiveMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLiveMonitor.Controls.Add(this.numPriceLimit);
            this.grpLiveMonitor.Controls.Add(this.btnLiveFareData);
            this.grpLiveMonitor.Controls.Add(this.lblPriceLimit);
            this.grpLiveMonitor.Controls.Add(this.btnLiveMonitor);
            this.grpLiveMonitor.Location = new System.Drawing.Point(782, 12);
            this.grpLiveMonitor.Name = "grpLiveMonitor";
            this.grpLiveMonitor.Size = new System.Drawing.Size(139, 112);
            this.grpLiveMonitor.TabIndex = 66;
            this.grpLiveMonitor.TabStop = false;
            this.grpLiveMonitor.Text = "Live Monitor";
            // 
            // fareBrowserTabs
            // 
            this.fareBrowserTabs.AllowDrop = true;
            this.fareBrowserTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fareBrowserTabs.Cursor = System.Windows.Forms.Cursors.Default;
            this.fareBrowserTabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.fareBrowserTabs.HotTrack = true;
            this.fareBrowserTabs.IndicatorWidth = 3;
            this.fareBrowserTabs.Location = new System.Drawing.Point(0, 128);
            this.fareBrowserTabs.Margin = new System.Windows.Forms.Padding(1);
            this.fareBrowserTabs.Name = "fareBrowserTabs";
            this.fareBrowserTabs.SelectedIndex = 0;
            this.fareBrowserTabs.ShowToolTips = true;
            this.fareBrowserTabs.Size = new System.Drawing.Size(929, 396);
            this.fareBrowserTabs.TabIndex = 22;
            this.fareBrowserTabs.TabPageClosing += new System.Windows.Forms.TabControlCancelEventHandler(this.fareBrowserTabs_TabPageClosing);
            this.fareBrowserTabs.TabPageReloading += new System.Windows.Forms.TabControlEventHandler(this.fareBrowserTabs_TabPageRefreshing);
            // 
            // CheckFareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 547);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.fareBrowserTabs);
            this.Controls.Add(this.grpLiveMonitor);
            this.Controls.Add(this.grpDetail);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::SkyDean.FareLiz.WinForm.Properties.Resources.LiveFareIcon;
            this.Name = "CheckFareForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Flexible Fare Scanner";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CheckFareForm_FormClosing);
            this.Load += new System.EventHandler(this.CheckFareForm_Load);
            this.Resize += new System.EventHandler(this.CheckFareForm_Resize);
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReturnDateRangeMinus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepartDateRangeMinus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReturnDateRangePlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepartDateRangePlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPriceLimit)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.grpLiveMonitor.ResumeLayout(false);
            this.grpLiveMonitor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AirportTextBox txtDestination;
        private AirportTextBox txtDeparture;
        private EnhancedDatePicker departureDatePicker;
        private EnhancedDatePicker returnDatePicker;
        private FlatTabControl fareBrowserTabs;
        private ImageButton btnExit;
        private SwitchButton btnGetFareAndSave;
        private SwitchButton btnShowFare;
        private SwitchButton btnLiveMonitor;
        private ImageButton btnLiveFareData;
        private ImageButton btnSave;
        private ImageButton btnUploadPackages;
        private System.Windows.Forms.Button btnNoDepartRange;
        private System.Windows.Forms.Button btnNoReturnRange;
        private System.Windows.Forms.Button btnSummary;
        private System.Windows.Forms.CheckBox chkReturnDate;
        private System.Windows.Forms.GroupBox grpLiveMonitor;
        private System.Windows.Forms.ImageList btnLiveMonitorImageList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblDeparture;
        private System.Windows.Forms.Label lblPriceLimit;
        private System.Windows.Forms.Label lblReturnPM;
        private System.Windows.Forms.Label lblTravelDate;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.NumericUpDown numDepartDateRangeMinus;
        private System.Windows.Forms.NumericUpDown numDepartDateRangePlus;
        private System.Windows.Forms.NumericUpDown numMaxDuration;
        private System.Windows.Forms.NumericUpDown numMinDuration;
        private System.Windows.Forms.NumericUpDown numPriceLimit;
        private System.Windows.Forms.NumericUpDown numReturnDateRangeMinus;
        private System.Windows.Forms.NumericUpDown numReturnDateRangePlus;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.GroupBox grpDetail;
    }
}