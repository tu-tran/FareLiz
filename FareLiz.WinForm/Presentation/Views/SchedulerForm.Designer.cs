using SkyDean.FareLiz.WinForm.Components.Controls.Button;
using SkyDean.FareLiz.WinForm.Components.Controls.Grid;
using SkyDean.FareLiz.WinForm.Components.Controls.ListView;
using SkyDean.FareLiz.WinForm.Components.Controls.TextBox;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    partial class SchedulerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SchedulerForm));
            this.btnClose = new System.Windows.Forms.Button();
            this.flightScannerProperties = new System.Windows.Forms.PropertyGrid();
            this.scannerSettingContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuExecute = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCopyParamsToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lvSchedules = new EnhancedListView();
            this.colScheduleItem = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddTask = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuDeleteTask = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteAll = new System.Windows.Forms.ToolStripMenuItem();
            this.imgListSchedules = new System.Windows.Forms.ImageList(this.components);
            this.overlayBackgroundPicture = new System.Windows.Forms.PictureBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblDeparture = new System.Windows.Forms.Label();
            this.grpScannerSettings = new System.Windows.Forms.GroupBox();
            this.txtDeparture = new AirportTextBox();
            this.txtDestination = new AirportTextBox();
            this.btnRefresh = new ImageButton();
            this.schedulerProperties = new ExPropertyGrid();
            this.btnExit = new ImageButton();
            this.btnSave = new ImageButton();
            this.scannerSettingContextMenu.SuspendLayout();
            this.taskContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayBackgroundPicture)).BeginInit();
            this.statusStrip.SuspendLayout();
            this.grpScannerSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(430, 495);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(85, 30);
            this.btnClose.TabIndex = 14;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // flightScannerProperties
            // 
            this.flightScannerProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flightScannerProperties.ContextMenuStrip = this.scannerSettingContextMenu;
            this.flightScannerProperties.Location = new System.Drawing.Point(9, 74);
            this.flightScannerProperties.Name = "flightScannerProperties";
            this.flightScannerProperties.Size = new System.Drawing.Size(425, 269);
            this.flightScannerProperties.TabIndex = 50;
            this.flightScannerProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.flightScannerProperties_PropertyValueChanged);
            this.flightScannerProperties.SelectedObjectsChanged += new System.EventHandler(this.flightScannerProperties_SelectedObjectsChanged);
            // 
            // scannerSettingContextMenu
            // 
            this.scannerSettingContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExecute,
            this.toolStripMenuItem1,
            this.mnuCopyParamsToClipboard});
            this.scannerSettingContextMenu.Name = "scannerSettingContextMenu";
            this.scannerSettingContextMenu.Size = new System.Drawing.Size(306, 54);
            this.scannerSettingContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.scannerSettingContextMenu_Opening);
            // 
            // mnuExecute
            // 
            this.mnuExecute.Name = "mnuExecute";
            this.mnuExecute.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.X)));
            this.mnuExecute.Size = new System.Drawing.Size(305, 22);
            this.mnuExecute.Text = "Execute";
            this.mnuExecute.Click += new System.EventHandler(this.mnuExecute_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(302, 6);
            // 
            // mnuCopyParamsToClipboard
            // 
            this.mnuCopyParamsToClipboard.Name = "mnuCopyParamsToClipboard";
            this.mnuCopyParamsToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.mnuCopyParamsToClipboard.Size = new System.Drawing.Size(305, 22);
            this.mnuCopyParamsToClipboard.Text = "Copy parameters to clipboard";
            this.mnuCopyParamsToClipboard.Click += new System.EventHandler(this.mnuCopyParamsToClipboard_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(324, 23);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(126, 13);
            this.lblInfo.TabIndex = 49;
            this.lblInfo.Text = "Scheduled Task Settings";
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.FlatAppearance.BorderSize = 0;
            this.btnDeleteAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteAll.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.DeleteAll;
            this.btnDeleteAll.Location = new System.Drawing.Point(16, 113);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(30, 46);
            this.btnDeleteAll.TabIndex = 48;
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDelete.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Delete;
            this.btnDelete.Location = new System.Drawing.Point(16, 68);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(30, 46);
            this.btnDelete.TabIndex = 47;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAdd.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Add;
            this.btnAdd.Location = new System.Drawing.Point(16, 23);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(30, 46);
            this.btnAdd.TabIndex = 46;
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lvSchedules
            // 
            this.lvSchedules.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvSchedules.CollapsibleGroupState = ListViewGroupState.Normal;
            this.lvSchedules.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colScheduleItem});
            this.lvSchedules.ContextMenuStrip = this.taskContextMenuStrip;
            this.lvSchedules.DoubleBuffering = false;
            this.lvSchedules.Filtered = true;
            this.lvSchedules.FullRowSelect = true;
            this.lvSchedules.GridLines = true;
            this.lvSchedules.HideSelection = false;
            this.lvSchedules.Location = new System.Drawing.Point(45, 23);
            this.lvSchedules.Name = "lvSchedules";
            this.lvSchedules.ShowItemToolTips = true;
            this.lvSchedules.Size = new System.Drawing.Size(269, 456);
            this.lvSchedules.SmallImageList = this.imgListSchedules;
            this.lvSchedules.TabIndex = 3;
            this.lvSchedules.UseCompatibleStateImageBehavior = false;
            this.lvSchedules.View = System.Windows.Forms.View.Details;
            this.lvSchedules.ItemsAdded += new ListViewItemsDelegate(this.lvSchedules_ItemsAdded);
            this.lvSchedules.ItemRemoved += new ListViewItemDelegate(this.lvSchedules_ItemRemoved);
            this.lvSchedules.SelectedIndexChanged += new System.EventHandler(this.lvSchedules_SelectedIndexChanged);
            // 
            // colScheduleItem
            // 
            this.colScheduleItem.Text = "Scheduled Items";
            this.colScheduleItem.Width = 299;
            // 
            // taskContextMenuStrip
            // 
            this.taskContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddTask,
            this.mnuSeparator,
            this.mnuDeleteTask,
            this.mnuDeleteAll});
            this.taskContextMenuStrip.Name = "taskContextMenuStrip";
            this.taskContextMenuStrip.Size = new System.Drawing.Size(240, 76);
            this.taskContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.taskContextMenuStrip_Opening);
            // 
            // mnuAddTask
            // 
            this.mnuAddTask.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Add;
            this.mnuAddTask.Name = "mnuAddTask";
            this.mnuAddTask.Size = new System.Drawing.Size(239, 22);
            this.mnuAddTask.Text = "Add new scheduled task";
            this.mnuAddTask.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // mnuSeparator
            // 
            this.mnuSeparator.Name = "mnuSeparator";
            this.mnuSeparator.Size = new System.Drawing.Size(236, 6);
            // 
            // mnuDeleteTask
            // 
            this.mnuDeleteTask.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Delete;
            this.mnuDeleteTask.Name = "mnuDeleteTask";
            this.mnuDeleteTask.Size = new System.Drawing.Size(239, 22);
            this.mnuDeleteTask.Text = "Delete selected scheduled tasks";
            this.mnuDeleteTask.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // mnuDeleteAll
            // 
            this.mnuDeleteAll.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.DeleteAll;
            this.mnuDeleteAll.Name = "mnuDeleteAll";
            this.mnuDeleteAll.Size = new System.Drawing.Size(239, 22);
            this.mnuDeleteAll.Text = "Delete all scheduled tasks";
            this.mnuDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // imgListSchedules
            // 
            this.imgListSchedules.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
            this.imgListSchedules.ImageSize = new System.Drawing.Size(16, 16);
            this.imgListSchedules.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // overlayBackgroundPicture
            // 
            this.overlayBackgroundPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.overlayBackgroundPicture.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.BackgroundScheduler;
            this.overlayBackgroundPicture.Location = new System.Drawing.Point(2, 326);
            this.overlayBackgroundPicture.Name = "overlayBackgroundPicture";
            this.overlayBackgroundPicture.Size = new System.Drawing.Size(256, 256);
            this.overlayBackgroundPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.overlayBackgroundPicture.TabIndex = 49;
            this.overlayBackgroundPicture.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 540);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 52;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3, 0, 5, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(26, 22);
            this.lblStatus.Text = "Idle";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblStatus.TextChanged += new System.EventHandler(this.lblStatus_TextChanged);
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.Location = new System.Drawing.Point(6, 49);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(63, 13);
            this.lblDestination.TabIndex = 57;
            this.lblDestination.Text = "Destination:";
            // 
            // lblDeparture
            // 
            this.lblDeparture.AutoSize = true;
            this.lblDeparture.Location = new System.Drawing.Point(11, 20);
            this.lblDeparture.Name = "lblDeparture";
            this.lblDeparture.Size = new System.Drawing.Size(57, 13);
            this.lblDeparture.TabIndex = 56;
            this.lblDeparture.Text = "Departure:";
            // 
            // grpScannerSettings
            // 
            this.grpScannerSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpScannerSettings.Controls.Add(this.txtDeparture);
            this.grpScannerSettings.Controls.Add(this.txtDestination);
            this.grpScannerSettings.Controls.Add(this.lblDeparture);
            this.grpScannerSettings.Controls.Add(this.lblDestination);
            this.grpScannerSettings.Controls.Add(this.flightScannerProperties);
            this.grpScannerSettings.Location = new System.Drawing.Point(324, 123);
            this.grpScannerSettings.Name = "grpScannerSettings";
            this.grpScannerSettings.Size = new System.Drawing.Size(448, 356);
            this.grpScannerSettings.TabIndex = 58;
            this.grpScannerSettings.TabStop = false;
            this.grpScannerSettings.Text = "Fare Scanner Settings";
            // 
            // txtDeparture
            // 
            this.txtDeparture.AlwaysShowSuggest = false;
            this.txtDeparture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeparture.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDeparture.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDeparture.CaseSensitive = false;
            this.txtDeparture.Enabled = false;
            this.txtDeparture.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDeparture.Location = new System.Drawing.Point(74, 17);
            this.txtDeparture.MinTypedCharacters = 1;
            this.txtDeparture.Name = "txtDeparture";
            this.txtDeparture.Size = new System.Drawing.Size(360, 20);
            this.txtDeparture.TabIndex = 54;
            this.txtDeparture.VisibleSuggestItems = 10;
            this.txtDeparture.SelectedItemChanged += new System.EventHandler(this.Location_SelectedItemChanged);
            // 
            // txtDestination
            // 
            this.txtDestination.AlwaysShowSuggest = false;
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDestination.CaseSensitive = false;
            this.txtDestination.Enabled = false;
            this.txtDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestination.Location = new System.Drawing.Point(74, 46);
            this.txtDestination.MinTypedCharacters = 1;
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(360, 20);
            this.txtDestination.TabIndex = 55;
            this.txtDestination.VisibleSuggestItems = 10;
            this.txtDestination.SelectedItemChanged += new System.EventHandler(this.Location_SelectedItemChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRefresh.Image = global::SkyDean.FareLiz.WinForm.Components.Properties.Resources.Refresh;
            this.btnRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRefresh.Location = new System.Drawing.Point(559, 495);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Padding = new System.Windows.Forms.Padding(4, 0, 13, 0);
            this.btnRefresh.Size = new System.Drawing.Size(96, 30);
            this.btnRefresh.TabIndex = 53;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // schedulerProperties
            // 
            this.schedulerProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.schedulerProperties.DescriptionAreaHeight = 6;
            this.schedulerProperties.DescriptionAreaLineCount = 0;
            this.schedulerProperties.Location = new System.Drawing.Point(324, 45);
            this.schedulerProperties.Name = "schedulerProperties";
            this.schedulerProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.schedulerProperties.Size = new System.Drawing.Size(448, 69);
            this.schedulerProperties.TabIndex = 0;
            this.schedulerProperties.ToolbarVisible = false;
            this.schedulerProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.schedulerProperties_PropertyValueChanged);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Exit;
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExit.Location = new System.Drawing.Point(324, 495);
            this.btnExit.Name = "btnExit";
            this.btnExit.Padding = new System.Windows.Forms.Padding(5, 0, 24, 0);
            this.btnExit.Size = new System.Drawing.Size(100, 30);
            this.btnExit.TabIndex = 16;
            this.btnExit.Text = "E&xit";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(661, 495);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(5, 0, 26, 0);
            this.btnSave.Size = new System.Drawing.Size(111, 30);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "&Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // SchedulerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.grpScannerSettings);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnDeleteAll);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lvSchedules);
            this.Controls.Add(this.schedulerProperties);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.overlayBackgroundPicture);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "SchedulerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Scheduler";
            this.Shown += new System.EventHandler(this.SchedulerForm_Shown);
            this.SizeChanged += new System.EventHandler(this.SchedulerForm_SizeChanged);
            this.Resize += new System.EventHandler(this.SchedulerForm_Resize);
            this.scannerSettingContextMenu.ResumeLayout(false);
            this.taskContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.overlayBackgroundPicture)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.grpScannerSettings.ResumeLayout(false);
            this.grpScannerSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private ImageButton btnSave;
        private ExPropertyGrid schedulerProperties;
        private EnhancedListView lvSchedules;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ColumnHeader colScheduleItem;
        private ImageButton btnExit;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.PropertyGrid flightScannerProperties;
        private System.Windows.Forms.ContextMenuStrip scannerSettingContextMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuCopyParamsToClipboard;
        private System.Windows.Forms.ToolStripMenuItem mnuExecute;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.PictureBox overlayBackgroundPicture;
        private System.Windows.Forms.ContextMenuStrip taskContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mnuAddTask;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteTask;
        private System.Windows.Forms.ToolStripSeparator mnuSeparator;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteAll;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private ImageButton btnRefresh;
        private AirportTextBox txtDeparture;
        private AirportTextBox txtDestination;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Label lblDeparture;
        private System.Windows.Forms.GroupBox grpScannerSettings;
        private System.Windows.Forms.ImageList imgListSchedules;

    }
}