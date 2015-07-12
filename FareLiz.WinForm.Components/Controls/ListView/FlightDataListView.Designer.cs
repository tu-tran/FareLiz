namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;

    using SkyDean.FareLiz.WinForm.Components.Controls.Custom;

    partial class FlightDataListView
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
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "[{0}] Enter lock and dispose FlightDataListView [{1}]", Thread.CurrentThread.ManagedThreadId, this.Name));
            lock (this._syncObject)
            {
                Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "[{0}] Lock obtained. Disposing FlightDataListView [{1}]", Thread.CurrentThread.ManagedThreadId, this.Name));
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lvFlightDataContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuChangeCurrency = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUseDataCurrency = new RadioToolStripMenuItem();
            this.mnuCurrencySeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuBookTrip = new System.Windows.Forms.ToolStripMenuItem();
            this.lblWaterMark = new System.Windows.Forms.Label();
            this.lvFlightData = new EnhancedListView();
            this.colDepartureAt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDepartureInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colReturn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colReturnInfo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colOperator = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPrice = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPeriod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colStayDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDataDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colAgency = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvFlightDataContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvFlightDataContextMenuStrip
            // 
            this.lvFlightDataContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRefreshToolStripMenuItem,
            this.toolStripMenuItem2,
            this.mnuChangeCurrency,
            this.mnuBookTrip});
            this.lvFlightDataContextMenuStrip.Name = "lvFlightDataContextMenuStrip";
            this.lvFlightDataContextMenuStrip.Size = new System.Drawing.Size(165, 76);
            this.lvFlightDataContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.lvFlightDataContextMenuStrip_Opening);
            // 
            // mnuRefreshToolStripMenuItem
            // 
            this.mnuRefreshToolStripMenuItem.Name = "mnuRefreshToolStripMenuItem";
            this.mnuRefreshToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.mnuRefreshToolStripMenuItem.Text = "Refresh";
            this.mnuRefreshToolStripMenuItem.Click += new System.EventHandler(this.mnuRefreshToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(161, 6);
            // 
            // mnuChangeCurrency
            // 
            this.mnuChangeCurrency.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuUseDataCurrency,
            this.mnuCurrencySeparator});
            this.mnuChangeCurrency.Name = "mnuChangeCurrency";
            this.mnuChangeCurrency.Size = new System.Drawing.Size(164, 22);
            this.mnuChangeCurrency.Text = "Change currency";
            this.mnuChangeCurrency.DropDownOpening += new System.EventHandler(this.mnuChangeCurrency_DropDownOpening);
            // 
            // mnuUseDataCurrency
            // 
            this.mnuUseDataCurrency.Checked = true;
            this.mnuUseDataCurrency.CheckOnClick = true;
            this.mnuUseDataCurrency.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuUseDataCurrency.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuUseDataCurrency.Name = "mnuUseDataCurrency";
            this.mnuUseDataCurrency.Size = new System.Drawing.Size(168, 22);
            this.mnuUseDataCurrency.Text = "Use data currency";
            this.mnuUseDataCurrency.Click += new System.EventHandler(this.changeCurrency_Click);
            // 
            // mnuCurrencySeparator
            // 
            this.mnuCurrencySeparator.Name = "mnuCurrencySeparator";
            this.mnuCurrencySeparator.Size = new System.Drawing.Size(165, 6);
            // 
            // mnuBookTrip
            // 
            this.mnuBookTrip.Name = "mnuBookTrip";
            this.mnuBookTrip.Size = new System.Drawing.Size(164, 22);
            this.mnuBookTrip.Text = "Book this trip";
            this.mnuBookTrip.Click += new System.EventHandler(this.BuyTicket_Click);
            // 
            // lblWaterMark
            // 
            this.lblWaterMark.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWaterMark.BackColor = System.Drawing.Color.White;
            this.lblWaterMark.Font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.FontFamily.Name, 10F, System.Drawing.FontStyle.Bold);
            this.lblWaterMark.Location = new System.Drawing.Point(2, 21);
            this.lblWaterMark.Name = "lblWaterMark";
            this.lblWaterMark.Padding = new System.Windows.Forms.Padding(15);
            this.lblWaterMark.Size = new System.Drawing.Size(895, 319);
            this.lblWaterMark.TabIndex = 10;
            this.lblWaterMark.Text = "Loading AirportData";
            this.lblWaterMark.Visible = false;
            // 
            // lvFlightData
            // 
            this.lvFlightData.CollapsibleGroupState = ListViewGroupState.Normal;
            this.lvFlightData.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDepartureAt,
            this.colDepartureInfo,
            this.colReturn,
            this.colReturnInfo,
            this.colOperator,
            this.colPrice,
            this.colPeriod,
            this.colStayDuration,
            this.colDataDate,
            this.colAgency});
            this.lvFlightData.ContextMenuStrip = this.lvFlightDataContextMenuStrip;
            this.lvFlightData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvFlightData.Filtered = true;
            this.lvFlightData.FullRowSelect = true;
            this.lvFlightData.GridLines = true;
            this.lvFlightData.Location = new System.Drawing.Point(0, 0);
            this.lvFlightData.Name = "lvFlightData";
            this.lvFlightData.Size = new System.Drawing.Size(900, 342);
            this.lvFlightData.TabIndex = 9;
            this.lvFlightData.UseCompatibleStateImageBehavior = false;
            this.lvFlightData.View = System.Windows.Forms.View.Details;
            this.lvFlightData.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.lvFlightData_RetrieveVirtualItem);
            this.lvFlightData.DoubleClick += new System.EventHandler(this.BuyTicket_Click);
            // 
            // colDepartureAt
            // 
            this.colDepartureAt.Text = "Departure";
            // 
            // colDepartureInfo
            // 
            this.colDepartureInfo.Text = "1st Leg";
            this.colDepartureInfo.Width = 100;
            // 
            // colReturn
            // 
            this.colReturn.Text = "Return";
            // 
            // colReturnInfo
            // 
            this.colReturnInfo.Text = "2nd Leg";
            this.colReturnInfo.Width = 100;
            // 
            // colOperator
            // 
            this.colOperator.Text = "Operator";
            this.colOperator.Width = 100;
            // 
            // colPrice
            // 
            this.colPrice.Text = "Price";
            this.colPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // colPeriod
            // 
            this.colPeriod.Text = "Travel Period";
            this.colPeriod.Width = 150;
            // 
            // colStayDuration
            // 
            this.colStayDuration.Text = "Duration";
            // 
            // colDataDate
            // 
            this.colDataDate.Text = "Data Date";
            this.colDataDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colDataDate.Width = 100;
            // 
            // colAgency
            // 
            this.colAgency.Text = "Agency";
            this.colAgency.Width = 120;
            // 
            // FlightDataListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblWaterMark);
            this.Controls.Add(this.lvFlightData);
            this.Name = "FlightDataListView";
            this.Size = new System.Drawing.Size(900, 342);
            this.lvFlightDataContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader colDepartureAt;
        private System.Windows.Forms.ColumnHeader colDepartureInfo;
        private System.Windows.Forms.ColumnHeader colReturn;
        private System.Windows.Forms.ColumnHeader colReturnInfo;
        private System.Windows.Forms.ColumnHeader colOperator;
        private System.Windows.Forms.ColumnHeader colPrice;
        private System.Windows.Forms.ColumnHeader colDataDate;
        private System.Windows.Forms.ColumnHeader colPeriod;
        private System.Windows.Forms.ColumnHeader colStayDuration;
        private System.Windows.Forms.ToolStripMenuItem mnuChangeCurrency;
        private System.Windows.Forms.ContextMenuStrip lvFlightDataContextMenuStrip;
        private RadioToolStripMenuItem mnuUseDataCurrency;
        private System.Windows.Forms.ToolStripSeparator mnuCurrencySeparator;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuRefreshToolStripMenuItem;
        private EnhancedListView lvFlightData;
        private System.Windows.Forms.Label lblWaterMark;
        private System.Windows.Forms.ColumnHeader colAgency;
        private System.Windows.Forms.ToolStripMenuItem mnuBookTrip;
    }
}
