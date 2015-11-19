namespace SkyDean.FareLiz.Core.Presentation
{
    using System;

    using log4net;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>The journey browser delegate.</summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    public delegate void JourneyBrowserDelegate(IFareBrowserControl sender, JourneyEventArgs e);

    /// <summary>The FareBrowserControl interface.</summary>
    public interface IFareBrowserControl
    {
        /// <summary>Gets a value indicating whether is stopping.</summary>
        bool IsStopping { get; }

        /// <summary>Gets the timeout in seconds.</summary>
        int TimeoutInSeconds { get; }

        /// <summary>Gets the data handler.</summary>
        IFareDataProvider DataHandler { get; }

        /// <summary>Gets the last data date.</summary>
        DateTime LastDataDate { get; }

        /// <summary>Gets the last request initiated date.</summary>
        DateTime LastRequestInitiatedDate { get; }

        /// <summary>Gets the last request started date.</summary>
        DateTime LastRequestStartedDate { get; }

        /// <summary>Gets the last exception.</summary>
        Exception LastException { get; }

        /// <summary>Gets the logger.</summary>
        ILogger Logger { get; }

        /// <summary>Gets the last retrieved route.</summary>
        TravelRoute LastRetrievedRoute { get; }

        /// <summary>Gets the request state.</summary>
        DataRequestState RequestState { get; }

        /// <summary>The get journey completed.</summary>
        event JourneyBrowserDelegate GetJourneyCompleted;

        /// <summary>
        /// The request data async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="TravelRoute"/>.
        /// </returns>
        TravelRoute RequestDataAsync(FlightFareRequest request);

        /// <summary>The reset.</summary>
        void Reset();

        /// <summary>The stop.</summary>
        void Stop();
    }

    /// <summary>The FareBrowserControlFactory interface.</summary>
    public interface IFareBrowserControlFactory
    {
        /// <summary>The get browser control.</summary>
        /// <returns>The <see cref="IFareBrowserControl" />.</returns>
        IFareBrowserControl GetBrowserControl();
    }
}