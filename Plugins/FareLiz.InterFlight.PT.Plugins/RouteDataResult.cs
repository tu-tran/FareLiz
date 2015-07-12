using SkyDean.FareLiz.Core.Data;

namespace SkyDean.FareLiz.InterFlight
{
    public class RouteDataResult
    {
        public DataResult ResultState { get; private set; }
        public TravelRoute ResultRoute { get; private set; }

        public RouteDataResult(DataResult resultState, TravelRoute resultRoute)
        {
            ResultState = resultState;
            ResultRoute = resultRoute;
        }
    }

    public enum DataResult { NotReady, Ready }
}
