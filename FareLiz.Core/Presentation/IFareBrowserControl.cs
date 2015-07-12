using System;
using log4net;
using SkyDean.FareLiz.Core.Data;

namespace SkyDean.FareLiz.Core.Presentation
{
    public delegate void JourneyBrowserDelegate(IFareBrowserControl sender, JourneyEventArgs e);

    public interface IFareBrowserControl
    {
        bool IsStopping { get; }

        int TimeoutInSeconds { get; }

        IFareDataProvider DataHandler { get; }

        DateTime LastDataDate { get; }

        DateTime LastRequestInitiatedDate { get; }

        DateTime LastRequestStartedDate { get; }

        Exception LastException { get; }

        ILog Logger { get; }

        TravelRoute LastRetrievedRoute { get; }

        DataRequestState RequestState { get; }

        event JourneyBrowserDelegate GetJourneyCompleted;

        TravelRoute RequestDataAsync(FlightFareRequest request);

        void Reset();

        void Stop();
    }

    public interface IFareBrowserControlFactory
    {
        IFareBrowserControl GetBrowserControl();
    }
}