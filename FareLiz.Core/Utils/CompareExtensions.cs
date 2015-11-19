namespace SkyDean.FareLiz.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>The compare extensions.</summary>
    public static class CompareExtensions
    {
        /// <summary>
        /// The distinct by.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="identitySelector">
        /// The identity selector.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TIdentity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(By(identitySelector));
        }

        /// <summary>
        /// The by.
        /// </summary>
        /// <param name="identitySelector">
        /// The identity selector.
        /// </param>
        /// <typeparam name="TSource">
        /// </typeparam>
        /// <typeparam name="TIdentity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEqualityComparer"/>.
        /// </returns>
        public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateComparer<TSource, TIdentity>(identitySelector);
        }

        /// <summary>
        /// The delegate comparer.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <typeparam name="TIdentity">
        /// </typeparam>
        private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
        {
            /// <summary>The identity selector.</summary>
            private readonly Func<T, TIdentity> identitySelector;

            /// <summary>
            /// Initializes a new instance of the <see cref="DelegateComparer{T,TIdentity}"/> class.
            /// </summary>
            /// <param name="identitySelector">
            /// The identity selector.
            /// </param>
            public DelegateComparer(Func<T, TIdentity> identitySelector)
            {
                this.identitySelector = identitySelector;
            }

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
            public bool Equals(T x, T y)
            {
                return Equals(this.identitySelector(x), this.identitySelector(y));
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
            public int GetHashCode(T obj)
            {
                return this.identitySelector(obj).GetHashCode();
            }
        }
    }
}