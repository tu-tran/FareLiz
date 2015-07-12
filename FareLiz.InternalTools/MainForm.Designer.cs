namespace FareLiz.InternalTools
{
    partial class MainForm
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
            this.txtRaw = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEncoded = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbGrep = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtRaw
            // 
            this.txtRaw.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRaw.Location = new System.Drawing.Point(69, 40);
            this.txtRaw.Multiline = true;
            this.txtRaw.Name = "txtRaw";
            this.txtRaw.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRaw.Size = new System.Drawing.Size(402, 86);
            this.txtRaw.TabIndex = 0;
            this.txtRaw.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.txtRaw.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Raw";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Encoded";
            // 
            // txtEncoded
            // 
            this.txtEncoded.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEncoded.Location = new System.Drawing.Point(69, 141);
            this.txtEncoded.Multiline = true;
            this.txtEncoded.Name = "txtEncoded";
            this.txtEncoded.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtEncoded.Size = new System.Drawing.Size(402, 103);
            this.txtEncoded.TabIndex = 2;
            this.txtEncoded.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            this.txtEncoded.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Grep";
            // 
            // cbGrep
            // 
            this.cbGrep.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGrep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGrep.FormattingEnabled = true;
            this.cbGrep.Location = new System.Drawing.Point(69, 12);
            this.cbGrep.Name = "cbGrep";
            this.cbGrep.Size = new System.Drawing.Size(402, 21);
            this.cbGrep.TabIndex = 5;
            this.cbGrep.SelectedIndexChanged += new System.EventHandler(this.cbGrep_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 256);
            this.Controls.Add(this.cbGrep);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtEncoded);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRaw);
            this.Name = "MainForm";
            this.Text = "Text Encoder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRaw;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEncoded;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbGrep;
    }
}

