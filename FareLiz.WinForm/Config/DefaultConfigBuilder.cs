using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;
using SkyDean.FareLiz.WinForm.Presentation;
using System;
using System.Globalization;
using SkyDean.FareLiz.WinForm.Presentation.Views;

namespace SkyDean.FareLiz.WinForm.Config
{
    /// <summary>
    /// Helper class for editing a configuration by specifying the fields individually
    /// </summary>
    public class DefaultConfigBuilder : IConfigBuilder
    {
        private readonly ILog _logger;

        public DefaultConfigBuilder(ILog logger)
        {
            _logger = logger;
        }

        public IConfig Configure(IPlugin targetPlugin)
        {
            IConfig currentConfig = targetPlugin.Configuration,
                    defaultConfig = targetPlugin.DefaultConfig,
                    targetConfig = currentConfig ?? defaultConfig;

            if (targetConfig != null && defaultConfig != null)
            {
                using (var configDialog = new ObjectBrowserDialog(String.Format(CultureInfo.InvariantCulture, "{0} Configuration", targetPlugin.GetDetail().Key).Trim(),
                    targetConfig, defaultConfig, _logger))
                {
                    var result = configDialog.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        return configDialog.ResultObject as IConfig;
                    }
                }
            }

            return currentConfig;
        }
    }
}
