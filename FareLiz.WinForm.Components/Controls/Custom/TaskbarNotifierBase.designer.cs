namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    partial class TaskbarNotifierBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            if (this._timer != null)
            {
                this._timer.Tick -= this.OnTimer;
                this._timer.Dispose();
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
            this.pnlMiddle = new System.Windows.Forms.Panel();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlTopRight = new System.Windows.Forms.Panel();
            this.imgClose = new System.Windows.Forms.PictureBox();
            this.pnlTopContent = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlTopLeft = new System.Windows.Forms.Panel();
            this.imgIcon = new System.Windows.Forms.PictureBox();
            this.pnlMiddle.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlTopRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgClose)).BeginInit();
            this.pnlTopContent.SuspendLayout();
            this.pnlTopLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMiddle
            // 
            this.pnlMiddle.BackColor = System.Drawing.Color.White;
            this.pnlMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMiddle.Location = new System.Drawing.Point(1, 25);
            this.pnlMiddle.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMiddle.Name = "pnlMiddle";
            this.pnlMiddle.Padding = new System.Windows.Forms.Padding(10);
            this.pnlMiddle.Size = new System.Drawing.Size(298, 71);
            this.pnlMiddle.TabIndex = 2;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(1, 96);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(298, 3);
            this.pnlBottom.TabIndex = 1;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pnlTopRight);
            this.pnlTop.Controls.Add(this.pnlTopContent);
            this.pnlTop.Controls.Add(this.pnlTopLeft);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(1, 1);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(298, 24);
            this.pnlTop.TabIndex = 0;
            // 
            // pnlTopRight
            // 
            this.pnlTopRight.Controls.Add(this.imgClose);
            this.pnlTopRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlTopRight.Location = new System.Drawing.Point(274, 0);
            this.pnlTopRight.Name = "pnlTopRight";
            this.pnlTopRight.Size = new System.Drawing.Size(24, 24);
            this.pnlTopRight.TabIndex = 0;
            // 
            // imgClose
            // 
            this.imgClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.imgClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgClose.Image = global::SkyDean.FareLiz.WinForm.Components.Properties.Resources.NotifierClose;
            this.imgClose.Location = new System.Drawing.Point(4, 4);
            this.imgClose.Name = "imgClose";
            this.imgClose.Size = new System.Drawing.Size(16, 16);
            this.imgClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgClose.TabIndex = 3;
            this.imgClose.TabStop = false;
            this.imgClose.Visible = false;
            this.imgClose.Click += new System.EventHandler(this.imgClose_Click);
            // 
            // pnlTopContent
            // 
            this.pnlTopContent.Controls.Add(this.lblTitle);
            this.pnlTopContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTopContent.Location = new System.Drawing.Point(24, 0);
            this.pnlTopContent.Name = "pnlTopContent";
            this.pnlTopContent.Padding = new System.Windows.Forms.Padding(2, 6, 0, 0);
            this.pnlTopContent.Size = new System.Drawing.Size(274, 24);
            this.pnlTopContent.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(2, 6);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(272, 18);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Notification";
            // 
            // pnlTopLeft
            // 
            this.pnlTopLeft.Controls.Add(this.imgIcon);
            this.pnlTopLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlTopLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlTopLeft.Name = "pnlTopLeft";
            this.pnlTopLeft.Size = new System.Drawing.Size(24, 24);
            this.pnlTopLeft.TabIndex = 2;
            // 
            // imgIcon
            // 
            this.imgIcon.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.imgIcon.Image = global::SkyDean.FareLiz.WinForm.Components.Properties.Resources.NotifierInfo;
            this.imgIcon.Location = new System.Drawing.Point(3, 3);
            this.imgIcon.Name = "imgIcon";
            this.imgIcon.Size = new System.Drawing.Size(18, 18);
            this.imgIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgIcon.TabIndex = 3;
            this.imgIcon.TabStop = false;
            // 
            // TaskbarNotifier
            // 
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(300, 100);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMiddle);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Font = System.Drawing.SystemFonts.DefaultFont;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TaskbarNotifier";
            this.Opacity = 0D;
            this.Padding = new System.Windows.Forms.Padding(1);
            this.ShowInTaskbar = false;
            this.Deactivate += new System.EventHandler(this.OnMouseLeave);
            this.pnlMiddle.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTopRight.ResumeLayout(false);
            this.pnlTopRight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgClose)).EndInit();
            this.pnlTopContent.ResumeLayout(false);
            this.pnlTopLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imgIcon)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlTopRight;
        private System.Windows.Forms.Panel pnlTopContent;
        private System.Windows.Forms.Panel pnlTopLeft;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlMiddle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.PictureBox imgClose;
        private System.Windows.Forms.PictureBox imgIcon;
    }
}