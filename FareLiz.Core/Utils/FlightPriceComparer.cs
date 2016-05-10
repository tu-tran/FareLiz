namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The flight price comparer.</summary>
    public class FlightPriceComparer : IComparer<Flight>
    {
        /// <summary>The instance.</summary>
        public static FlightPriceComparer Instance = new FlightPriceComparer();

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Compare(Flight x, Flight y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }

                return -1;
            }

            if (y == null)
            {
                return 1;
            }

            return x.Price.CompareTo(y.Price);
        }
    }
}