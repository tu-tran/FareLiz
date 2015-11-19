namespace SkyDean.FareLiz.WinForm.Config
{
    using System.Globalization;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Presentation.Views;

    /// <summary>Helper class for editing a configuration by specifying the fields individually</summary>
    public class DefaultConfigBuilder : IConfigBuilder
    {
        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultConfigBuilder"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DefaultConfigBuilder(ILogger logger)
        {
            this._logger = logger;
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
            IConfig currentConfig = targetPlugin.Configuration, 
                    defaultConfig = targetPlugin.DefaultConfig, 
                    targetConfig = currentConfig ?? defaultConfig;

            if (targetConfig != null && defaultConfig != null)
            {
                using (
                    var configDialog =
                        new ObjectBrowserDialog(
                            string.Format(CultureInfo.InvariantCulture, "{0} Configuration", targetPlugin.GetDetail().Key).Trim(), 
                            targetConfig, 
                            defaultConfig, 
                            this._logger))
                {
                    var result = configDialog.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        return configDialog.ResultObject as IConfig;
                    }
                }
            }

            return currentConfig;
        }
    }
}