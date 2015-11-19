namespace SkyDean.FareLiz.Service.LiveUpdate
{
    using System;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;

    /// <summary>Configuration for LiveUpdate</summary>
    [Serializable]
    public class LiveUpdateConfiguration : IConfig
    {
        /// <summary>Automatically check for update in background</summary>
        public bool AutoUpdate { get; set; }

        /// <summary>The interval between each attempt to check for new updates (in hours) This property requires AutoUpdate to be enabled</summary>
        public int AutoUpdateIntervalHours { get; set; }

        /// <summary>Silent mode status</summary>
        public bool Silent { get; set; }

        /// <summary>
        /// Validate the configuration
        /// </summary>
        /// <returns>
        /// The <see cref="ValidateResult"/>.
        /// </returns>
        public ValidateResult Validate()
        {
            if (this.AutoUpdateIntervalHours <= 0)
            {
                this.AutoUpdateIntervalHours = 6;
            }

            return ValidateResult.Success;
        }
    }
}