namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    ///     A Typed List of the CheckBox items.
    ///     Simply a wrapper for the CheckBoxComboBox.Items. A list of CheckBoxComboBoxItem objects.
    ///     This List is automatically synchronised with the Items of the ComboBox and extended to
    ///     handle the additional boolean value. That said, do not Add or Remove using this List,
    ///     it will be lost or regenerated from the ComboBox.Items.
    /// </summary>
    [ToolboxItem(false)]
    public class CheckBoxComboBoxItemList : List<CheckBoxComboBoxItem>
    {
        #region CONSTRUCTORS

        public CheckBoxComboBoxItemList(CheckBoxComboBox checkBoxComboBox)
        {
            this._CheckBoxComboBox = checkBoxComboBox;
        }

        #endregion

        #region PRIVATE FIELDS

        private readonly CheckBoxComboBox _CheckBoxComboBox;

        #endregion

        #region EVENTS, This could be moved to the list control if needed

        public event EventHandler CheckBoxCheckedChanged;

        protected void OnCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            EventHandler handler = this.CheckBoxCheckedChanged;
            if (handler != null)
                handler(sender, e);
        }

        private void item_CheckedChanged(object sender, EventArgs e)
        {
            this.OnCheckBoxCheckedChanged(sender, e);
        }

        #endregion

        #region LIST MEMBERS & OBSOLETE INDICATORS

        public new void Add(CheckBoxComboBoxItem item)
        {
            item.CheckedChanged += this.item_CheckedChanged;
            base.Add(item);
        }

        public new void AddRange(IEnumerable<CheckBoxComboBoxItem> collection)
        {
            foreach (CheckBoxComboBoxItem Item in collection)
                Item.CheckedChanged += this.item_CheckedChanged;
            base.AddRange(collection);
        }

        public new void Clear()
        {
            foreach (CheckBoxComboBoxItem Item in this)
                Item.CheckedChanged -= this.item_CheckedChanged;
            base.Clear();
        }

        public new bool Remove(CheckBoxComboBoxItem item)
        {
            item.CheckedChanged -= this.item_CheckedChanged;
            return base.Remove(item);
        }

        #endregion

        #region DEFAULT PROPERTIES

        /// <summary>
        ///     Returns the item with the specified displayName or Text.
        /// </summary>
        public CheckBoxComboBoxItem this[string displayName]
        {
            get
            {
                int StartIndex =
                    // An invisible item exists in this scenario to help 
                    // with the Text displayed in the TextBox of the Combo
                    this._CheckBoxComboBox.DropDownStyle == ComboBoxStyle.DropDownList
                    && this._CheckBoxComboBox.DataSource == null
                        ? 1
                    // Ubiklou : 2008-04-28 : Ignore first item. (http://www.codeproject.com/KB/combobox/extending_combobox.aspx?fid=476622&df=90&mpp=25&noise=3&sort=Position&view=Quick&select=2526813&fr=1#xx2526813xx)
                        : 0;
                for (int Index = StartIndex; Index <= this.Count - 1; Index++)
                {
                    CheckBoxComboBoxItem Item = this[Index];

                    string Text;
                    // The binding might not be active yet
                    if (string.IsNullOrEmpty(Item.Text)
                        // Ubiklou : 2008-04-28 : No databinding
                        && Item.DataBindings != null
                        && Item.DataBindings["Text"] != null
                        )
                    {
                        PropertyInfo PropertyInfo
                            = Item.ComboBoxItem.GetType().GetProperty(
                                Item.DataBindings["Text"].BindingMemberInfo.BindingMember);
                        Text = (string)PropertyInfo.GetValue(Item.ComboBoxItem, null);
                    }
                    else
                        Text = Item.Text;
                    if (String.Compare(Text, displayName, StringComparison.OrdinalIgnoreCase) == 0)
                        return Item;
                }
                throw new ArgumentOutOfRangeException(String.Format("\"{0}\" does not exist in this combo box.",
                                                                    displayName));
            }
        }

        #endregion
    }
}
