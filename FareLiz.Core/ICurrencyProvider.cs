namespace SkyDean.FareLiz.Core
{
    using System.Collections.Generic;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>Interface for providing currency conversion</summary>
    public interface ICurrencyProvider : IHelperService
    {
        /// <summary>Gets or sets the allowed currencies.</summary>
        List<string> AllowedCurrencies { get; set; }

        /// <summary>
        /// The convert.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="fromCurrency">
        /// The from currency.
        /// </param>
        /// <param name="toCurrency">
        /// The to currency.
        /// </param>
        /// <param name="convertedValue">
        /// The converted value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Convert(double value, string fromCurrency, string toCurrency, out double convertedValue);

        /// <summary>
        /// The get currency info.
        /// </summary>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyInfo"/>.
        /// </returns>
        CurrencyInfo GetCurrencyInfo(string currencyCode);

        /// <summary>The get currencies.</summary>
        /// <returns>The <see cref="Dictionary" />.</returns>
        Dictionary<string, CurrencyInfo> GetCurrencies();
    }
}