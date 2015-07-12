namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System.Diagnostics;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Dialog;

    public class BrowserUtils
    {
        public static void Open(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                try
                {
                    // How about IE?
                    Process.Start("iexplore", url);
                }
                catch
                {
                    var browserForm = new SmartForm { Text = "Popup Browser", Icon = SkyDean.FareLiz.Core.Properties.Resources.SkyDeanBlackIcon, WindowState = FormWindowState.Maximized };
                    var browser = new WebBrowser { Dock = DockStyle.Fill };
                    browser.Navigate(url);
                    browserForm.Controls.Add(browser);
                    browserForm.Show();
                }

            }
        }
    }
}
