namespace SkyDean.FareLiz.Data.Monitoring
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    public interface IFareStorage
    {
        List<StorageRoute> GetRoutes();
        List<DatePeriod> GetTravelDates(StorageRoute route);
        List<DatePeriod> GetDataDates(StorageRoute route, DatePeriod travelDate);
        void SaveLiveFare(IEnumerable<Flight> flights);
    }
}
