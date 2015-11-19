#if DEBUG

#endif
namespace SkyDean.FareLiz.DropBox
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Utils;
    using System.Windows.Forms;

    /// <summary>Helper object for configuring and authorizing DropBox account</summary>
    public class DropBoxSyncConfigBuilder : IConfigBuilder
    {
        /// <summary>
        /// The api key.
        /// </summary>
        internal static readonly byte[] ApiKey =
            {
                0x65, 0x38, 0x38, 0x64, 0x35, 0x64, 0x35, 0x38, 0x33, 0x61, 0x38, 0x38, 0x62, 0x39, 0x33, 0x34, 
                0x66, 0x34, 0x64, 0x61, 0x63, 0x39, 0x36, 0x32, 0x30, 0x64, 0x65, 0x32, 0x66, 0x63, 0x65, 0x62, 
                0x65, 0x31, 0x39, 0x64, 0x63, 0x64, 0x37, 0x33, 0x34, 0x34, 0x62, 0x62, 0x65, 0x64, 0x64, 0x32
            };

        /// <summary>
        /// The api sec.
        /// </summary>
        internal static readonly byte[] ApiSec =
            {
                0x65, 0x38, 0x38, 0x64, 0x35, 0x64, 0x35, 0x38, 0x33, 0x61, 0x38, 0x38, 0x62, 0x39, 0x33, 0x34, 
                0x65, 0x63, 0x39, 0x63, 0x38, 0x62, 0x36, 0x30, 0x34, 0x62, 0x61, 0x30, 0x66, 0x62, 0x62, 0x62, 
                0x65, 0x32, 0x39, 0x63, 0x63, 0x30, 0x37, 0x62, 0x34, 0x61, 0x61, 0x30, 0x66, 0x36, 0x64, 0x32
            };

        /// <summary>
        /// The seed.
        /// </summary>
        internal static readonly byte[] Seed = { 0x17, 0x08, 0x88, 0x13, 0x86, 0x89, 0x05, 0x09 };

        /// <summary>
        /// The _data grep.
        /// </summary>
        private readonly DataGrep _dataGrep;

        /// <summary>
        /// The _logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropBoxSyncConfigBuilder"/> class.
        /// </summary>
        public DropBoxSyncConfigBuilder()
            : this(LogUtil.GetLogger())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DropBoxSyncConfigBuilder"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DropBoxSyncConfigBuilder(ILogger logger)
        {
            this._logger = logger;
            this._dataGrep = new DataGrep(Seed);
        }

        /// <summary>
        /// The configure.
        /// </summary>
        /// <param name="targetPlugin">
        /// The target plugin.
        /// </param>
        /// <returns>
        /// The <see cref="IConfig"/>.
        /// </returns>
        public IConfig Configure(IPlugin targetPlugin)
        {
            using (
                var configDialog = new DropBoxConfigDialog(
                    ApiKey,
                    ApiSec,
                    targetPlugin.Configuration as DropBoxSyncerConfig,
                    this._dataGrep,
                    this._logger))
            {
                var result = configDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (configDialog.ResultConfig != null)
                    {
                        return configDialog.ResultConfig;
                    }
                }
            }

            return targetPlugin.Configuration;
        }
    }
}