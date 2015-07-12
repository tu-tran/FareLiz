namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Security.Permissions;
    using System.Windows.Forms;

    using SkyDean.FareLiz.WinForm.Components.Utils;

    /// <summary>
    ///     CodeProject.com "Simple pop-up control" "http://www.codeproject.com/cs/miscctrl/simplepopup.asp".
    ///     Represents a Windows combo box control with a custom popup control attached.
    /// </summary>
    [ToolboxBitmap(typeof(ComboBox)), ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms"),
     Description("Displays an editable text box with a drop-down list of permitted values.")]
    public partial class PopupComboBox : ComboBox
    {
        /// <summary>
        ///     The pop-up wrapper for the dropDownControl.
        ///     Made PROTECTED instead of PRIVATE so descendent classes can set its Resizable property.
        ///     Note however the pop-up properties must be set after the dropDownControl is assigned, since this
        ///     popup wrapper is recreated when the dropDownControl is assigned.
        /// </summary>
        protected Popup dropDown;

        private Control dropDownControl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PopupControl.PopupComboBox" /> class.
        /// </summary>
        public PopupComboBox()
        {
            this.InitializeComponent();
            base.DropDownHeight = base.DropDownWidth = 1;
            base.IntegralHeight = false;
        }

        /// <summary>
        ///     Gets or sets the drop down control.
        /// </summary>
        /// <value>The drop down control.</value>
        public Control DropDownControl
        {
            get { return this.dropDownControl; }
            set
            {
                if (this.dropDownControl == value)
                    return;

                this.dropDownControl = value;
                this.dropDown = new Popup(value);
            }
        }

        /// <summary>
        ///     Shows the drop down.
        /// </summary>
        public void ShowDropDown()
        {
            if (this.dropDown != null)
            {
                this.dropDown.Show(this);
            }
        }

        /// <summary>
        ///     Hides the drop down.
        /// </summary>
        public void HideDropDown()
        {
            if (this.dropDown != null)
            {
                this.dropDown.Hide();
            }
        }

        /// <summary>
        ///     Processes Windows messages.
        /// </summary>
        /// <param name="m">
        ///     The Windows <see cref="T:System.Windows.Forms.Message" /> to process.
        /// </param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == ((int)W32_WM.WM_REFLECT + (int)W32_WM.WM_COMMAND))
            {
                if (NativeMethods.HiWord(m.WParam) == NativeMethods.CBN_DROPDOWN)
                {
                    // Blocks a redisplay when the user closes the control by clicking 
                    // on the combobox.
                    TimeSpan TimeSpan = DateTime.Now.Subtract(this.dropDown.LastClosedTimeStamp);
                    if (TimeSpan.TotalMilliseconds > 500)
                        this.ShowDropDown();
                    return;
                }
            }
            base.WndProc(ref m);
        }

        #region " Unused Properties "

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownWidth
        {
            get { return base.DropDownWidth; }
            set { base.DropDownWidth = value; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new int DropDownHeight
        {
            get { return base.DropDownHeight; }
            set
            {
                this.dropDown.Height = value;
                base.DropDownHeight = value;
            }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new bool IntegralHeight
        {
            get { return base.IntegralHeight; }
            set { base.IntegralHeight = value; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new ObjectCollection Items
        {
            get { return base.Items; }
        }

        /// <summary>This property is not relevant for this class.</summary>
        /// <returns>This property is not relevant for this class.</returns>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
         EditorBrowsable(EditorBrowsableState.Never)]
        public new int ItemHeight
        {
            get { return base.ItemHeight; }
            set { base.ItemHeight = value; }
        }

        #endregion
    }
}