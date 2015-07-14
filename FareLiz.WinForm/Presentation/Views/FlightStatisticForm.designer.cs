using SkyDean.FareLiz.WinForm.Components.Controls.Button;
using SkyDean.FareLiz.WinForm.Components.Controls.ComboBox;
using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
using SkyDean.FareLiz.WinForm.Components.Utils;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    partial class FlightStatisticForm
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
            this.checkBoxProperties2 = new CheckBoxProperties();
            this.grpDetail = new System.Windows.Forms.GroupBox();
            this.cbDestination = new AirportComboBox();
            this.cbDeparture = new AirportComboBox();
            this.numPriceLimit = new System.Windows.Forms.NumericUpDown();
            this.numMaxDuration = new System.Windows.Forms.NumericUpDown();
            this.numMinDuration = new System.Windows.Forms.NumericUpDown();
            this.btnGetPackage = new ImageButton();
            this.progressBar = new Windows7ProgressBar();
            this.cbAirlines = new CheckBoxComboBox();
            this.btnAllReturn = new System.Windows.Forms.Button();
            this.btnGraph = new ImageButton();
            this.btnRefresh = new ImageButton();
            this.btnToCsv = new ImageButton();
            this.btnAllDepart = new System.Windows.Forms.Button();
            this.btnAllDates = new System.Windows.Forms.Button();
            this.btnNextWeek = new System.Windows.Forms.Button();
            this.btnPrevWeek = new System.Windows.Forms.Button();
            this.btnRetNext = new System.Windows.Forms.Button();
            this.btnRetPrev = new System.Windows.Forms.Button();
            this.btnDeptNext = new System.Windows.Forms.Button();
            this.btnDeptPrev = new System.Windows.Forms.Button();
            this.lblAirlines = new System.Windows.Forms.Label();
            this.numAmountOfPrices = new System.Windows.Forms.NumericUpDown();
            this.chkAllDataHistory = new System.Windows.Forms.CheckBox();
            this.lblViewMultiple = new System.Windows.Forms.Label();
            this.cbReturnDate = new System.Windows.Forms.ComboBox();
            this.lblInboundDate = new System.Windows.Forms.Label();
            this.cbDepartureDate = new System.Windows.Forms.ComboBox();
            this.lblOutboundDate = new System.Windows.Forms.Label();
            this.lvFlightData = new FlightDataListView();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDeparture = new System.Windows.Forms.Label();
            this.lblPriceLimit = new System.Windows.Forms.Label();
            this.overlayBackgroundPicture = new System.Windows.Forms.PictureBox();
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.springLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helpMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuQuickStart = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCheckForUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConfig = new ImageButton();
            this.btnHelp = new SplitButton();
            this.btnScheduler = new ImageButton();
            this.btnExit = new ImageButton();
            this.btnCheckFareFlex = new ImageButton();
            this.grpDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPriceLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAmountOfPrices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlayBackgroundPicture)).BeginInit();
            this.overlayBackgroundPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.helpMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpDetail
            // 
            this.grpDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDetail.Controls.Add(this.cbDestination);
            this.grpDetail.Controls.Add(this.cbDeparture);
            this.grpDetail.Controls.Add(this.numPriceLimit);
            this.grpDetail.Controls.Add(this.numMaxDuration);
            this.grpDetail.Controls.Add(this.numMinDuration);
            this.grpDetail.Controls.Add(this.btnGetPackage);
            this.grpDetail.Controls.Add(this.progressBar);
            this.grpDetail.Controls.Add(this.cbAirlines);
            this.grpDetail.Controls.Add(this.btnAllReturn);
            this.grpDetail.Controls.Add(this.btnGraph);
            this.grpDetail.Controls.Add(this.btnRefresh);
            this.grpDetail.Controls.Add(this.btnToCsv);
            this.grpDetail.Controls.Add(this.btnAllDepart);
            this.grpDetail.Controls.Add(this.btnAllDates);
            this.grpDetail.Controls.Add(this.btnNextWeek);
            this.grpDetail.Controls.Add(this.btnPrevWeek);
            this.grpDetail.Controls.Add(this.btnRetNext);
            this.grpDetail.Controls.Add(this.btnRetPrev);
            this.grpDetail.Controls.Add(this.btnDeptNext);
            this.grpDetail.Controls.Add(this.btnDeptPrev);
            this.grpDetail.Controls.Add(this.lblAirlines);
            this.grpDetail.Controls.Add(this.numAmountOfPrices);
            this.grpDetail.Controls.Add(this.chkAllDataHistory);
            this.grpDetail.Controls.Add(this.lblViewMultiple);
            this.grpDetail.Controls.Add(this.cbReturnDate);
            this.grpDetail.Controls.Add(this.lblInboundDate);
            this.grpDetail.Controls.Add(this.cbDepartureDate);
            this.grpDetail.Controls.Add(this.lblOutboundDate);
            this.grpDetail.Controls.Add(this.lvFlightData);
            this.grpDetail.Controls.Add(this.label2);
            this.grpDetail.Controls.Add(this.lblDeparture);
            this.grpDetail.Controls.Add(this.lblPriceLimit);
            this.grpDetail.Location = new System.Drawing.Point(12, 12);
            this.grpDetail.Name = "grpDetail";
            this.grpDetail.Size = new System.Drawing.Size(903, 397);
            this.grpDetail.TabIndex = 21;
            this.grpDetail.TabStop = false;
            this.grpDetail.Text = "Travel Information";
            // 
            // cbDestination
            // 
            this.cbDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbDestination.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbDestination.FormattingEnabled = true;
            this.cbDestination.Location = new System.Drawing.Point(73, 50);
            this.cbDestination.MaxDropDownItems = 10;
            this.cbDestination.Name = "cbDestination";
            this.cbDestination.SelectedAirport = null;
            this.cbDestination.SelectedAirportCode = null;
            this.cbDestination.Size = new System.Drawing.Size(261, 21);
            this.cbDestination.TabIndex = 1;
            this.cbDestination.ValueMember = "IATA";
            this.cbDestination.SelectedIndexChanged += new System.EventHandler(this.SelectedLocationChanged);
            // 
            // cbDeparture
            // 
            this.cbDeparture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDeparture.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbDeparture.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbDeparture.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cbDeparture.FormattingEnabled = true;
            this.cbDeparture.Location = new System.Drawing.Point(73, 22);
            this.cbDeparture.MaxDropDownItems = 10;
            this.cbDeparture.Name = "cbDeparture";
            this.cbDeparture.SelectedAirport = null;
            this.cbDeparture.SelectedAirportCode = null;
            this.cbDeparture.Size = new System.Drawing.Size(261, 21);
            this.cbDeparture.TabIndex = 0;
            this.cbDeparture.ValueMember = "IATA";
            this.cbDeparture.SelectedIndexChanged += new System.EventHandler(this.SelectedLocationChanged);
            // 
            // numPriceLimit
            // 
            this.numPriceLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numPriceLimit.Location = new System.Drawing.Point(371, 22);
            this.numPriceLimit.Maximum = decimal.MaxValue;
            this.numPriceLimit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPriceLimit.Name = "numPriceLimit";
            this.numPriceLimit.Size = new System.Drawing.Size(61, 20);
            this.numPriceLimit.TabIndex = 5;
            this.numPriceLimit.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numPriceLimit.ValueChanged += new System.EventHandler(this.FlightFilter_Changed);
            // 
            // numMaxDuration
            // 
            this.numMaxDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numMaxDuration.Location = new System.Drawing.Point(686, 22);
            this.numMaxDuration.Maximum = decimal.MaxValue;
            this.numMaxDuration.Minimum = 0;
            this.numMaxDuration.Name = "numMaxDuration";
            this.numMaxDuration.Size = new System.Drawing.Size(40, 20);
            this.numMaxDuration.TabIndex = 9;
            this.numMaxDuration.Value = new decimal(new int[] {
            42,
            0,
            0,
            0});
            this.numMaxDuration.ValueChanged += new System.EventHandler(this.FlightFilter_Changed);
            // 
            // numMinDuration
            // 
            this.numMinDuration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numMinDuration.Location = new System.Drawing.Point(630, 22);
            this.numMinDuration.Maximum = decimal.MaxValue;
            this.numMinDuration.Minimum = 0;
            this.numMinDuration.Name = "numMinDuration";
            this.numMinDuration.Size = new System.Drawing.Size(40, 20);
            this.numMinDuration.TabIndex = 8;
            this.numMinDuration.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numMinDuration.ValueChanged += new System.EventHandler(this.numMinduration_ValueChanged);
            // 
            // btnGetPackage
            // 
            this.btnGetPackage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetPackage.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.DownloadPackage;
            this.btnGetPackage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGetPackage.Location = new System.Drawing.Point(766, 46);
            this.btnGetPackage.Name = "btnGetPackage";
            this.btnGetPackage.Padding = new System.Windows.Forms.Padding(0, 0, 7, 0);
            this.btnGetPackage.Size = new System.Drawing.Size(120, 27);
            this.btnGetPackage.TabIndex = 16;
            this.btnGetPackage.Text = "Get &Packages";
            this.btnGetPackage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGetPackage.UseVisualStyleBackColor = true;
            this.btnGetPackage.Click += new System.EventHandler(this.btnGetPackage_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.ContainerControl = this;
            this.progressBar.Location = new System.Drawing.Point(19, 384);
            this.progressBar.Name = "progressBar";
            this.progressBar.ShowInTaskbar = false;
            this.progressBar.Size = new System.Drawing.Size(867, 6);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 51;
            this.progressBar.Visible = false;
            // 
            // cbAirlines
            // 
            this.cbAirlines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            checkBoxProperties2.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            checkBoxProperties2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbAirlines.CheckBoxProperties = checkBoxProperties2;
            this.cbAirlines.DisplayMemberSingleItem = "";
            this.cbAirlines.FormattingEnabled = true;
            this.cbAirlines.Location = new System.Drawing.Point(381, 50);
            this.cbAirlines.Name = "cbAirlines";
            this.cbAirlines.Size = new System.Drawing.Size(375, 21);
            this.cbAirlines.TabIndex = 6;
            this.cbAirlines.SelectedValueChanged += new System.EventHandler(this.FlightFilter_Changed);
            this.cbAirlines.TextChanged += new System.EventHandler(this.FlightFilter_Changed);
            // 
            // btnAllReturn
            // 
            this.btnAllReturn.FlatAppearance.BorderSize = 0;
            this.btnAllReturn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAllReturn.Location = new System.Drawing.Point(470, 78);
            this.btnAllReturn.Name = "btnAllReturn";
            this.btnAllReturn.Size = new System.Drawing.Size(15, 21);
            this.btnAllReturn.TabIndex = 46;
            this.btnAllReturn.Text = "*";
            this.btnAllReturn.UseVisualStyleBackColor = true;
            this.btnAllReturn.Click += new System.EventHandler(this.btnAllReturn_Click);
            // 
            // btnGraph
            // 
            this.btnGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGraph.Enabled = false;
            this.btnGraph.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Graph;
            this.btnGraph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGraph.Location = new System.Drawing.Point(609, 75);
            this.btnGraph.Name = "btnGraph";
            this.btnGraph.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
            this.btnGraph.Size = new System.Drawing.Size(93, 27);
            this.btnGraph.TabIndex = 12;
            this.btnGraph.Text = "&Graph";
            this.btnGraph.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGraph.UseVisualStyleBackColor = true;
            this.btnGraph.Click += new System.EventHandler(this.btnGraph_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Image = global::SkyDean.FareLiz.WinForm.Components.Properties.Resources.Refresh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(798, 75);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Padding = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnRefresh.Size = new System.Drawing.Size(88, 27);
            this.btnRefresh.TabIndex = 14;
            this.btnRefresh.Text = "&Reload";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.btnRefresh, "Upload data to DropBox");
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnToCsv
            // 
            this.btnToCsv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnToCsv.Enabled = false;
            this.btnToCsv.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.CSV;
            this.btnToCsv.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnToCsv.Location = new System.Drawing.Point(707, 75);
            this.btnToCsv.Name = "btnToCsv";
            this.btnToCsv.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnToCsv.Size = new System.Drawing.Size(86, 27);
            this.btnToCsv.TabIndex = 13;
            this.btnToCsv.Text = "To &CSV";
            this.btnToCsv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnToCsv.UseVisualStyleBackColor = true;
            this.btnToCsv.Click += new System.EventHandler(this.btnToCsv_Click);
            // 
            // btnAllDepart
            // 
            this.btnAllDepart.FlatAppearance.BorderSize = 0;
            this.btnAllDepart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAllDepart.Location = new System.Drawing.Point(236, 78);
            this.btnAllDepart.Name = "btnAllDepart";
            this.btnAllDepart.Size = new System.Drawing.Size(15, 21);
            this.btnAllDepart.TabIndex = 45;
            this.btnAllDepart.Text = "*";
            this.btnAllDepart.UseVisualStyleBackColor = true;
            this.btnAllDepart.Click += new System.EventHandler(this.btnAllDepart_Click);
            // 
            // btnAllDates
            // 
            this.btnAllDates.FlatAppearance.BorderSize = 0;
            this.btnAllDates.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAllDates.Location = new System.Drawing.Point(542, 78);
            this.btnAllDates.Name = "btnAllDates";
            this.btnAllDates.Size = new System.Drawing.Size(30, 21);
            this.btnAllDates.TabIndex = 44;
            this.btnAllDates.Text = "*";
            this.toolTip.SetToolTip(this.btnAllDates, "View all stored journeys");
            this.btnAllDates.UseVisualStyleBackColor = true;
            this.btnAllDates.Click += new System.EventHandler(this.btnAllDates_Click);
            // 
            // btnNextWeek
            // 
            this.btnNextWeek.FlatAppearance.BorderSize = 0;
            this.btnNextWeek.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNextWeek.Location = new System.Drawing.Point(513, 78);
            this.btnNextWeek.Name = "btnNextWeek";
            this.btnNextWeek.Size = new System.Drawing.Size(30, 21);
            this.btnNextWeek.TabIndex = 39;
            this.btnNextWeek.Text = ">>";
            this.toolTip.SetToolTip(this.btnNextWeek, "Shift journey 1 week later");
            this.btnNextWeek.UseVisualStyleBackColor = true;
            this.btnNextWeek.Click += new System.EventHandler(this.btnNextWeek_Click);
            // 
            // btnPrevWeek
            // 
            this.btnPrevWeek.FlatAppearance.BorderSize = 0;
            this.btnPrevWeek.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrevWeek.Location = new System.Drawing.Point(484, 78);
            this.btnPrevWeek.Name = "btnPrevWeek";
            this.btnPrevWeek.Size = new System.Drawing.Size(30, 21);
            this.btnPrevWeek.TabIndex = 38;
            this.btnPrevWeek.Text = "<<";
            this.toolTip.SetToolTip(this.btnPrevWeek, "Shift journey 1 week earlier");
            this.btnPrevWeek.UseVisualStyleBackColor = true;
            this.btnPrevWeek.Click += new System.EventHandler(this.btnPrevWeek_Click);
            // 
            // btnRetNext
            // 
            this.btnRetNext.FlatAppearance.BorderSize = 0;
            this.btnRetNext.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRetNext.Location = new System.Drawing.Point(456, 78);
            this.btnRetNext.Name = "btnRetNext";
            this.btnRetNext.Size = new System.Drawing.Size(15, 21);
            this.btnRetNext.TabIndex = 37;
            this.btnRetNext.Text = ">";
            this.btnRetNext.UseVisualStyleBackColor = true;
            this.btnRetNext.Click += new System.EventHandler(this.btnRetNext_Click);
            // 
            // btnRetPrev
            // 
            this.btnRetPrev.FlatAppearance.BorderSize = 0;
            this.btnRetPrev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRetPrev.Location = new System.Drawing.Point(308, 78);
            this.btnRetPrev.Name = "btnRetPrev";
            this.btnRetPrev.Size = new System.Drawing.Size(15, 21);
            this.btnRetPrev.TabIndex = 36;
            this.btnRetPrev.Text = "<";
            this.btnRetPrev.UseVisualStyleBackColor = true;
            this.btnRetPrev.Click += new System.EventHandler(this.btnRetPrev_Click);
            // 
            // btnDeptNext
            // 
            this.btnDeptNext.FlatAppearance.BorderSize = 0;
            this.btnDeptNext.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeptNext.Location = new System.Drawing.Point(222, 78);
            this.btnDeptNext.Name = "btnDeptNext";
            this.btnDeptNext.Size = new System.Drawing.Size(15, 21);
            this.btnDeptNext.TabIndex = 35;
            this.btnDeptNext.Text = ">";
            this.btnDeptNext.UseVisualStyleBackColor = true;
            this.btnDeptNext.Click += new System.EventHandler(this.btnDeptNext_Click);
            // 
            // btnDeptPrev
            // 
            this.btnDeptPrev.FlatAppearance.BorderSize = 0;
            this.btnDeptPrev.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeptPrev.Location = new System.Drawing.Point(73, 78);
            this.btnDeptPrev.Name = "btnDeptPrev";
            this.btnDeptPrev.Size = new System.Drawing.Size(15, 21);
            this.btnDeptPrev.TabIndex = 34;
            this.btnDeptPrev.Text = "<";
            this.btnDeptPrev.UseVisualStyleBackColor = true;
            this.btnDeptPrev.Click += new System.EventHandler(this.btnDeptPrev_Click);
            // 
            // lblAirlines
            // 
            this.lblAirlines.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAirlines.AutoSize = true;
            this.lblAirlines.Location = new System.Drawing.Point(340, 54);
            this.lblAirlines.Name = "lblAirlines";
            this.lblAirlines.Size = new System.Drawing.Size(43, 13);
            this.lblAirlines.TabIndex = 24;
            this.lblAirlines.Text = "Airlines:";
            // 
            // numAmountOfPrices
            // 
            this.numAmountOfPrices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numAmountOfPrices.Location = new System.Drawing.Point(467, 22);
            this.numAmountOfPrices.Maximum = decimal.MaxValue;
            this.numAmountOfPrices.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAmountOfPrices.Name = "numAmountOfPrices";
            this.numAmountOfPrices.Size = new System.Drawing.Size(49, 20);
            this.numAmountOfPrices.TabIndex = 7;
            this.numAmountOfPrices.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.numAmountOfPrices.ValueChanged += new System.EventHandler(this.FlightFilter_Changed);
            // 
            // chkAllDataHistory
            // 
            this.chkAllDataHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAllDataHistory.AutoSize = true;
            this.chkAllDataHistory.Location = new System.Drawing.Point(768, 23);
            this.chkAllDataHistory.Name = "chkAllDataHistory";
            this.chkAllDataHistory.Size = new System.Drawing.Size(114, 17);
            this.chkAllDataHistory.TabIndex = 10;
            this.chkAllDataHistory.Text = "Show Data History";
            this.chkAllDataHistory.UseVisualStyleBackColor = true;
            this.chkAllDataHistory.CheckedChanged += new System.EventHandler(this.FlightFilter_Changed);
            // 
            // lblViewMultiple
            // 
            this.lblViewMultiple.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewMultiple.AutoSize = true;
            this.lblViewMultiple.Location = new System.Drawing.Point(438, 24);
            this.lblViewMultiple.Name = "lblViewMultiple";
            this.lblViewMultiple.Size = new System.Drawing.Size(318, 13);
            this.lblViewMultiple.TabIndex = 53;
            this.lblViewMultiple.Text = "View                   cheapest flights within                 -                 " +
    "days";
            // 
            // cbReturnDate
            // 
            this.cbReturnDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReturnDate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbReturnDate.FormattingEnabled = true;
            this.cbReturnDate.Items.AddRange(new object[] {
            ""});
            this.cbReturnDate.Location = new System.Drawing.Point(324, 78);
            this.cbReturnDate.Name = "cbReturnDate";
            this.cbReturnDate.Size = new System.Drawing.Size(131, 21);
            this.cbReturnDate.TabIndex = 4;
            this.cbReturnDate.SelectedIndexChanged += new System.EventHandler(this.FlightFilter_Changed);
            // 
            // lblInboundDate
            // 
            this.lblInboundDate.AutoSize = true;
            this.lblInboundDate.Location = new System.Drawing.Point(257, 81);
            this.lblInboundDate.Name = "lblInboundDate";
            this.lblInboundDate.Size = new System.Drawing.Size(49, 13);
            this.lblInboundDate.TabIndex = 12;
            this.lblInboundDate.Text = "Inbound:";
            // 
            // cbDepartureDate
            // 
            this.cbDepartureDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDepartureDate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbDepartureDate.FormattingEnabled = true;
            this.cbDepartureDate.Items.AddRange(new object[] {
            ""});
            this.cbDepartureDate.Location = new System.Drawing.Point(89, 78);
            this.cbDepartureDate.Name = "cbDepartureDate";
            this.cbDepartureDate.Size = new System.Drawing.Size(132, 21);
            this.cbDepartureDate.TabIndex = 3;
            this.cbDepartureDate.SelectedIndexChanged += new System.EventHandler(this.FlightFilter_Changed);
            // 
            // lblOutboundDate
            // 
            this.lblOutboundDate.AutoSize = true;
            this.lblOutboundDate.Location = new System.Drawing.Point(14, 81);
            this.lblOutboundDate.Name = "lblOutboundDate";
            this.lblOutboundDate.Size = new System.Drawing.Size(57, 13);
            this.lblOutboundDate.TabIndex = 10;
            this.lblOutboundDate.Text = "Outbound:";
            // 
            // lvFlightData
            // 
            this.lvFlightData.ActiveCurrency = null;
            this.lvFlightData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFlightData.Location = new System.Drawing.Point(19, 108);
            this.lvFlightData.Name = "lvFlightData";
            this.lvFlightData.Size = new System.Drawing.Size(867, 275);
            this.lvFlightData.TabIndex = 11;
            this.lvFlightData.VirtualModeThreshold = 3000;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Destination:";
            // 
            // lblDeparture
            // 
            this.lblDeparture.AutoSize = true;
            this.lblDeparture.Location = new System.Drawing.Point(15, 25);
            this.lblDeparture.Name = "lblDeparture";
            this.lblDeparture.Size = new System.Drawing.Size(57, 13);
            this.lblDeparture.TabIndex = 5;
            this.lblDeparture.Text = "Departure:";
            // 
            // lblPriceLimit
            // 
            this.lblPriceLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPriceLimit.AutoSize = true;
            this.lblPriceLimit.Location = new System.Drawing.Point(339, 25);
            this.lblPriceLimit.Name = "lblPriceLimit";
            this.lblPriceLimit.Size = new System.Drawing.Size(31, 13);
            this.lblPriceLimit.TabIndex = 25;
            this.lblPriceLimit.Text = "Limit:";
            // 
            // overlayBackgroundPicture
            // 
            this.overlayBackgroundPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.overlayBackgroundPicture.Controls.Add(this.imgLogo);
            this.overlayBackgroundPicture.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.BackgroundPlane;
            this.overlayBackgroundPicture.Location = new System.Drawing.Point(-13, 99);
            this.overlayBackgroundPicture.Name = "overlayBackgroundPicture";
            this.overlayBackgroundPicture.Size = new System.Drawing.Size(585, 450);
            this.overlayBackgroundPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.overlayBackgroundPicture.TabIndex = 48;
            this.overlayBackgroundPicture.TabStop = false;
            // 
            // imgLogo
            // 
            this.imgLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imgLogo.BackColor = System.Drawing.Color.Transparent;
            this.imgLogo.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.SkyDean;
            this.imgLogo.Location = new System.Drawing.Point(24, 314);
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.Size = new System.Drawing.Size(150, 46);
            this.imgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgLogo.TabIndex = 22;
            this.imgLogo.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.springLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 465);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(929, 22);
            this.statusStrip.TabIndex = 16;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(26, 17);
            this.lblStatus.Text = "Idle";
            // 
            // springLabel
            // 
            this.springLabel.Name = "springLabel";
            this.springLabel.Size = new System.Drawing.Size(888, 17);
            this.springLabel.Spring = true;
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayIcon.Text = "Flight Monitor by SkyDean";
            this.trayIcon.Visible = true;
            // 
            // helpMenuStrip
            // 
            this.helpMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuQuickStart,
            this.mnuCheckForUpdate,
            this.toolStripMenuItem1,
            this.mnuAbout});
            this.helpMenuStrip.Name = "helpMenuStrip";
            this.helpMenuStrip.Size = new System.Drawing.Size(181, 76);
            // 
            // mnuQuickStart
            // 
            this.mnuQuickStart.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Help;
            this.mnuQuickStart.Name = "mnuQuickStart";
            this.mnuQuickStart.Size = new System.Drawing.Size(180, 22);
            this.mnuQuickStart.Text = "&Quick Start Guide";
            this.mnuQuickStart.Click += new System.EventHandler(this.mnuQuickStart_Click);
            // 
            // mnuCheckForUpdate
            // 
            this.mnuCheckForUpdate.Name = "mnuCheckForUpdate";
            this.mnuCheckForUpdate.Size = new System.Drawing.Size(180, 22);
            this.mnuCheckForUpdate.Text = "Check for Updates...";
            this.mnuCheckForUpdate.Click += new System.EventHandler(this.mnuCheckForUpdate_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(180, 22);
            this.mnuAbout.Text = "About";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfig.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Config;
            this.btnConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfig.Location = new System.Drawing.Point(437, 421);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Padding = new System.Windows.Forms.Padding(3, 0, 23, 0);
            this.btnConfig.Size = new System.Drawing.Size(116, 34);
            this.btnConfig.TabIndex = 15;
            this.btnConfig.Text = "&Settings";
            this.btnConfig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHelp.AutoSize = true;
            this.btnHelp.Image = null;
            this.btnHelp.Location = new System.Drawing.Point(360, 422);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(65, 33);
            this.btnHelp.SplitMenuStrip = this.helpMenuStrip;
            this.btnHelp.TabIndex = 49;
            this.btnHelp.Text = "&Help";
            this.btnHelp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnScheduler
            // 
            this.btnScheduler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScheduler.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Scheduler;
            this.btnScheduler.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnScheduler.Location = new System.Drawing.Point(691, 421);
            this.btnScheduler.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.btnScheduler.Name = "btnScheduler";
            this.btnScheduler.Padding = new System.Windows.Forms.Padding(7, 0, 19, 0);
            this.btnScheduler.Size = new System.Drawing.Size(123, 34);
            this.btnScheduler.TabIndex = 19;
            this.btnScheduler.Text = "S&cheduler";
            this.btnScheduler.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnScheduler.UseVisualStyleBackColor = true;
            this.btnScheduler.Click += new System.EventHandler(this.btnScheduler_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Exit;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(825, 421);
            this.btnExit.Name = "btnExit";
            this.btnExit.Padding = new System.Windows.Forms.Padding(7, 0, 18, 0);
            this.btnExit.Size = new System.Drawing.Size(90, 34);
            this.btnExit.TabIndex = 20;
            this.btnExit.Text = "E&xit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCheckFareFlex
            // 
            this.btnCheckFareFlex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCheckFareFlex.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCheckFareFlex.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.FareScanner;
            this.btnCheckFareFlex.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCheckFareFlex.Location = new System.Drawing.Point(556, 421);
            this.btnCheckFareFlex.Name = "btnCheckFareFlex";
            this.btnCheckFareFlex.Padding = new System.Windows.Forms.Padding(3, 0, 8, 0);
            this.btnCheckFareFlex.Size = new System.Drawing.Size(132, 34);
            this.btnCheckFareFlex.TabIndex = 18;
            this.btnCheckFareFlex.Text = "&Fare Scanner";
            this.btnCheckFareFlex.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCheckFareFlex.UseVisualStyleBackColor = true;
            this.btnCheckFareFlex.Click += new System.EventHandler(this.btnCheckFareFlex_Click);
            // 
            // FlightStatisticForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 487);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnScheduler);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.grpDetail);
            this.Controls.Add(this.btnCheckFareFlex);
            this.Controls.Add(this.overlayBackgroundPicture);
            this.Icon = global::SkyDean.FareLiz.WinForm.Properties.Resources.FareLizIcon;
            this.Name = "FlightStatisticForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Flight Fare Statistics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FlightStatisticForm_FormClosing);
            this.Shown += new System.EventHandler(this.FlightStatisticForm_Shown);
            this.grpDetail.ResumeLayout(false);
            this.grpDetail.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPriceLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAmountOfPrices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlayBackgroundPicture)).EndInit();
            this.overlayBackgroundPicture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.helpMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ImageButton btnGraph;
        private System.Windows.Forms.ComboBox cbReturnDate;
        private System.Windows.Forms.Label lblInboundDate;
        private System.Windows.Forms.ComboBox cbDepartureDate;
        private System.Windows.Forms.Label lblOutboundDate;
        private FlightDataListView lvFlightData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDeparture;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Label lblPriceLimit;
        private ImageButton btnScheduler;
        private ImageButton btnExit;
        private System.Windows.Forms.PictureBox imgLogo;
        private CheckBoxProperties checkBoxProperties2;
        private System.Windows.Forms.GroupBox grpDetail;
        private ImageButton btnGetPackage;
        private CheckBoxComboBox cbAirlines;
        private System.Windows.Forms.Button btnAllReturn;
        private System.Windows.Forms.Button btnAllDepart;
        private System.Windows.Forms.Button btnAllDates;
        private ImageButton btnRefresh;
        private System.Windows.Forms.Button btnDeptPrev;
        private ImageButton btnToCsv;
        private System.Windows.Forms.Button btnRetNext;
        private System.Windows.Forms.Button btnRetPrev;
        private System.Windows.Forms.Button btnDeptNext;
        private System.Windows.Forms.Button btnNextWeek;
        private System.Windows.Forms.Button btnPrevWeek;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private ImageButton btnConfig;
        private System.Windows.Forms.Label lblAirlines;
        private System.Windows.Forms.NumericUpDown numAmountOfPrices;
        private System.Windows.Forms.CheckBox chkAllDataHistory;
        private System.Windows.Forms.Label lblViewMultiple;
        private ImageButton btnCheckFareFlex;
        private Windows7ProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel springLabel;
        private System.Windows.Forms.PictureBox overlayBackgroundPicture;
        private System.Windows.Forms.NumericUpDown numMaxDuration;
        private System.Windows.Forms.NumericUpDown numMinDuration;
        private System.Windows.Forms.NumericUpDown numPriceLimit;
        private SplitButton btnHelp;
        private System.Windows.Forms.ContextMenuStrip helpMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuQuickStart;
        private System.Windows.Forms.ToolStripMenuItem mnuAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuCheckForUpdate;
        private AirportComboBox cbDeparture;
        private AirportComboBox cbDestination;
    }
}

