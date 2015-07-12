using System;
using System.ComponentModel;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;
using SkyDean.FareLiz.Core.Utils;

namespace SkyDean.FareLiz.DropBox
{
    /// <summary>
    /// Configuration object for DropBox synchronizing
    /// </summary>
    [Serializable]
    public class DropBoxSyncerConfig : IConfig
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] ApiKey { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] ApiSecret { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] UserToken { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] UserSecret { get; set; }

        [DisplayName("DropBox Base Location")]
        [Description("Base location in DropBox account")]
        public string DropBoxBaseFolder { get; set; }

        public DropBoxSyncerConfig()
        {
            DropBoxBaseFolder = "/";
        }

        public ValidateResult Validate()
        {
            string error = null;

            if (ApiKey == null || ApiKey.Length < 1 || ApiSecret == null || ApiSecret.Length < 1)
                error = "User did not configure DropBox account";
            else
            {
                bool sameAppKey = ObjectExtension.AreEquals(ApiKey, DropBoxSyncConfigBuilder.ApiKey)
                    && ObjectExtension.AreEquals(ApiSecret, DropBoxSyncConfigBuilder.ApiSec);
                if (sameAppKey)
                {
                    if (UserToken == null || UserToken.Length < 1 || UserSecret == null || UserSecret.Length < 1)
                        error = "User did not authorize with DropBox";
                }
                else
                    error = "The authorization token is invalid. It is necessary to re-authenticate with DropBox!";
            }

            var result = new ValidateResult(error == null, error);
            DropBoxBaseFolder = "/" + DropBoxBaseFolder.Replace('\\', '/').Trim('/');
            return result;
        }
    }
}
