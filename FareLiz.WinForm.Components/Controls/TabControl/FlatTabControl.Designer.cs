namespace SkyDean.FareLiz.WinForm.Components.Controls.TabControl
{
    partial class FlatTabControl
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
            if (disposing)
            {
                if (this.leftRightImages != null)
                    this.leftRightImages.Dispose();
                if (this.components != null)
                    this.components.Dispose();
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
            this.tabPageContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCloseTab = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloseAllTabs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReload = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCloseRightTabs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloseLeftTabs = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPageContextMenu.SuspendLayout();
            this.SuspendLayout();
            //
            // leftRightImages
            //
            this.leftRightImages = new System.Windows.Forms.ImageList();
            this.leftRightImages.ImageSize = new System.Drawing.Size(16, 16);
            this.leftRightImages.Images.AddStrip(Properties.Resources.FlatTabUpDown);
            // 
            // tabPageContextMenu
            // 
            this.tabPageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCloseTab,
            this.mnuCloseAllTabs,
            this.mnuCloseLeftTabs,
            this.mnuCloseRightTabs,
            this.mnuSeparator,
            this.mnuReload});
            this.tabPageContextMenu.Name = "tabPageContextMenu";
            this.tabPageContextMenu.Size = new System.Drawing.Size(191, 120);
            this.tabPageContextMenu.Opening += this.TabPageContextMenu_Opening;
            // 
            // mnuCloseTab
            // 
            this.mnuCloseTab.Name = "mnuCloseTab";
            this.mnuCloseTab.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.mnuCloseTab.Size = new System.Drawing.Size(190, 22);
            this.mnuCloseTab.Text = "Close tab";
            this.mnuCloseTab.Click += this.mnuCloseTab_Click;
            // 
            // mnuCloseAllTabs
            // 
            this.mnuCloseAllTabs.Name = "mnuCloseAllTabs";
            this.mnuCloseAllTabs.Size = new System.Drawing.Size(190, 22);
            this.mnuCloseAllTabs.Text = "Close all tabs";
            this.mnuCloseAllTabs.Click += this.mnuCloseAllTabs_Click;
            // 
            // mnuReload
            // 
            this.mnuReload.Name = "mnuReload";
            this.mnuReload.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuReload.Size = new System.Drawing.Size(190, 22);
            this.mnuReload.Text = "&Reload";
            this.mnuReload.Click += this.mnuReload_Click;
            // 
            // mnuSeparator
            // 
            this.mnuSeparator.Name = "mnuSeparator";
            this.mnuSeparator.Size = new System.Drawing.Size(187, 6);
            // 
            // mnuCloseRightTabs
            // 
            this.mnuCloseRightTabs.Name = "mnuCloseRightTabs";
            this.mnuCloseRightTabs.Size = new System.Drawing.Size(190, 22);
            this.mnuCloseRightTabs.Text = "Close tabs to the right";
            this.mnuCloseRightTabs.Click += this.mnuCloseRightTabs_Click;
            // 
            // mnuCloseLeftTabs
            // 
            this.mnuCloseLeftTabs.Name = "mnuCloseLeftTabs";
            this.mnuCloseLeftTabs.Size = new System.Drawing.Size(190, 22);
            this.mnuCloseLeftTabs.Text = "Close tabs to the left";
            this.mnuCloseLeftTabs.Click += this.mnuCloseLeftTabs_Click;
            //
            // FlatTabControl
            //            
            this.Padding = new System.Drawing.Point(2, 5);
            this.ControlAdded += this.FlatTabControl_ControlAdded;
            this.ControlRemoved += this.FlatTabControl_ControlRemoved;
            this.KeyDown += this.FlatTabControl_KeyDown;
            this.SelectedIndexChanged += this.FlatTabControl_SelectedIndexChanged;
            this.tabPageContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }        

        #endregion

        private System.Windows.Forms.ImageList leftRightImages;
        private System.Windows.Forms.ContextMenuStrip tabPageContextMenu;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseTab;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseAllTabs;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseLeftTabs;
        private System.Windows.Forms.ToolStripMenuItem mnuCloseRightTabs;
        private System.Windows.Forms.ToolStripSeparator mnuSeparator;
        private System.Windows.Forms.ToolStripMenuItem mnuReload;
    }
}
