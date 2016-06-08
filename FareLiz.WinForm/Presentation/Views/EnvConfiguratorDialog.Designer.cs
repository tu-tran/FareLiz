using SkyDean.FareLiz.WinForm.Components.Controls.Button;
using SkyDean.FareLiz.WinForm.Components.Controls.TextBox;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    partial class EnvConfiguratorDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EnvConfiguratorDialog));
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnResetDbSyncer = new System.Windows.Forms.Button();
            this.btnResetFareDatabase = new System.Windows.Forms.Button();
            this.btnResetFareProvider = new System.Windows.Forms.Button();
            this.btnResetArchiveManager = new System.Windows.Forms.Button();
            this.imgSyncerStatus = new System.Windows.Forms.PictureBox();
            this.imgDatabaseStatus = new System.Windows.Forms.PictureBox();
            this.imgHandlerStatus = new System.Windows.Forms.PictureBox();
            this.imgArchiveStatus = new System.Windows.Forms.PictureBox();
            this.btnInfoDbSyncer = new System.Windows.Forms.Button();
            this.btnInfoFareDatabase = new System.Windows.Forms.Button();
            this.btnInfoFareProvider = new System.Windows.Forms.Button();
            this.btnInfoArchiveManager = new System.Windows.Forms.Button();
            this.btnConfigDbSyncer = new System.Windows.Forms.Button();
            this.btnConfigFareDatabase = new System.Windows.Forms.Button();
            this.btnConfigFareProvider = new System.Windows.Forms.Button();
            this.btnConfigArchiveManager = new System.Windows.Forms.Button();
            this.cbDbSyncer = new System.Windows.Forms.ComboBox();
            this.cbFareDatabase = new System.Windows.Forms.ComboBox();
            this.cbFareDataProvider = new System.Windows.Forms.ComboBox();
            this.configTypeName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbArchiveManager = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnImportToDatabase = new ImageButton();
            this.btnDbStat = new System.Windows.Forms.Button();
            this.btnResetDatabase = new System.Windows.Forms.Button();
            this.btnRepairDatabase = new System.Windows.Forms.Button();
            this.btnExportDatabase = new SplitButton();
            this.exportDbMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exportDbXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDbBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpDatabaseSync = new System.Windows.Forms.GroupBox();
            this.btnBackupSyncDb = new ImageButton();
            this.btnRestoreSyncDb = new ImageButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnDefaultCurrency = new System.Windows.Forms.Button();
            this.lstCurrency = new System.Windows.Forms.CheckedListBox();
            this.btnSelectNoneCurrency = new System.Windows.Forms.Button();
            this.btnSelectAllCurrency = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnlContent = new System.Windows.Forms.Panel();
            this.btnExportConfig = new ImageButton();
            this.btnImportConfig = new ImageButton();
            this.grpDefaultSetting = new System.Windows.Forms.GroupBox();
            this.txtDeparture = new AirportTextBox();
            this.txtDestination = new AirportTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSave = new ImageButton();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblInstruction = new System.Windows.Forms.Label();
            this.overlayBackgroundPicture = new System.Windows.Forms.PictureBox();
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgSyncerStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgDatabaseStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHandlerStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgArchiveStatus)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.exportDbMenuStrip.SuspendLayout();
            this.grpDatabaseSync.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.grpDefaultSetting.SuspendLayout();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayBackgroundPicture)).BeginInit();
            this.overlayBackgroundPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(581, 404);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 31);
            this.btnCancel.TabIndex = 51;
            this.btnCancel.Text = "Ca&ncel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnResetDbSyncer);
            this.groupBox1.Controls.Add(this.btnResetFareDatabase);
            this.groupBox1.Controls.Add(this.btnResetFareProvider);
            this.groupBox1.Controls.Add(this.btnResetArchiveManager);
            this.groupBox1.Controls.Add(this.imgSyncerStatus);
            this.groupBox1.Controls.Add(this.imgDatabaseStatus);
            this.groupBox1.Controls.Add(this.imgHandlerStatus);
            this.groupBox1.Controls.Add(this.imgArchiveStatus);
            this.groupBox1.Controls.Add(this.btnInfoDbSyncer);
            this.groupBox1.Controls.Add(this.btnInfoFareDatabase);
            this.groupBox1.Controls.Add(this.btnInfoFareProvider);
            this.groupBox1.Controls.Add(this.btnInfoArchiveManager);
            this.groupBox1.Controls.Add(this.btnConfigDbSyncer);
            this.groupBox1.Controls.Add(this.btnConfigFareDatabase);
            this.groupBox1.Controls.Add(this.btnConfigFareProvider);
            this.groupBox1.Controls.Add(this.btnConfigArchiveManager);
            this.groupBox1.Controls.Add(this.cbDbSyncer);
            this.groupBox1.Controls.Add(this.cbFareDatabase);
            this.groupBox1.Controls.Add(this.cbFareDataProvider);
            this.groupBox1.Controls.Add(this.configTypeName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbArchiveManager);
            this.groupBox1.Location = new System.Drawing.Point(15, 100);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(677, 138);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Plugin Select";
            // 
            // btnResetDbSyncer
            // 
            this.btnResetDbSyncer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetDbSyncer.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnResetDbSyncer.BackgroundImage")));
            this.btnResetDbSyncer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnResetDbSyncer.Location = new System.Drawing.Point(544, 73);
            this.btnResetDbSyncer.Name = "btnResetDbSyncer";
            this.btnResetDbSyncer.Size = new System.Drawing.Size(22, 22);
            this.btnResetDbSyncer.TabIndex = 67;
            this.toolTip.SetToolTip(this.btnResetDbSyncer, "Reset the configuration for selected plugin");
            this.btnResetDbSyncer.UseVisualStyleBackColor = true;
            this.btnResetDbSyncer.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnResetFareDatabase
            // 
            this.btnResetFareDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetFareDatabase.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnResetFareDatabase.BackgroundImage")));
            this.btnResetFareDatabase.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnResetFareDatabase.Location = new System.Drawing.Point(544, 46);
            this.btnResetFareDatabase.Name = "btnResetFareDatabase";
            this.btnResetFareDatabase.Size = new System.Drawing.Size(22, 22);
            this.btnResetFareDatabase.TabIndex = 66;
            this.toolTip.SetToolTip(this.btnResetFareDatabase, "Reset the configuration for selected plugin");
            this.btnResetFareDatabase.UseVisualStyleBackColor = true;
            this.btnResetFareDatabase.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnResetFareProvider
            // 
            this.btnResetFareProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetFareProvider.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnResetFareProvider.BackgroundImage")));
            this.btnResetFareProvider.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnResetFareProvider.Location = new System.Drawing.Point(543, 19);
            this.btnResetFareProvider.Name = "btnResetFareProvider";
            this.btnResetFareProvider.Size = new System.Drawing.Size(22, 22);
            this.btnResetFareProvider.TabIndex = 65;
            this.toolTip.SetToolTip(this.btnResetFareProvider, "Reset the configuration for selected plugin");
            this.btnResetFareProvider.UseVisualStyleBackColor = true;
            this.btnResetFareProvider.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnResetArchiveManager
            // 
            this.btnResetArchiveManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetArchiveManager.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnResetArchiveManager.BackgroundImage")));
            this.btnResetArchiveManager.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnResetArchiveManager.Location = new System.Drawing.Point(543, 100);
            this.btnResetArchiveManager.Name = "btnResetArchiveManager";
            this.btnResetArchiveManager.Size = new System.Drawing.Size(22, 22);
            this.btnResetArchiveManager.TabIndex = 64;
            this.toolTip.SetToolTip(this.btnResetArchiveManager, "Reset the configuration for selected plugin");
            this.btnResetArchiveManager.UseVisualStyleBackColor = true;
            this.btnResetArchiveManager.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // imgSyncerStatus
            // 
            this.imgSyncerStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgSyncerStatus.Location = new System.Drawing.Point(643, 76);
            this.imgSyncerStatus.Name = "imgSyncerStatus";
            this.imgSyncerStatus.Size = new System.Drawing.Size(18, 18);
            this.imgSyncerStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgSyncerStatus.TabIndex = 63;
            this.imgSyncerStatus.TabStop = false;
            // 
            // imgDatabaseStatus
            // 
            this.imgDatabaseStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgDatabaseStatus.Location = new System.Drawing.Point(643, 48);
            this.imgDatabaseStatus.Name = "imgDatabaseStatus";
            this.imgDatabaseStatus.Size = new System.Drawing.Size(18, 18);
            this.imgDatabaseStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgDatabaseStatus.TabIndex = 62;
            this.imgDatabaseStatus.TabStop = false;
            // 
            // imgHandlerStatus
            // 
            this.imgHandlerStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgHandlerStatus.Location = new System.Drawing.Point(643, 19);
            this.imgHandlerStatus.Name = "imgHandlerStatus";
            this.imgHandlerStatus.Size = new System.Drawing.Size(18, 18);
            this.imgHandlerStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgHandlerStatus.TabIndex = 40;
            this.imgHandlerStatus.TabStop = false;
            // 
            // imgArchiveStatus
            // 
            this.imgArchiveStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgArchiveStatus.Location = new System.Drawing.Point(643, 102);
            this.imgArchiveStatus.Name = "imgArchiveStatus";
            this.imgArchiveStatus.Size = new System.Drawing.Size(18, 18);
            this.imgArchiveStatus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgArchiveStatus.TabIndex = 39;
            this.imgArchiveStatus.TabStop = false;
            // 
            // btnInfoDbSyncer
            // 
            this.btnInfoDbSyncer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfoDbSyncer.Location = new System.Drawing.Point(521, 73);
            this.btnInfoDbSyncer.Name = "btnInfoDbSyncer";
            this.btnInfoDbSyncer.Size = new System.Drawing.Size(22, 22);
            this.btnInfoDbSyncer.TabIndex = 38;
            this.btnInfoDbSyncer.Text = "?";
            this.toolTip.SetToolTip(this.btnInfoDbSyncer, "Information about selected plugin");
            this.btnInfoDbSyncer.UseVisualStyleBackColor = true;
            this.btnInfoDbSyncer.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnInfoFareDatabase
            // 
            this.btnInfoFareDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfoFareDatabase.Location = new System.Drawing.Point(521, 46);
            this.btnInfoFareDatabase.Name = "btnInfoFareDatabase";
            this.btnInfoFareDatabase.Size = new System.Drawing.Size(22, 22);
            this.btnInfoFareDatabase.TabIndex = 37;
            this.btnInfoFareDatabase.Text = "?";
            this.toolTip.SetToolTip(this.btnInfoFareDatabase, "Information about selected plugin");
            this.btnInfoFareDatabase.UseVisualStyleBackColor = true;
            this.btnInfoFareDatabase.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnInfoFareProvider
            // 
            this.btnInfoFareProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfoFareProvider.Location = new System.Drawing.Point(520, 19);
            this.btnInfoFareProvider.Name = "btnInfoFareProvider";
            this.btnInfoFareProvider.Size = new System.Drawing.Size(22, 22);
            this.btnInfoFareProvider.TabIndex = 36;
            this.btnInfoFareProvider.Text = "?";
            this.toolTip.SetToolTip(this.btnInfoFareProvider, "Information about selected plugin");
            this.btnInfoFareProvider.UseVisualStyleBackColor = true;
            this.btnInfoFareProvider.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnInfoArchiveManager
            // 
            this.btnInfoArchiveManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfoArchiveManager.Location = new System.Drawing.Point(520, 100);
            this.btnInfoArchiveManager.Name = "btnInfoArchiveManager";
            this.btnInfoArchiveManager.Size = new System.Drawing.Size(22, 22);
            this.btnInfoArchiveManager.TabIndex = 35;
            this.btnInfoArchiveManager.Text = "?";
            this.toolTip.SetToolTip(this.btnInfoArchiveManager, "Information about selected plugin");
            this.btnInfoArchiveManager.UseVisualStyleBackColor = true;
            this.btnInfoArchiveManager.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnConfigDbSyncer
            // 
            this.btnConfigDbSyncer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigDbSyncer.Location = new System.Drawing.Point(566, 74);
            this.btnConfigDbSyncer.Name = "btnConfigDbSyncer";
            this.btnConfigDbSyncer.Size = new System.Drawing.Size(73, 22);
            this.btnConfigDbSyncer.TabIndex = 34;
            this.btnConfigDbSyncer.Text = "Configure";
            this.toolTip.SetToolTip(this.btnConfigDbSyncer, "Show configurations for the selected plugin");
            this.btnConfigDbSyncer.UseVisualStyleBackColor = true;
            this.btnConfigDbSyncer.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnConfigFareDatabase
            // 
            this.btnConfigFareDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigFareDatabase.Location = new System.Drawing.Point(566, 46);
            this.btnConfigFareDatabase.Name = "btnConfigFareDatabase";
            this.btnConfigFareDatabase.Size = new System.Drawing.Size(73, 22);
            this.btnConfigFareDatabase.TabIndex = 33;
            this.btnConfigFareDatabase.Text = "Configure";
            this.toolTip.SetToolTip(this.btnConfigFareDatabase, "Show configurations for the selected plugin");
            this.btnConfigFareDatabase.UseVisualStyleBackColor = true;
            this.btnConfigFareDatabase.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnConfigFareProvider
            // 
            this.btnConfigFareProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigFareProvider.Location = new System.Drawing.Point(566, 17);
            this.btnConfigFareProvider.Name = "btnConfigFareProvider";
            this.btnConfigFareProvider.Size = new System.Drawing.Size(73, 22);
            this.btnConfigFareProvider.TabIndex = 32;
            this.btnConfigFareProvider.Text = "Configure";
            this.toolTip.SetToolTip(this.btnConfigFareProvider, "Show configurations for the selected plugin");
            this.btnConfigFareProvider.UseVisualStyleBackColor = true;
            this.btnConfigFareProvider.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnConfigArchiveManager
            // 
            this.btnConfigArchiveManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfigArchiveManager.Location = new System.Drawing.Point(566, 100);
            this.btnConfigArchiveManager.Name = "btnConfigArchiveManager";
            this.btnConfigArchiveManager.Size = new System.Drawing.Size(73, 22);
            this.btnConfigArchiveManager.TabIndex = 31;
            this.btnConfigArchiveManager.Text = "Configure";
            this.toolTip.SetToolTip(this.btnConfigArchiveManager, "Show configurations for the selected plugin");
            this.btnConfigArchiveManager.UseVisualStyleBackColor = true;
            this.btnConfigArchiveManager.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // cbDbSyncer
            // 
            this.cbDbSyncer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDbSyncer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDbSyncer.FormattingEnabled = true;
            this.cbDbSyncer.Location = new System.Drawing.Point(161, 74);
            this.cbDbSyncer.Name = "cbDbSyncer";
            this.cbDbSyncer.Size = new System.Drawing.Size(357, 21);
            this.cbDbSyncer.TabIndex = 5;
            this.toolTip.SetToolTip(this.cbDbSyncer, "If the selected Fare Database supports data synchronization, you can select a plu" +
        "gin which is used for downloading/uploading data to different data storage servi" +
        "ces");
            this.cbDbSyncer.SelectedValueChanged += new System.EventHandler(this.configChanged_EventHandler);
            // 
            // cbFareDatabase
            // 
            this.cbFareDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFareDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFareDatabase.FormattingEnabled = true;
            this.cbFareDatabase.Location = new System.Drawing.Point(161, 47);
            this.cbFareDatabase.Name = "cbFareDatabase";
            this.cbFareDatabase.Size = new System.Drawing.Size(357, 21);
            this.cbFareDatabase.TabIndex = 4;
            this.toolTip.SetToolTip(this.cbFareDatabase, "Fare Database is responsible for storing historical fare data");
            this.cbFareDatabase.SelectedIndexChanged += new System.EventHandler(this.cbFareDatabase_SelectedIndexChanged);
            this.cbFareDatabase.SelectedValueChanged += new System.EventHandler(this.configChanged_EventHandler);
            // 
            // cbFareDataProvider
            // 
            this.cbFareDataProvider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFareDataProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFareDataProvider.FormattingEnabled = true;
            this.cbFareDataProvider.Location = new System.Drawing.Point(161, 19);
            this.cbFareDataProvider.Name = "cbFareDataProvider";
            this.cbFareDataProvider.Size = new System.Drawing.Size(357, 21);
            this.cbFareDataProvider.TabIndex = 3;
            this.toolTip.SetToolTip(this.cbFareDataProvider, "Fare Data Handler is responsible for processing, retrieving fare data from differ" +
        "ent service providers");
            this.cbFareDataProvider.SelectedValueChanged += new System.EventHandler(this.configChanged_EventHandler);
            // 
            // configTypeName
            // 
            this.configTypeName.AutoSize = true;
            this.configTypeName.Location = new System.Drawing.Point(83, 105);
            this.configTypeName.Name = "configTypeName";
            this.configTypeName.Size = new System.Drawing.Size(72, 13);
            this.configTypeName.TabIndex = 23;
            this.configTypeName.Text = "Data Archiver";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(59, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Fare Data Provider";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(78, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Fare Database";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(141, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Fare Database Synchronizer";
            // 
            // cbArchiveManager
            // 
            this.cbArchiveManager.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbArchiveManager.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArchiveManager.FormattingEnabled = true;
            this.cbArchiveManager.Location = new System.Drawing.Point(161, 100);
            this.cbArchiveManager.Name = "cbArchiveManager";
            this.cbArchiveManager.Size = new System.Drawing.Size(357, 21);
            this.cbArchiveManager.TabIndex = 6;
            this.toolTip.SetToolTip(this.cbArchiveManager, "Archive Manager is responsible for saving/importing data archives");
            this.cbArchiveManager.SelectedValueChanged += new System.EventHandler(this.configChanged_EventHandler);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnImportToDatabase);
            this.groupBox2.Controls.Add(this.btnDbStat);
            this.groupBox2.Controls.Add(this.btnResetDatabase);
            this.groupBox2.Controls.Add(this.btnRepairDatabase);
            this.groupBox2.Controls.Add(this.btnExportDatabase);
            this.groupBox2.Location = new System.Drawing.Point(15, 247);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(226, 90);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Database Maintenance";
            // 
            // btnImportToDatabase
            // 
            this.btnImportToDatabase.AutoAlign = false;
            this.btnImportToDatabase.Image = null;
            this.btnImportToDatabase.Location = new System.Drawing.Point(118, 20);
            this.btnImportToDatabase.Name = "btnImportToDatabase";
            this.btnImportToDatabase.Size = new System.Drawing.Size(97, 27);
            this.btnImportToDatabase.TabIndex = 12;
            this.btnImportToDatabase.Text = "Import";
            this.btnImportToDatabase.UseVisualStyleBackColor = true;
            this.btnImportToDatabase.Click += new System.EventHandler(this.btnImportToDatabase_Click);
            // 
            // btnDbStat
            // 
            this.btnDbStat.Location = new System.Drawing.Point(147, 50);
            this.btnDbStat.Name = "btnDbStat";
            this.btnDbStat.Size = new System.Drawing.Size(68, 27);
            this.btnDbStat.TabIndex = 15;
            this.btnDbStat.Text = "Statistics";
            this.btnDbStat.UseVisualStyleBackColor = true;
            this.btnDbStat.Click += new System.EventHandler(this.btnDbStat_Click);
            // 
            // btnResetDatabase
            // 
            this.btnResetDatabase.Location = new System.Drawing.Point(15, 50);
            this.btnResetDatabase.Name = "btnResetDatabase";
            this.btnResetDatabase.Size = new System.Drawing.Size(64, 27);
            this.btnResetDatabase.TabIndex = 13;
            this.btnResetDatabase.Text = "Reset";
            this.btnResetDatabase.UseVisualStyleBackColor = true;
            this.btnResetDatabase.Click += new System.EventHandler(this.btnResetDatabase_Click);
            // 
            // btnRepairDatabase
            // 
            this.btnRepairDatabase.Location = new System.Drawing.Point(81, 50);
            this.btnRepairDatabase.Name = "btnRepairDatabase";
            this.btnRepairDatabase.Size = new System.Drawing.Size(64, 27);
            this.btnRepairDatabase.TabIndex = 14;
            this.btnRepairDatabase.Text = "Repair";
            this.btnRepairDatabase.UseVisualStyleBackColor = true;
            this.btnRepairDatabase.Click += new System.EventHandler(this.btnRepairDatabase_Click);
            // 
            // btnExportDatabase
            // 
            this.btnExportDatabase.AutoAlign = false;
            this.btnExportDatabase.AutoSize = true;
            this.btnExportDatabase.Image = null;
            this.btnExportDatabase.Location = new System.Drawing.Point(15, 20);
            this.btnExportDatabase.Name = "btnExportDatabase";
            this.btnExportDatabase.Size = new System.Drawing.Size(97, 27);
            this.btnExportDatabase.SplitMenuStrip = this.exportDbMenuStrip;
            this.btnExportDatabase.TabIndex = 11;
            this.btnExportDatabase.Text = "Export";
            this.btnExportDatabase.UseVisualStyleBackColor = true;
            this.btnExportDatabase.Click += new System.EventHandler(this.btnExportDatabase_Click);
            // 
            // exportDbMenuStrip
            // 
            this.exportDbMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportDbXmlToolStripMenuItem,
            this.exportDbBinaryToolStripMenuItem});
            this.exportDbMenuStrip.Name = "exportDbMenuStrip";
            this.exportDbMenuStrip.Size = new System.Drawing.Size(108, 48);
            // 
            // exportDbXmlToolStripMenuItem
            // 
            this.exportDbXmlToolStripMenuItem.Name = "exportDbXmlToolStripMenuItem";
            this.exportDbXmlToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exportDbXmlToolStripMenuItem.Text = "XML";
            this.exportDbXmlToolStripMenuItem.Click += new System.EventHandler(this.exportDbXmlToolStripMenuItem_Click);
            // 
            // exportDbBinaryToolStripMenuItem
            // 
            this.exportDbBinaryToolStripMenuItem.Name = "exportDbBinaryToolStripMenuItem";
            this.exportDbBinaryToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.exportDbBinaryToolStripMenuItem.Text = "Binary";
            this.exportDbBinaryToolStripMenuItem.Click += new System.EventHandler(this.exportDbBinaryToolStripMenuItem_Click);
            // 
            // grpDatabaseSync
            // 
            this.grpDatabaseSync.Controls.Add(this.btnBackupSyncDb);
            this.grpDatabaseSync.Controls.Add(this.btnRestoreSyncDb);
            this.grpDatabaseSync.Location = new System.Drawing.Point(15, 348);
            this.grpDatabaseSync.Name = "grpDatabaseSync";
            this.grpDatabaseSync.Size = new System.Drawing.Size(226, 94);
            this.grpDatabaseSync.TabIndex = 60;
            this.grpDatabaseSync.TabStop = false;
            this.grpDatabaseSync.Text = "Database Synchronization";
            // 
            // btnBackupSyncDb
            // 
            this.btnBackupSyncDb.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Upload;
            this.btnBackupSyncDb.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBackupSyncDb.Location = new System.Drawing.Point(14, 27);
            this.btnBackupSyncDb.Name = "btnBackupSyncDb";
            this.btnBackupSyncDb.Padding = new System.Windows.Forms.Padding(8, 0, 10, 0);
            this.btnBackupSyncDb.Size = new System.Drawing.Size(95, 51);
            this.btnBackupSyncDb.TabIndex = 16;
            this.btnBackupSyncDb.Text = "Backup";
            this.btnBackupSyncDb.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBackupSyncDb.UseVisualStyleBackColor = true;
            this.btnBackupSyncDb.Click += new System.EventHandler(this.btnBackupSyncDb_Click);
            // 
            // btnRestoreSyncDb
            // 
            this.btnRestoreSyncDb.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Download;
            this.btnRestoreSyncDb.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnRestoreSyncDb.Location = new System.Drawing.Point(117, 27);
            this.btnRestoreSyncDb.Name = "btnRestoreSyncDb";
            this.btnRestoreSyncDb.Padding = new System.Windows.Forms.Padding(8, 0, 10, 0);
            this.btnRestoreSyncDb.Size = new System.Drawing.Size(95, 51);
            this.btnRestoreSyncDb.TabIndex = 17;
            this.btnRestoreSyncDb.Text = "Restore";
            this.btnRestoreSyncDb.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRestoreSyncDb.UseVisualStyleBackColor = true;
            this.btnRestoreSyncDb.Click += new System.EventHandler(this.btnRestoreSyncDb_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btnDefaultCurrency);
            this.groupBox4.Controls.Add(this.lstCurrency);
            this.groupBox4.Controls.Add(this.btnSelectNoneCurrency);
            this.groupBox4.Controls.Add(this.btnSelectAllCurrency);
            this.groupBox4.Location = new System.Drawing.Point(248, 247);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(327, 189);
            this.groupBox4.TabIndex = 61;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Available currencies for converting";
            // 
            // btnDefaultCurrency
            // 
            this.btnDefaultCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDefaultCurrency.Location = new System.Drawing.Point(229, 149);
            this.btnDefaultCurrency.Name = "btnDefaultCurrency";
            this.btnDefaultCurrency.Size = new System.Drawing.Size(81, 27);
            this.btnDefaultCurrency.TabIndex = 10;
            this.btnDefaultCurrency.Text = "&Default";
            this.btnDefaultCurrency.UseVisualStyleBackColor = true;
            this.btnDefaultCurrency.Click += new System.EventHandler(this.btnDefaultCurrency_Click);
            // 
            // lstCurrency
            // 
            this.lstCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCurrency.FormattingEnabled = true;
            this.lstCurrency.Location = new System.Drawing.Point(16, 20);
            this.lstCurrency.Name = "lstCurrency";
            this.lstCurrency.Size = new System.Drawing.Size(294, 109);
            this.lstCurrency.TabIndex = 7;
            // 
            // btnSelectNoneCurrency
            // 
            this.btnSelectNoneCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectNoneCurrency.Location = new System.Drawing.Point(121, 149);
            this.btnSelectNoneCurrency.Name = "btnSelectNoneCurrency";
            this.btnSelectNoneCurrency.Size = new System.Drawing.Size(102, 27);
            this.btnSelectNoneCurrency.TabIndex = 9;
            this.btnSelectNoneCurrency.Text = "Select &None";
            this.btnSelectNoneCurrency.UseVisualStyleBackColor = true;
            this.btnSelectNoneCurrency.Click += new System.EventHandler(this.btnSelectNoneCurrency_Click);
            // 
            // btnSelectAllCurrency
            // 
            this.btnSelectAllCurrency.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAllCurrency.Location = new System.Drawing.Point(15, 149);
            this.btnSelectAllCurrency.Name = "btnSelectAllCurrency";
            this.btnSelectAllCurrency.Size = new System.Drawing.Size(102, 27);
            this.btnSelectAllCurrency.TabIndex = 8;
            this.btnSelectAllCurrency.Text = "Select &All";
            this.btnSelectAllCurrency.UseVisualStyleBackColor = true;
            this.btnSelectAllCurrency.Click += new System.EventHandler(this.btnSelectAllCurrency_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.Controls.Add(this.btnExportConfig);
            this.pnlContent.Controls.Add(this.btnImportConfig);
            this.pnlContent.Controls.Add(this.grpDefaultSetting);
            this.pnlContent.Controls.Add(this.groupBox4);
            this.pnlContent.Controls.Add(this.grpDatabaseSync);
            this.pnlContent.Controls.Add(this.groupBox2);
            this.pnlContent.Controls.Add(this.groupBox1);
            this.pnlContent.Controls.Add(this.btnCancel);
            this.pnlContent.Controls.Add(this.btnSave);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 74);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(704, 448);
            this.pnlContent.TabIndex = 65;
            // 
            // btnExportConfig
            // 
            this.btnExportConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExportConfig.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Export;
            this.btnExportConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExportConfig.Location = new System.Drawing.Point(581, 289);
            this.btnExportConfig.Name = "btnExportConfig";
            this.btnExportConfig.Padding = new System.Windows.Forms.Padding(3, 0, 8, 0);
            this.btnExportConfig.Size = new System.Drawing.Size(112, 31);
            this.btnExportConfig.TabIndex = 64;
            this.btnExportConfig.Text = "&Export Config";
            this.btnExportConfig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExportConfig.UseVisualStyleBackColor = true;
            this.btnExportConfig.Click += new System.EventHandler(this.btnExportConfig_Click);
            // 
            // btnImportConfig
            // 
            this.btnImportConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnImportConfig.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Import;
            this.btnImportConfig.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnImportConfig.Location = new System.Drawing.Point(581, 252);
            this.btnImportConfig.Name = "btnImportConfig";
            this.btnImportConfig.Padding = new System.Windows.Forms.Padding(2, 0, 9, 0);
            this.btnImportConfig.Size = new System.Drawing.Size(112, 31);
            this.btnImportConfig.TabIndex = 63;
            this.btnImportConfig.Text = "&Import Config";
            this.btnImportConfig.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnImportConfig.UseVisualStyleBackColor = true;
            this.btnImportConfig.Click += new System.EventHandler(this.btnImportConfig_Click);
            // 
            // grpDefaultSetting
            // 
            this.grpDefaultSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDefaultSetting.Controls.Add(this.txtDeparture);
            this.grpDefaultSetting.Controls.Add(this.txtDestination);
            this.grpDefaultSetting.Controls.Add(this.label5);
            this.grpDefaultSetting.Controls.Add(this.label6);
            this.grpDefaultSetting.Location = new System.Drawing.Point(15, 9);
            this.grpDefaultSetting.Name = "grpDefaultSetting";
            this.grpDefaultSetting.Size = new System.Drawing.Size(677, 82);
            this.grpDefaultSetting.TabIndex = 62;
            this.grpDefaultSetting.TabStop = false;
            this.grpDefaultSetting.Text = "Startup Settings";
            // 
            // txtDeparture
            // 
            this.txtDeparture.AlwaysShowSuggest = false;
            this.txtDeparture.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeparture.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDeparture.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDeparture.CaseSensitive = false;
            this.txtDeparture.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.txtDeparture.Location = new System.Drawing.Point(117, 19);
            this.txtDeparture.MinTypedCharacters = 1;
            this.txtDeparture.Name = "txtDeparture";
            this.txtDeparture.Size = new System.Drawing.Size(544, 20);
            this.txtDeparture.TabIndex = 1;
            // 
            // txtDestination
            // 
            this.txtDestination.AlwaysShowSuggest = false;
            this.txtDestination.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestination.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.txtDestination.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.txtDestination.CaseSensitive = false;
            this.txtDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.txtDestination.Location = new System.Drawing.Point(117, 48);
            this.txtDestination.MinTypedCharacters = 1;
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(544, 20);
            this.txtDestination.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 66;
            this.label5.Text = "Default Destination";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 13);
            this.label6.TabIndex = 65;
            this.label6.Text = "Default Departure";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.Save;
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSave.Location = new System.Drawing.Point(581, 367);
            this.btnSave.Name = "btnSave";
            this.btnSave.Padding = new System.Windows.Forms.Padding(3, 0, 27, 0);
            this.btnSave.Size = new System.Drawing.Size(112, 31);
            this.btnSave.TabIndex = 50;
            this.btnSave.Text = "&Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(130)))), ((int)(((byte)(148)))));
            this.pnlTop.Controls.Add(this.lblWelcome);
            this.pnlTop.Controls.Add(this.lblInstruction);
            this.pnlTop.Controls.Add(this.overlayBackgroundPicture);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(704, 74);
            this.pnlTop.TabIndex = 64;
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Location = new System.Drawing.Point(10, 18);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(281, 20);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Welcome to FareLiz Configuration";
            // 
            // lblInstruction
            // 
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            this.lblInstruction.ForeColor = System.Drawing.Color.White;
            this.lblInstruction.Location = new System.Drawing.Point(22, 45);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new System.Drawing.Size(285, 13);
            this.lblInstruction.TabIndex = 1;
            this.lblInstruction.Text = "Please select the plugins to be used with FareLiz";
            // 
            // overlayBackgroundPicture
            // 
            this.overlayBackgroundPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.overlayBackgroundPicture.Controls.Add(this.imgLogo);
            this.overlayBackgroundPicture.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.BackgroundPlane;
            this.overlayBackgroundPicture.Location = new System.Drawing.Point(182, -272);
            this.overlayBackgroundPicture.Name = "overlayBackgroundPicture";
            this.overlayBackgroundPicture.Size = new System.Drawing.Size(585, 450);
            this.overlayBackgroundPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.overlayBackgroundPicture.TabIndex = 50;
            this.overlayBackgroundPicture.TabStop = false;
            // 
            // imgLogo
            // 
            this.imgLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.imgLogo.BackColor = System.Drawing.Color.Transparent;
            this.imgLogo.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.SkyDean;
            this.imgLogo.Location = new System.Drawing.Point(370, 289);
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.Size = new System.Drawing.Size(150, 46);
            this.imgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgLogo.TabIndex = 51;
            this.imgLogo.TabStop = false;
            // 
            // EnvConfiguratorDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(704, 522);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::SkyDean.FareLiz.WinForm.Properties.Resources.FareLizIcon;
            this.MinimumSize = new System.Drawing.Size(720, 560);
            this.Name = "EnvConfiguratorDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Environment Configuration";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgSyncerStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgDatabaseStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgHandlerStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgArchiveStatus)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.exportDbMenuStrip.ResumeLayout(false);
            this.grpDatabaseSync.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.grpDefaultSetting.ResumeLayout(false);
            this.grpDefaultSetting.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overlayBackgroundPicture)).EndInit();
            this.overlayBackgroundPicture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ImageButton btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnInfoDbSyncer;
        private System.Windows.Forms.Button btnInfoFareDatabase;
        private System.Windows.Forms.Button btnInfoFareProvider;
        private System.Windows.Forms.Button btnInfoArchiveManager;
        private System.Windows.Forms.Button btnConfigDbSyncer;
        private System.Windows.Forms.Button btnConfigFareDatabase;
        private System.Windows.Forms.Button btnConfigFareProvider;
        private System.Windows.Forms.Button btnConfigArchiveManager;
        private System.Windows.Forms.ComboBox cbDbSyncer;
        private System.Windows.Forms.ComboBox cbFareDatabase;
        private System.Windows.Forms.ComboBox cbFareDataProvider;
        private System.Windows.Forms.Label configTypeName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbArchiveManager;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnResetDatabase;
        private System.Windows.Forms.Button btnRepairDatabase;
        private SplitButton btnExportDatabase;
        private System.Windows.Forms.Button btnDbStat;
        private ImageButton btnImportToDatabase;
        private ImageButton btnRestoreSyncDb;
        private ImageButton btnBackupSyncDb;
        private System.Windows.Forms.GroupBox grpDatabaseSync;
        private System.Windows.Forms.ContextMenuStrip exportDbMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exportDbXmlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDbBinaryToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSelectNoneCurrency;
        private System.Windows.Forms.Button btnSelectAllCurrency;
        private System.Windows.Forms.CheckedListBox lstCurrency;
        private System.Windows.Forms.PictureBox imgSyncerStatus;
        private System.Windows.Forms.PictureBox imgDatabaseStatus;
        private System.Windows.Forms.PictureBox imgHandlerStatus;
        private System.Windows.Forms.PictureBox imgArchiveStatus;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.Button btnDefaultCurrency;
        private System.Windows.Forms.Button btnResetDbSyncer;
        private System.Windows.Forms.Button btnResetFareDatabase;
        private System.Windows.Forms.Button btnResetFareProvider;
        private System.Windows.Forms.Button btnResetArchiveManager;
        private System.Windows.Forms.GroupBox grpDefaultSetting;
        private AirportTextBox txtDeparture;
        private AirportTextBox txtDestination;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox overlayBackgroundPicture;
        private System.Windows.Forms.PictureBox imgLogo;
        private ImageButton btnExportConfig;
        private ImageButton btnImportConfig;
    }
}