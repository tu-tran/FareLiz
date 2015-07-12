using System.Diagnostics;

namespace SkyDean.FareLiz.Core.Data
{
    public enum DataRequestState { Pending = 0, Requested = 1, NoData = 3, Failed = 4, Ok = 5 }

    [DebuggerDisplay("{RequestState} - {ResultRoute}")]
    public struct DataRequestResult
    {
        public readonly DataRequestState RequestState;
        public readonly TravelRoute ResultRoute;

        public DataRequestResult(DataRequestState requestState, TravelRoute resultRoute)
        {
            RequestState = requestState;
            ResultRoute = resultRoute;
        }

        public static readonly DataRequestResult Empty = new DataRequestResult(DataRequestState.Pending, null);
    }
}
