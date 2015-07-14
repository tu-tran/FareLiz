namespace SkyDean.FareLiz.Data.Csv
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>The cached csv reader.</summary>
    public partial class CachedCsvReader : CsvReader
    {
        /// <summary>Represents a binding list wrapper for a CSV reader.</summary>
        private class CsvBindingList : IBindingList, ITypedList, IList<string[]>, IList
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the CsvBindingList class.
            /// </summary>
            /// <param name="csv">
            /// </param>
            public CsvBindingList(CachedCsvReader csv)
            {
                this._csv = csv;
                this._count = -1;
                this._direction = ListSortDirection.Ascending;
            }

            #endregion

            #region IEnumerable Members

            /// <summary>The get enumerator.</summary>
            /// <returns>The <see cref="IEnumerator" />.</returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion

            #region IEnumerable<string[]> Members

            /// <summary>The get enumerator.</summary>
            /// <returns>The <see cref="IEnumerator" />.</returns>
            public IEnumerator<string[]> GetEnumerator()
            {
                return this._csv.GetEnumerator();
            }

            #endregion

            #region Fields

            /// <summary>Contains the linked CSV reader.</summary>
            private readonly CachedCsvReader _csv;

            /// <summary>Contains the cached record count.</summary>
            private int _count;

            /// <summary>Contains the cached property descriptors.</summary>
            private PropertyDescriptorCollection _properties;

            /// <summary>Contains the current sort property.</summary>
            private CsvPropertyDescriptor _sort;

            /// <summary>Contains the current sort direction.</summary>
            private ListSortDirection _direction;

            #endregion

            #region IBindingList members

            /// <summary>
            /// The add index.
            /// </summary>
            /// <param name="property">
            /// The property.
            /// </param>
            public void AddIndex(PropertyDescriptor property)
            {
            }

            /// <summary>Gets a value indicating whether allow new.</summary>
            public bool AllowNew
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// The apply sort.
            /// </summary>
            /// <param name="property">
            /// The property.
            /// </param>
            /// <param name="direction">
            /// The direction.
            /// </param>
            public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
            {
                this._sort = (CsvPropertyDescriptor)property;
                this._direction = direction;

                this._csv.ReadToEnd();

                this._csv._records.Sort(new CsvRecordComparer(this._sort.Index, this._direction));
            }

            /// <summary>Gets the sort property.</summary>
            public PropertyDescriptor SortProperty
            {
                get
                {
                    return this._sort;
                }
            }

            /// <summary>
            /// The find.
            /// </summary>
            /// <param name="property">
            /// The property.
            /// </param>
            /// <param name="key">
            /// The key.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            public int Find(PropertyDescriptor property, object key)
            {
                var fieldIndex = ((CsvPropertyDescriptor)property).Index;
                var value = (string)key;

                var recordIndex = 0;
                var count = this.Count;

                while (recordIndex < count && this._csv[recordIndex, fieldIndex] != value)
                {
                    recordIndex++;
                }

                if (recordIndex == count)
                {
                    return -1;
                }

                return recordIndex;
            }

            /// <summary>Gets a value indicating whether supports sorting.</summary>
            public bool SupportsSorting
            {
                get
                {
                    return true;
                }
            }

            /// <summary>Gets a value indicating whether is sorted.</summary>
            public bool IsSorted
            {
                get
                {
                    return this._sort != null;
                }
            }

            /// <summary>Gets a value indicating whether allow remove.</summary>
            public bool AllowRemove
            {
                get
                {
                    return false;
                }
            }

            /// <summary>Gets a value indicating whether supports searching.</summary>
            public bool SupportsSearching
            {
                get
                {
                    return true;
                }
            }

            /// <summary>Gets the sort direction.</summary>
            public ListSortDirection SortDirection
            {
                get
                {
                    return this._direction;
                }
            }

            /// <summary>The list changed.</summary>
            public event ListChangedEventHandler ListChanged
            {
                add
                {
                }

                remove
                {
                }
            }

            /// <summary>Gets a value indicating whether supports change notification.</summary>
            public bool SupportsChangeNotification
            {
                get
                {
                    return false;
                }
            }

            /// <summary>The remove sort.</summary>
            public void RemoveSort()
            {
                this._sort = null;
                this._direction = ListSortDirection.Ascending;
            }

            /// <summary>The add new.</summary>
            /// <returns>The <see cref="object" />.</returns>
            /// <exception cref="NotSupportedException"></exception>
            public object AddNew()
            {
                throw new NotSupportedException();
            }

            /// <summary>Gets a value indicating whether allow edit.</summary>
            public bool AllowEdit
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            /// The remove index.
            /// </summary>
            /// <param name="property">
            /// The property.
            /// </param>
            public void RemoveIndex(PropertyDescriptor property)
            {
            }

            #endregion

            #region ITypedList Members

            /// <summary>
            /// The get item properties.
            /// </summary>
            /// <param name="listAccessors">
            /// The list accessors.
            /// </param>
            /// <returns>
            /// The <see cref="PropertyDescriptorCollection"/>.
            /// </returns>
            public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
            {
                if (this._properties == null)
                {
                    var properties = new PropertyDescriptor[this._csv.FieldCount];

                    for (var i = 0; i < properties.Length; i++)
                    {
                        properties[i] = new CsvPropertyDescriptor(((System.Data.IDataReader)this._csv).GetName(i), i);
                    }

                    this._properties = new PropertyDescriptorCollection(properties);
                }

                return this._properties;
            }

            /// <summary>
            /// The get list name.
            /// </summary>
            /// <param name="listAccessors">
            /// The list accessors.
            /// </param>
            /// <returns>
            /// The <see cref="string"/>.
            /// </returns>
            public string GetListName(PropertyDescriptor[] listAccessors)
            {
                return string.Empty;
            }

            #endregion

            #region IList<string[]> Members

            /// <summary>
            /// The index of.
            /// </summary>
            /// <param name="item">
            /// The item.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public int IndexOf(string[] item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The insert.
            /// </summary>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <param name="item">
            /// The item.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public void Insert(int index, string[] item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The remove at.
            /// </summary>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public void RemoveAt(int index)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The this.
            /// </summary>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            /// <returns>
            /// The <see cref="string[]"/>.
            /// </returns>
            public string[] this[int index]
            {
                get
                {
                    this._csv.MoveTo(index);
                    return this._csv._records[index];
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            #endregion

            #region ICollection<string[]> Members

            /// <summary>
            /// The add.
            /// </summary>
            /// <param name="item">
            /// The item.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public void Add(string[] item)
            {
                throw new NotSupportedException();
            }

            /// <summary>The clear.</summary>
            /// <exception cref="NotSupportedException"></exception>
            public void Clear()
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The contains.
            /// </summary>
            /// <param name="item">
            /// The item.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public bool Contains(string[] item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The copy to.
            /// </summary>
            /// <param name="array">
            /// The array.
            /// </param>
            /// <param name="arrayIndex">
            /// The array index.
            /// </param>
            public void CopyTo(string[][] array, int arrayIndex)
            {
                this._csv.MoveToStart();

                while (this._csv.ReadNextRecord())
                {
                    this._csv.CopyCurrentRecordTo(array[arrayIndex++]);
                }
            }

            /// <summary>Gets the count.</summary>
            public int Count
            {
                get
                {
                    if (this._count < 0)
                    {
                        this._csv.ReadToEnd();
                        this._count = (int)this._csv.CurrentRecordIndex + 1;
                    }

                    return this._count;
                }
            }

            /// <summary>Gets a value indicating whether is read only.</summary>
            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
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
            /// <exception cref="NotSupportedException">
            /// </exception>
            public bool Remove(string[] item)
            {
                throw new NotSupportedException();
            }

            #endregion

            #region IList Members

            /// <summary>
            /// The add.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public int Add(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The contains.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <returns>
            /// The <see cref="bool"/>.
            /// </returns>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public bool Contains(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The index of.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public int IndexOf(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The insert.
            /// </summary>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public void Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>Gets a value indicating whether is fixed size.</summary>
            public bool IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            /// The remove.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            public void Remove(object value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// The i list.this.
            /// </summary>
            /// <param name="index">
            /// The index.
            /// </param>
            /// <exception cref="NotSupportedException">
            /// </exception>
            /// <returns>
            /// The <see cref="object"/>.
            /// </returns>
            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            #endregion

            #region ICollection Members

            /// <summary>
            /// The copy to.
            /// </summary>
            /// <param name="array">
            /// The array.
            /// </param>
            /// <param name="index">
            /// The index.
            /// </param>
            public void CopyTo(Array array, int index)
            {
                this._csv.MoveToStart();

                while (this._csv.ReadNextRecord())
                {
                    this._csv.CopyCurrentRecordTo((string[])array.GetValue(index++));
                }
            }

            /// <summary>Gets a value indicating whether is synchronized.</summary>
            public bool IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            /// <summary>Gets the sync root.</summary>
            public object SyncRoot
            {
                get
                {
                    return null;
                }
            }

            #endregion
        }
    }
}