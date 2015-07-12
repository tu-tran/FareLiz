using SkyDean.FareLiz.Core.Data;

namespace SkyDean.FareLiz.Core.Presentation
{
    using System.Collections.Generic;    

    public interface IFlightNotifier
    {
        void Show(string title, string header, IList<FlightMonitorItem> data, int timeToStay, NotificationType type, bool waitTillHidden);
    }
}