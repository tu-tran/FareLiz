namespace SkyDean.FareLiz.DropBox
{
    using System;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>Configuration object for DropBox synchronizing</summary>
    [Serializable]
    public class DropBoxSyncerConfig : IConfig
    {
        /// <summary>Initializes a new instance of the <see cref="DropBoxSyncerConfig" /> class.</summary>
        public DropBoxSyncerConfig()
        {
            this.DropBoxBaseFolder = "/";
        }

        /// <summary>Gets or sets the api key.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] ApiKey { get; set; }

        /// <summary>Gets or sets the api secret.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] ApiSecret { get; set; }

        /// <summary>Gets or sets the user token.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] UserToken { get; set; }

        /// <summary>Gets or sets the user secret.</summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] UserSecret { get; set; }

        /// <summary>Gets or sets the drop box base folder.</summary>
        [DisplayName("DropBox Base Location")]
        [Description("Base location in DropBox account")]
        public string DropBoxBaseFolder { get; set; }

        /// <summary>The validate.</summary>
        /// <returns>The <see cref="ValidateResult" />.</returns>
        public ValidateResult Validate()
        {
            string error = null;

            if (this.ApiKey == null || this.ApiKey.Length < 1 || this.ApiSecret == null || this.ApiSecret.Length < 1)
            {
                error = "User did not configure DropBox account";
            }
            else
            {
                var sameAppKey = ObjectExtension.AreEquals(this.ApiKey, DropBoxSyncConfigBuilder.ApiKey)
                                 && ObjectExtension.AreEquals(this.ApiSecret, DropBoxSyncConfigBuilder.ApiSec);
                if (sameAppKey)
                {
                    if (this.UserToken == null || this.UserToken.Length < 1 || this.UserSecret == null || this.UserSecret.Length < 1)
                    {
                        error = "User did not authorize with DropBox";
                    }
                }
                else
                {
                    error = "The authorization token is invalid. It is necessary to re-authenticate with DropBox!";
                }
            }

            var result = new ValidateResult(error == null, error);
            this.DropBoxBaseFolder = "/" + this.DropBoxBaseFolder.Replace('\\', '/').Trim('/');
            return result;
        }
    }
}