using System;

namespace SkyDean.FareLiz.Core.Presentation
{
    using SkyDean.FareLiz.Core.Data;

    public class JourneyEventArgs : EventArgs
    {
        public TravelRoute ResultRoute { get; private set; }
        public DataRequestState RequestState { get; private set; }
        public DateTime RequestInitiatedDate { get; private set; }

        public JourneyEventArgs(TravelRoute route, DataRequestState requestState, DateTime initiatedDate)
        {
            this.ResultRoute = route;
            this.RequestState = requestState;
            this.RequestInitiatedDate = initiatedDate;
        }

        public static readonly new JourneyEventArgs Empty = new JourneyEventArgs(null, DataRequestState.Pending, DateTime.MinValue);
    }
}
