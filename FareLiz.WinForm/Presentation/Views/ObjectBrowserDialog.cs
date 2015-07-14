namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    using System;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>The object browser dialog.</summary>
    public partial class ObjectBrowserDialog : SmartForm
    {
        /// <summary>The _default object.</summary>
        private readonly object _defaultObject;

        /// <summary>The _logger.</summary>
        private readonly ILogger _logger;

        /// <summary>The _original object.</summary>
        private readonly object _originalObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectBrowserDialog"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="targetObject">
        /// The target object.
        /// </param>
        /// <param name="defaultObject">
        /// The default object.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        public ObjectBrowserDialog(string title, object targetObject, object defaultObject, ILogger logger)
        {
            this.InitializeComponent();
            this._originalObject = targetObject;
            this._defaultObject = defaultObject;
            this._logger = logger;
            this.propertyGrid.SelectedObject = this._originalObject.ReflectionDeepClone(this._logger);
            this.Text = title;

            this.btnResetDefault.Visible = this._defaultObject != null;
        }

        /// <summary>Gets the result object.</summary>
        public object ResultObject
        {
            get
            {
                return this.propertyGrid.SelectedObject;
            }
        }

        /// <summary>
        /// The btn reset_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            this.propertyGrid.SelectedObject = this._originalObject.ReflectionDeepClone(this._logger);
        }

        /// <summary>
        /// The btn reset default_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            this.propertyGrid.SelectedObject = this._defaultObject.ReflectionDeepClone(this._logger);
        }
    }
}