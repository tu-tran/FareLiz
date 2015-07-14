namespace SkyDean.FareLiz.Core
{
    using System.ComponentModel;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The journey progress changed event args.</summary>
    public class JourneyProgressChangedEventArgs : ProgressChangedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JourneyProgressChangedEventArgs"/> class.
        /// </summary>
        /// <param name="percentage">
        /// The percentage.
        /// </param>
        /// <param name="resultRoute">
        /// The result route.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="stateObj">
        /// The state obj.
        /// </param>
        public JourneyProgressChangedEventArgs(int percentage, TravelRoute resultRoute, FlightFareRequest request, object stateObj)
            : base(percentage, stateObj)
        {
            this.ResultRoute = resultRoute;
            this.Request = request;
        }

        /// <summary>Gets the request.</summary>
        public FlightFareRequest Request { get; private set; }

        /// <summary>Gets the result route.</summary>
        public TravelRoute ResultRoute { get; private set; }
    }
}