namespace SkyDean.FareLiz.Core.Data
{
    using System.Diagnostics;

    /// <summary>The data request state.</summary>
    public enum DataRequestState
    {
        /// <summary>The pending.</summary>
        Pending = 0, 

        /// <summary>The requested.</summary>
        Requested = 1, 

        /// <summary>The no data.</summary>
        NoData = 3, 

        /// <summary>The failed.</summary>
        Failed = 4, 

        /// <summary>The ok.</summary>
        Ok = 5
    }

    /// <summary>The data request result.</summary>
    [DebuggerDisplay("{RequestState} - {ResultRoute}")]
    public struct DataRequestResult
    {
        /// <summary>The empty.</summary>
        public static readonly DataRequestResult Empty = new DataRequestResult(DataRequestState.Pending, null);

        /// <summary>The request state.</summary>
        public readonly DataRequestState RequestState;

        /// <summary>The result route.</summary>
        public readonly TravelRoute ResultRoute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRequestResult"/> struct.
        /// </summary>
        /// <param name="requestState">
        /// The request state.
        /// </param>
        /// <param name="resultRoute">
        /// The result route.
        /// </param>
        public DataRequestResult(DataRequestState requestState, TravelRoute resultRoute)
        {
            this.RequestState = requestState;
            this.ResultRoute = resultRoute;
        }
    }
}