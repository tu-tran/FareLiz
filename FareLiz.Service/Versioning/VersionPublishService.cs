namespace SkyDean.FareLiz.Service.Versioning
{

    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Service used for publishing a new version</summary>
    public partial class VersionPublishService : IServiceRunner
    {
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public IConfig Configuration { get; set; }

        /// <summary>
        /// Gets the default config.
        /// </summary>
        public IConfig DefaultConfig
        {
            get
            {
                return null;
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
        public ILogger Logger { get; set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
        }

        /// <summary>
        /// The run service.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public void RunService(string[] args)
        {
            var parameters = PublishParameter.FromCommandLine(args);
            var publisher = new OnlineVersionPublisher(this.Logger);
            publisher.Publish(parameters);
        }
    }
}