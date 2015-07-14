namespace SkyDean.FareLiz.Data.Monitoring
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The FareStorage interface.</summary>
    public interface IFareStorage
    {
        /// <summary>The get routes.</summary>
        /// <returns>The <see cref="List" />.</returns>
        List<StorageRoute> GetRoutes();

        /// <summary>
        /// The get travel dates.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<DatePeriod> GetTravelDates(StorageRoute route);

        /// <summary>
        /// The get data dates.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="travelDate">
        /// The travel date.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        List<DatePeriod> GetDataDates(StorageRoute route, DatePeriod travelDate);

        /// <summary>
        /// The save live fare.
        /// </summary>
        /// <param name="flights">
        /// The flights.
        /// </param>
        void SaveLiveFare(IEnumerable<Flight> flights);
    }
}