namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     Martin Lottering : 2007-10-27
    ///     --------------------------------
    ///     This is a usefull control in Filters. Allows you to save space and can replace a Grouped Box of CheckBoxes.
    ///     Currently used on the TasksFilter for TaskStatusses, which means the user can select which Statusses to include
    ///     in the "Search".
    ///     This control does not implement a CheckBoxListBox, instead it adds a wrapper for the normal ComboBox and Items.
    ///     See the CheckBoxItems property.
    ///     ----------------
    ///     ALSO IMPORTANT: In Data Binding when setting the DataSource. The ValueMember must be a bool type property, because it will
    ///     be binded to the Checked property of the displayed CheckBox. Also see the DisplayMemberSingleItem for more information.
    ///     ----------------
    ///     Extends the CodeProject PopupComboBox "Simple pop-up control" "http://www.codeproject.com/cs/miscctrl/simplepopup.asp"
    ///     by Lukasz Swiatkowski.
    /// </summary>
    public partial class CheckBoxComboBox : PopupComboBox
    {
        #region CONSTRUCTOR

        public CheckBoxComboBox()
        {
            this._CheckBoxProperties = new CheckBoxProperties();
            this._CheckBoxProperties.PropertyChanged += this._CheckBoxProperties_PropertyChanged;
            // Dumps the ListControl in a(nother) Container to ensure the ScrollBar on the ListControl does not
            // Paint over the Size grip. Setting the Padding or Margin on the Popup or host control does
            // not work as I expected. I don't think it can work that way.
            var ContainerControl = new CheckBoxComboBoxListControlContainer();
            this._CheckBoxComboBoxListControl = new CheckBoxComboBoxListControl(this);
            this._CheckBoxComboBoxListControl.Items.CheckBoxCheckedChanged += this.Items_CheckBoxCheckedChanged;
            ContainerControl.Controls.Add(this._CheckBoxComboBoxListControl);
            // This padding spaces neatly on the left-hand side and allows space for the size grip at the bottom.
            ContainerControl.Padding = new Padding(4, 0, 0, 14);
            // The ListControl FILLS the ListContainer.
            this._CheckBoxComboBoxListControl.Dock = DockStyle.Fill;
            // The DropDownControl used by the base class. Will be wrapped in a popup by the base class.
            this.DropDownControl = ContainerControl;
            // Must be set after the DropDownControl is set, since the popup is recreated.
            // NOTE: I made the dropDown protected so that it can be accessible here. It was private.
            this.dropDown.Resizable = true;
        }

        #endregion

        #region PRIVATE FIELDS

        /// <summary>
        ///     The checkbox list control. The public CheckBoxItems property provides a direct reference to its Items.
        /// </summary>
        internal CheckBoxComboBoxListControl _CheckBoxComboBoxListControl;

        /// <summary>
        ///     In DataBinding operations, this property will be used as the DisplayMember in the CheckBoxComboBoxListBox.
        ///     The normal/existing "DisplayMember" property is used by the TextBox of the ComboBox to display
        ///     a concatenated Text of the ite'elected. This concatenation and its formatting however is controlled
        ///     by the Binded object, since it owns that property.
        /// </summary>
        private string _DisplayMemberSingleItem;

        internal bool _MustAddHiddenItem = false;

        #endregion

        #region PRIVATE OPERATIONS

        /// <summary>
        ///     Builds a CSV string of the items selected.
        /// </summary>
        internal string GetCSVText(bool skipFirstItem)
        {
            string ListText = String.Empty;
            int StartIndex =
                this.DropDownStyle == ComboBoxStyle.DropDownList
                && this.DataSource == null
                && skipFirstItem
                    ? 1
                    : 0;
            for (int Index = StartIndex; Index <= this._CheckBoxComboBoxListControl.Items.Count - 1; Index++)
            {
                CheckBoxComboBoxItem Item = this._CheckBoxComboBoxListControl.Items[Index];
                if (Item.Checked)
                    ListText += string.IsNullOrEmpty(ListText) ? Item.Text : String.Format(", {0}", Item.Text);
            }
            return ListText;
        }

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        ///     A direct reference to the Items of CheckBoxComboBoxListControl.
        ///     You can use it to Get or Set the Checked status of items manually if you want.
        ///     But do not manipulate the List itself directly, e.g. Adding and Removing,
        ///     since the list is synchronised when shown with the ComboBox.Items. So for changing
        ///     the list contents, use Items instead.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public CheckBoxComboBoxItemList CheckBoxItems
        {
            get
            {
                // Added to ensure the CheckBoxItems are ALWAYS
                // available for modification via code.
                if (this._CheckBoxComboBoxListControl.Items.Count != this.Items.Count)
                    this._CheckBoxComboBoxListControl.SynchroniseControlsWithComboBoxItems();
                return this._CheckBoxComboBoxListControl.Items;
            }
        }

        /// <summary>
        ///     The DataSource of the combobox. Refreshes the CheckBox wrappers when this is set.
        /// </summary>
        public new object DataSource
        {
            get { return base.DataSource; }
            set
            {
                base.DataSource = value;
                if (!string.IsNullOrEmpty(this.ValueMember))
                    // This ensures that at least the checkboxitems are available to be initialised.
                    this._CheckBoxComboBoxListControl.SynchroniseControlsWithComboBoxItems();
            }
        }

        /// <summary>
        ///     The ValueMember of the combobox. Refreshes the CheckBox wrappers when this is set.
        /// </summary>
        public new string ValueMember
        {
            get { return base.ValueMember; }
            set
            {
                base.ValueMember = value;
                if (!string.IsNullOrEmpty(this.ValueMember))
                    // This ensures that at least the checkboxitems are available to be initialised.
                    this._CheckBoxComboBoxListControl.SynchroniseControlsWithComboBoxItems();
            }
        }

        /// <summary>
        ///     In DataBinding operations, this property will be used as the DisplayMember in the CheckBoxComboBoxListBox.
        ///     The normal/existing "DisplayMember" property is used by the TextBox of the ComboBox to display
        ///     a concatenated Text of the items selected. This concatenation however is controlled by the Binded
        ///     object, since it owns that property.
        /// </summary>
        public string DisplayMemberSingleItem
        {
            get
            {
                if (string.IsNullOrEmpty(this._DisplayMemberSingleItem)) return this.DisplayMember;
                else return this._DisplayMemberSingleItem;
            }
            set { this._DisplayMemberSingleItem = value; }
        }

        /// <summary>
        ///     Made this property Browsable again, since the Base Popup hides it. This class uses it again.
        ///     Gets an object representing the collection of the items contained in this
        ///     System.Windows.Forms.ComboBox.
        /// </summary>
        /// <returns>
        ///     A System.Windows.Forms.ComboBox.ObjectCollection representing the items in
        ///     the System.Windows.Forms.ComboBox.
        /// </returns>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new ObjectCollection Items
        {
            get { return base.Items; }
        }

        #endregion

        #region EVENTS & EVENT HANDLERS

        public event EventHandler CheckBoxCheckedChanged;

        private void Items_CheckBoxCheckedChanged(object sender, EventArgs e)
        {
            this.OnCheckBoxCheckedChanged(sender, e);
        }

        #endregion

        #region EVENT CALLERS and OVERRIDES e.g. OnResize()

        protected void OnCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            string ListText = this.GetCSVText(true);
            // The DropDownList style seems to require that the text
            // part of the "textbox" should match a single item.
            if (this.DropDownStyle != ComboBoxStyle.DropDownList)
                this.Text = ListText;
            // This refreshes the Text of the first item (which is not visible)
            else if (this.DataSource == null)
            {
                this.Items[0] = ListText;
                // Keep the hidden item and first checkbox item in 
                // sync in order to ensure the Synchronise process
                // can match the items.
                this.CheckBoxItems[0].ComboBoxItem = ListText;
            }

            EventHandler handler = this.CheckBoxCheckedChanged;
            if (handler != null)
                handler(sender, e);
        }

        /// <summary>
        ///     Will add an invisible item when the style is DropDownList,
        ///     to help maintain the correct text in main TextBox.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            base.OnDropDownStyleChanged(e);

            if (this.DropDownStyle == ComboBoxStyle.DropDownList
                && this.DataSource == null
                && !this.DesignMode)
                this._MustAddHiddenItem = true;
        }

        protected override void OnResize(EventArgs e)
        {
            // When the ComboBox is resized, the width of the dropdown 
            // is also resized to match the width of the ComboBox. I think it looks better.
            var Size = new Size(this.Width, this.DropDownControl.Height);
            this.dropDown.Size = Size;
            base.OnResize(e);
        }

        #endregion

        #region PUBLIC OPERATIONS

        /// <summary>
        ///     A function to clear/reset the list.
        ///     (Ubiklou : http://www.codeproject.com/KB/combobox/extending_combobox.aspx?msg=2526813#xx2526813xx)
        /// </summary>
        public void Clear()
        {
            this.Items.Clear();
            if (this.DropDownStyle == ComboBoxStyle.DropDownList && this.DataSource == null)
                this._MustAddHiddenItem = true;
        }

        /// <summary>
        ///     Uncheck all items.
        /// </summary>
        public void ClearSelection()
        {
            foreach (CheckBoxComboBoxItem Item in this.CheckBoxItems)
                if (Item.Checked)
                    Item.Checked = false;
        }

        #endregion

        #region CHECKBOX PROPERTIES (DEFAULTS)

        private CheckBoxProperties _CheckBoxProperties;

        /// <summary>
        ///     The properties that will be assigned to the checkboxes as default values.
        /// </summary>
        [Description("The properties that will be assigned to the checkboxes as default values.")]
        [Browsable(true)]
        public CheckBoxProperties CheckBoxProperties
        {
            get { return this._CheckBoxProperties; }
            set
            {
                this._CheckBoxProperties = value;
                this._CheckBoxProperties_PropertyChanged(this, EventArgs.Empty);
            }
        }

        private void _CheckBoxProperties_PropertyChanged(object sender, EventArgs e)
        {
            foreach (CheckBoxComboBoxItem Item in this.CheckBoxItems)
                Item.ApplyProperties(this.CheckBoxProperties);
        }

        #endregion

        protected override void WndProc(ref Message m)
        {
            // 323 : Item Added
            // 331 : Clearing
            if (m.Msg == 331
                && this.DropDownStyle == ComboBoxStyle.DropDownList
                && this.DataSource == null)
            {
                this._MustAddHiddenItem = true;
            }

            base.WndProc(ref m);
        }
    }    
}