namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.ComponentModel;
    using System.Data;

    /// <summary>
    /// Used together with the ListSelectionWrapper in order to wrap data sources for a CheckBoxComboBox. It helps to ensure you don't add an extra
    /// "Selected" property to a class that don't really need or want that information.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class ObjectSelectionWrapper<T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectSelectionWrapper{T}"/> class.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        public ObjectSelectionWrapper(T item, ListSelectionWrapper<T> container)
        {
            this._Container = container;
            this.Item = item;
        }

        #region PRIVATE PROPERTIES

        /// <summary>The containing list for these selections.</summary>
        private readonly ListSelectionWrapper<T> _Container;

        /// <summary>Is this item selected.</summary>
        private bool _Selected;

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        /// An indicator of how many items with the specified status is available for the current filter level. Thaught this would make the app a bit
        /// more user-friendly and help not to miss items in Statusses that are not often used.
        /// </summary>
        public int Count { get; set; }

        /// <summary>A reference to the item wrapped.</summary>
        public T Item { get; set; }

        /// <summary>The item display value. If ShowCount is true, it displays the "Name [Count]".</summary>
        public string Name
        {
            get
            {
                string Name = null;
                if (string.IsNullOrEmpty(this._Container.DisplayNameProperty))
                {
                    Name = this.Item.ToString();
                }
                else if (this.Item is DataRow)
                {
                    // A specific implementation for DataRow
                    Name = ((DataRow)((Object)this.Item))[this._Container.DisplayNameProperty].ToString();
                }
                else
                {
                    var PDs = TypeDescriptor.GetProperties(this.Item);
                    foreach (PropertyDescriptor PD in PDs)
                    {
                        if (string.Compare(PD.Name, this._Container.DisplayNameProperty, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            Name = PD.GetValue(this.Item).ToString();
                            break;
                        }
                    }

                    if (string.IsNullOrEmpty(Name))
                    {
                        var PI = this.Item.GetType().GetProperty(this._Container.DisplayNameProperty);
                        if (PI == null)
                        {
                            throw new Exception(
                                string.Format("Property {0} cannot be found on {1}.", this._Container.DisplayNameProperty, this.Item.GetType()));
                        }

                        Name = PI.GetValue(this.Item, null).ToString();
                    }
                }

                return this._Container.ShowCounts ? string.Format("{0} [{1}]", Name, this.Count) : Name;
            }
        }

        /// <summary>The textbox display value. The names concatenated.</summary>
        public string NameConcatenated
        {
            get
            {
                return this._Container.SelectedNames;
            }
        }

        /// <summary>Indicates whether the item is selected.</summary>
        public bool Selected
        {
            get
            {
                return this._Selected;
            }

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

        /// <summary>The property changed.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}