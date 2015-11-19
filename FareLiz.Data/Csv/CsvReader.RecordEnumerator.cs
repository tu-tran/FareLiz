//	SkyDean.FareLiz.WinForm.Utils.CsvReader.RecordEnumerator
//	Copyright (c) 2005 Sébastien Lorion
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
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The csv reader.
    /// </summary>
    public partial class CsvReader
    {
        /// <summary>Supports a simple iteration over the records of a <see cref="T:CsvReader" />.</summary>
        public struct RecordEnumerator : IEnumerator<string[]>, IEnumerator
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RecordEnumerator"/> struct. 
            /// Initializes a new instance of the <see cref="T:RecordEnumerator"/> class.
            /// </summary>
            /// <param name="reader">
            /// The <see cref="T:CsvReader"/> to iterate over.
            /// </param>
            /// <exception cref="T:ArgumentNullException">
            /// <paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            public RecordEnumerator(CsvReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader");
                }

                this._reader = reader;
                this._current = null;

                this._currentRecordIndex = reader._currentRecordIndex;
            }

            #endregion

            #region IDisposable Members

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose()
            {
                this._reader = null;
                this._current = null;
            }

            #endregion

            #region Fields

            /// <summary>Contains the enumerated <see cref="T:CsvReader" />.</summary>
            private CsvReader _reader;

            /// <summary>Contains the current record.</summary>
            private string[] _current;

            /// <summary>Contains the current record index.</summary>
            private long _currentRecordIndex;

            #endregion

            #region IEnumerator<string[]> Members

            /// <summary>Gets the current record.</summary>
            public string[] Current
            {
                get
                {
                    return this._current;
                }
            }

            /// <summary>Advances the enumerator to the next record of the CSV.</summary>
            /// <returns>
            /// <see langword="true" /> if the enumerator was successfully advanced to the next record, <see langword="false" /> if the enumerator has passed the end
            /// of the CSV.
            /// </returns>
            public bool MoveNext()
            {
                if (this._reader._currentRecordIndex != this._currentRecordIndex)
                {
                    throw new InvalidOperationException(CsvException.EnumerationVersionCheckFailed);
                }

                if (this._reader.ReadNextRecord())
                {
                    this._current = new string[this._reader._fieldCount];

                    this._reader.CopyCurrentRecordTo(this._current);
                    this._currentRecordIndex = this._reader._currentRecordIndex;

                    return true;
                }

                this._current = null;
                this._currentRecordIndex = this._reader._currentRecordIndex;

                return false;
            }

            #endregion

            #region IEnumerator Members

            /// <summary>Sets the enumerator to its initial position, which is before the first record in the CSV.</summary>
            public void Reset()
            {
                if (this._reader._currentRecordIndex != this._currentRecordIndex)
                {
                    throw new InvalidOperationException(CsvException.EnumerationVersionCheckFailed);
                }

                this._reader.MoveTo(-1);

                this._current = null;
                this._currentRecordIndex = this._reader._currentRecordIndex;
            }

            /// <summary>Gets the current record.</summary>
            object IEnumerator.Current
            {
                get
                {
                    if (this._reader._currentRecordIndex != this._currentRecordIndex)
                    {
                        throw new InvalidOperationException(CsvException.EnumerationVersionCheckFailed);
                    }

                    return this.Current;
                }
            }

            #endregion
        }
    }
}