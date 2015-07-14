namespace SkyDean.FareLiz.Core.Utils
{
    using System.Collections;

    /// <summary>The numeric comparer.</summary>
    public class NumericComparer : IComparer
    {
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
        public int Compare(object x, object y)
        {
            return StringLogicalComparer.Compare(x.ToString(), y.ToString());
        }
    }
}