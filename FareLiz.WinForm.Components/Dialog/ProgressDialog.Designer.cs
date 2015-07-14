namespace SkyDean.FareLiz.WinForm.Components.Dialog
{
    using SkyDean.FareLiz.WinForm.Components.Utils;

    public partial class ProgressDialog
    {
        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
            this.lblText = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.imgProgress = new System.Windows.Forms.PictureBox();
            this.progressBar = new Windows7ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.imgProgress)).BeginInit();
            this.SuspendLayout();
            // 
            // lblText
            // 
            this.lblText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblText.Location = new System.Drawing.Point(35, 9);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(399, 23);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "Progress Text";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(359, 35);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Ca&ncel";
            // 
            // imgProgress
            // 
            this.imgProgress.Image = global::SkyDean.FareLiz.Core.Properties.Resources.Loading;
            this.imgProgress.Location = new System.Drawing.Point(10, 9);
            this.imgProgress.Name = "imgProgress";
            this.imgProgress.Size = new System.Drawing.Size(19, 19);
            this.imgProgress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imgProgress.TabIndex = 3;
            this.imgProgress.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.ContainerControl = this;
            this.progressBar.Location = new System.Drawing.Point(8, 35);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(343, 23);
            this.progressBar.TabIndex = 1;
            // 
            // ProgressDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(441, 69);
            this.Controls.Add(this.imgProgress);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblText);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProgressDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressDialog_FormClosing);
            this.Load += new System.EventHandler(this.ProgressDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgProgress)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
