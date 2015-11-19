namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>The journey date equality comparer.</summary>
    public class JourneyDateEqualityComparer : IEqualityComparer<Journey>
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
        public bool Equals(Journey x, Journey y)
        {
            return (x.DepartureDate.Date == y.DepartureDate.Date) && (x.ReturnDate.Date == y.ReturnDate.Date);
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
        public int GetHashCode(Journey obj)
        {
            return obj.DepartureDate.GetHashCode() + obj.ReturnDate.GetHashCode();
        }
    }
}