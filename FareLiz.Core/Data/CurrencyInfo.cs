namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>
    /// Store currency information (symbol and name)
    /// </summary>
    public struct CurrencyInfo
    {
        /// <summary>
        /// Currency symbol
        /// </summary>
        public string Symbol;

        /// <summary>
        /// Currency name
        /// </summary>
        public string FullName;

        public CurrencyInfo(string symbol, string fullName)
        {
            Symbol = symbol;
            FullName = fullName;
        }

        public override string ToString()
        {
            return FullName + " " + Symbol;
        }
    }
}
