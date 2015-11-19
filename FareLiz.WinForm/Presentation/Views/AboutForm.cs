namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System.ComponentModel;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The about form.</summary>
    public sealed partial class AboutForm : SmartForm
    {
        /// <summary>Initializes a new instance of the <see cref="AboutForm" /> class.</summary>
        public AboutForm()
        {
            this.InitializeComponent();

            this.Text = this.lblProduct.Text = AppUtil.CompanyName + " " + AppUtil.ProductName;
            this.lblVersion.Text = "Version: " + AppUtil.ProductVersion;
            this.lblCopyright.Text = AppUtil.ProductCopyright;

            this.lnkEmail.Text = AppUtil.PublisherEmail;
            this.lnkEmail.Tag = "mailto:" + this.lnkEmail.Text;
            this.lnkWebsite.Text = AppUtil.PublisherUrl;
            this.lnkWebsite.Tag = this.lnkWebsite.Text;
        }

        /// <summary>
        /// The about form_ help button clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AboutForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            using (var intro = new IntroForm()) intro.ShowDialog();
        }

        /// <summary>
        /// The lnk email_ link clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void lnkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var lbl = sender as Label;
            if (!(lbl == null || lbl.Tag == null))
            {
                BrowserUtils.Open(lbl.Tag.ToString());
            }
        }
    }
}