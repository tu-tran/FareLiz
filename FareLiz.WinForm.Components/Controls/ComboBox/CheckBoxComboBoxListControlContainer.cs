namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     A container control for the ListControl to ensure the ScrollBar on the ListControl does not
    ///     Paint over the Size grip. Setting the Padding or Margin on the Popup or host control does
    ///     not work as I expected.
    /// </summary>
    [ToolboxItem(false)]
    internal class CheckBoxComboBoxListControlContainer : UserControl
    {
        #region CONSTRUCTOR

        public CheckBoxComboBoxListControlContainer()
        {
            this.BackColor = SystemColors.Window;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.AutoScaleMode = AutoScaleMode.Inherit;
            this.ResizeRedraw = true;
            // If you don't set this, then resize operations cause an error in the base class.
            this.MinimumSize = new Size(1, 1);
            this.MaximumSize = new Size(500, 500);
        }

        #endregion

        #region RESIZE OVERRIDE REQUIRED BY THE POPUP CONTROL

        /// <summary>
        ///     Prescribed by the Popup class to ensure Resize operations work correctly.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if ((this.Parent as Popup).ProcessResizing(ref m))
            {
                return;
            }
            base.WndProc(ref m);
        }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // CheckBoxComboBoxListControlContainer
            // 
            this.Name = "CheckBoxComboBoxListControlContainer";
            this.ResumeLayout(false);
        }
    }
}
