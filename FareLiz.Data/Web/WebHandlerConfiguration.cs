namespace SkyDean.FareLiz.Data.Web
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The web-based handler configuration.
    /// </summary>
    [Serializable]
    public class WebHandlerConfiguration : IConfig
    {
        /// <summary>
        /// The _max airline count.
        /// </summary>
        private int _maxAirlineCount = 15;

        /// <summary>
        /// The _max flights per airline.
        /// </summary>
        private int _maxFlightsPerAirline = 3;

        /// <summary>
        /// The _min price margin.
        /// </summary>
        private int _minPriceMargin = 1;

        /// <summary>
        /// The _simultaneous requests.
        /// </summary>
        private int _simultaneousRequests = 3;

        /// <summary>
        /// The _time out in seconds.
        /// </summary>
        private int _timeOutInSeconds = 300;

        /// <summary>
        /// Gets or sets the simultaneous requests.
        /// </summary>
        [DisplayName("Number of maximum simultaneous requests")]
        [Description("The number of fare data requests to be sent at once to the service")]
        [Category("Data Requests")]
        [DefaultValue(3)]
        public int SimultaneousRequests
        {
            get
            {
                return this._simultaneousRequests;
            }

            set
            {
                this._simultaneousRequests = value;
            }
        }

        /// <summary>
        /// Gets or sets the timeout in seconds.
        /// </summary>
        [DisplayName("Reuest Timeout (s)")]
        [Description("The timeout for each request to get fare data")]
        [Category("Data Requests")]
        [DefaultValue(300)]
        public int TimeoutInSeconds
        {
            get
            {
                return this._timeOutInSeconds;
            }

            set
            {
                this._timeOutInSeconds = value;
            }
        }

        /// <summary>
        /// Gets or sets the max airline count.
        /// </summary>
        [DisplayName("Maximum number of airlines")]
        [Description("Maximum number of airlines fare to retrieve from the service")]
        [Category("Data Handling")]
        [DefaultValue(15)]
        public int MaxAirlineCount
        {
            get
            {
                return this._maxAirlineCount;
            }

            set
            {
                this._maxAirlineCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the max flights per airline.
        /// </summary>
        [DisplayName("Maximum flights per airline")]
        [Description("Maximum number of flights per airline to retrieve from the service")]
        [Category("Data Handling")]
        [DefaultValue(3)]
        public int MaxFlightsPerAirline
        {
            get
            {
                return this._maxFlightsPerAirline;
            }

            set
            {
                this._maxFlightsPerAirline = value;
            }
        }

        /// <summary>
        /// Gets or sets the min price margin.
        /// </summary>
        [DisplayName("Minimum Price Margin")]
        [Description("The minimum price difference between different flights")]
        [Category("Data Handling")]
        [DefaultValue(1)]
        public int MinPriceMargin
        {
            get
            {
                return this._minPriceMargin;
            }

            set
            {
                this._minPriceMargin = value;
            }
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <returns>
        /// The <see cref="ValidateResult"/>.
        /// </returns>
        public ValidateResult Validate()
        {
            if (this.SimultaneousRequests < 1)
            {
                this.SimultaneousRequests = 1;
            }

            if (this.MaxAirlineCount < 1)
            {
                this.MaxAirlineCount = 15;
            }

            if (this.MaxFlightsPerAirline < 1)
            {
                this.MaxFlightsPerAirline = 3;
            }

            if (this.MinPriceMargin < 0)
            {
                this.MinPriceMargin = 1;
            }

            if (this.TimeoutInSeconds < 30)
            {
                this.TimeoutInSeconds = 300;
            }

            return ValidateResult.Success;
        }
    }
}