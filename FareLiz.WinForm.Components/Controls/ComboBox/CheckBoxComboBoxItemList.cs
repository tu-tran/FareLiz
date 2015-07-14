namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;

    /// <summary>
    /// A Typed List of the CheckBox items. Simply a wrapper for the CheckBoxComboBox.Items. A list of CheckBoxComboBoxItem objects. This List is
    /// automatically synchronised with the Items of the ComboBox and extended to handle the additional boolean value. That said, do not Add or Remove using
    /// this List, it will be lost or regenerated from the ComboBox.Items.
    /// </summary>
    [ToolboxItem(false)]
    public class CheckBoxComboBoxItemList : List<CheckBoxComboBoxItem>
    {
        #region PRIVATE FIELDS

        /// <summary>The _ check box combo box.</summary>
        private readonly CheckBoxComboBox _CheckBoxComboBox;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxComboBoxItemList"/> class.
        /// </summary>
        /// <param name="checkBoxComboBox">
        /// The check box combo box.
        /// </param>
        public CheckBoxComboBoxItemList(CheckBoxComboBox checkBoxComboBox)
        {
            this._CheckBoxComboBox = checkBoxComboBox;
        }

        #endregion

        #region DEFAULT PROPERTIES

        /// <summary>
        /// Returns the item with the specified displayName or Text.
        /// </summary>
        /// <param name="displayName">
        /// The display Name.
        /// </param>
        /// <returns>
        /// The <see cref="CheckBoxComboBoxItem"/>.
        /// </returns>
        public CheckBoxComboBoxItem this[string displayName]
        {
            get
            {
                var StartIndex =

                    // An invisible item exists in this scenario to help 
                    // with the Text displayed in the TextBox of the Combo
                    this._CheckBoxComboBox.DropDownStyle == ComboBoxStyle.DropDownList && this._CheckBoxComboBox.DataSource == null
                        ? 1

                        // Ubiklou : 2008-04-28 : Ignore first item. (http://www.codeproject.com/KB/combobox/extending_combobox.aspx?fid=476622&df=90&mpp=25&noise=3&sort=Position&view=Quick&select=2526813&fr=1#xx2526813xx)
                        : 0;
                for (var Index = StartIndex; Index <= this.Count - 1; Index++)
                {
                    var Item = this[Index];

                    string Text;

                    // The binding might not be active yet
                    if (string.IsNullOrEmpty(Item.Text)

                        // Ubiklou : 2008-04-28 : No databinding
                        && Item.DataBindings != null && Item.DataBindings["Text"] != null)
                    {
                        var PropertyInfo = Item.ComboBoxItem.GetType().GetProperty(Item.DataBindings["Text"].BindingMemberInfo.BindingMember);
                        Text = (string)PropertyInfo.GetValue(Item.ComboBoxItem, null);
                    }
                    else
                    {
                        Text = Item.Text;
                    }

                    if (string.Compare(Text, displayName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return Item;
                    }
                }

                throw new ArgumentOutOfRangeException(string.Format("\"{0}\" does not exist in this combo box.", displayName));
            }
        }

        #endregion

        #region EVENTS, This could be moved to the list control if needed

        /// <summary>The check box checked changed.</summary>
        public event EventHandler CheckBoxCheckedChanged;

        /// <summary>
        /// The on check box checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void OnCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            var handler = this.CheckBoxCheckedChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        /// <summary>
        /// The item_ checked changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void item_CheckedChanged(object sender, EventArgs e)
        {
            this.OnCheckBoxCheckedChanged(sender, e);
        }

        #endregion

        #region LIST MEMBERS & OBSOLETE INDICATORS

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public new void Add(CheckBoxComboBoxItem item)
        {
            item.CheckedChanged += this.item_CheckedChanged;
            base.Add(item);
        }

        /// <summary>
        /// The add range.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        public new void AddRange(IEnumerable<CheckBoxComboBoxItem> collection)
        {
            foreach (var Item in collection)
            {
                Item.CheckedChanged += this.item_CheckedChanged;
            }

            base.AddRange(collection);
        }

        /// <summary>The clear.</summary>
        public new void Clear()
        {
            foreach (var Item in this)
            {
                Item.CheckedChanged -= this.item_CheckedChanged;
            }

            base.Clear();
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public new bool Remove(CheckBoxComboBoxItem item)
        {
            item.CheckedChanged -= this.item_CheckedChanged;
            return base.Remove(item);
        }

        #endregion
    }
}