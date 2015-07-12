using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Xml;

namespace SkyDean.FareLiz.WinForm.Data
{
    public class OnlineCurrencyProvider : ICurrencyProvider
    {
        private static readonly Random Rand = new Random(DateTime.Now.Millisecond);
        private static readonly Dictionary<string, CurrencyInfo> CurrencySymbols;
        private static Dictionary<string, float> _exchangeRates = new Dictionary<string, float>();
        private static volatile HelperServiceStatus _status;
        private static readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        public void Initialize() { }

        CurrencyProviderConfig _config;
        public IConfig Configuration { get { return _config; } set { _config = value as CurrencyProviderConfig; } }
        public IConfig DefaultConfig { get { return new CurrencyProviderConfig { AllowedCurrencies = new List<string> { "EUR", "USD", "VND" } }; } }
        public IConfigBuilder CustomConfigBuilder { get { return null; } }
        public ILog Logger { get; set; }

        public List<string> AllowedCurrencies
        {
            get { return _config == null ? null : _config.AllowedCurrencies; }
            set
            {
                if (_config == null)
                    _config = new CurrencyProviderConfig();
                _config.AllowedCurrencies = value;
            }
        }

        public HelperServiceStatus Status
        {
            get { lock (Rand) { return _status; } }
            private set { lock (Rand) { _status = value; } }
        }

        public OnlineCurrencyProvider()
        {
            Status = HelperServiceStatus.Stopped;
        }

        static OnlineCurrencyProvider()
        {
            CurrencySymbols = new Dictionary<string, CurrencyInfo>();
            using (Stream dataStream = typeof(OnlineCurrencyProvider).Assembly.GetManifestResourceStream(typeof(OnlineCurrencyProvider).Namespace + "." + "Currency.txt"))
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

        public CurrencyInfo GetCurrencyInfo(string currencyCode)
        {
            CurrencyInfo result;
            if (!CurrencySymbols.TryGetValue(currencyCode, out result))
                result = new CurrencyInfo(currencyCode, String.Empty);
            return result;
        }

        public Dictionary<string, CurrencyInfo> GetCurrencies()
        {
            return new Dictionary<string, CurrencyInfo>(CurrencySymbols);
        }

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

        public void Start()
        {
            var delay = TimeSpan.FromHours(3);
            var delayStep = TimeSpan.FromSeconds(5);

            Status = HelperServiceStatus.Running;
            ThreadPool.QueueUserWorkItem(o =>
                {
                    AppUtil.NameCurrentThread(GetType().Name + "-BGCurrencyData");
                    while (Status == HelperServiceStatus.Running)
                    {
                        cacheLock.EnterWriteLock();
                        try
                        {
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.Message);
                        }
                        finally
                        {
                            cacheLock.ExitWriteLock();
                        }

                        TimeSpan slept = TimeSpan.Zero;
                        while (slept < delay)
                        {
                            if (Status != HelperServiceStatus.Running)  // Stop the delay if the service is no longer running
                                break;

                            Thread.Sleep(delayStep);
                            slept += delayStep;
                        }
                    }

                    Status = HelperServiceStatus.Stopped;
                });
        }

        public void Stop()
        {
            Status = HelperServiceStatus.Stopping;
        }

        private static void LoadData()
        {
            string url = "http://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=xml&random=" + Rand.Next().ToString(CultureInfo.InvariantCulture);
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
                                                symbol = reader.ReadElementContentAsString().Replace("=X", "");
                                            if (reader.GetAttribute("name") == "price")
                                                price = reader.ReadElementContentAsFloat();
                                        }
                                        while (reader.ReadToNextSibling("field"));
                                    }

                                    if (!String.IsNullOrEmpty(symbol) && price > -1)
                                        result.Add(symbol, price);
                                }
                            }
                            while (reader.ReadToNextSibling("resource"));
                        }
                    }
                }
            }

            if (!result.ContainsKey("USD"))
                result.Add("USD", 1);
            if (result.Count > 0)
                _exchangeRates = result;
        }
    }
}
