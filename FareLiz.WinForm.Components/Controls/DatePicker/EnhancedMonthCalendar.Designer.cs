namespace SkyDean.FareLiz.WinForm.Components.Controls.DatePicker
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    partial class EnhancedMonthCalendar
    {
        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="EnhancedMonthCalendar"/> control and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            if (this.headerFont != null)
                this.headerFont.Dispose();
            if (this.footerFont != null)
                this.footerFont.Dispose();
            if (this.dayHeaderFont != null)
                this.dayHeaderFont.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes the context menus.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.monthMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiJan = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFeb = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMar = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiApr = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMay = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiJun = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiJul = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAug = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSep = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOct = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiNov = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDez = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiA1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiA2 = new System.Windows.Forms.ToolStripMenuItem();
            this.yearMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiYear1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear4 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear5 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear6 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear7 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear8 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiYear9 = new System.Windows.Forms.ToolStripMenuItem();
            this.monthMenu.SuspendLayout();
            this.yearMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // monthMenu
            // 
            this.monthMenu.Items.AddRange(new ToolStripItem[] {
            this.tsmiJan,
            this.tsmiFeb,
            this.tsmiMar,
            this.tsmiApr,
            this.tsmiMay,
            this.tsmiJun,
            this.tsmiJul,
            this.tsmiAug,
            this.tsmiSep,
            this.tsmiOct,
            this.tsmiNov,
            this.tsmiDez,
            this.tsmiA1,
            this.tsmiA2});
            this.monthMenu.Name = "monthMenu";
            this.monthMenu.ShowImageMargin = false;
            this.monthMenu.Size = new Size(54, 312);
            this.monthMenu.Closed += this.MonthMenuClosed;
            // 
            // tsmiJan
            // 
            this.tsmiJan.Size = new Size(78, 22);
            this.tsmiJan.Tag = 1;
            this.tsmiJan.Click += this.MonthClick;
            // 
            // tsmiFeb
            // 
            this.tsmiFeb.Size = new Size(78, 22);
            this.tsmiFeb.Tag = 2;
            this.tsmiFeb.Click += this.MonthClick;
            // 
            // tsmiMar
            // 
            this.tsmiMar.Size = new Size(78, 22);
            this.tsmiMar.Tag = 3;
            this.tsmiMar.Click += this.MonthClick;
            // 
            // tsmiApr
            // 
            this.tsmiApr.Size = new Size(78, 22);
            this.tsmiApr.Tag = 4;
            this.tsmiApr.Click += this.MonthClick;
            // 
            // tsmiMay
            // 
            this.tsmiMay.Size = new Size(78, 22);
            this.tsmiMay.Tag = 5;
            this.tsmiMay.Click += this.MonthClick;
            // 
            // tsmiJun
            // 
            this.tsmiJun.Size = new Size(78, 22);
            this.tsmiJun.Tag = 6;
            this.tsmiJun.Click += this.MonthClick;
            // 
            // tsmiJul
            // 
            this.tsmiJul.Size = new Size(78, 22);
            this.tsmiJul.Tag = 7;
            this.tsmiJul.Click += this.MonthClick;
            // 
            // tsmiAug
            // 
            this.tsmiAug.Size = new Size(78, 22);
            this.tsmiAug.Tag = 8;
            this.tsmiAug.Click += this.MonthClick;
            // 
            // tsmiSep
            // 
            this.tsmiSep.Size = new Size(78, 22);
            this.tsmiSep.Tag = 9;
            this.tsmiSep.Click += this.MonthClick;
            // 
            // tsmiOct
            // 
            this.tsmiOct.Size = new Size(78, 22);
            this.tsmiOct.Tag = 10;
            this.tsmiOct.Click += this.MonthClick;
            // 
            // tsmiNov
            // 
            this.tsmiNov.Size = new Size(78, 22);
            this.tsmiNov.Tag = 11;
            this.tsmiNov.Click += this.MonthClick;
            // 
            // tsmiDez
            // 
            this.tsmiDez.Size = new Size(78, 22);
            this.tsmiDez.Tag = 12;
            this.tsmiDez.Click += this.MonthClick;
            // 
            // tsmiA1
            // 
            this.tsmiA1.Size = new Size(78, 22);
            this.tsmiA1.Click += this.MonthClick;
            // 
            // tsmiA2
            // 
            this.tsmiA2.Size = new Size(78, 22);
            this.tsmiA2.Click += this.MonthClick;

            // 
            // yearMenu
            // 
            this.yearMenu.Items.AddRange(new ToolStripItem[] {
            this.tsmiYear1,
            this.tsmiYear2,
            this.tsmiYear3,
            this.tsmiYear4,
            this.tsmiYear5,
            this.tsmiYear6,
            this.tsmiYear7,
            this.tsmiYear8,
            this.tsmiYear9});
            this.yearMenu.Name = "yearMenu";
            this.yearMenu.ShowImageMargin = false;
            this.yearMenu.ShowItemToolTips = false;
            this.yearMenu.Size = new Size(54, 202);
            this.yearMenu.Closed += this.YearMenuClosed;

            // 
            // tsmiYear1
            // 
            this.tsmiYear1.Size = new Size(53, 22);
            this.tsmiYear1.Click += this.YearClick;
            // 
            // tsmiYear2
            // 
            this.tsmiYear2.Size = new Size(53, 22);
            this.tsmiYear2.Click += this.YearClick;
            // 
            // tsmiYear3
            // 
            this.tsmiYear3.Size = new Size(53, 22);
            this.tsmiYear3.Click += this.YearClick;
            // 
            // tsmiYear4
            // 
            this.tsmiYear4.Size = new Size(53, 22);
            this.tsmiYear4.Click += this.YearClick;
            // 
            // tsmiYear5
            // 
            this.tsmiYear5.Size = new Size(53, 22);
            this.tsmiYear5.Click += this.YearClick;
            // 
            // tsmiYear6
            // 
            this.tsmiYear6.Size = new Size(53, 22);
            this.tsmiYear6.Click += this.YearClick;
            // 
            // tsmiYear7
            // 
            this.tsmiYear7.Size = new Size(53, 22);
            this.tsmiYear7.Click += this.YearClick;
            // 
            // tsmiYear8
            // 
            this.tsmiYear8.Size = new Size(53, 22);
            this.tsmiYear8.Click += this.YearClick;
            // 
            // tsmiYear9
            // 
            this.tsmiYear9.Size = new Size(53, 22);
            this.tsmiYear9.Click += this.YearClick;
            // 
            // EnhancedMonthCalendar
            // 
            this.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.monthMenu.ResumeLayout(false);
            this.yearMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private IContainer components;
        private ContextMenuStrip monthMenu;
        private ToolStripMenuItem tsmiJan;
        private ToolStripMenuItem tsmiFeb;
        private ToolStripMenuItem tsmiMar;
        private ToolStripMenuItem tsmiApr;
        private ToolStripMenuItem tsmiMay;
        private ToolStripMenuItem tsmiJun;
        private ToolStripMenuItem tsmiJul;
        private ToolStripMenuItem tsmiAug;
        private ToolStripMenuItem tsmiSep;
        private ToolStripMenuItem tsmiOct;
        private ToolStripMenuItem tsmiNov;
        private ToolStripMenuItem tsmiDez;
        private ContextMenuStrip yearMenu;
        private ToolStripMenuItem tsmiYear1;
        private ToolStripMenuItem tsmiYear2;
        private ToolStripMenuItem tsmiYear3;
        private ToolStripMenuItem tsmiYear4;
        private ToolStripMenuItem tsmiYear5;
        private ToolStripMenuItem tsmiYear6;
        private ToolStripMenuItem tsmiYear7;
        private ToolStripMenuItem tsmiYear8;
        private ToolStripMenuItem tsmiYear9;
        private ToolStripMenuItem tsmiA1;
        private ToolStripMenuItem tsmiA2;
    }
}
