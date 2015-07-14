namespace SkyDean.FareLiz.Data.Csv
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>The csv reader.</summary>
    public partial class CsvReader
    {
        /// <summary>Supports a simple iteration over the records of a <see cref="T:CsvReader" />.</summary>
        public struct RecordEnumerator : IEnumerator<string[]>, IEnumerator
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RecordEnumerator"/> struct. Initializes a new instance of the
            /// <see cref="T:RecordEnumerator"/> class.
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
            /// <see langword="true" /> if the enumerator was successfully advanced to the next record,
            /// <see langword="false" /> if the enumerator has passed the end of the CSV.
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