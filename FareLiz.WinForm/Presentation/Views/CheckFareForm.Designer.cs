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
            this.journeyInfoTable = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.lblDuration = new System.Windows.Forms.Label();
            this.btnExit = new SkyDean.FareLiz.WinForm.Components.Controls.Button.ImageButton();
            this.btnGetFareAndSave = new SkyDean.FareLiz.WinForm.Components.Controls.Button.SwitchButton();
            this.btnUploadPackages = new SkyDean.FareLiz.WinForm.Components.Controls.Button.ImageButton();
            this.btnSummary = new System.Windows.Forms.Button();
            this.numMaxDuration = new System.Windows.Forms.NumericUpDown();
            this.numMinDuration = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new SkyDean.FareLiz.WinForm.Components.Controls.Button.ImageButton();
            this.btnShowFare = new SkyDean.FareLiz.WinForm.Components.Controls.Button.SwitchButton();
            this.txtDeparture = new SkyDean.FareLiz.WinForm.Components.Controls.TextBox.AirportTextBox();
            this.txtDestination = new SkyDean.FareLiz.WinForm.Components.Controls.TextBox.AirportTextBox();
            this.lblDeparture = new System.Windows.Forms.Label();
            this.lblDestination = new System.Windows.Forms.Label();
            this.btnNoReturnRange = new System.Windows.Forms.Button();
            this.lblTravelDate = new System.Windows.Forms.Label();
            this.btnNoDepartRange = new System.Windows.Forms.Button();
            this.chkReturnDate = new System.Windows.Forms.CheckBox();
            this.departureDatePicker = new SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker.EnhancedDatePicker();
            this.returnDatePicker = new SkyDean.FareLiz.WinForm.Components.Controls.DatePicker.DatePicker.EnhancedDatePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.numReturnDateRangeMinus = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numDepartDateRangeMinus = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numReturnDateRangePlus = new System.Windows.Forms.NumericUpDown();
            this.numDepartDateRangePlus = new System.Windows.Forms.NumericUpDown();
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.fareBrowserTabs = new SkyDean.FareLiz.WinForm.Components.Controls.TabControl.FlatTabControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.grpDetail.SuspendLayout();
            this.journeyInfoTable.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReturnDateRangeMinus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepartDateRangeMinus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numReturnDateRangePlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDepartDateRangePlus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPriceLimit)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.grpLiveMonitor.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDetail
            // 
            this.grpDetail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetail.Controls.Add(this.journeyInfoTable);
            this.grpDetail.Location = new System.Drawing.Point(3, 3);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(706, 113);
            this.grpDetail.TabIndex = 4;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Journey Information";
            // 
            // journeyInfoTable
            // 
            this.journeyInfoTable.AutoSize = true;
            this.journeyInfoTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.journeyInfoTable.ColumnCount = 9;
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.journeyInfoTable.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.journeyInfoTable.Controls.Add(this.txtDeparture, 1, 0);
            this.journeyInfoTable.Controls.Add(this.txtDestination, 1, 1);
            this.journeyInfoTable.Controls.Add(this.lblDeparture, 0, 0);
            this.journeyInfoTable.Controls.Add(this.lblDestination, 0, 1);
            this.journeyInfoTable.Controls.Add(this.btnNoReturnRange, 8, 1);
            this.journeyInfoTable.Controls.Add(this.lblTravelDate, 2, 0);
            this.journeyInfoTable.Controls.Add(this.btnNoDepartRange, 8, 0);
            this.journeyInfoTable.Controls.Add(this.chkReturnDate, 2, 1);
            this.journeyInfoTable.Controls.Add(this.departureDatePicker, 3, 0);
            this.journeyInfoTable.Controls.Add(this.returnDatePicker, 3, 1);
            this.journeyInfoTable.Controls.Add(this.label5, 4, 0);
            this.journeyInfoTable.Controls.Add(this.numReturnDateRangeMinus, 7, 1);
            this.journeyInfoTable.Controls.Add(this.label1, 4, 1);
            this.journeyInfoTable.Controls.Add(this.numDepartDateRangeMinus, 7, 0);
            this.journeyInfoTable.Controls.Add(this.label3, 6, 1);
            this.journeyInfoTable.Controls.Add(this.label2, 6, 0);
            this.journeyInfoTable.Controls.Add(this.numReturnDateRangePlus, 5, 1);
            this.journeyInfoTable.Controls.Add(this.numDepartDateRangePlus, 5, 0);
            this.journeyInfoTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.journeyInfoTable.Location = new System.Drawing.Point(3, 16);
            this.journeyInfoTable.Name = "journeyInfoTable";
            this.journeyInfoTable.RowCount = 3;
            this.journeyInfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.journeyInfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.journeyInfoTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.journeyInfoTable.Size = new System.Drawing.Size(700, 94);
            this.journeyInfoTable.TabIndex = 67;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 10;
            this.journeyInfoTable.SetColumnSpan(this.tableLayoutPanel1, 9);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDuration, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnExit, 9, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGetFareAndSave, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnUploadPackages, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSummary, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.numMaxDuration, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.numMinDuration, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSave, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnShowFare, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 56);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(694, 38);
            this.tableLayoutPanel1.TabIndex = 68;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(96, 12);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 32;
            this.label4.Text = "-";
            // 
            // lblDuration
            // 
            this.lblDuration.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDuration.AutoSize = true;
            this.lblDuration.Location = new System.Drawing.Point(0, 12);
            this.lblDuration.Margin = new System.Windows.Forms.Padding(0);
            this.lblDuration.Name = "lblDuration";
            this.lblDuration.Size = new System.Drawing.Size(50, 13);
            this.lblDuration.TabIndex = 31;
            this.lblDuration.Text = "Duration:";
            // 
            // btnExit
            // 
            this.btnExit.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExit.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Exit;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(632, 6);
            this.btnExit.Name = "btnExit";
            this.btnExit.Padding = new System.Windows.Forms.Padding(3, 0, 5, 0);
            this.btnExit.Size = new System.Drawing.Size(59, 26);
            this.btnExit.TabIndex = 21;
            this.btnExit.Text = "E&xit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnGetFareAndSave
            // 
            this.btnGetFareAndSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnGetFareAndSave.AutoSwitchStateOnClick = false;
            this.btnGetFareAndSave.FirstStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.Export;
            this.btnGetFareAndSave.FirstStateText = "Get Fare && Save";
            this.btnGetFareAndSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGetFareAndSave.IsSecondState = false;
            this.btnGetFareAndSave.Location = new System.Drawing.Point(503, 6);
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
            // btnUploadPackages
            // 
            this.btnUploadPackages.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnUploadPackages.Enabled = false;
            this.btnUploadPackages.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Upload;
            this.btnUploadPackages.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnUploadPackages.Location = new System.Drawing.Point(332, 6);
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
            // btnSummary
            // 
            this.btnSummary.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSummary.Enabled = false;
            this.btnSummary.Location = new System.Drawing.Point(418, 6);
            this.btnSummary.Name = "btnSummary";
            this.btnSummary.Size = new System.Drawing.Size(79, 26);
            this.btnSummary.TabIndex = 17;
            this.btnSummary.Text = "Summary";
            this.toolTip.SetToolTip(this.btnSummary, "View summary of all visible fare data which is shown on screen");
            this.btnSummary.UseVisualStyleBackColor = true;
            this.btnSummary.Click += new System.EventHandler(this.btnSummary_Click);
            // 
            // numMaxDuration
            // 
            this.numMaxDuration.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numMaxDuration.Location = new System.Drawing.Point(109, 9);
            this.numMaxDuration.Name = "numMaxDuration";
            this.numMaxDuration.Size = new System.Drawing.Size(40, 20);
            this.numMaxDuration.TabIndex = 14;
            this.numMaxDuration.Value = new decimal(new int[] {
            42,
            0,
            0,
            0});
            // 
            // numMinDuration
            // 
            this.numMinDuration.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numMinDuration.Location = new System.Drawing.Point(53, 9);
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
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSave.Enabled = false;
            this.btnSave.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(256, 6);
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
            // btnShowFare
            // 
            this.btnShowFare.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnShowFare.AutoSwitchStateOnClick = false;
            this.btnShowFare.FirstStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.Search;
            this.btnShowFare.FirstStateText = "Get Fare";
            this.btnShowFare.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnShowFare.IsSecondState = false;
            this.btnShowFare.Location = new System.Drawing.Point(155, 6);
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
            // txtDeparture
            // 
            this.txtDeparture.AlwaysShowSuggest = false;
            this.txtDeparture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeparture.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDeparture.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDeparture.CaseSensitive = false;
            this.txtDeparture.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDeparture.Location = new System.Drawing.Point(72, 4);
            this.txtDeparture.MinTypedCharacters = 1;
            this.txtDeparture.Name = "txtDeparture";
            this.txtDeparture.Size = new System.Drawing.Size(225, 20);
            this.txtDeparture.TabIndex = 1;
            // 
            // txtDestination
            // 
            this.txtDestination.AlwaysShowSuggest = false;
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDestination.CaseSensitive = false;
            this.txtDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestination.Location = new System.Drawing.Point(72, 32);
            this.txtDestination.MinTypedCharacters = 1;
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(225, 20);
            this.txtDestination.TabIndex = 2;
            // 
            // lblDeparture
            // 
            this.lblDeparture.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDeparture.AutoSize = true;
            this.lblDeparture.Location = new System.Drawing.Point(3, 7);
            this.lblDeparture.Name = "lblDeparture";
            this.lblDeparture.Size = new System.Drawing.Size(57, 13);
            this.lblDeparture.TabIndex = 5;
            this.lblDeparture.Text = "Departure:";
            // 
            // lblDestination
            // 
            this.lblDestination.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(3, 35);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(63, 13);
            this.lblDestination.TabIndex = 7;
            this.lblDestination.Text = "Destination:";
            // 
            // btnNoReturnRange
            // 
            this.btnNoReturnRange.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnNoReturnRange.FlatAppearance.BorderSize = 0;
            this.btnNoReturnRange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNoReturnRange.Location = new System.Drawing.Point(677, 32);
            this.btnNoReturnRange.Name = "btnNoReturnRange";
            this.btnNoReturnRange.Size = new System.Drawing.Size(20, 20);
            this.btnNoReturnRange.TabIndex = 10;
            this.btnNoReturnRange.Text = "-";
            this.toolTip.SetToolTip(this.btnNoReturnRange, "Shift journey 1 week later");
            this.btnNoReturnRange.UseVisualStyleBackColor = true;
            this.btnNoReturnRange.Click += new System.EventHandler(this.btnNoReturnRange_Click);
            // 
            // lblTravelDate
            // 
            this.lblTravelDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTravelDate.AutoSize = true;
            this.lblTravelDate.Location = new System.Drawing.Point(303, 7);
            this.lblTravelDate.Name = "lblTravelDate";
            this.lblTravelDate.Size = new System.Drawing.Size(80, 13);
            this.lblTravelDate.TabIndex = 10;
            this.lblTravelDate.Text = "Departure Date";
            // 
            // btnNoDepartRange
            // 
            this.btnNoDepartRange.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnNoDepartRange.FlatAppearance.BorderSize = 0;
            this.btnNoDepartRange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNoDepartRange.Location = new System.Drawing.Point(677, 4);
            this.btnNoDepartRange.Name = "btnNoDepartRange";
            this.btnNoDepartRange.Size = new System.Drawing.Size(20, 19);
            this.btnNoDepartRange.TabIndex = 7;
            this.btnNoDepartRange.Text = "-";
            this.toolTip.SetToolTip(this.btnNoDepartRange, "Shift journey 1 week later");
            this.btnNoDepartRange.UseVisualStyleBackColor = true;
            this.btnNoDepartRange.Click += new System.EventHandler(this.btnNoDepartRange_Click);
            // 
            // chkReturnDate
            // 
            this.chkReturnDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkReturnDate.AutoSize = true;
            this.chkReturnDate.Checked = true;
            this.chkReturnDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkReturnDate.Location = new System.Drawing.Point(303, 33);
            this.chkReturnDate.Name = "chkReturnDate";
            this.chkReturnDate.Size = new System.Drawing.Size(84, 17);
            this.chkReturnDate.TabIndex = 65;
            this.chkReturnDate.Text = "Return Date";
            this.chkReturnDate.UseVisualStyleBackColor = true;
            this.chkReturnDate.CheckedChanged += new System.EventHandler(this.chkReturnDate_CheckedChanged);
            // 
            // departureDatePicker
            // 
            this.departureDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.departureDatePicker.BackColor = System.Drawing.SystemColors.Window;
            this.departureDatePicker.Culture = new System.Globalization.CultureInfo("en-US");
            this.departureDatePicker.CultureCalendar = ((System.Globalization.Calendar)(resources.GetObject("departureDatePicker.CultureCalendar")));
            this.departureDatePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.departureDatePicker.InvalidForeColor = System.Drawing.SystemColors.ControlText;
            this.departureDatePicker.Location = new System.Drawing.Point(393, 3);
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
            // returnDatePicker
            // 
            this.returnDatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.returnDatePicker.BackColor = System.Drawing.SystemColors.Window;
            this.returnDatePicker.Culture = new System.Globalization.CultureInfo("en-US");
            this.returnDatePicker.CultureCalendar = ((System.Globalization.Calendar)(resources.GetObject("returnDatePicker.CultureCalendar")));
            this.returnDatePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.returnDatePicker.InvalidForeColor = System.Drawing.SystemColors.ControlText;
            this.returnDatePicker.Location = new System.Drawing.Point(393, 31);
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
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(567, 7);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "+";
            // 
            // numReturnDateRangeMinus
            // 
            this.numReturnDateRangeMinus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numReturnDateRangeMinus.Location = new System.Drawing.Point(635, 32);
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
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(567, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 13);
            this.label1.TabIndex = 66;
            this.label1.Text = "+";
            // 
            // numDepartDateRangeMinus
            // 
            this.numDepartDateRangeMinus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numDepartDateRangeMinus.Location = new System.Drawing.Point(635, 4);
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
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(622, 35);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 68;
            this.label3.Text = "-";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(622, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 67;
            this.label2.Text = "-";
            // 
            // numReturnDateRangePlus
            // 
            this.numReturnDateRangePlus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numReturnDateRangePlus.Location = new System.Drawing.Point(583, 32);
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
            // numDepartDateRangePlus
            // 
            this.numDepartDateRangePlus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numDepartDateRangePlus.Location = new System.Drawing.Point(583, 4);
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
            // numPriceLimit
            // 
            this.numPriceLimit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numPriceLimit.Location = new System.Drawing.Point(63, 3);
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
            this.numPriceLimit.Size = new System.Drawing.Size(74, 20);
            this.numPriceLimit.TabIndex = 15;
            this.numPriceLimit.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // lblPriceLimit
            // 
            this.lblPriceLimit.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPriceLimit.AutoSize = true;
            this.lblPriceLimit.Location = new System.Drawing.Point(3, 6);
            this.lblPriceLimit.Name = "lblPriceLimit";
            this.lblPriceLimit.Size = new System.Drawing.Size(54, 13);
            this.lblPriceLimit.TabIndex = 34;
            this.lblPriceLimit.Text = "Price limit:";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 525);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(864, 22);
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
            this.btnLiveFareData.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel2.SetColumnSpan(this.btnLiveFareData, 2);
            this.btnLiveFareData.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.DataBrowser;
            this.btnLiveFareData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLiveFareData.Location = new System.Drawing.Point(6, 64);
            this.btnLiveFareData.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.btnLiveFareData.Name = "btnLiveFareData";
            this.btnLiveFareData.Padding = new System.Windows.Forms.Padding(0, 0, 13, 0);
            this.btnLiveFareData.Size = new System.Drawing.Size(124, 27);
            this.btnLiveFareData.TabIndex = 12;
            this.btnLiveFareData.Text = "Historical Data";
            this.btnLiveFareData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnLiveFareData, "View all historical fare data");
            this.btnLiveFareData.UseVisualStyleBackColor = true;
            this.btnLiveFareData.Click += new System.EventHandler(this.btnLiveFareData_Click);
            // 
            // btnLiveMonitor
            // 
            this.btnLiveMonitor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLiveMonitor.AutoSwitchStateOnClick = false;
            this.btnLiveMonitor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.tableLayoutPanel2.SetColumnSpan(this.btnLiveMonitor, 2);
            this.btnLiveMonitor.FirstStateImage = global::SkyDean.FareLiz.WinForm.Properties.Resources.LiveMonitor;
            this.btnLiveMonitor.FirstStateText = "Live Monitor";
            this.btnLiveMonitor.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLiveMonitor.IsSecondState = false;
            this.btnLiveMonitor.Location = new System.Drawing.Point(8, 29);
            this.btnLiveMonitor.Name = "btnLiveMonitor";
            this.btnLiveMonitor.Padding = new System.Windows.Forms.Padding(0, 0, 14, 0);
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
            this.grpLiveMonitor.Controls.Add(this.tableLayoutPanel2);
            this.grpLiveMonitor.Location = new System.Drawing.Point(715, 3);
            this.grpLiveMonitor.Name = "grpLiveMonitor";
            this.grpLiveMonitor.Size = new System.Drawing.Size(146, 113);
            this.grpLiveMonitor.TabIndex = 66;
            this.grpLiveMonitor.TabStop = false;
            this.grpLiveMonitor.Text = "Live Monitor";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblPriceLimit, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.numPriceLimit, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnLiveMonitor, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnLiveFareData, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(140, 94);
            this.tableLayoutPanel2.TabIndex = 35;
            // 
            // fareBrowserTabs
            // 
            this.fareBrowserTabs.AllowDrop = true;
            this.tableLayoutPanel3.SetColumnSpan(this.fareBrowserTabs, 2);
            this.fareBrowserTabs.Cursor = System.Windows.Forms.Cursors.Default;
            this.fareBrowserTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fareBrowserTabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.fareBrowserTabs.HotTrack = true;
            this.fareBrowserTabs.IndicatorWidth = 3;
            this.fareBrowserTabs.Location = new System.Drawing.Point(1, 120);
            this.fareBrowserTabs.Margin = new System.Windows.Forms.Padding(1);
            this.fareBrowserTabs.Name = "fareBrowserTabs";
            this.fareBrowserTabs.SelectedIndex = 0;
            this.fareBrowserTabs.ShowToolTips = true;
            this.fareBrowserTabs.Size = new System.Drawing.Size(862, 404);
            this.fareBrowserTabs.TabIndex = 22;
            this.fareBrowserTabs.TabPageClosing += new System.Windows.Forms.TabControlCancelEventHandler(this.fareBrowserTabs_TabPageClosing);
            this.fareBrowserTabs.TabPageReloading += new System.Windows.Forms.TabControlEventHandler(this.fareBrowserTabs_TabPageRefreshing);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.fareBrowserTabs, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.grpDetail, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.grpLiveMonitor, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(864, 525);
            this.tableLayoutPanel3.TabIndex = 67;
            // 
            // CheckFareForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 547);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.statusStrip);
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
            this.journeyInfoTable.ResumeLayout(false);
            this.journeyInfoTable.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
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
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Label lblDuration;
        private System.Windows.Forms.Label lblPriceLimit;
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
        private System.Windows.Forms.TableLayoutPanel journeyInfoTable;
        private System.Windows.Forms.Label lblDeparture;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}