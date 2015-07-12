using log4net;
using SkyDean.FareLiz.Core.Utils;
using SkyDean.FareLiz.WinForm.Components.Dialog;

namespace SkyDean.FareLiz.WinForm.Presentation.Views
{
    public partial class ObjectBrowserDialog : SmartForm
    {
        public object ResultObject { get { return propertyGrid.SelectedObject; } }
        private readonly object _originalObject;
        private readonly object _defaultObject;
        private readonly ILog _logger;

        public ObjectBrowserDialog(string title, object targetObject, object defaultObject, ILog logger)
        {
            InitializeComponent();
            _originalObject = targetObject;
            _defaultObject = defaultObject;
            _logger = logger;
            propertyGrid.SelectedObject = _originalObject.ReflectionDeepClone(_logger);
            Text = title;

            btnResetDefault.Visible = (_defaultObject != null);
        }

        private void btnReset_Click(object sender, System.EventArgs e)
        {
            propertyGrid.SelectedObject = _originalObject.ReflectionDeepClone(_logger);
        }

        private void btnResetDefault_Click(object sender, System.EventArgs e)
        {
            propertyGrid.SelectedObject = _defaultObject.ReflectionDeepClone(_logger);
        }
    }
}
