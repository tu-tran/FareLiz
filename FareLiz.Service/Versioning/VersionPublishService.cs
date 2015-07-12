using log4net;
using SkyDean.FareLiz.Core.Config;

namespace SkyDean.FareLiz.Service.Versioning
{
    /// <summary>
    /// Service used for publishing a new version
    /// </summary>
    public partial class VersionPublishService : IServiceRunner
    {
        public IConfig Configuration { get; set; }
        public IConfig DefaultConfig { get { return null; } }
        public IConfigBuilder CustomConfigBuilder { get { return null; } }
        public ILog Logger { get; set; }

        public void Initialize()
        {
        }

        public void RunService(string[] args)
        {
            var parameters = PublishParameter.FromCommandLine(args);
            var publisher = new OnlineVersionPublisher(Logger);
            publisher.Publish(parameters);
        }
    }
}
