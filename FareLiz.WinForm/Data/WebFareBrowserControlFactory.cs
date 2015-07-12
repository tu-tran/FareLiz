using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Presentation;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.WinForm.Components.Controls.Custom;

namespace SkyDean.FareLiz.WinForm.Data
{
    internal sealed class WebFareBrowserControlFactory : Singleton<WebFareBrowserControlFactory>, IFareBrowserControlFactory
    {
        public IFareBrowserControl GetBrowserControl()
        {
            return new WebFareBrowserControl(AppContext.MonitorEnvironment.FareDataProvider);
        }
    }
}
