namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    ///     This ListControl that pops up to the User. It contains the CheckBoxComboBoxItems.
    ///     The items are docked DockStyle.Top in this control.
    /// </summary>
    [ToolboxItem(false)]
    internal class CheckBoxComboBoxListControl : ScrollableControl
    {
        #region CONSTRUCTOR

        public CheckBoxComboBoxListControl(CheckBoxComboBox owner)
        {
            this.DoubleBuffered = true;
            this._CheckBoxComboBox = owner;
            this._Items = new CheckBoxComboBoxItemList(this._CheckBoxComboBox);
            this.BackColor = SystemColors.Window;
            // AutoScaleMode = AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.ResizeRedraw = true;
            // if you don't set this, a Resize operation causes an error in the base class.
            this.MinimumSize = new Size(1, 1);
            this.MaximumSize = new Size(500, 500);
        }

        #endregion

        #region PRIVATE PROPERTIES

        /// <summary>
        ///     Simply a reference to the CheckBoxComboBox.
        /// </summary>
        private readonly CheckBoxComboBox _CheckBoxComboBox;

        /// <summary>
        ///     A Typed list of ComboBoxCheckBoxItems.
        /// </summary>
        private readonly CheckBoxComboBoxItemList _Items;

        #endregion

        public CheckBoxComboBoxItemList Items
        {
            get { return this._Items; }
        }

        #region RESIZE OVERRIDE REQUIRED BY THE POPUP CONTROL

        /// <summary>
        ///     Prescribed by the Popup control to enable Resize operations.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            if ((this.Parent.Parent as Popup).ProcessResizing(ref m))
            {
                return;
            }
            base.WndProc(ref m);
        }

        #endregion

        #region PROTECTED MEMBERS

        protected override void OnVisibleChanged(EventArgs e)
        {
            // Synchronises the CheckBox list with the items in the ComboBox.
            this.SynchroniseControlsWithComboBoxItems();
            base.OnVisibleChanged(e);
        }

        /// <summary>
        ///     Maintains the controls displayed in the list by keeping them in sync with the actual
        ///     items in the combobox. (e.g. removing and adding as well as ordering)
        /// </summary>
        public void SynchroniseControlsWithComboBoxItems()
        {
            this.SuspendLayout();
            if (this._CheckBoxComboBox._MustAddHiddenItem)
            {
                this._CheckBoxComboBox.Items.Insert(
                    0, this._CheckBoxComboBox.GetCSVText(false)); // INVISIBLE ITEM
                this._CheckBoxComboBox.SelectedIndex = 0;
                this._CheckBoxComboBox._MustAddHiddenItem = false;
            }
            this.Controls.Clear();

            #region Disposes all items that are no longer in the combo box list

            for (int Index = this._Items.Count - 1; Index >= 0; Index--)
            {
                CheckBoxComboBoxItem Item = this._Items[Index];
                if (!this._CheckBoxComboBox.Items.Contains(Item.ComboBoxItem))
                {
                    this._Items.Remove(Item);
                    Item.Dispose();
                }
            }

            #endregion

            #region Recreate the list in the same order of the combo box items

            bool HasHiddenItem =
                this._CheckBoxComboBox.DropDownStyle == ComboBoxStyle.DropDownList
                && this._CheckBoxComboBox.DataSource == null
                && !this.DesignMode;

            var NewList = new CheckBoxComboBoxItemList(this._CheckBoxComboBox);
            for (int Index0 = 0; Index0 <= this._CheckBoxComboBox.Items.Count - 1; Index0++)
            {
                object Object = this._CheckBoxComboBox.Items[Index0];
                CheckBoxComboBoxItem Item = null;
                // The hidden item could match any other item when only
                // one other item was selected.
                if (Index0 == 0 && HasHiddenItem && this._Items.Count > 0)
                    Item = this._Items[0];
                else
                {
                    int StartIndex = HasHiddenItem
                                         ? 1 // Skip the hidden item, it could match 
                                         : 0;
                    for (int Index1 = StartIndex; Index1 <= this._Items.Count - 1; Index1++)
                    {
                        if (this._Items[Index1].ComboBoxItem == Object)
                        {
                            Item = this._Items[Index1];
                            break;
                        }
                    }
                }
                if (Item == null)
                {
                    Item = new CheckBoxComboBoxItem(this._CheckBoxComboBox, Object);
                    Item.ApplyProperties(this._CheckBoxComboBox.CheckBoxProperties);
                }
                NewList.Add(Item);
                Item.Dock = DockStyle.Top;
            }
            this._Items.Clear();
            this._Items.AddRange(NewList);

            #endregion

            #region Add the items to the controls in reversed order to maintain correct docking order

            if (NewList.Count > 0)
            {
                // This reverse helps to maintain correct docking order.
                NewList.Reverse();
                // If you get an error here that "Cannot convert to the desired 
                // type, it probably means the controls are not binding correctly.
                // The Checked property is binded to the ValueMember property. 
                // It must be a bool for example.
                this.Controls.AddRange(NewList.ToArray());
            }

            #endregion

            // Keep the first item invisible
            if (this._CheckBoxComboBox.DropDownStyle == ComboBoxStyle.DropDownList
                && this._CheckBoxComboBox.DataSource == null
                && !this.DesignMode)
                this._CheckBoxComboBox.CheckBoxItems[0].Visible = false;

            this.ResumeLayout();
        }

        #endregion
    }
}
