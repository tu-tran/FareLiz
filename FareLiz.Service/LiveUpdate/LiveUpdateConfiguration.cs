using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;

namespace SkyDean.FareLiz.Service.LiveUpdate
{
    /// <summary>
    /// Configuration for LiveUpdate
    /// </summary>
    [Serializable]
    public class LiveUpdateConfiguration : IConfig
    {
        /// <summary>
        /// Automatically check for update in background
        /// </summary>
        public bool AutoUpdate { get; set; }

        /// <summary>
        /// The interval between each attempt to check for new updates (in hours)
        /// This property requires AutoUpdate to be enabled
        /// </summary>
        public int AutoUpdateIntervalHours { get; set; }

        /// <summary>
        /// Silent mode status
        /// </summary>
        public bool Silent { get; set; }

        /// <summary>
        /// Validate the configuration
        /// </summary>
        public ValidateResult Validate()
        {
            if (AutoUpdateIntervalHours <= 0)
                AutoUpdateIntervalHours = 6;

            return ValidateResult.Success;
        }
    }
}
