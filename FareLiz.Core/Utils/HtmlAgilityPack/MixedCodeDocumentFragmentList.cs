// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>

namespace SkyDean.FareLiz.Core.Utils.HtmlAgilityPack
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of mixed code fragments.
    /// </summary>
    public class MixedCodeDocumentFragmentList : IEnumerable
    {
        #region Fields

        private MixedCodeDocument _doc;
        private IList<MixedCodeDocumentFragment> _items = new List<MixedCodeDocumentFragment>();

        #endregion

        #region Constructors

        internal MixedCodeDocumentFragmentList(MixedCodeDocument doc)
        {
            this._doc = doc;
        }

        #endregion

        #region Properties

        ///<summary>
        /// Gets the Document
        ///</summary>
        public MixedCodeDocument Doc
        {
            get { return this._doc; }
        }

        /// <summary>
        /// Gets the number of fragments contained in the list.
        /// </summary>
        public int Count
        {
            get { return this._items.Count; }
        }

        /// <summary>
        /// Gets a fragment from the list using its index.
        /// </summary>
        public MixedCodeDocumentFragment this[int index]
        {
            get { return this._items[index] as MixedCodeDocumentFragment; }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Gets an enumerator that can iterate through the fragment list.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Appends a fragment to the list of fragments.
        /// </summary>
        /// <param name="newFragment">The fragment to append. May not be null.</param>
        public void Append(MixedCodeDocumentFragment newFragment)
        {
            if (newFragment == null)
            {
                throw new ArgumentNullException("newFragment");
            }
            this._items.Add(newFragment);
        }

        /// <summary>
        /// Gets an enumerator that can iterate through the fragment list.
        /// </summary>
        public MixedCodeDocumentFragmentEnumerator GetEnumerator()
        {
            return new MixedCodeDocumentFragmentEnumerator(this._items);
        }

        /// <summary>
        /// Prepends a fragment to the list of fragments.
        /// </summary>
        /// <param name="newFragment">The fragment to append. May not be null.</param>
        public void Prepend(MixedCodeDocumentFragment newFragment)
        {
            if (newFragment == null)
            {
                throw new ArgumentNullException("newFragment");
            }
            this._items.Insert(0, newFragment);
        }

        /// <summary>
        /// Remove a fragment from the list of fragments. If this fragment was not in the list, an exception will be raised.
        /// </summary>
        /// <param name="fragment">The fragment to remove. May not be null.</param>
        public void Remove(MixedCodeDocumentFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }
            int index = this.GetFragmentIndex(fragment);
            if (index == -1)
            {
                throw new IndexOutOfRangeException();
            }
            this.RemoveAt(index);
        }

        /// <summary>
        /// Remove all fragments from the list.
        /// </summary>
        public void RemoveAll()
        {
            this._items.Clear();
        }

        /// <summary>
        /// Remove a fragment from the list of fragments, using its index in the list.
        /// </summary>
        /// <param name="index">The index of the fragment to remove.</param>
        public void RemoveAt(int index)
        {
            //MixedCodeDocumentFragment frag = (MixedCodeDocumentFragment) _items[index];
            this._items.RemoveAt(index);
        }

        #endregion

        #region Internal Methods

        internal void Clear()
        {
            this._items.Clear();
        }

        internal int GetFragmentIndex(MixedCodeDocumentFragment fragment)
        {
            if (fragment == null)
            {
                throw new ArgumentNullException("fragment");
            }
            for (int i = 0; i < this._items.Count; i++)
            {
                if ((this._items[i]) == fragment)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region Nested type: MixedCodeDocumentFragmentEnumerator

        /// <summary>
        /// Represents a fragment enumerator.
        /// </summary>
        public class MixedCodeDocumentFragmentEnumerator : IEnumerator
        {
            #region Fields

            private int _index;
            private IList<MixedCodeDocumentFragment> _items;

            #endregion

            #region Constructors

            internal MixedCodeDocumentFragmentEnumerator(IList<MixedCodeDocumentFragment> items)
            {
                this._items = items;
                this._index = -1;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public MixedCodeDocumentFragment Current
            {
                get { return (MixedCodeDocumentFragment) (this._items[this._index]); }
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            object IEnumerator.Current
            {
                get { return (this.Current); }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            public bool MoveNext()
            {
                this._index++;
                return (this._index < this._items.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                this._index = -1;
            }

            #endregion
        }

        #endregion
    }
}