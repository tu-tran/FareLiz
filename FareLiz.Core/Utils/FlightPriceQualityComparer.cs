namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    public class FlightPriceQualityComparer : IEqualityComparer<Flight>
    {
        public bool Equals(Flight x, Flight y)
        {
            return (x.InboundLeg.Departure == y.InboundLeg.Departure) &&
                   (x.OutboundLeg.Departure == y.OutboundLeg.Departure) && (x.Price == y.Price) &&
                   (x.Operator == y.Operator);
        }

        public int GetHashCode(Flight obj)
        {
            return obj.Operator.GetHashCode() + obj.OutboundLeg.Departure.GetHashCode() + obj.Price.GetHashCode();
        }
    }
}