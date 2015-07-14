namespace SkyDean.FareLiz.WinForm.Components.Controls.Custom
{
    using System.Collections.Generic;
    using System.Globalization;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The journey comparer.</summary>
    internal class JourneyComparer : IEqualityComparer<Journey>
    {
        /// <summary>The has h_ date.</summary>
        private const string HASH_DATE = "yyMMdd";

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
        public bool Equals(Journey x, Journey y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            {
                return false;
            }

            return x.DepartureDate == y.DepartureDate && x.ReturnDate == y.ReturnDate;
        }

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <param name="journey">
        /// The journey.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetHashCode(Journey journey)
        {
            // Check whether the object is null 
            if (ReferenceEquals(journey, null))
            {
                return 0;
            }

            return int.Parse(
                string.Format("{0}{1}", journey.DepartureDate.ToString(HASH_DATE), journey.ReturnDate.ToString(HASH_DATE)), 
                CultureInfo.InvariantCulture);
        }
    }
}