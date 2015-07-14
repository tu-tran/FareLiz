namespace SkyDean.FareLiz.Core.Presentation
{
    using System;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The journey event args.</summary>
    public class JourneyEventArgs : EventArgs
    {
        /// <summary>The empty.</summary>
        public static new readonly JourneyEventArgs Empty = new JourneyEventArgs(null, DataRequestState.Pending, DateTime.MinValue);

        /// <summary>
        /// Initializes a new instance of the <see cref="JourneyEventArgs"/> class.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="requestState">
        /// The request state.
        /// </param>
        /// <param name="initiatedDate">
        /// The initiated date.
        /// </param>
        public JourneyEventArgs(TravelRoute route, DataRequestState requestState, DateTime initiatedDate)
        {
            this.ResultRoute = route;
            this.RequestState = requestState;
            this.RequestInitiatedDate = initiatedDate;
        }

        /// <summary>Gets the result route.</summary>
        public TravelRoute ResultRoute { get; private set; }

        /// <summary>Gets the request state.</summary>
        public DataRequestState RequestState { get; private set; }

        /// <summary>Gets the request initiated date.</summary>
        public DateTime RequestInitiatedDate { get; private set; }
    }
}