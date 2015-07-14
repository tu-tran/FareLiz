namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    partial class OverlayAnimation
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
            this.imgArrow = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgArrow)).BeginInit();
            this.SuspendLayout();
            // 
            // imgArrow
            // 
            this.imgArrow.BackColor = System.Drawing.Color.Transparent;
            this.imgArrow.Image = global::SkyDean.FareLiz.WinForm.Properties.Resources.DownArrowBig;
            this.imgArrow.Location = new System.Drawing.Point(12, 12);
            this.imgArrow.Name = "imgArrow";
            this.imgArrow.Size = new System.Drawing.Size(128, 148);
            this.imgArrow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.imgArrow.TabIndex = 55;
            this.imgArrow.TabStop = false;
            // 
            // timer
            // 
            this.timer.Interval = 50;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.DimGray;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(12, 163);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Padding = new System.Windows.Forms.Padding(5);
            this.lblTitle.Size = new System.Drawing.Size(62, 36);
            this.lblTitle.TabIndex = 57;
            this.lblTitle.Text = "Title";
            // 
            // OverlayAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.ClientSize = new System.Drawing.Size(150, 200);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.imgArrow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OverlayAnimation";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "OverlayAnimation";
            this.TransparencyKey = System.Drawing.Color.LightPink;
            this.Shown += new System.EventHandler(this.OverlayAnimation_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.imgArrow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgArrow;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label lblTitle;
    }
}