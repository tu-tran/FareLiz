namespace SkyDean.FareLiz.Core
{
    using SkyDean.FareLiz.Core.Data;
    using System.IO;

    /// <summary>Interface for helper objects which are used to process the fare data</summary>
    public interface IFareDataProvider : IPlugin
    {
        /// <summary>Name of the data provider</summary>
        string ServiceName { get; }

        /// <summary>Support for multiple requests at the same time</summary>
        int SimultaneousRequests { get; }

        /// <summary>The timeout for each request (in seconds)</summary>
        int TimeoutInSeconds { get; }

        /// <summary>Currency Provider object</summary>
        ICurrencyProvider CurrencyProvider { get; set; }

        /// <summary>
        /// Query fare data asynchronously
        /// </summary>
        /// <param name="request">
        /// </param>
        /// <param name="progressChangedHandler">
        /// The progress Changed Handler.
        /// </param>
        /// <returns>
        /// The <see cref="DataRequestResult"/>.
        /// </returns>
        DataRequestResult QueryData(FlightFareRequest request, JourneyProgressChangedEventHandler progressChangedHandler);

        /// <summary>
        /// Export data into stream
        /// </summary>
        /// <param name="targetStream">
        /// The target Stream.
        /// </param>
        /// <param name="journey">
        /// The journey.
        /// </param>
        void ExportData(Stream targetStream, TravelRoute journey);

        /// <summary>
        /// Convert string data into journey data
        /// </summary>
        /// <param name="routeStringData">
        /// Input string (e.g. XML, base64)
        /// </param>
        /// <returns>
        /// Journey data
        /// </returns>
        TravelRoute ReadData(string routeStringData);
    }

    /// <summary>The journey progress changed event handler.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The event args.</param>
    public delegate void JourneyProgressChangedEventHandler(object sender, JourneyProgressChangedEventArgs eventArgs);
}