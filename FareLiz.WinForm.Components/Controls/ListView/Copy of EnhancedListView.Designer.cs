using System.Windows.Forms;

namespace SkyDean.FareLiz.WinForm.Controls
{
    /// <summary>
    ///     This is a specialization of a ListView control that defaults to
    ///     details mode and includes the ability to filter data on column
    ///     headers and to specify column data sort comparison type.
    /// </summary>
    public partial class EnhancedListView
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

        #region Component Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        ///     NOTE: We have modified this method to include the context
        ///     menu and items creation as if this was a form control.
        /// </summary>
        private void InitializeComponent()
        {
            this.mnuAlignCenter = new System.Windows.Forms.MenuItem();
            this.mnuAlignLeft = new System.Windows.Forms.MenuItem();
            this.mnuAlignRight = new System.Windows.Forms.MenuItem();
            this.mnuAlignment = new System.Windows.Forms.MenuItem();
            this.mnuClearFilter = new System.Windows.Forms.MenuItem();
            this.mnuClearAllFilter = new System.Windows.Forms.MenuItem();
            this.mnuFilterButton = new System.Windows.Forms.ContextMenu();
            this.mnuIgnoreCase = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // mnuAlignCenter
            // 
            this.mnuAlignCenter.RadioCheck = true;
            this.mnuAlignCenter.Text = "&Center";
            this.mnuAlignCenter.Click += new System.EventHandler(this.FilterButtonMenuItem_Click);
            // 
            // mnuAlignLeft
            // 
            this.mnuAlignLeft.RadioCheck = true;
            this.mnuAlignLeft.Text = "&Left";
            this.mnuAlignLeft.Click += new System.EventHandler(this.FilterButtonMenuItem_Click);
            // 
            // mnuAlignRight
            // 
            this.mnuAlignRight.RadioCheck = true;
            this.mnuAlignRight.Text = "&Right";
            this.mnuAlignRight.Click += new System.EventHandler(this.FilterButtonMenuItem_Click);
            // 
            // mnuAlignment
            // 
            this.mnuAlignment.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAlignLeft,
            this.mnuAlignRight,
            this.mnuAlignCenter});
            this.mnuAlignment.Text = "&Alignment";
            // 
            // mnuIgnoreCase
            // 
            this.mnuIgnoreCase.Text = "&Ignore Case";
            this.mnuIgnoreCase.Click += new System.EventHandler(this.FilterButtonMenuItem_Click);
            // 
            // mnuClearFilter
            // 
            this.mnuClearFilter.DefaultItem = true;
            this.mnuClearFilter.Text = "&Clear Filter";
            this.mnuClearFilter.Click += new System.EventHandler(this.FilterButtonMenuItem_Click);
            // 
            // mnuClearAllFilter
            // 
            this.mnuClearAllFilter.Text = "Clear &All Filters";
            this.mnuClearAllFilter.Click += new System.EventHandler(this.FilterButtonMenuItem_Click);
            // 
            // mnuFilterButton
            // 
            this.mnuFilterButton.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuAlignment,
            this.mnuIgnoreCase,
            this.mnuClearFilter,
            this.mnuClearAllFilter
            });
            this.mnuFilterButton.Popup += FilterButtonMenu_Popup;
            // 
            // EnhancedListView
            // 
            this.Name = "EnhancedListView";
            this.ResumeLayout(false);

        }
        #endregion

        private MenuItem mnuAlignCenter;
        private MenuItem mnuAlignLeft;
        private MenuItem mnuAlignRight;
        private MenuItem mnuAlignment;
        private MenuItem mnuIgnoreCase;
        private MenuItem mnuClearFilter;
        private MenuItem mnuClearAllFilter;
        private ContextMenu mnuFilterButton;
    }
}