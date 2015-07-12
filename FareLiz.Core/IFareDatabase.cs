using SkyDean.FareLiz.Core.Data;
using System.Collections.Generic;
using SkyDean.FareLiz.Core.Presentation;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for fare database
    /// </summary>
    public interface IFareDatabase : IObjectDatabase<TravelRoute>, IPlugin
    {
        /// <summary>
        /// Load all routes for all destinations
        /// </summary>
        /// <param name="loadJourneys">Load journeys</param>
        /// <param name="loadJourneyData">Load journey data</param>
        /// <param name="loadHistory">Load all history data. If false, only the latest data is retrieved</param>
        /// <param name="loadFlights">Load all flights for all history data</param>
        /// <returns></returns>
        IList<TravelRoute> GetRoutes(bool loadJourneys, bool loadJourneyData, bool loadHistory, bool loadFlights, IProgressCallback callback);

        /// <summary>
        /// Load journeys for selected travel route
        /// </summary>
        /// <param name="route">Selected travel route</param>
        /// <param name="loadJourneyData">Load the data for the journeys</param>
        /// <param name="loadHistory">Load all data history for the journeys</param>
        /// <param name="loadFlights">Load all flights for the journey data</param>
        void LoadData(TravelRoute route, bool loadJourneyData, bool loadHistory, bool loadFlights, IProgressCallback callback);

        /// <summary>
        /// Load flight for all history data of these journeys
        /// </summary>
        void LoadData(IList<Journey> journeys, bool loadHistory, IProgressCallback callback);

        /// <summary>
        /// Load flight for all history data of the journey
        /// </summary>
        void LoadData(Journey journey, bool loadHistory, IProgressCallback callback);

        /// <summary>
        /// Get a description string representing the statistics of the database
        /// </summary>
        /// <returns>Database statistics description</returns>
        string GetStatistics(IProgressCallback callback);
    }
}