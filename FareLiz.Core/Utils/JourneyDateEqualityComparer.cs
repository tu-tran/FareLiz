namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    public class JourneyDateEqualityComparer : IEqualityComparer<Journey>
    {
        public bool Equals(Journey x, Journey y)
        {
            return (x.DepartureDate.Date == y.DepartureDate.Date) && (x.ReturnDate.Date == y.ReturnDate.Date);
        }

        public int GetHashCode(Journey obj)
        {
            return obj.DepartureDate.GetHashCode() + obj.ReturnDate.GetHashCode();
        }
    }
}