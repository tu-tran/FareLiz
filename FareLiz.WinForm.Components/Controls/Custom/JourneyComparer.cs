namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using SkyDean.FareLiz.Core.Data;

    internal class JourneyComparer : IEqualityComparer<Journey>
    {
        private const string HASH_DATE = "yyMMdd";

        public bool Equals(Journey x, Journey y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;
            return (x.DepartureDate == y.DepartureDate && x.ReturnDate == y.ReturnDate);
        }

        public int GetHashCode(Journey journey)
        {
            //Check whether the object is null 
            if (ReferenceEquals(journey, null)) return 0;

            return
                Int32.Parse(String.Format("{0}{1}", journey.DepartureDate.ToString(HASH_DATE),
                                          journey.ReturnDate.ToString(HASH_DATE)), CultureInfo.InvariantCulture);
        }
    }
}