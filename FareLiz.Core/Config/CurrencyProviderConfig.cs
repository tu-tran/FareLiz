namespace SkyDean.FareLiz.Core.Config
{
    using System;
    using System.Collections.Generic;

    /// <summary>The currency provider config.</summary>
    [Serializable]
    public class CurrencyProviderConfig : IConfig
    {
        /// <summary>Gets or sets the allowed currencies.</summary>
        public List<string> AllowedCurrencies { get; set; }

        /// <summary>The validate.</summary>
        /// <returns>The <see cref="ValidateResult" />.</returns>
        public ValidateResult Validate()
        {
            return ValidateResult.Success;
        }
    }
}