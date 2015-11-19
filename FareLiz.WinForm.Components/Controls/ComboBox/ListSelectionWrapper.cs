namespace SkyDean.FareLiz.WinForm.Components.Controls.ComboBox
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    /// <summary>
    /// The list selection wrapper.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class ListSelectionWrapper<T> : List<ObjectSelectionWrapper<T>>
    {
        #region PRIVATE PROPERTIES

        /// <summary>The original List of values wrapped. A "Selected" and possibly "Count" functionality is added.</summary>
        private readonly IEnumerable _Source;

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// The list selection wrapper_ list changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ListSelectionWrapper_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    this.Add(this.CreateSelectionWrapper((IEnumerator)((IBindingList)this._Source)[e.NewIndex]));
                    break;
                case ListChangedType.ItemDeleted:
                    this.Remove(this.FindObjectWithItem((T)((IBindingList)this._Source)[e.OldIndex]));
                    break;
                case ListChangedType.Reset:
                    this.Populate();
                    break;
            }
        }

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSelectionWrapper{T}"/> class. 
        /// No property on the object is specified for display purposes, so simple ToString() operation will be performed. And no Counts will be
        /// displayed
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        public ListSelectionWrapper(IEnumerable source)
            : this(source, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSelectionWrapper{T}"/> class. 
        /// No property on the object is specified for display purposes, so simple ToString() operation will be performed.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="showCounts">
        /// The show Counts.
        /// </param>
        public ListSelectionWrapper(IEnumerable source, bool showCounts)
        {
            this._Source = source;
            this.ShowCounts = showCounts;
            if (this._Source is IBindingList)
            {
                ((IBindingList)this._Source).ListChanged += this.ListSelectionWrapper_ListChanged;
            }

            this.Populate();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSelectionWrapper{T}"/> class. 
        /// A Display "Name" property is specified. ToString() will not be performed on items. This is specifically useful on DataTable implementations, or where
        /// PropertyDescriptors are used to read the values. If a PropertyDescriptor is not found, a Property will be used.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="usePropertyAsDisplayName">
        /// The use Property As Display Name.
        /// </param>
        public ListSelectionWrapper(IEnumerable source, string usePropertyAsDisplayName)
            : this(source, false, usePropertyAsDisplayName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSelectionWrapper{T}"/> class. 
        /// A Display "Name" property is specified. ToString() will not be performed on items. This is specifically useful on DataTable implementations, or where
        /// PropertyDescriptors are used to read the values. If a PropertyDescriptor is not found, a Property will be used.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="showCounts">
        /// The show Counts.
        /// </param>
        /// <param name="usePropertyAsDisplayName">
        /// The use Property As Display Name.
        /// </param>
        public ListSelectionWrapper(IEnumerable source, bool showCounts, string usePropertyAsDisplayName)
            : this(source, showCounts)
        {
            this.DisplayNameProperty = usePropertyAsDisplayName;
        }

        #endregion

        #region PUBLIC PROPERTIES

        /// <summary>
        /// When specified, indicates that ToString() should not be performed on the items. This property will be read instead. This is specifically useful on
        /// DataTable implementations, where PropertyDescriptors are used to read the values.
        /// </summary>
        public string DisplayNameProperty { get; set; }

        /// <summary>Builds a concatenation list of selected items in the list.</summary>
        public string SelectedNames
        {
            get
            {
                string Text = string.Empty;
                foreach (var Item in this)
                {
                    if (Item.Selected)
                    {
                        Text += string.IsNullOrEmpty(Text) ? string.Format("\"{0}\"", Item.Name) : string.Format(" & \"{0}\"", Item.Name);
                    }
                }

                return Text;
            }
        }

        /// <summary>Indicates whether the Item display value (Name) should include a count.</summary>
        public bool ShowCounts { get; set; }

        #endregion

        #region HELPER MEMBERS

        /// <summary>Reset all counts to zero.</summary>
        public void ClearCounts()
        {
            foreach (var Item in this)
            {
                Item.Count = 0;
            }
        }

        /// <summary>
        /// Creates a ObjectSelectionWrapper item. Note that the constructor signature of sub classes classes are important.
        /// </summary>
        /// <param name="enumerator">
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerator"/>.
        /// </returns>
        private ObjectSelectionWrapper<T> CreateSelectionWrapper(IEnumerator enumerator)
        {
            var Types = new[] { typeof(T), this.GetType() };
            ConstructorInfo CI = typeof(ObjectSelectionWrapper<T>).GetConstructor(Types);
            if (CI == null)
            {
                throw new ArgumentException(
                    string.Format(
                        "The selection wrapper class {0} must have a constructor with ({1} Item, {2} Container) parameters.", 
                        typeof(ObjectSelectionWrapper<T>), 
                        typeof(T), 
                        this.GetType()));
            }

            var parameters = new[] { enumerator.Current, this };
            object result = CI.Invoke(parameters);
            return (ObjectSelectionWrapper<T>)result;
        }

        /// <summary>
        /// The find object with item.
        /// </summary>
        /// <param name="Object">
        /// The object.
        /// </param>
        /// <returns>
        /// The <see cref="ObjectSelectionWrapper"/>.
        /// </returns>
        public ObjectSelectionWrapper<T> FindObjectWithItem(T Object)
        {
            return this.Find(delegate(ObjectSelectionWrapper<T> target) { return target.Item.Equals(Object); });
        }

        /*
        public TSelectionWrapper FindObjectWithKey(object key)
        {
            return FindObjectWithKey(new object[] { key });
        }

        public TSelectionWrapper FindObjectWithKey(object[] keys)
        {
            return Find(new Predicate<TSelectionWrapper>(
                            delegate(TSelectionWrapper target)
                            {
                                return
                                    ReflectionHelper.CompareKeyValues(
                                        ReflectionHelper.GetKeyValuesFromObject(target.Item, target.Item.TableInfo),
                                        keys);
                            }));
        }

        public object[] GetArrayOfSelectedKeys()
        {
            List<object> List = new List<object>();
            foreach (TSelectionWrapper Item in this)
                if (Item.Selected)
                {
                    if (Item.Item.TableInfo.KeyProperties.Length == 1)
                        List.Add(ReflectionHelper.GetKeyValueFromObject(Item.Item, Item.Item.TableInfo));
                    else
                        List.Add(ReflectionHelper.GetKeyValuesFromObject(Item.Item, Item.Item.TableInfo));
                }
            return List.ToArray();
        }

        public T[] GetArrayOfSelectedKeys<T>()
        {
            List<T> List = new List<T>();
            foreach (TSelectionWrapper Item in this)
                if (Item.Selected)
                {
                    if (Item.Item.TableInfo.KeyProperties.Length == 1)
                        List.Add((T)ReflectionHelper.GetKeyValueFromObject(Item.Item, Item.Item.TableInfo));
                    else
                        throw new LibraryException("This generator only supports single value keys.");
                    // List.Add((T)ReflectionHelper.GetKeyValuesFromObject(Item.Item, Item.Item.TableInfo));
                }
            return List.ToArray();
        }
        */

        /// <summary>
        /// The populate.
        /// </summary>
        private void Populate()
        {
            this.Clear();

            /*
            for(int Index = 0; Index <= _Source.Count -1; Index++)
                Add(CreateSelectionWrapper(_Source[Index]));
             */
            IEnumerator Enumerator = this._Source.GetEnumerator();
            if (Enumerator != null)
            {
                while (Enumerator.MoveNext())
                {
                    this.Add(this.CreateSelectionWrapper(Enumerator));
                }
            }
        }

        #endregion
    }
}