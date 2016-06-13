namespace SkyDean.FareLiz.Data.Web
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    using System.IO;

    /// <summary>Web-based data provider.</summary>
    public abstract class WebDataProviderBase : IFareDataProvider
    {
        /// <summary>
        /// The _config.
        /// </summary>
        private WebHandlerConfiguration _config;

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
        public abstract string ServiceName { get; }

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
                this._config = value as WebHandlerConfiguration;
            }
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public WebHandlerConfiguration Config
        {
            get
            {
                return this._config;
            }

            set
            {
                this._config = value;
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
                return new WebHandlerConfiguration { SimultaneousRequests = 2, MaxAirlineCount = 15, MaxFlightsPerAirline = 3 };
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
        /// Query fare data asynchronously
        /// </summary>
        /// <param name="request">
        /// </param>
        /// <param name="progressChangedHandler">
        /// The progress Changed Handler.
        /// </param>
        /// <returns>
        /// The <see cref="DataRequestResult"/>.
        /// </returns>
        public abstract DataRequestResult QueryData(FlightFareRequest request, JourneyProgressChangedEventHandler progressChangedHandler);

        /// <summary>
        /// The export data.
        /// </summary>
        /// <param name="targetStream">
        /// The target stream.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        public abstract void ExportData(Stream targetStream, TravelRoute route);

        /// <summary>
        /// The read data.
        /// </summary>
        /// <param name="routeStringData">
        /// The route string data.
        /// </param>
        /// <returns>
        /// The <see cref="TravelRoute"/>.
        /// </returns>
        public abstract TravelRoute ReadData(string routeStringData);
    }
}