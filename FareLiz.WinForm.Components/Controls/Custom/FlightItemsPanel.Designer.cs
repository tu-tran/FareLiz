namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System.Windows.Forms;

    partial class FlightItemsPanel
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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            if (this._borderPen != null)
                this._borderPen.Dispose();

            if (this._priceFont != null)
                this._priceFont.Dispose();

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblHeader = new System.Windows.Forms.Label();
            this.gvFlightItems = new System.Windows.Forms.DataGridView();
            this.colIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.colOperator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFirstPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFirstPriceCurrency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.colSecondPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSecondPriceCurrency = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolTip = new PopupMessageToolTip();
            ((System.ComponentModel.ISupportInitialize)(this.gvFlightItems)).BeginInit();
            this.SuspendLayout();
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.Location = new System.Drawing.Point(0, 0);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Padding = new System.Windows.Forms.Padding(3, 3, 0, 10);
            this.lblHeader.Size = new System.Drawing.Size(3, 26);
            this.lblHeader.TabIndex = 1;
            // 
            // gvFlightItems
            // 
            this.gvFlightItems.AllowUserToAddRows = false;
            this.gvFlightItems.AllowUserToDeleteRows = false;
            this.gvFlightItems.AllowUserToResizeColumns = false;
            this.gvFlightItems.AllowUserToResizeRows = false;
            this.gvFlightItems.BackgroundColor = System.Drawing.Color.White;
            this.gvFlightItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gvFlightItems.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gvFlightItems.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gvFlightItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gvFlightItems.ColumnHeadersVisible = false;
            this.gvFlightItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIcon,
            this.colOperator,
            this.colDuration,
            this.colFirstPrice,
            this.colFirstPriceCurrency,
            this.colStatus,
            this.colSecondPrice,
            this.colSecondPriceCurrency});
            this.gvFlightItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvFlightItems.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gvFlightItems.Location = new System.Drawing.Point(0, 26);
            this.gvFlightItems.MultiSelect = false;
            this.gvFlightItems.Name = "gvFlightItems";
            this.gvFlightItems.ReadOnly = true;
            this.gvFlightItems.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.gvFlightItems.RowHeadersVisible = false;
            this.gvFlightItems.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.gvFlightItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvFlightItems.ShowCellErrors = false;
            this.gvFlightItems.ShowEditingIcon = false;
            this.gvFlightItems.ShowRowErrors = false;
            this.gvFlightItems.Size = new System.Drawing.Size(319, 112);
            this.gvFlightItems.StandardTab = true;
            this.gvFlightItems.TabIndex = 2;
            this.gvFlightItems.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvFlightItems_CellDoubleClick);
            this.gvFlightItems.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvFlightItems_CellMouseEnter);
            this.gvFlightItems.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvFlightItems_CellMouseLeave);
            this.gvFlightItems.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gvFlightItems_CellPainting);
            this.gvFlightItems.SelectionChanged += new System.EventHandler(this.gvFlightItems_SelectionChanged);
            this.gvFlightItems.MouseEnter += new System.EventHandler(this.gvFlightItems_MouseEnter);
            this.gvFlightItems.MouseLeave += new System.EventHandler(this.gvFlightItems_MouseLeave);
            // 
            // colIcon
            // 
            this.colIcon.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.NullValue = null;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.colIcon.DefaultCellStyle = dataGridViewCellStyle1;
            this.colIcon.HeaderText = "Icon";
            this.colIcon.MinimumWidth = 24;
            this.colIcon.Name = "colIcon";
            this.colIcon.ReadOnly = true;
            // 
            // colOperator
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colOperator.DefaultCellStyle = dataGridViewCellStyle2;
            this.colOperator.HeaderText = "Operator";
            this.colOperator.Name = "colOperator";
            this.colOperator.ReadOnly = true;
            this.colOperator.Width = 5;
            // 
            // colDuration
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.colDuration.DefaultCellStyle = dataGridViewCellStyle3;
            this.colDuration.HeaderText = "Duration";
            this.colDuration.Name = "colDuration";
            this.colDuration.ReadOnly = true;
            this.colDuration.Width = 5;
            // 
            // colFirstPrice
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colFirstPrice.DefaultCellStyle = dataGridViewCellStyle4;
            this.colFirstPrice.HeaderText = "FirstPrice";
            this.colFirstPrice.Name = "colFirstPrice";
            this.colFirstPrice.ReadOnly = true;
            this.colFirstPrice.Width = 5;
            // 
            // colFirstPriceCurrency
            // 
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.colFirstPriceCurrency.DefaultCellStyle = dataGridViewCellStyle5;
            this.colFirstPriceCurrency.HeaderText = "";
            this.colFirstPriceCurrency.Name = "colFirstPriceCurrency";
            this.colFirstPriceCurrency.ReadOnly = true;
            // 
            // colStatus
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.NullValue = null;
            this.colStatus.DefaultCellStyle = dataGridViewCellStyle6;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 5;
            // 
            // colSecondPrice
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colSecondPrice.DefaultCellStyle = dataGridViewCellStyle7;
            this.colSecondPrice.HeaderText = "SecondPrice";
            this.colSecondPrice.Name = "colSecondPrice";
            this.colSecondPrice.ReadOnly = true;
            this.colSecondPrice.Width = 5;
            // 
            // colSecondPriceCurrency
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(67)))), ((int)(((byte)(67)))));
            this.colSecondPriceCurrency.DefaultCellStyle = dataGridViewCellStyle8;
            this.colSecondPriceCurrency.HeaderText = "";
            this.colSecondPriceCurrency.Name = "colSecondPriceCurrency";
            this.colSecondPriceCurrency.ReadOnly = true;
            // 
            // toolTip
            // 
            this.toolTip.BackColor = System.Drawing.Color.DimGray;
            this.toolTip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolTip.ForeColor = System.Drawing.Color.White;
            this.toolTip.OwnerDraw = true;
            this.toolTip.Padding = new System.Windows.Forms.Padding(5);
            this.toolTip.ShowAlways = true;
            // 
            // FlightItemsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.gvFlightItems);
            this.Controls.Add(this.lblHeader);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "FlightItemsPanel";
            this.Size = new System.Drawing.Size(319, 138);
            ((System.ComponentModel.ISupportInitialize)(this.gvFlightItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private DataGridView gvFlightItems;
        private PopupMessageToolTip toolTip;
        private DataGridViewImageColumn colIcon;
        private DataGridViewTextBoxColumn colOperator;
        private DataGridViewTextBoxColumn colDuration;
        private DataGridViewTextBoxColumn colFirstPrice;
        private DataGridViewTextBoxColumn colFirstPriceCurrency;
        private DataGridViewImageColumn colStatus;
        private DataGridViewTextBoxColumn colSecondPrice;
        private DataGridViewTextBoxColumn colSecondPriceCurrency;
    }
}
