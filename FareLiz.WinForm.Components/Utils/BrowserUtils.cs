namespace SkyDean.FareLiz.WinForm.Components.Utils
{
    using System.Diagnostics;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Properties;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>
    /// The browser utils.
    /// </summary>
    public class BrowserUtils
    {
        /// <summary>
        /// The open.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
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
                    var browserForm = new SmartForm
                                          {
                                              Text = "Popup Browser", 
                                              Icon = Resources.SkyDeanBlackIcon, 
                                              WindowState = FormWindowState.Maximized
                                          };
                    var browser = new WebBrowser { Dock = DockStyle.Fill };
                    browser.Navigate(url);
                    browserForm.Controls.Add(browser);
                    browserForm.Show();
                }
            }
        }
    }
}