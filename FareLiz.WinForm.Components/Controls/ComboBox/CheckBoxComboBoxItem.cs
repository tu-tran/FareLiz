namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>The CheckBox items displayed in the Popup of the ComboBox.</summary>
    [ToolboxItem(false)]
    public class CheckBoxComboBoxItem : CheckBox
    {
        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxComboBoxItem"/> class.
        /// </summary>
        /// <param name="owner">
        /// A reference to the CheckBoxComboBox.
        /// </param>
        /// <param name="comboBoxItem">
        /// A reference to the item in the ComboBox.Items that this object is extending.
        /// </param>
        public CheckBoxComboBoxItem(CheckBoxComboBox owner, object comboBoxItem)
        {
            this.DoubleBuffered = true;
            this._CheckBoxComboBox = owner;
            this._ComboBoxItem = comboBoxItem;
            if (this._CheckBoxComboBox.DataSource != null)
            {
                this.AddBindings();
            }
            else
            {
                this.Text = comboBoxItem.ToString();
            }
        }

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>A reference to the Item in ComboBox.Items that this object is extending.</summary>
        public object ComboBoxItem
        {
            get
            {
                return this._ComboBoxItem;
            }

            internal set
            {
                this._ComboBoxItem = value;
            }
        }

        #endregion

        #region BINDING HELPER

        /// <summary>When using Data Binding operations via the DataSource property of the ComboBox. This adds the required Bindings for the CheckBoxes.</summary>
        public void AddBindings()
        {
            // Note, the text uses "DisplayMemberSingleItem", not "DisplayMember" (unless its not assigned)
            this.DataBindings.Add("Text", this._ComboBoxItem, this._CheckBoxComboBox.DisplayMemberSingleItem);

            // The ValueMember must be a bool type property usable by the CheckBox.Checked.
            this.DataBindings.Add(
                "Checked", 
                this._ComboBoxItem, 
                this._CheckBoxComboBox.ValueMember, 
                false, 

                // This helps to maintain proper selection RequestState in the Binded object,
                // even when the controls are added and removed.
                DataSourceUpdateMode.OnPropertyChanged, 
                false, 
                null, 
                null);

            // Helps to maintain the Checked status of this
            // checkbox before the control is visible
            if (this._ComboBoxItem is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)this._ComboBoxItem).PropertyChanged += this.CheckBoxComboBoxItem_PropertyChanged;
            }
        }

        #endregion

        #region PROTECTED MEMBERS

        /// <summary>
        /// The on checked changed.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnCheckedChanged(EventArgs e)
        {
            // Found that when this event is raised, the bool value of the binded item is not yet updated.
            if (this._CheckBoxComboBox.DataSource != null)
            {
                var PI = this.ComboBoxItem.GetType().GetProperty(this._CheckBoxComboBox.ValueMember);
                PI.SetValue(this.ComboBoxItem, this.Checked, null);
            }

            base.OnCheckedChanged(e);

            // Forces a refresh of the Text displayed in the main TextBox of the ComboBox,
            // since that Text will most probably represent a concatenation of selected values.
            // Also see DisplayMemberSingleItem on the CheckBoxComboBox for more information.
            if (this._CheckBoxComboBox.DataSource != null)
            {
                var OldDisplayMember = this._CheckBoxComboBox.DisplayMember;
                this._CheckBoxComboBox.DisplayMember = null;
                this._CheckBoxComboBox.DisplayMember = OldDisplayMember;
            }
        }

        #endregion

        #region HELPER MEMBERS

        /// <summary>
        /// The apply properties.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        internal void ApplyProperties(CheckBoxProperties properties)
        {
            this.Appearance = properties.Appearance;
            this.AutoCheck = properties.AutoCheck;
            this.AutoEllipsis = properties.AutoEllipsis;
            this.AutoSize = properties.AutoSize;
            this.CheckAlign = properties.CheckAlign;
            this.FlatAppearance.BorderColor = properties.FlatAppearanceBorderColor;
            this.FlatAppearance.BorderSize = properties.FlatAppearanceBorderSize;
            this.FlatAppearance.CheckedBackColor = properties.FlatAppearanceCheckedBackColor;
            this.FlatAppearance.MouseDownBackColor = properties.FlatAppearanceMouseDownBackColor;
            this.FlatAppearance.MouseOverBackColor = properties.FlatAppearanceMouseOverBackColor;
            this.FlatStyle = properties.FlatStyle;
            this.ForeColor = properties.ForeColor;
            this.RightToLeft = properties.RightToLeft;
            this.TextAlign = properties.TextAlign;
            this.ThreeState = properties.ThreeState;
        }

        #endregion

        #region EVENT HANDLERS - ComboBoxItem (DataSource)

        /// <summary>
        /// Added this handler because the control doesn't seem to initialize correctly until shown for the first time, which also means the summary
        /// text value of the combo is out of sync initially.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CheckBoxComboBoxItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this._CheckBoxComboBox.ValueMember)
            {
                this.Checked = (bool)this._ComboBoxItem.GetType().GetProperty(this._CheckBoxComboBox.ValueMember).GetValue(this._ComboBoxItem, null);
            }
        }

        #endregion

        #region PRIVATE PROPERTIES

        /// <summary>A reference to the CheckBoxComboBox.</summary>
        private readonly CheckBoxComboBox _CheckBoxComboBox;

        /// <summary>A reference to the Item in ComboBox.Items that this object is extending.</summary>
        private object _ComboBoxItem;

        #endregion
    }
}