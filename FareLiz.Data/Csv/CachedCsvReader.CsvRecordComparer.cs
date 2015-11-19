//	SkyDean.FareLiz.WinForm.Utils.CachedCsvReader.CsvRecordComparer
//	Copyright (c) 2006 Sébastien Lorion
//	MIT license (http://en.wikipedia.org/wiki/MIT_License)
//	Permission is hereby granted, free of charge, to any person obtaining a copy
//	of this software and associated documentation files (the "Software"), to deal
//	in the Software without restriction, including without limitation the rights 
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//	of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//	The above copyright notice and this permission notice shall be included in all 
//	copies or substantial portions of the Software.
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
//	PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
//	FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//	ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
namespace SkyDean.FareLiz.Data.Csv
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// The cached csv reader.
    /// </summary>
    public partial class CachedCsvReader : CsvReader
    {
        /// <summary>Represents a CSV record comparer.</summary>
        private class CsvRecordComparer : IComparer<string[]>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the CsvRecordComparer class.
            /// </summary>
            /// <param name="field">
            /// The field index of the values to compare.
            /// </param>
            /// <param name="direction">
            /// The sort direction.
            /// </param>
            public CsvRecordComparer(int field, ListSortDirection direction)
            {
                if (field < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "field", 
                        field, 
                        string.Format(CultureInfo.InvariantCulture, CsvException.FieldIndexOutOfRange, field));
                }

                this._field = field;
                this._direction = direction;
            }

            #endregion

            #region IComparer<string[]> Members

            /// <summary>
            /// The compare.
            /// </summary>
            /// <param name="x">
            /// The x.
            /// </param>
            /// <param name="y">
            /// The y.
            /// </param>
            /// <returns>
            /// The <see cref="int"/>.
            /// </returns>
            public int Compare(string[] x, string[] y)
            {
                Debug.Assert(x != null && y != null && x.Length == y.Length && this._field < x.Length);

                int result = string.Compare(x[this._field], y[this._field], StringComparison.CurrentCulture);

                return this._direction == ListSortDirection.Ascending ? result : -result;
            }

            #endregion

            #region Fields

            /// <summary>Contains the field index of the values to compare.</summary>
            private int _field;

            /// <summary>Contains the sort direction.</summary>
            private ListSortDirection _direction;

            #endregion
        }
    }
}