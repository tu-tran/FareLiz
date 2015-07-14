namespace SkyDean.FareLiz.Data.Csv
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>Represents a reader that provides fast, cached, dynamic access to CSV data.</summary>
    /// <remarks>The number of records is limited to <see cref="System.Int32.MaxValue" /> - 1.</remarks>
    public partial class CachedCsvReader : CsvReader, IListSource
    {
        #region Indexers

        /// <summary>
        /// Gets the field at the specified index.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <value>
        /// The field at the specified index.
        /// </value>
        /// <exception cref="T:ArgumentOutOfRangeException">
        /// <paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        /// No record read yet. Call ReadLine() first.
        /// </exception>
        /// <exception cref="MissingFieldCsvException">
        /// The CSV data appears to be missing a field.
        /// </exception>
        /// <exception cref="T:MalformedCsvException">
        /// The CSV appears to be corrupt at the current position.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override String this[int field]
        {
            get
            {
                if (this._readingStream)
                {
                    return base[field];
                }

                if (this._currentRecordIndex > -1)
                {
                    if (field > -1 && field < this.FieldCount)
                    {
                        return this._records[(int)this._currentRecordIndex][field];
                    }

                    throw new ArgumentOutOfRangeException(
                        "field", 
                        field, 
                        string.Format(CultureInfo.InvariantCulture, CsvException.FieldIndexOutOfRange, field));
                }

                throw new InvalidOperationException(CsvException.NoCurrentRecord);
            }
        }

        #endregion

        #region Fields

        /// <summary>Contains the cached records.</summary>
        private readonly List<string[]> _records;

        /// <summary>Contains the current record index (inside the cached records array).</summary>
        private long _currentRecordIndex;

        /// <summary>Indicates if a new record is being read from the CSV stream.</summary>
        private bool _readingStream;

        /// <summary>Contains the binding list linked to this reader.</summary>
        private CsvBindingList _bindingList;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCsvReader"/> class. Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">
        /// A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        /// <see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>
        /// .
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders)
            : this(reader, hasHeaders, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCsvReader"/> class. Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">
        /// A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        /// <see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>
        /// .
        /// </param>
        /// <param name="bufferSize">
        /// The buffer size in bytes.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, int bufferSize)
            : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCsvReader"/> class. Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">
        /// A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        /// <see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>
        /// .
        /// </param>
        /// <param name="delimiter">
        /// The delimiter character separating each field (default is ',').
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter)
            : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCsvReader"/> class. Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">
        /// A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        /// <see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>
        /// .
        /// </param>
        /// <param name="delimiter">
        /// The delimiter character separating each field (default is ',').
        /// </param>
        /// <param name="bufferSize">
        /// The buffer size in bytes.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize)
            : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCsvReader"/> class. Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">
        /// A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        /// <see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>
        /// .
        /// </param>
        /// <param name="delimiter">
        /// The delimiter character separating each field (default is ',').
        /// </param>
        /// <param name="quote">
        /// The quotation character wrapping every field (default is ''').
        /// </param>
        /// <param name="escape">
        /// The escape character letting insert quotation characters inside a quoted field (default is '\'). If no escape character, set to
        /// '\0' to gain some performance.
        /// </param>
        /// <param name="comment">
        /// The comment character indicating that a line is commented out (default is '#').
        /// </param>
        /// <param name="trimmingOptions">
        /// Determines how values should be trimmed.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CachedCsvReader(TextReader reader, 
                               bool hasHeaders, 
                               char delimiter, 
                               char quote, 
                               char escape, 
                               char comment, 
                               ValueTrimmingOptions trimmingOptions)
            : this(reader, hasHeaders, delimiter, quote, escape, comment, trimmingOptions, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCsvReader"/> class. Initializes a new instance of the CsvReader class.
        /// </summary>
        /// <param name="reader">
        /// A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        /// <see langword="true"/> if field names are located on the first non commented line, otherwise, <see langword="false"/>
        /// .
        /// </param>
        /// <param name="delimiter">
        /// The delimiter character separating each field (default is ',').
        /// </param>
        /// <param name="quote">
        /// The quotation character wrapping every field (default is ''').
        /// </param>
        /// <param name="escape">
        /// The escape character letting insert quotation characters inside a quoted field (default is '\'). If no escape character, set to
        /// '\0' to gain some performance.
        /// </param>
        /// <param name="comment">
        /// The comment character indicating that a line is commented out (default is '#').
        /// </param>
        /// <param name="trimmingOptions">
        /// The trimming Options.
        /// </param>
        /// <param name="bufferSize">
        /// The buffer size in bytes.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="bufferSize"/> must be 1 or more.
        /// </exception>
        public CachedCsvReader(TextReader reader, 
                               bool hasHeaders, 
                               char delimiter, 
                               char quote, 
                               char escape, 
                               char comment, 
                               ValueTrimmingOptions trimmingOptions, 
                               int bufferSize)
            : base(reader, hasHeaders, delimiter, quote, escape, comment, trimmingOptions, bufferSize)
        {
            this._records = new List<string[]>();
            this._currentRecordIndex = -1;
        }

        #endregion

        #region Properties

        #region RequestState

        /// <summary>Gets the current record index in the CSV file.</summary>
        /// <value>The current record index in the CSV file.</value>
        public override long CurrentRecordIndex
        {
            get
            {
                return this._currentRecordIndex;
            }
        }

        /// <summary>Gets a value that indicates whether the current stream position is at the end of the stream.</summary>
        /// <value>
        /// <see langword="true" /> if the current stream position is at the end of the stream; otherwise
        /// <see langword="false" />.
        /// </value>
        public override bool EndOfStream
        {
            get
            {
                if (this._currentRecordIndex < base.CurrentRecordIndex)
                {
                    return false;
                }

                return base.EndOfStream;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Read

        /// <summary>Reads the CSV stream from the current position to the end of the stream.</summary>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public virtual void ReadToEnd()
        {
            this._currentRecordIndex = base.CurrentRecordIndex;

            while (this.ReadNextRecord())
            {
                ;
            }
        }

        /// <summary>
        /// Reads the next record.
        /// </summary>
        /// <param name="onlyReadHeaders">
        /// Indicates if the reader will proceed to the next record after having read headers.
        /// <see langword="true"/> if it stops after having read headers; otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="skipToNextLine">
        /// Indicates if the reader will skip directly to the next line without parsing the current one. To be used when an error
        /// occurs.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a record has been successfully reads; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        protected override bool ReadNextRecord(bool onlyReadHeaders, bool skipToNextLine)
        {
            if (this._currentRecordIndex < base.CurrentRecordIndex)
            {
                this._currentRecordIndex++;
                return true;
            }

            this._readingStream = true;

            try
            {
                var canRead = base.ReadNextRecord(onlyReadHeaders, skipToNextLine);

                if (canRead)
                {
                    var record = new string[this.FieldCount];

                    if (base.CurrentRecordIndex > -1)
                    {
                        this.CopyCurrentRecordTo(record);
                        this._records.Add(record);
                    }
                    else
                    {
                        if (this.MoveTo(0))
                        {
                            this.CopyCurrentRecordTo(record);
                        }

                        this.MoveTo(-1);
                    }

                    if (!onlyReadHeaders)
                    {
                        this._currentRecordIndex++;
                    }
                }
                else
                {
                    // No more records to read, so set array size to only what is needed
                    this._records.Capacity = this._records.Count;
                }

                return canRead;
            }
            finally
            {
                this._readingStream = false;
            }
        }

        #endregion

        #region Move

        /// <summary>Moves before the first record.</summary>
        public void MoveToStart()
        {
            this._currentRecordIndex = -1;
        }

        /// <summary>Moves to the last record read so far.</summary>
        public void MoveToLastCachedRecord()
        {
            this._currentRecordIndex = base.CurrentRecordIndex;
        }

        /// <summary>
        /// Moves to the specified record index.
        /// </summary>
        /// <param name="record">
        /// The record index.
        /// </param>
        /// <returns>
        /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        public override bool MoveTo(long record)
        {
            if (record < -1)
            {
                record = -1;
            }

            if (record <= base.CurrentRecordIndex)
            {
                this._currentRecordIndex = record;
                return true;
            }

            this._currentRecordIndex = base.CurrentRecordIndex;
            return base.MoveTo(record);
        }

        #endregion

        #endregion

        #region IListSource Members

        /// <summary>Gets a value indicating whether contains list collection.</summary>
        bool IListSource.ContainsListCollection
        {
            get
            {
                return false;
            }
        }

        /// <summary>The get list.</summary>
        /// <returns>The <see cref="IList" />.</returns>
        IList IListSource.GetList()
        {
            if (this._bindingList == null)
            {
                this._bindingList = new CsvBindingList(this);
            }

            return this._bindingList;
        }

        #endregion
    }
}