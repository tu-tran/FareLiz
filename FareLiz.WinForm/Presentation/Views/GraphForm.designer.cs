using SkyDean.FareLiz.WinForm.Components.Controls.ListView;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using ZedGraph;

    partial class GraphForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GraphForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pnlDataDate = new System.Windows.Forms.Panel();
            this.lvDataDate = new EnhancedListView();
            this.colDataDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnNoDataDate = new System.Windows.Forms.Button();
            this.btnAllDataDate = new System.Windows.Forms.Button();
            this.pnlTravelPeriod = new System.Windows.Forms.Panel();
            this.lvTravelPeriod = new EnhancedListView();
            this.colTravePeriod = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnNoTravelPeriod = new System.Windows.Forms.Button();
            this.btnAllTravelPeriod = new System.Windows.Forms.Button();
            this.lblSeparator = new System.Windows.Forms.Label();
            this.graph = new ZedGraphControl();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.pnlDataDate.SuspendLayout();
            this.pnlTravelPeriod.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pnlDataDate);
            this.splitContainer1.Panel1.Controls.Add(this.pnlTravelPeriod);
            this.splitContainer1.Panel1.Controls.Add(this.lblSeparator);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.graph);
            this.splitContainer1.Size = new System.Drawing.Size(1008, 536);
            this.splitContainer1.SplitterDistance = 335;
            this.splitContainer1.TabIndex = 2;
            // 
            // pnlDataDate
            // 
            this.pnlDataDate.Controls.Add(this.lvDataDate);
            this.pnlDataDate.Controls.Add(this.btnNoDataDate);
            this.pnlDataDate.Controls.Add(this.btnAllDataDate);
            this.pnlDataDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDataDate.Location = new System.Drawing.Point(0, 301);
            this.pnlDataDate.Margin = new System.Windows.Forms.Padding(0);
            this.pnlDataDate.Name = "pnlDataDate";
            this.pnlDataDate.Padding = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.pnlDataDate.Size = new System.Drawing.Size(335, 235);
            this.pnlDataDate.TabIndex = 12;
            // 
            // lvDataDate
            // 
            this.lvDataDate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDataDate.CheckBoxes = true;
            this.lvDataDate.CollapsibleGroupState = ListViewGroupState.Normal;
            this.lvDataDate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDataDate});
            this.lvDataDate.DoubleBuffering = true;
            this.lvDataDate.Filtered = true;
            this.lvDataDate.FullRowSelect = true;
            this.lvDataDate.Location = new System.Drawing.Point(10, 0);
            this.lvDataDate.Name = "lvDataDate";
            this.lvDataDate.Size = new System.Drawing.Size(315, 194);
            this.lvDataDate.TabIndex = 13;
            this.lvDataDate.UseCompatibleStateImageBehavior = false;
            this.lvDataDate.View = System.Windows.Forms.View.Details;
            // 
            // colDataDate
            // 
            this.colDataDate.Text = "Data Date";
            this.colDataDate.Width = 301;
            // 
            // btnNoDataDate
            // 
            this.btnNoDataDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNoDataDate.Location = new System.Drawing.Point(148, 197);
            this.btnNoDataDate.Name = "btnNoDataDate";
            this.btnNoDataDate.Size = new System.Drawing.Size(87, 27);
            this.btnNoDataDate.TabIndex = 12;
            this.btnNoDataDate.Text = "Select &None";
            this.btnNoDataDate.UseVisualStyleBackColor = true;
            this.btnNoDataDate.Click += new System.EventHandler(this.btnNoDataDate_Click);
            // 
            // btnAllDataDate
            // 
            this.btnAllDataDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAllDataDate.Location = new System.Drawing.Point(238, 197);
            this.btnAllDataDate.Name = "btnAllDataDate";
            this.btnAllDataDate.Size = new System.Drawing.Size(87, 27);
            this.btnAllDataDate.TabIndex = 11;
            this.btnAllDataDate.Text = "Select &All";
            this.btnAllDataDate.UseVisualStyleBackColor = true;
            this.btnAllDataDate.Click += new System.EventHandler(this.btnAllDataDate_Click);
            // 
            // pnlTravelPeriod
            // 
            this.pnlTravelPeriod.Controls.Add(this.lvTravelPeriod);
            this.pnlTravelPeriod.Controls.Add(this.btnNoTravelPeriod);
            this.pnlTravelPeriod.Controls.Add(this.btnAllTravelPeriod);
            this.pnlTravelPeriod.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTravelPeriod.Location = new System.Drawing.Point(0, 13);
            this.pnlTravelPeriod.Margin = new System.Windows.Forms.Padding(0);
            this.pnlTravelPeriod.Name = "pnlTravelPeriod";
            this.pnlTravelPeriod.Padding = new System.Windows.Forms.Padding(10, 0, 10, 10);
            this.pnlTravelPeriod.Size = new System.Drawing.Size(335, 288);
            this.pnlTravelPeriod.TabIndex = 11;
            // 
            // lvTravelPeriod
            // 
            this.lvTravelPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTravelPeriod.CheckBoxes = true;
            this.lvTravelPeriod.CollapsibleGroupState = ListViewGroupState.Normal;
            this.lvTravelPeriod.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colTravePeriod});
            this.lvTravelPeriod.DoubleBuffering = true;
            this.lvTravelPeriod.Filtered = true;
            this.lvTravelPeriod.FullRowSelect = true;
            this.lvTravelPeriod.Location = new System.Drawing.Point(10, 0);
            this.lvTravelPeriod.Name = "lvTravelPeriod";
            this.lvTravelPeriod.Size = new System.Drawing.Size(315, 247);
            this.lvTravelPeriod.TabIndex = 13;
            this.lvTravelPeriod.UseCompatibleStateImageBehavior = false;
            this.lvTravelPeriod.View = System.Windows.Forms.View.Details;
            // 
            // colTravePeriod
            // 
            this.colTravePeriod.Text = "Travel Period";
            this.colTravePeriod.Width = 301;
            // 
            // btnNoTravelPeriod
            // 
            this.btnNoTravelPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNoTravelPeriod.Location = new System.Drawing.Point(148, 250);
            this.btnNoTravelPeriod.Name = "btnNoTravelPeriod";
            this.btnNoTravelPeriod.Size = new System.Drawing.Size(87, 27);
            this.btnNoTravelPeriod.TabIndex = 12;
            this.btnNoTravelPeriod.Text = "Select &None";
            this.btnNoTravelPeriod.UseVisualStyleBackColor = true;
            this.btnNoTravelPeriod.Click += new System.EventHandler(this.btnNoTravelPeriod_Click);
            // 
            // btnAllTravelPeriod
            // 
            this.btnAllTravelPeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAllTravelPeriod.Location = new System.Drawing.Point(238, 250);
            this.btnAllTravelPeriod.Name = "btnAllTravelPeriod";
            this.btnAllTravelPeriod.Size = new System.Drawing.Size(87, 27);
            this.btnAllTravelPeriod.TabIndex = 11;
            this.btnAllTravelPeriod.Text = "Select &All";
            this.btnAllTravelPeriod.UseVisualStyleBackColor = true;
            this.btnAllTravelPeriod.Click += new System.EventHandler(this.btnAllTravelPeriod_Click);
            // 
            // lblSeparator
            // 
            this.lblSeparator.AutoSize = true;
            this.lblSeparator.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSeparator.Location = new System.Drawing.Point(0, 0);
            this.lblSeparator.Name = "lblSeparator";
            this.lblSeparator.Size = new System.Drawing.Size(0, 13);
            this.lblSeparator.TabIndex = 1;
            // 
            // graph
            // 
            this.graph.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graph.IsShowCopyMessage = false;
            this.graph.IsShowPointValues = true;
            this.graph.Location = new System.Drawing.Point(0, 0);
            this.graph.Name = "graph";
            this.graph.ScrollGrace = 0D;
            this.graph.ScrollMaxX = 0D;
            this.graph.ScrollMaxY = 0D;
            this.graph.ScrollMaxY2 = 0D;
            this.graph.ScrollMinX = 0D;
            this.graph.ScrollMinY = 0D;
            this.graph.ScrollMinY2 = 0D;
            this.graph.Size = new System.Drawing.Size(669, 536);
            this.graph.TabIndex = 1;
            this.graph.PointValueEvent += new ZedGraphControl.PointValueHandler(this.graph_PointValueEvent);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 539);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1008, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(26, 17);
            this.lblStatus.Text = "Idle";
            // 
            // GraphForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GraphForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Flight Fare Graph";
            this.Shown += new System.EventHandler(this.GraphForm_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.pnlDataDate.ResumeLayout(false);
            this.pnlTravelPeriod.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ZedGraphControl graph;
        private System.Windows.Forms.Label lblSeparator;
        private System.Windows.Forms.Panel pnlTravelPeriod;
        private EnhancedListView lvTravelPeriod;
        private System.Windows.Forms.Button btnNoTravelPeriod;
        private System.Windows.Forms.Button btnAllTravelPeriod;
        private System.Windows.Forms.Panel pnlDataDate;
        private EnhancedListView lvDataDate;
        private System.Windows.Forms.Button btnNoDataDate;
        private System.Windows.Forms.Button btnAllDataDate;
        private System.Windows.Forms.ColumnHeader colDataDate;
        private System.Windows.Forms.ColumnHeader colTravePeriod;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;

    }
}

