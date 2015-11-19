namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Properties;

    /// <summary>
    /// The intro form.
    /// </summary>
    public partial class IntroForm : SmartForm
    {
        /// <summary>
        /// The _is navigate.
        /// </summary>
        private bool _isNavigate;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntroForm"/> class.
        /// </summary>
        public IntroForm()
        {
            this.InitializeComponent();
            this.Text = AppUtil.CompanyName + " " + AppUtil.ProductName;
            this.imageList.Images.Add(Resources.SkyDeanIcon_BlackSmall);
        }

        /// <summary>
        /// The intro form_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void IntroForm_Shown(object sender, EventArgs e)
        {
            this.UpdateViews(sender as Control);
        }

        /// <summary>
        /// The update views.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        private void UpdateViews(Control sender)
        {
            int activeIdx = this.helpTab.SelectedIndex;
            int total = this.helpTab.TabPages.Count;
            this.btnPrevious.Enabled = activeIdx > 0;
            this.btnNext.Enabled = activeIdx < total - 1;

            for (int i = 0; i < total; i++)
            {
                var tabPgae = this.helpTab.TabPages[i];
                if (i == activeIdx)
                {
                    tabPgae.ImageIndex = 0;
                    tabPgae.Text = i + 1 + "/" + total;
                }
                else
                {
                    tabPgae.ImageIndex = -1;
                    tabPgae.Text = null;
                }
            }

            if (sender == this.btnNext)
            {
                if (activeIdx < total - 1)
                {
                    this.btnNext.Focus();
                }
                else
                {
                    this.btnCancel.Focus();
                }
            }
            else if (sender == this.btnPrevious)
            {
                if (activeIdx == 0)
                {
                    this.btnNext.Focus();
                }
                else
                {
                    this.btnPrevious.Focus();
                }
            }
        }

        /// <summary>
        /// The navigate button_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void NavigateButton_Click(object sender, EventArgs e)
        {
            int activeIdx = this.helpTab.SelectedIndex;

            this._isNavigate = true;
            if (sender == this.btnPrevious)
            {
                this.helpTab.SelectedIndex = --activeIdx;
            }
            else
            {
                this.helpTab.SelectedIndex = ++activeIdx;
            }

            this._isNavigate = false;

            this.UpdateViews(sender as Control);
        }

        /// <summary>
        /// The help tab_ selecting.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void helpTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = !this._isNavigate;
        }

        /// <summary>
        /// The intro form_ help button clicked.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void IntroForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            using (var about = new AboutForm()) about.ShowDialog();
            e.Cancel = true;
        }
    }
}