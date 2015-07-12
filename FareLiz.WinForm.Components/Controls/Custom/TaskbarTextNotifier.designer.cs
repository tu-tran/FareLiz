namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    partial class TaskbarTextNotifier
    {
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtContent = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // txtContent
            // 
            this.txtContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtContent.AutoWordSelection = true;
            this.txtContent.BackColor = System.Drawing.Color.Ivory;
            this.txtContent.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtContent.Location = new System.Drawing.Point(10, 10);
            this.txtContent.Name = "txtContent";
            this.txtContent.ReadOnly = true;
            this.txtContent.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.txtContent.Size = new System.Drawing.Size(278, 51);
            this.txtContent.TabIndex = 0;
            this.txtContent.Text = "";
            this.txtContent.WordWrap = false;
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.RichTextBox txtContent;
    }
}