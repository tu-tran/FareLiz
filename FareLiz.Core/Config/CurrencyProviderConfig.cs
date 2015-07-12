using System;
using System.Collections.Generic;

namespace SkyDean.FareLiz.Core.Config
{
    [Serializable]
    public class CurrencyProviderConfig : IConfig
    {
        public List<string> AllowedCurrencies { get; set; }

        public CurrencyProviderConfig() { }

        public ValidateResult Validate()
        {
            return ValidateResult.Success;
        }
    }
}
