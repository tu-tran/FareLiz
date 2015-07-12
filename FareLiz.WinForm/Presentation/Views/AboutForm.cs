using System.Windows.Forms;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.WinForm.Components.Dialog;
using SkyDean.FareLiz.WinForm.Components.Utils;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    public sealed partial class AboutForm : SmartForm
    {
        public AboutForm()
        {
            InitializeComponent();

            Text = lblProduct.Text = AppUtil.CompanyName + " " + AppUtil.ProductName;
            lblVersion.Text = "Version: " + AppUtil.ProductVersion;
            lblCopyright.Text = AppUtil.ProductCopyright;

            lnkEmail.Text = AppUtil.PublisherEmail;
            lnkEmail.Tag = "mailto:" + lnkEmail.Text;
            lnkWebsite.Text = AppUtil.PublisherUrl;
            lnkWebsite.Tag = lnkWebsite.Text;
        }

        private void AboutForm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (var intro = new IntroForm())
                intro.ShowDialog();
        }

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
