namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    public class FlightPriceComparer : IComparer<Flight>
    {
        public int Compare(Flight x, Flight y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                    return 1;
                else
                    return x.Price.CompareTo(y.Price);
            }
        }

        public static FlightPriceComparer Instance = new FlightPriceComparer();
    }
}