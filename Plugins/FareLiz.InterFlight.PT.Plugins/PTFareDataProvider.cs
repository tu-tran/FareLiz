using System.ComponentModel;
using System.IO;
using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;
using SkyDean.FareLiz.Core.Data;

namespace SkyDean.FareLiz.InterFlight
{
    /// <summary>
    /// Data handler for International Flights (PT)

    /// </summary>
    [DisplayName("International Flight Data Handler (PT)")]
    [Description("Best covered for international flight routes")]
    public sealed partial class PTFareDataProvider : IFareDataProvider
    {
        public string ServiceName
        {
            get { return "PT"; }
        }

        private PTHandlerConfiguration _config;
        public IConfig Configuration { get { return _config; } set { _config = value as PTHandlerConfiguration; } }
        public ICurrencyProvider CurrencyProvider { get; set; }
        public bool IsConfigurable { get { return true; } }
        public IConfig DefaultConfig { get { return new PTHandlerConfiguration() { SimultaneousRequests = 2, MaxAirlineCount = 15, MaxFlightsPerAirline = 3 }; } }
        public IConfigBuilder CustomConfigBuilder { get { return null; } }
        public int SimultaneousRequests { get { return _config.SimultaneousRequests; } }
        public int TimeoutInSeconds { get { return _config.TimeoutInSeconds; } }
        public ILog Logger { get; set; }

        private const int _minPriceMargin = 3;

        public void Initialize() { }

        public void ExportData(Stream targetStream, TravelRoute route)
        {
            if (route == null || route.Journeys.Count < 1)
                return;

            var exporter = new PTDataExporter();
            exporter.ExportData(targetStream, route);
        }

        public TravelRoute ReadData(string routeStringData)
        {
            var parser = GetParser();
            var result = parser.ParseWebArchive(routeStringData);
            return (result == null ? null : result.ResultRoute);
        }

        private PTDataParser GetParser()
        {
            var parser = new PTDataParser(_root_, _config.MaxFlightsPerAirline, _config.MaxAirlineCount, 1);
            return parser;
        }
    }
}