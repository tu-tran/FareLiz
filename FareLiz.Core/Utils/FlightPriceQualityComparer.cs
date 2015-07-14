namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The flight price quality comparer.</summary>
    public class FlightPriceQualityComparer : IEqualityComparer<Flight>
    {
        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(Flight x, Flight y)
        {
            return (x.InboundLeg.Departure == y.InboundLeg.Departure) && (x.OutboundLeg.Departure == y.OutboundLeg.Departure) && (x.Price == y.Price)
                   && (x.Operator == y.Operator);
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetHashCode(Flight obj)
        {
            return obj.Operator.GetHashCode() + obj.OutboundLeg.Departure.GetHashCode() + obj.Price.GetHashCode();
        }
    }
}