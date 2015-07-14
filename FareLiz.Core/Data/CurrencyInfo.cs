namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>Store currency information (symbol and name)</summary>
    public struct CurrencyInfo
    {
        /// <summary>Currency name</summary>
        public string FullName;

        /// <summary>Currency symbol</summary>
        public string Symbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyInfo"/> struct.
        /// </summary>
        /// <param name="symbol">
        /// The symbol.
        /// </param>
        /// <param name="fullName">
        /// The full name.
        /// </param>
        public CurrencyInfo(string symbol, string fullName)
        {
            this.Symbol = symbol;
            this.FullName = fullName;
        }

        /// <summary>The to string.</summary>
        /// <returns>The <see cref="string" />.</returns>
        public override string ToString()
        {
            return this.FullName + " " + this.Symbol;
        }
    }
}