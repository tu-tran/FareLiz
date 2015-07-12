using SkyDean.FareLiz.WinForm.Components.Controls.ListView;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    partial class LiveFareDataForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LiveFareDataForm));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOperator = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOutbound = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colInbound = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvFareDataContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lbFlightDate = new System.Windows.Forms.ListBox();
            this.btnRefreshDataDate = new System.Windows.Forms.Button();
            this.lbRoute = new System.Windows.Forms.ListBox();
            this.lblTravelDate = new System.Windows.Forms.Label();
            this.grpJourney = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbDataPeriod = new System.Windows.Forms.ComboBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnPrevDataDate = new System.Windows.Forms.Button();
            this.btnNextDataDate = new System.Windows.Forms.Button();
            this.grpData = new System.Windows.Forms.GroupBox();
            this.lvFareData = new CsvListView();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.grpJourney.SuspendLayout();
            this.grpData.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // colDate
            // 
            this.colDate.Text = "Data Date";
            this.colDate.Width = 193;
            // 
            // colPrice
            // 
            this.colPrice.Text = "Price";
            this.colPrice.Width = 70;
            // 
            // colOperator
            // 
            this.colOperator.Text = "Operator";
            this.colOperator.Width = 170;
            // 
            // colOutbound
            // 
            this.colOutbound.Text = "Outbound";
            this.colOutbound.Width = 170;
            // 
            // colInbound
            // 
            this.colInbound.Text = "Inbound";
            this.colInbound.Width = 170;
            // 
            // lvFareDataContextMenuStrip
            // 
            this.lvFareDataContextMenuStrip.Name = "lvFareDataContextMenuStrip";
            this.lvFareDataContextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // lbFlightDate
            // 
            this.lbFlightDate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFlightDate.FormattingEnabled = true;
            this.lbFlightDate.HorizontalScrollbar = true;
            this.lbFlightDate.Location = new System.Drawing.Point(16, 230);
            this.lbFlightDate.Name = "lbFlightDate";
            this.lbFlightDate.Size = new System.Drawing.Size(297, 225);
            this.lbFlightDate.TabIndex = 2;
            this.lbFlightDate.SelectedIndexChanged += new System.EventHandler(this.lbFlightDate_SelectedIndexChanged);
            // 
            // btnRefreshDataDate
            // 
            this.btnRefreshDataDate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshDataDate.Location = new System.Drawing.Point(16, 455);
            this.btnRefreshDataDate.Name = "btnRefreshDataDate";
            this.btnRefreshDataDate.Size = new System.Drawing.Size(297, 27);
            this.btnRefreshDataDate.TabIndex = 38;
            this.btnRefreshDataDate.Text = "&Refresh";
            this.btnRefreshDataDate.UseVisualStyleBackColor = true;
            this.btnRefreshDataDate.Click += new System.EventHandler(this.btnRefreshDataDate_Click);
            // 
            // lbRoute
            // 
            this.lbRoute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRoute.FormattingEnabled = true;
            this.lbRoute.HorizontalScrollbar = true;
            this.lbRoute.Location = new System.Drawing.Point(16, 26);
            this.lbRoute.Name = "lbRoute";
            this.lbRoute.Size = new System.Drawing.Size(297, 173);
            this.lbRoute.TabIndex = 39;
            this.lbRoute.SelectedIndexChanged += new System.EventHandler(this.lbRoute_SelectedIndexChanged);
            // 
            // lblTravelDate
            // 
            this.lblTravelDate.AutoSize = true;
            this.lblTravelDate.Location = new System.Drawing.Point(13, 209);
            this.lblTravelDate.Name = "lblTravelDate";
            this.lblTravelDate.Size = new System.Drawing.Size(63, 13);
            this.lblTravelDate.TabIndex = 40;
            this.lblTravelDate.Text = "Travel Date";
            // 
            // grpJourney
            // 
            this.grpJourney.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpJourney.Controls.Add(this.lblTravelDate);
            this.grpJourney.Controls.Add(this.lbRoute);
            this.grpJourney.Controls.Add(this.btnRefreshDataDate);
            this.grpJourney.Controls.Add(this.lbFlightDate);
            this.grpJourney.Location = new System.Drawing.Point(12, 7);
            this.grpJourney.Name = "grpJourney";
            this.grpJourney.Size = new System.Drawing.Size(329, 493);
            this.grpJourney.TabIndex = 0;
            this.grpJourney.TabStop = false;
            this.grpJourney.Text = "Journey Details";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Period";
            // 
            // cbDataPeriod
            // 
            this.cbDataPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDataPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataPeriod.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbDataPeriod.FormattingEnabled = true;
            this.cbDataPeriod.Location = new System.Drawing.Point(75, 23);
            this.cbDataPeriod.Name = "cbDataPeriod";
            this.cbDataPeriod.Size = new System.Drawing.Size(137, 21);
            this.cbDataPeriod.TabIndex = 2;
            this.cbDataPeriod.SelectedIndexChanged += new System.EventHandler(this.cbDataPeriod_SelectedIndexChanged);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(321, 19);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(85, 29);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(235, 19);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 29);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnPrevDataDate
            // 
            this.btnPrevDataDate.FlatAppearance.BorderSize = 0;
            this.btnPrevDataDate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrevDataDate.Location = new System.Drawing.Point(60, 23);
            this.btnPrevDataDate.Name = "btnPrevDataDate";
            this.btnPrevDataDate.Size = new System.Drawing.Size(15, 21);
            this.btnPrevDataDate.TabIndex = 36;
            this.btnPrevDataDate.Text = "<";
            this.btnPrevDataDate.UseVisualStyleBackColor = true;
            this.btnPrevDataDate.Click += new System.EventHandler(this.btnPrevDataDate_Click);
            // 
            // btnNextDataDate
            // 
            this.btnNextDataDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextDataDate.FlatAppearance.BorderSize = 0;
            this.btnNextDataDate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNextDataDate.Location = new System.Drawing.Point(212, 23);
            this.btnNextDataDate.Name = "btnNextDataDate";
            this.btnNextDataDate.Size = new System.Drawing.Size(15, 21);
            this.btnNextDataDate.TabIndex = 37;
            this.btnNextDataDate.Text = ">";
            this.btnNextDataDate.UseVisualStyleBackColor = true;
            this.btnNextDataDate.Click += new System.EventHandler(this.btnNextDataDate_Click);
            // 
            // grpData
            // 
            this.grpData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpData.Controls.Add(this.btnNextDataDate);
            this.grpData.Controls.Add(this.btnPrevDataDate);
            this.grpData.Controls.Add(this.btnRefresh);
            this.grpData.Controls.Add(this.btnClose);
            this.grpData.Controls.Add(this.cbDataPeriod);
            this.grpData.Controls.Add(this.label1);
            this.grpData.Controls.Add(this.lvFareData);
            this.grpData.Location = new System.Drawing.Point(3, 7);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(421, 493);
            this.grpData.TabIndex = 2;
            this.grpData.TabStop = false;
            this.grpData.Text = "Fare AirportData";
            // 
            // lvFareData
            // 
            this.lvFareData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvFareData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDate,
            this.colPrice,
            this.colOperator,
            this.colOutbound,
            this.colInbound});
            this.lvFareData.ContextMenuStrip = this.lvFareDataContextMenuStrip;
            this.lvFareData.DoubleBuffering = true;
            this.lvFareData.Filtered = true;
            this.lvFareData.FullRowSelect = true;
            this.lvFareData.Location = new System.Drawing.Point(18, 54);
            this.lvFareData.Name = "lvFareData";
            this.lvFareData.Size = new System.Drawing.Size(388, 428);
            this.lvFareData.TabIndex = 0;
            this.lvFareData.UseCompatibleStateImageBehavior = false;
            this.lvFareData.View = System.Windows.Forms.View.Details;
            this.lvFareData.VirtualMode = true;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.grpJourney);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.grpData);
            this.splitContainer.Size = new System.Drawing.Size(784, 512);
            this.splitContainer.SplitterDistance = 344;
            this.splitContainer.TabIndex = 3;
            // 
            // LiveFareDataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(784, 512);
            this.Controls.Add(this.splitContainer);
            this.Font = System.Drawing.SystemFonts.DefaultFont;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(800, 550);
            this.Name = "LiveFareDataForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LiveFare Data Browser";
            this.Shown += new System.EventHandler(this.LiveFareDataForm_Shown);
            this.SizeChanged += new System.EventHandler(this.LiveFareDataForm_SizeChanged);
            this.grpJourney.ResumeLayout(false);
            this.grpJourney.PerformLayout();
            this.grpData.ResumeLayout(false);
            this.grpData.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip lvFareDataContextMenuStrip;
        private System.Windows.Forms.ListBox lbFlightDate;
        private System.Windows.Forms.Button btnRefreshDataDate;
        private System.Windows.Forms.ListBox lbRoute;
        private System.Windows.Forms.Label lblTravelDate;
        private System.Windows.Forms.GroupBox grpJourney;
        private CsvListView lvFareData;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.ColumnHeader colPrice;
        private System.Windows.Forms.ColumnHeader colOperator;
        private System.Windows.Forms.ColumnHeader colOutbound;
        private System.Windows.Forms.ColumnHeader colInbound;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbDataPeriod;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnPrevDataDate;
        private System.Windows.Forms.Button btnNextDataDate;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.SplitContainer splitContainer;
    }
}