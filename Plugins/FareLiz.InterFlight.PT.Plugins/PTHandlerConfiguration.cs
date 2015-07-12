using System;
using System.ComponentModel;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;

namespace SkyDean.FareLiz.InterFlight
{
    [Serializable]
    class PTHandlerConfiguration : IConfig
    {
        private int _simultaneousRequests = 3;
        [DisplayName("Number of maximum simultaneous requests")]
        [Description("The number of fare data requests to be sent at once to the service"), Category("Data Requests"), DefaultValue(3)]
        public int SimultaneousRequests { get { return _simultaneousRequests; } set { _simultaneousRequests = value; } }

        private int _timeOutInSeconds = 300;
        [DisplayName("Reuest Timeout (s)")]
        [Description("The timeout for each request to get fare data"), Category("Data Requests"), DefaultValue(300)]
        public int TimeoutInSeconds { get { return _timeOutInSeconds; } set { _timeOutInSeconds = value; } }

        private int _maxAirlineCount = 15;
        [DisplayName("Maximum number of airlines")]
        [Description("Maximum number of airlines fare to retrieve from the service"), Category("Data Handling"), DefaultValue(15)]
        public int MaxAirlineCount { get { return _maxAirlineCount; } set { _maxAirlineCount = value; } }

        private int _maxFlightsPerAirline = 3;
        [DisplayName("Maximum flights per airline")]
        [Description("Maximum number of flights per airline to retrieve from the service"), Category("Data Handling"), DefaultValue(3)]
        public int MaxFlightsPerAirline { get { return _maxFlightsPerAirline; } set { _maxFlightsPerAirline = value; } }

        private int _minPriceMargin = 1;
        [DisplayName("Minimum Price Margin")]
        [Description("The minimum price difference between different flights"), Category("Data Handling"), DefaultValue(1)]
        public int MinPriceMargin { get { return _minPriceMargin; } set { _minPriceMargin = value; } }

        public ValidateResult Validate()
        {
            if (SimultaneousRequests < 1)
                SimultaneousRequests = 1;
            if (MaxAirlineCount < 1)
                MaxAirlineCount = 15;
            if (MaxFlightsPerAirline < 1)
                MaxFlightsPerAirline = 3;
            if (MinPriceMargin < 0)
                MinPriceMargin = 1;
            if (TimeoutInSeconds < 30)
                TimeoutInSeconds = 300;

            return ValidateResult.Success;
        }
    }
}
