namespace SkyDean.FareLiz.InterFlight
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using System.ComponentModel;
    using System.IO;

    using SkyDean.FareLiz.Data.Monitoring;

    /// <summary>Data handler for International Flights (PT)</summary>
    [DisplayName("International Flight Data Handler (PT)")]
    [Description("Best covered for international flight routes")]
    public sealed partial class PTFareDataProvider : IFareDataProvider
    {
        /// <summary>
        /// The _min price margin.
        /// </summary>
        private const int _minPriceMargin = 3;

        /// <summary>
        /// The _config.
        /// </summary>
        private PTHandlerConfiguration _config;

        /// <summary>
        /// Gets a value indicating whether is configurable.
        /// </summary>
        public bool IsConfigurable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the service name.
        /// </summary>
        public string ServiceName
        {
            get
            {
                return "PT";
            }
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
                this._config = value as PTHandlerConfiguration;
            }
        }

        /// <summary>
        /// Gets or sets the currency provider.
        /// </summary>
        public ICurrencyProvider CurrencyProvider { get; set; }

        /// <summary>
        /// Gets the default config.
        /// </summary>
        public IConfig DefaultConfig
        {
            get
            {
                return new PTHandlerConfiguration { SimultaneousRequests = 2, MaxAirlineCount = 15, MaxFlightsPerAirline = 3 };
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
        /// Gets the simultaneous requests.
        /// </summary>
        public int SimultaneousRequests
        {
            get
            {
                return this._config.SimultaneousRequests;
            }
        }

        /// <summary>
        /// Gets the timeout in seconds.
        /// </summary>
        public int TimeoutInSeconds
        {
            get
            {
                return this._config.TimeoutInSeconds;
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// The export data.
        /// </summary>
        /// <param name="targetStream">
        /// The target stream.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        public void ExportData(Stream targetStream, TravelRoute route)
        {
            if (route == null || route.Journeys.Count < 1)
            {
                return;
            }

            var exporter = new PTDataExporter();
            exporter.ExportData(targetStream, route);
        }

        /// <summary>
        /// The read data.
        /// </summary>
        /// <param name="routeStringData">
        /// The route string data.
        /// </param>
        /// <returns>
        /// The <see cref="TravelRoute"/>.
        /// </returns>
        public TravelRoute ReadData(string routeStringData)
        {
            //TODO: FIX ME
            var parser = this.GetParser(null);
            var result = parser.ParseWebArchive(routeStringData);
            return result == null ? null : result.ResultRoute;
        }

        /// <summary>
        /// The get parser.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The <see cref="PTDataParser" />.
        /// </returns>
        private PTDataParser GetParser(FlightFareRequest request)
        {
            var parser = new PTDataParser(this._root_, this._config.MaxFlightsPerAirline, this._config.MaxAirlineCount, 1)
                             {
                                 Departure = request.Departure,
                                 Destination = request.Destination
                             };
            return parser;
        }
    }
}