﻿namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Reflection;

    /// <summary>
    ///     Used together with the ListSelectionWrapper in order to wrap data sources for a CheckBoxComboBox.
    ///     It helps to ensure you don't add an extra "Selected" property to a class that don't really need or want that information.
    /// </summary>
    internal class ObjectSelectionWrapper<T> : INotifyPropertyChanged
    {
        public ObjectSelectionWrapper(T item, ListSelectionWrapper<T> container)
        {
            this._Container = container;
            this.Item = item;
        }

        #region PRIVATE PROPERTIES

        /// <summary>
        ///     The containing list for these selections.
        /// </summary>
        private readonly ListSelectionWrapper<T> _Container;

        /// <summary>
        ///     Is this item selected.
        /// </summary>
        private bool _Selected;

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        ///     An indicator of how many items with the specified status is available for the current filter level.
        ///     Thaught this would make the app a bit more user-friendly and help not to miss items in Statusses
        ///     that are not often used.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     A reference to the item wrapped.
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        ///     The item display value. If ShowCount is true, it displays the "Name [Count]".
        /// </summary>
        public string Name
        {
            get
            {
                string Name = null;
                if (string.IsNullOrEmpty(this._Container.DisplayNameProperty))
                    Name = this.Item.ToString();
                else if (this.Item is DataRow) // A specific implementation for DataRow
                    Name = ((DataRow)((Object)this.Item))[this._Container.DisplayNameProperty].ToString();
                else
                {
                    PropertyDescriptorCollection PDs = TypeDescriptor.GetProperties(this.Item);
                    foreach (PropertyDescriptor PD in PDs)
                        if (String.Compare(PD.Name, this._Container.DisplayNameProperty, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            Name = PD.GetValue(this.Item).ToString();
                            break;
                        }
                    if (string.IsNullOrEmpty(Name))
                    {
                        PropertyInfo PI = this.Item.GetType().GetProperty(this._Container.DisplayNameProperty);
                        if (PI == null)
                            throw new Exception(String.Format(
                                "Property {0} cannot be found on {1}.",
                                this._Container.DisplayNameProperty,
                                this.Item.GetType()));
                        Name = PI.GetValue(this.Item, null).ToString();
                    }
                }
                return this._Container.ShowCounts ? String.Format("{0} [{1}]", Name, this.Count) : Name;
            }
        }

        /// <summary>
        ///     The textbox display value. The names concatenated.
        /// </summary>
        public string NameConcatenated
        {
            get { return this._Container.SelectedNames; }
        }

        /// <summary>
        ///     Indicates whether the item is selected.
        /// </summary>
        public bool Selected
        {
            get { return this._Selected; }
            set
            {
                if (this._Selected != value)
                {
                    this._Selected = value;
                    this.OnPropertyChanged("Selected");
                    this.OnPropertyChanged("NameConcatenated");
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}