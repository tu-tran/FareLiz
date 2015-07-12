namespace SkyDean.FareLiz.Core
{
    using System.ComponentModel;

    using SkyDean.FareLiz.Core.Data;

    public class JourneyProgressChangedEventArgs : ProgressChangedEventArgs
    {
        public FlightFareRequest Request { get; private set; }
        public TravelRoute ResultRoute { get; private set; }
        public JourneyProgressChangedEventArgs(int percentage, TravelRoute resultRoute, FlightFareRequest request, object stateObj)
            : base(percentage, stateObj)
        {
            this.ResultRoute = resultRoute;
            this.Request = request;
        }
    }
}