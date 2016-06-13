namespace SkyDean.FareLiz.InterFlight
{
    using SkyDean.FareLiz.Core.Data;

    /// <summary>
    /// The route data result.
    /// </summary>
    public class RouteDataResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteDataResult"/> class.
        /// </summary>
        /// <param name="resultState">
        /// The result state.
        /// </param>
        /// <param name="resultRoute">
        /// The result route.
        /// </param>
        public RouteDataResult(DataResult resultState, TravelRoute resultRoute)
        {
            this.ResultState = resultState;
            this.ResultRoute = resultRoute;
        }

        /// <summary>
        /// Gets the result state.
        /// </summary>
        public DataResult ResultState { get; private set; }

        /// <summary>
        /// Gets the result route.
        /// </summary>
        public TravelRoute ResultRoute { get; private set; }
    }

    /// <summary>
    /// The data result.
    /// </summary>
    public enum DataResult
    {
        /// <summary>
        /// The not ready.
        /// </summary>
        NotReady, 

        /// <summary>
        /// The ready.
        /// </summary>
        Ready
    }
}