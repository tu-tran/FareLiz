namespace SkyDean.FareLiz.DropBox
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using DropNet;
    using DropNet.Models;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Controls;
    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>The drop box config dialog.</summary>
    public partial class DropBoxConfigDialog : SmartForm
    {
        /// <summary>The validate str.</summary>
        private const string validateStr = "Validating current DropBox authorization status...";

        /// <summary>The success str.</summary>
        private const string successStr = "DropBox authorization is successful";

        /// <summary>The fail str.</summary>
        private const string failStr = "DropBox authorization failed!";

        /// <summary>The _client.</summary>
        private readonly DropNetClient _client;

        /// <summary>The _data grep.</summary>
        private readonly DataGrep _dataGrep;

        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>The _need save.</summary>
        private readonly bool _needSave;

        /// <summary>
        /// Initializes a new instance of the <see cref="DropBoxConfigDialog"/> class.
        /// </summary>
        /// <param name="apiKey">
        /// The api key.
        /// </param>
        /// <param name="apiSecret">
        /// The api secret.
        /// </param>
        /// <param name="curConfig">
        /// The cur config.
        /// </param>
        /// <param name="dataGrep">
        /// The data grep.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public DropBoxConfigDialog(byte[] apiKey, byte[] apiSecret, DropBoxSyncerConfig curConfig, DataGrep dataGrep, ILogger logger)
        {
            this.InitializeComponent();
            this._dataGrep = dataGrep;
            this._client = new DropNetClient(
                this._dataGrep.Convert(apiKey), 
                this._dataGrep.Convert(apiSecret), 
                this._dataGrep.Convert(curConfig.UserToken), 
                this._dataGrep.Convert(curConfig.UserSecret));
            this.ResultConfig = curConfig == null ? new DropBoxSyncerConfig() : curConfig.ReflectionDeepClone(logger);
            this.ResultConfig.ApiKey = apiKey;
            this.ResultConfig.ApiSecret = apiSecret;
            this.txtDropBoxFolder.DataBindings.Clear();
            this.txtDropBoxFolder.DataBindings.Add("Text", this.ResultConfig, "DropBoxBaseFolder");

            if (curConfig.ApiKey != null && curConfig.ApiSecret != null)
            {
                this._needSave = !ObjectExtension.AreEquals(curConfig.ApiKey, apiKey) || !ObjectExtension.AreEquals(curConfig.ApiSecret, apiSecret);
            }

            this._logger = logger;
        }

        /// <summary>Gets the result config.</summary>
        public DropBoxSyncerConfig ResultConfig { get; private set; }

        /// <summary>
        /// The btn authorize_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnAuthorize.Enabled = false;
                var url = this.GetAuthorizeUrl();
                AppContext.ExecuteTask(
                    null, 
                    "Authorize", 
                    "Authorizing...", 
                    this.GetType() + "-Authorize", 
                    ProgressStyle.Marquee, 
                    this._logger, 
                    callback =>
                        {
                            callback.Inform(
                                this, 
                                "In order for " + AppUtil.ProductName
                                + @" to synchronize your fare data with DropBox, you need to login into your account and authorize via DropBox website.

After you have authorized, the application will ONLY have RESTRICTED access to its own data folder. DropBox will create a new folder for "
                                + AppUtil.ProductName
                                + @" under [Apps] folder. If you delete that folder accidentally, you will need to return here and re-authenticate!

You will now be redirected to DropBox Authentication website. Please return to this form after you have authorized the application!
The web URL will also be copied to your clipboard. In case a new browser window is not automatically opened, you can also paste the URL to your favorite web browser!", 
                                "DropBox Authorization", 
                                NotificationType.Info);
                        });
                Clipboard.SetText(url);
                BrowserUtils.Open(url);
                this.lblStatus.Text = validateStr;
                this.lblStatus.ForeColor = Color.DarkGoldenrod;

                UserLogin accessToken = null;
                ThreadPool.QueueUserWorkItem(
                    o =>
                        {
                            AppUtil.NameCurrentThread(this.GetType().Name + "-Validate");
                            Thread.Sleep(3000);
                            while (accessToken == null)
                            {
                                try
                                {
                                    accessToken = this._client.GetAccessToken();

                                    // Get token                            
                                }
                                catch
                                {
                                }

                                if (this.IsDestructed())
                                {
                                    return;
                                }

                                Thread.Sleep(1000);
                            }

                            this.ResultConfig.UserToken = this._dataGrep.Convert(accessToken.Token);
                            this.ResultConfig.UserSecret = this._dataGrep.Convert(accessToken.Secret);

                            this.SafeInvoke(
                                new Action(
                                    () =>
                                        {
                                            this.lblStatus.Text = successStr;
                                            this.lblStatus.ForeColor = Color.ForestGreen;
                                            this.btnAuthorize.Enabled = true;
                                        }));
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not connect to DropBox. Make sure that you are connected to the Internet and your connection setting is correct", 
                    "Connection problem", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                this._logger.Error("Failed to authorize DropBox", ex);
                this.btnAuthorize.Enabled = true;
            }
        }

        /// <summary>The get authorize url.</summary>
        /// <returns>The <see cref="string" />.</returns>
        private string GetAuthorizeUrl()
        {
            string result = null;
            AppContext.ExecuteTask(
                this, 
                "DropBox Configuration", 
                "Connecting to DropBox...", 
                "AuthorizeDropBox", 
                ProgressStyle.Marquee, 
                this._logger, 
                callback =>
                    {
                        this._client.GetToken();
                        result = this._client.BuildAuthorizeUrl();
                    });
            return result;
        }

        /// <summary>
        /// The drop box config dialog_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void DropBoxConfigDialog_Load(object sender, EventArgs e)
        {
            if (this._needSave)
            {
                MessageBox.Show(
                    this, 
                    "The current version of plugin has been changed and the configuration needs to be updated. Please save the configuration afterwards!", 
                    "Configuration Changes");
            }

            this.lblStatus.Text = validateStr;
            ThreadPool.QueueUserWorkItem(
                o =>
                    {
                        AppUtil.NameCurrentThread(this.GetType().Name + "-InitialLoad-Validate");
                        var valid = false;

                        if (this._client.UserLogin != null && this._client.UserLogin.Token != null && this._client.UserLogin.Secret != null)
                        {
                            try
                            {
                                var acc = this._client.AccountInfo();
                                valid = true;
                            }
                            catch (Exception ex)
                            {
                                var realEx = DropBoxExceptionHandler.HandleException(ex);
                                if (realEx != null)
                                {
                                    var err = "Failed to authenticate with DropBox: " + realEx.Message;
                                    this._logger.Warn(err);
                                }

                                valid = false;
                            }
                        }

                        if (this.IsDestructed())
                        {
                            return;
                        }

                        this.SafeInvoke(
                            new Action(
                                () =>
                                    {
                                        if (this.lblStatus.Text == validateStr)
                                        {
                                            if (valid)
                                            {
                                                this.lblStatus.Text = successStr;
                                                this.lblStatus.ForeColor = Color.ForestGreen;
                                            }
                                            else
                                            {
                                                this.lblStatus.Text = failStr;
                                                this.lblStatus.ForeColor = Color.Red;
                                            }
                                        }
                                    }));
                    });
        }
    }
}