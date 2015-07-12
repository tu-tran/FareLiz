using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DropNet;
using DropNet.Models;
using log4net;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Presentation;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.WinForm.Components.Controls;
using SkyDean.FareLiz.WinForm.Components.Dialog;
using SkyDean.FareLiz.WinForm.Components.Utils;

namespace SkyDean.FareLiz.DropBox
{
    public partial class DropBoxConfigDialog : SmartForm
    {
        const string validateStr = "Validating current DropBox authorization status...",
                     successStr = "DropBox authorization is successful",
                     failStr = "DropBox authorization failed!";

        public DropBoxSyncerConfig ResultConfig { get; private set; }
        private readonly DropNetClient _client;
        private readonly bool _needSave = false;
        private readonly DataGrep _dataGrep;
        private ILog _logger;

        public DropBoxConfigDialog(byte[] apiKey, byte[] apiSecret, DropBoxSyncerConfig curConfig, DataGrep dataGrep, ILog logger)
        {
            InitializeComponent();
            _dataGrep = dataGrep;
            _client = new DropNetClient(_dataGrep.Convert(apiKey), _dataGrep.Convert(apiSecret),
                _dataGrep.Convert(curConfig.UserToken), _dataGrep.Convert(curConfig.UserSecret));
            ResultConfig = (curConfig == null ? new DropBoxSyncerConfig() : curConfig.ReflectionDeepClone(logger));
            ResultConfig.ApiKey = apiKey;
            ResultConfig.ApiSecret = apiSecret;
            txtDropBoxFolder.DataBindings.Clear();
            txtDropBoxFolder.DataBindings.Add("Text", ResultConfig, "DropBoxBaseFolder");

            if (curConfig.ApiKey != null && curConfig.ApiSecret != null)
                _needSave = (!ObjectExtension.AreEquals(curConfig.ApiKey, apiKey) || !ObjectExtension.AreEquals(curConfig.ApiSecret, apiSecret));

            _logger = logger;
        }

        private void btnAuthorize_Click(object sender, System.EventArgs e)
        {
            try
            {
                btnAuthorize.Enabled = false;
                string url = GetAuthorizeUrl();
                AppContext.ProgressCallback.Inform(this, "In order for " + AppUtil.ProductName + @" to synchronize your fare data with DropBox, you need to login into your account and authorize via DropBox website.

After you have authorized, the application will ONLY have RESTRICTED access to its own data folder. DropBox will create a new folder for " + AppUtil.ProductName + @" under [Apps] folder. If you delete that folder accidentally, you will need to return here and re-authenticate!

You will now be redirected to DropBox Authentication website. Please return to this form after you have authorized the application!
The web URL will also be copied to your clipboard. In case a new browser window is not automatically opened, you can also paste the URL to your favorite web browser!", "DropBox Authorization", NotificationType.Info);
                Clipboard.SetText(url);
                BrowserUtils.Open(url);
                lblStatus.Text = validateStr;
                lblStatus.ForeColor = Color.DarkGoldenrod;

                UserLogin accessToken = null;
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                    {
                        AppUtil.NameCurrentThread(GetType().Name + "-Validate");
                        Thread.Sleep(3000);
                        while (accessToken == null)
                        {
                            try
                            {
                                accessToken = _client.GetAccessToken(); // Get token                            
                            }
                            catch { }

                            if (this.IsDestructed())
                                return;

                            Thread.Sleep(1000);
                        }

                        ResultConfig.UserToken = _dataGrep.Convert(accessToken.Token);
                        ResultConfig.UserSecret = _dataGrep.Convert(accessToken.Secret);

                        this.SafeInvoke(new Action(() =>
                        {
                            lblStatus.Text = successStr;
                            lblStatus.ForeColor = Color.ForestGreen;
                            btnAuthorize.Enabled = true;
                        }));
                    }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not connect to DropBox. Make sure that you are connected to the Internet and your connection setting is correct", "Connection problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _logger.Error("Failed to authorize DropBox", ex);
                btnAuthorize.Enabled = true;
            }
        }

        private string GetAuthorizeUrl()
        {
            string result = null;
            ProgressDialog.ExecuteTask(this, "DropBox Configuration", "Connecting to DropBox...", "AuthorizeDropBox", ProgressBarStyle.Marquee, _logger, new CallbackDelegate(callback =>
            {
                _client.GetToken();
                result = _client.BuildAuthorizeUrl();
            }));
            return result;
        }

        private void DropBoxConfigDialog_Load(object sender, EventArgs e)
        {
            if (_needSave)
                MessageBox.Show(this, "The current version of plugin has been changed and the configuration needs to be updated. Please save the configuration afterwards!", "Configuration Changes");

            lblStatus.Text = validateStr;
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                {
                    AppUtil.NameCurrentThread(GetType().Name + "-InitialLoad-Validate");
                    bool valid = false;

                    if (_client.UserLogin != null && _client.UserLogin.Token != null && _client.UserLogin.Secret != null)
                    {
                        try
                        {
                            var acc = _client.AccountInfo();
                            valid = true;
                        }
                        catch (Exception ex)
                        {
                            var realEx = DropBoxExceptionHandler.HandleException(ex);
                            if (realEx != null)
                            {
                                string err = "Failed to authenticate with DropBox: " + realEx.Message;
                                _logger.Warn(err);
                            }
                            valid = false;
                        }
                    }

                    if (this.IsDestructed())
                        return;

                    this.SafeInvoke(new Action(() =>
                    {
                        if (lblStatus.Text == validateStr)
                        {
                            if (valid)
                            {
                                lblStatus.Text = successStr;
                                lblStatus.ForeColor = Color.ForestGreen;
                            }
                            else
                            {
                                lblStatus.Text = failStr;
                                lblStatus.ForeColor = Color.Red;
                            }
                        }
                    }));
                }));
        }
    }
}
