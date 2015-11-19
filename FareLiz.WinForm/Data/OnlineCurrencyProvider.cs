namespace SkyDean.FareLiz.WinForm.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Xml;

    using log4net;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>
    /// The online currency provider.
    /// </summary>
    public class OnlineCurrencyProvider : ICurrencyProvider
    {
        /// <summary>
        /// The rand.
        /// </summary>
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// The currency symbols.
        /// </summary>
        private static readonly Dictionary<string, CurrencyInfo> CurrencySymbols;

        /// <summary>
        /// The _exchange rates.
        /// </summary>
        private static Dictionary<string, float> _exchangeRates = new Dictionary<string, float>();

        /// <summary>
        /// The _status.
        /// </summary>
        private static volatile HelperServiceStatus _status;

        /// <summary>
        /// The cache lock.
        /// </summary>
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        /// <summary>
        /// The _config.
        /// </summary>
        private CurrencyProviderConfig _config;

        /// <summary>
        /// Initializes static members of the <see cref="OnlineCurrencyProvider"/> class.
        /// </summary>
        static OnlineCurrencyProvider()
        {
            CurrencySymbols = new Dictionary<string, CurrencyInfo>();
            using (
                Stream dataStream =
                    typeof(OnlineCurrencyProvider).Assembly.GetManifestResourceStream(typeof(OnlineCurrencyProvider).Namespace + "." + "Currency.txt")
                )
            {
                using (StreamReader sr = new StreamReader(dataStream))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var parts = line.Split('\t');
                        CurrencySymbols.Add(parts[0], new CurrencyInfo(parts[1], parts[2]));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineCurrencyProvider"/> class.
        /// </summary>
        public OnlineCurrencyProvider()
        {
            this.Status = HelperServiceStatus.Stopped;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public IConfig Configuration
        {
            get
            {
                return this._config;
            }

            set
            {
                this._config = value as CurrencyProviderConfig;
            }
        }

        /// <summary>
        /// Gets the default config.
        /// </summary>
        public IConfig DefaultConfig
        {
            get
            {
                return new CurrencyProviderConfig { AllowedCurrencies = new List<string> { "EUR", "USD", "VND" } };
            }
        }

        /// <summary>
        /// Gets the custom config builder.
        /// </summary>
        public IConfigBuilder CustomConfigBuilder
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// Gets or sets the allowed currencies.
        /// </summary>
        public List<string> AllowedCurrencies
        {
            get
            {
                return this._config == null ? null : this._config.AllowedCurrencies;
            }

            set
            {
                if (this._config == null)
                {
                    this._config = new CurrencyProviderConfig();
                }

                this._config.AllowedCurrencies = value;
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        public HelperServiceStatus Status
        {
            get
            {
                lock (Rand)
                {
                    return _status;
                }
            }

            private set
            {
                lock (Rand)
                {
                    _status = value;
                }
            }
        }

        /// <summary>
        /// The get currency info.
        /// </summary>
        /// <param name="currencyCode">
        /// The currency code.
        /// </param>
        /// <returns>
        /// The <see cref="CurrencyInfo"/>.
        /// </returns>
        public CurrencyInfo GetCurrencyInfo(string currencyCode)
        {
            CurrencyInfo result;
            if (!CurrencySymbols.TryGetValue(currencyCode, out result))
            {
                result = new CurrencyInfo(currencyCode, string.Empty);
            }

            return result;
        }

        /// <summary>
        /// The get currencies.
        /// </summary>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        public Dictionary<string, CurrencyInfo> GetCurrencies()
        {
            return new Dictionary<string, CurrencyInfo>(CurrencySymbols);
        }

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
        public bool Convert(double value, string fromCurrency, string toCurrency, out double convertedValue)
        {
            cacheLock.EnterReadLock();
            try
            {
                float fromRate, toRate;
                if (_exchangeRates.TryGetValue(fromCurrency, out fromRate) && _exchangeRates.TryGetValue(toCurrency, out toRate))
                {
                    convertedValue = value / fromRate * toRate;
                    return true;
                }

                convertedValue = value;
                return false;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            var delay = TimeSpan.FromHours(3);
            var delayStep = TimeSpan.FromSeconds(5);

            this.Status = HelperServiceStatus.Running;
            ThreadPool.QueueUserWorkItem(
                o =>
                    {
                        AppUtil.NameCurrentThread(this.GetType().Name + "-BGCurrencyData");
                        while (this.Status == HelperServiceStatus.Running)
                        {
                            cacheLock.EnterWriteLock();
                            try
                            {
                                LoadData();
                            }
                            catch (Exception ex)
                            {
                                this.Logger.Error(ex.Message);
                            }
                            finally
                            {
                                cacheLock.ExitWriteLock();
                            }

                            TimeSpan slept = TimeSpan.Zero;
                            while (slept < delay)
                            {
                                if (this.Status != HelperServiceStatus.Running)
                                {
                                    // Stop the delay if the service is no longer running
                                    break;
                                }

                                Thread.Sleep(delayStep);
                                slept += delayStep;
                            }
                        }

                        this.Status = HelperServiceStatus.Stopped;
                    });
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.Status = HelperServiceStatus.Stopping;
        }

        /// <summary>
        /// The load data.
        /// </summary>
        private static void LoadData()
        {
            string url = "http://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=xml&random="
                         + Rand.Next().ToString(CultureInfo.InvariantCulture);
            var wc = new WebClient();
            var resultStr = wc.DownloadString(url);
            var result = new Dictionary<string, float>();

            using (var strReader = new StringReader(resultStr))
            {
                using (var reader = XmlReader.Create(strReader))
                {
                    if (reader.ReadToFollowing("resources"))
                    {
                        if (reader.ReadToDescendant("resource"))
                        {
                            do
                            {
                                if (reader.GetAttribute("classname") == "Quote")
                                {
                                    string symbol = null;
                                    float price = -1;
                                    if (reader.ReadToDescendant("field"))
                                    {
                                        do
                                        {
                                            if (reader.GetAttribute("name") == "symbol")
                                            {
                                                symbol = reader.ReadElementContentAsString().Replace("=X", string.Empty);
                                            }

                                            if (reader.GetAttribute("name") == "price")
                                            {
                                                price = reader.ReadElementContentAsFloat();
                                            }
                                        }
                                        while (reader.ReadToNextSibling("field"));
                                    }

                                    if (!string.IsNullOrEmpty(symbol) && price > -1)
                                    {
                                        result.Add(symbol, price);
                                    }
                                }
                            }
                            while (reader.ReadToNextSibling("resource"));
                        }
                    }
                }
            }

            if (!result.ContainsKey("USD"))
            {
                result.Add("USD", 1);
            }

            if (result.Count > 0)
            {
                _exchangeRates = result;
            }
        }
    }
}