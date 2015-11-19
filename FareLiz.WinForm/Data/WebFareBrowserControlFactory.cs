namespace SkyDean.FareLiz.WinForm.Data
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Controls.Custom;

    /// <summary>
    /// The web fare browser control factory.
    /// </summary>
    internal sealed class WebFareBrowserControlFactory : Singleton<WebFareBrowserControlFactory>, IFareBrowserControlFactory
    {
        /// <summary>
        /// The get browser control.
        /// </summary>
        /// <returns>
        /// The <see cref="IFareBrowserControl"/>.
        /// </returns>
        public IFareBrowserControl GetBrowserControl()
        {
            return new WebFareBrowserControl(AppContext.MonitorEnvironment.FareDataProvider);
        }
    }
}