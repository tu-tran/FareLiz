namespace SkyDean.FareLiz.Service.LiveUpdate
{
    #region

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Dialog;
    using SkyDean.FareLiz.WinForm.Components.Utils;

    #endregion

    /// <summary>
    /// This class provides GUI for LiveUpdate (used in the main app) The next version content must also contain the Updater Workflow: Download package to
    /// local folder -> Start updater inside that local folder (Kill process and overwrite the file inside the application folder)
    /// </summary>
    public partial class LiveUpdateClientForm : SmartForm
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveUpdateClientForm"/> class.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="logo">
        /// The logo.
        /// </param>
        public LiveUpdateClientForm(UpdateRequest request, LiveUpdateClient client, Image logo)
        {
            this.InitializeComponent();
            this._request = request;
            this._client = client;
            this.lblProduct.Text = this.lblProduct.Text.Replace("%PRODUCT_NAME%", request.ProductName);
            this.txtCurrentVersion.Text = request.FromVersion.VersionNumber.ToString();
            this.txtNewVersion.Text = request.ToVersion.VersionNumber.ToString();
            this.imgLogo.Image = logo;
        }

        #endregion

        #region Fields

        /// <summary>The _client.</summary>
        private readonly LiveUpdateClient _client;

        /// <summary>The _request.</summary>
        private readonly UpdateRequest _request;

        #endregion

        #region Methods

        /// <summary>
        /// The live update client form_ shown.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LiveUpdateClientForm_Shown(object sender, EventArgs e)
        {
            NativeMethods.ShowToFront(this.Handle);
        }

        /// <summary>
        /// On event: User click on "Install Updates" button
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnInstallUpdates_Click(object sender, EventArgs e)
        {
            this.btnInstallUpdates.Enabled = false;
            this._client.RunUpdate(this._request);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #endregion
    }
}