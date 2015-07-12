using System;
using System.ComponentModel;
using System.Windows.Forms;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.WinForm.Components.Dialog;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    public partial class IntroForm : SmartForm
    {
        private bool _isNavigate = false;
        public IntroForm()
        {
            InitializeComponent();
            Text = AppUtil.CompanyName + " " + AppUtil.ProductName;
            imageList.Images.Add(Properties.Resources.SkyDeanIcon_BlackSmall);
        }

        private void IntroForm_Shown(object sender, EventArgs e)
        {
            UpdateViews(sender as Control);
        }

        private void UpdateViews(Control sender)
        {
            int activeIdx = helpTab.SelectedIndex;
            int total = helpTab.TabPages.Count;
            btnPrevious.Enabled = activeIdx > 0;
            btnNext.Enabled = activeIdx < total - 1;

            for (int i = 0; i < total; i++)
            {
                var tabPgae = helpTab.TabPages[i];
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

            if (sender == btnNext)
            {
                if (activeIdx < total - 1)
                    btnNext.Focus();
                else
                    btnCancel.Focus();
            }
            else if (sender == btnPrevious)
            {
                if (activeIdx == 0)
                    btnNext.Focus();
                else
                    btnPrevious.Focus();
            }
        }

        private void NavigateButton_Click(object sender, EventArgs e)
        {
            int activeIdx = helpTab.SelectedIndex;

            _isNavigate = true;
            if (sender == btnPrevious)
                helpTab.SelectedIndex = --activeIdx;
            else
                helpTab.SelectedIndex = ++activeIdx;
            _isNavigate = false;

            UpdateViews(sender as Control);
        }

        private void helpTab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            e.Cancel = !_isNavigate;
        }

        private void IntroForm_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            using (var about = new AboutForm())
                about.ShowDialog();
            e.Cancel = true;
        }
    }
}
