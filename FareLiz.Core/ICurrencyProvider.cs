using SkyDean.FareLiz.Core.Data;
using System.Collections.Generic;

namespace SkyDean.FareLiz.Core
{
    /// <summary>
    /// Interface for providing currency conversion
    /// </summary>
    public interface ICurrencyProvider : IHelperService
    {
        bool Convert(double value, string fromCurrency, string toCurrency, out double convertedValue);
        CurrencyInfo GetCurrencyInfo(string currencyCode);
        Dictionary<string, CurrencyInfo> GetCurrencies();

        List<string> AllowedCurrencies { get; set; }
    }
}
