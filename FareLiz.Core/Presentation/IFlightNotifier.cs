namespace SkyDean.FareLiz.Core.Presentation
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The FlightNotifier interface.</summary>
    public interface IFlightNotifier
    {
        /// <summary>
        /// The show.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="header">
        /// The header.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="timeToStay">
        /// The time to stay.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="waitTillHidden">
        /// The wait till hidden.
        /// </param>
        void Show(string title, string header, IList<FlightMonitorItem> data, int timeToStay, NotificationType type, bool waitTillHidden);
    }
}