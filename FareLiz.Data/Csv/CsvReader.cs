namespace SkyDean.FareLiz.Data.Csv
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>Represents a reader that provides fast, non-cached, forward-only access to CSV data.</summary>
    public partial class CsvReader : IDataReader, IEnumerable<string[]>, IDisposable
    {
        #region IEnumerable Members

        /// <summary>Returns an <see cref="T:System.Collections.IEnumerator" />  that can iterate through CSV records.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" />  that can iterate through CSV records.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Constants

        /// <summary>Defines the default buffer size.</summary>
        public const int DefaultBufferSize = 0x1000;

        /// <summary>Defines the default delimiter character separating each field.</summary>
        public const char DefaultDelimiter = ',';

        /// <summary>Defines the default quote character wrapping every field.</summary>
        public const char DefaultQuote = '"';

        /// <summary>Defines the default escape character letting insert quotation characters inside a quoted field.</summary>
        public const char DefaultEscape = '"';

        /// <summary>Defines the default comment character indicating that a line is commented out.</summary>
        public const char DefaultComment = '#';

        #endregion

        #region Fields

        /// <summary>Contains the field header comparer.</summary>
        private static readonly StringComparer _fieldHeaderComparer = StringComparer.CurrentCultureIgnoreCase;

        #region Settings

        /// <summary>Contains the <see cref="T:TextReader" /> pointing to the CSV file.</summary>
        private TextReader _reader;

        /// <summary>Contains the buffer size.</summary>
        private readonly int _bufferSize;

        /// <summary>Contains the comment character indicating that a line is commented out.</summary>
        private readonly char _comment;

        /// <summary>Contains the escape character letting insert quotation characters inside a quoted field.</summary>
        private readonly char _escape;

        /// <summary>Contains the delimiter character separating each field.</summary>
        private readonly char _delimiter;

        /// <summary>Contains the quotation character wrapping every field.</summary>
        private readonly char _quote;

        /// <summary>Determines which values should be trimmed.</summary>
        private readonly ValueTrimmingOptions _trimmingOptions;

        /// <summary>Indicates if field names are located on the first non commented line.</summary>
        private readonly bool _hasHeaders;

        /// <summary>Contains the default action to take when a parsing error has occured.</summary>
        private ParseErrorAction _defaultParseErrorAction;

        /// <summary>Contains the action to take when a field is missing.</summary>
        private MissingFieldAction _missingFieldAction;

        /// <summary>Indicates if the reader supports multiline.</summary>
        private bool _supportsMultiline;

        /// <summary>Indicates if the reader will skip empty lines.</summary>
        private bool _skipEmptyLines;

        #endregion

        #region RequestState

        /// <summary>Indicates if the class is initialized.</summary>
        private bool _initialized;

        /// <summary>Contains the field headers.</summary>
        private string[] _fieldHeaders;

        /// <summary>Contains the dictionary of field indexes by header. The key is the field name and the value is its index.</summary>
        private Dictionary<string, int> _fieldHeaderIndexes;

        /// <summary>
        /// Contains the current record index in the CSV file. A value of <see cref="M:Int32.MinValue" /> means that the reader has not been initialized
        /// yet. Otherwise, a negative value means that no record has been read yet.
        /// </summary>
        private long _currentRecordIndex;

        /// <summary>Contains the starting position of the next unread field.</summary>
        private int _nextFieldStart;

        /// <summary>Contains the index of the next unread field.</summary>
        private int _nextFieldIndex;

        /// <summary>Contains the array of the field values for the current record. A null value indicates that the field have not been parsed.</summary>
        private string[] _fields;

        /// <summary>Contains the maximum number of fields to retrieve for each record.</summary>
        private int _fieldCount;

        /// <summary>Contains the read buffer.</summary>
        private char[] _buffer;

        /// <summary>Contains the current read buffer length.</summary>
        private int _bufferLength;

        /// <summary>Indicates if the end of the reader has been reached.</summary>
        private bool _eof;

        /// <summary>Indicates if the last read operation reached an EOL character.</summary>
        private bool _eol;

        /// <summary>
        /// Indicates if the first record is in cache. This can happen when initializing a reader with no headers because one record must be read to get
        /// the field count automatically
        /// </summary>
        private bool _firstRecordInCache;

        /// <summary>Indicates if one or more field are missing for the current record. Resets after each successful record read.</summary>
        private bool _missingFieldFlag;

        /// <summary>Indicates if a parse error occured for the current record. Resets after each successful record read.</summary>
        private bool _parseErrorFlag;

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
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
        public CsvReader(TextReader reader, bool hasHeaders)
            : this(
                reader, 
                hasHeaders, 
                DefaultDelimiter, 
                DefaultQuote, 
                DefaultEscape, 
                DefaultComment, 
                ValueTrimmingOptions.UnquotedOnly, 
                DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
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
        public CsvReader(TextReader reader, bool hasHeaders, int bufferSize)
            : this(reader, hasHeaders, DefaultDelimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
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
        public CsvReader(TextReader reader, bool hasHeaders, char delimiter)
            : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, DefaultBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
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
        public CsvReader(TextReader reader, bool hasHeaders, char delimiter, int bufferSize)
            : this(reader, hasHeaders, delimiter, DefaultQuote, DefaultEscape, DefaultComment, ValueTrimmingOptions.UnquotedOnly, bufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvReader class.
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
        /// Determines which values should be trimmed.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CsvReader(TextReader reader, 
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
        /// Initializes a new instance of the CsvReader class.
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
        /// Determines which values should be trimmed.
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
        public CsvReader(TextReader reader, 
                         bool hasHeaders, 
                         char delimiter, 
                         char quote, 
                         char escape, 
                         char comment, 
                         ValueTrimmingOptions trimmingOptions, 
                         int bufferSize)
        {
#if DEBUG
            this._allocStack = new StackTrace();
#endif

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize", bufferSize, CsvException.BufferSizeTooSmall);
            }

            this._bufferSize = bufferSize;

            if (reader is StreamReader)
            {
                var stream = ((StreamReader)reader).BaseStream;

                if (stream.CanSeek)
                {
                    // Handle bad implementations returning 0 or less
                    if (stream.Length > 0)
                    {
                        this._bufferSize = (int)Math.Min(bufferSize, stream.Length);
                    }
                }
            }

            this._reader = reader;
            this._delimiter = delimiter;
            this._quote = quote;
            this._escape = escape;
            this._comment = comment;

            this._hasHeaders = hasHeaders;
            this._trimmingOptions = trimmingOptions;
            this._supportsMultiline = true;
            this._skipEmptyLines = true;
            this.DefaultHeaderName = "Column";

            this._currentRecordIndex = -1;
            this._defaultParseErrorAction = ParseErrorAction.RaiseEvent;
        }

        #endregion

        #region Events

        /// <summary>Occurs when there is an error while parsing the CSV stream.</summary>
        public event EventHandler<ParseErrorEventArgs> ParseError;

        /// <summary>
        /// Raises the <see cref="M:ParseError"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ParseErrorEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnParseError(ParseErrorEventArgs e)
        {
            var handler = this.ParseError;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Properties

        #region Settings

        /// <summary>Gets the comment character indicating that a line is commented out.</summary>
        /// <value>The comment character indicating that a line is commented out.</value>
        public char Comment
        {
            get
            {
                return this._comment;
            }
        }

        /// <summary>Gets the escape character letting insert quotation characters inside a quoted field.</summary>
        /// <value>The escape character letting insert quotation characters inside a quoted field.</value>
        public char Escape
        {
            get
            {
                return this._escape;
            }
        }

        /// <summary>Gets the delimiter character separating each field.</summary>
        /// <value>The delimiter character separating each field.</value>
        public char Delimiter
        {
            get
            {
                return this._delimiter;
            }
        }

        /// <summary>Gets the quotation character wrapping every field.</summary>
        /// <value>The quotation character wrapping every field.</value>
        public char Quote
        {
            get
            {
                return this._quote;
            }
        }

        /// <summary>Indicates if field names are located on the first non commented line.</summary>
        /// <value>
        /// <see langword="true" /> if field names are located on the first non commented line, otherwise,
        /// <see langword="false" />.
        /// </value>
        public bool HasHeaders
        {
            get
            {
                return this._hasHeaders;
            }
        }

        /// <summary>Indicates if spaces at the start and end of a field are trimmed.</summary>
        /// <value>
        /// <see langword="true" /> if spaces at the start and end of a field are trimmed, otherwise,
        /// <see langword="false" />.
        /// </value>
        public ValueTrimmingOptions TrimmingOption
        {
            get
            {
                return this._trimmingOptions;
            }
        }

        /// <summary>Gets the buffer size.</summary>
        public int BufferSize
        {
            get
            {
                return this._bufferSize;
            }
        }

        /// <summary>Gets or sets the default action to take when a parsing error has occured.</summary>
        /// <value>The default action to take when a parsing error has occured.</value>
        public ParseErrorAction DefaultParseErrorAction
        {
            get
            {
                return this._defaultParseErrorAction;
            }

            set
            {
                this._defaultParseErrorAction = value;
            }
        }

        /// <summary>Gets or sets the action to take when a field is missing.</summary>
        /// <value>The action to take when a field is missing.</value>
        public MissingFieldAction MissingFieldAction
        {
            get
            {
                return this._missingFieldAction;
            }

            set
            {
                this._missingFieldAction = value;
            }
        }

        /// <summary>Gets or sets a value indicating if the reader supports multiline fields.</summary>
        /// <value>A value indicating if the reader supports multiline field.</value>
        public bool SupportsMultiline
        {
            get
            {
                return this._supportsMultiline;
            }

            set
            {
                this._supportsMultiline = value;
            }
        }

        /// <summary>Gets or sets a value indicating if the reader will skip empty lines.</summary>
        /// <value>A value indicating if the reader will skip empty lines.</value>
        public bool SkipEmptyLines
        {
            get
            {
                return this._skipEmptyLines;
            }

            set
            {
                this._skipEmptyLines = value;
            }
        }

        /// <summary>
        /// Gets or sets the default header name when it is an empty string or only whitespaces. The header index will be appended to the specified
        /// name.
        /// </summary>
        /// <value>The default header name when it is an empty string or only whitespaces.</value>
        public string DefaultHeaderName { get; set; }

        #endregion

        #region RequestState

        /// <summary>Gets the maximum number of fields to retrieve for each record.</summary>
        /// <value>The maximum number of fields to retrieve for each record.</value>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public int FieldCount
        {
            get
            {
                this.EnsureInitialize();
                return this._fieldCount;
            }
        }

        /// <summary>Gets a value that indicates whether the current stream position is at the end of the stream.</summary>
        /// <value>
        /// <see langword="true" /> if the current stream position is at the end of the stream; otherwise
        /// <see langword="false" />.
        /// </value>
        public virtual bool EndOfStream
        {
            get
            {
                return this._eof;
            }
        }

        /// <summary>Gets the field headers.</summary>
        /// <returns>The field headers or an empty array if headers are not supported.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public string[] GetFieldHeaders()
        {
            this.EnsureInitialize();
            Debug.Assert(this._fieldHeaders != null, "Field headers must be non null.");

            var fieldHeaders = new string[this._fieldHeaders.Length];

            for (var i = 0; i < fieldHeaders.Length; i++)
            {
                fieldHeaders[i] = this._fieldHeaders[i];
            }

            return fieldHeaders;
        }

        /// <summary>Gets the current record index in the CSV file.</summary>
        /// <value>The current record index in the CSV file.</value>
        public virtual long CurrentRecordIndex
        {
            get
            {
                return this._currentRecordIndex;
            }
        }

        /// <summary>Indicates if one or more field are missing for the current record. Resets after each successful record read.</summary>
        public bool MissingFieldFlag
        {
            get
            {
                return this._missingFieldFlag;
            }
        }

        /// <summary>Indicates if a parse error occured for the current record. Resets after each successful record read.</summary>
        public bool ParseErrorFlag
        {
            get
            {
                return this._parseErrorFlag;
            }
        }

        #endregion

        #endregion

        #region Indexers

        /// <summary>
        /// Gets the field with the specified name and record position. <see cref="M:hasHeaders"/> must be
        /// <see langword="true"/>
        /// .
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <value>
        /// The field with the specified name and record position.
        /// </value>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="field"/> is <see langword="null"/> or an empty string.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        /// The CSV does not have headers (<see cref="M:HasHeaders"/> property is
        /// <see langword="false"/>).
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// <paramref name="field"/> not found.
        /// </exception>
        /// <exception cref="T:ArgumentOutOfRangeException">
        /// Record index must be &gt; 0.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        /// Cannot move to a previous record in forward-only mode.
        /// </exception>
        /// <exception cref="T:EndOfStreamException">
        /// Cannot read record at <paramref name="record"/>.
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
        public string this[int record, string field]
        {
            get
            {
                if (!this.MoveTo(record))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, CsvException.CannotReadRecordAtIndex, record));
                }

                return this[field];
            }
        }

        /// <summary>
        /// Gets the field at the specified index and record position.
        /// </summary>
        /// <param name="record">
        /// The record.
        /// </param>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <value>
        /// The field at the specified index and record position. A <see langword="null"/> is returned if the field cannot be found for the record.
        /// </value>
        /// <exception cref="T:ArgumentOutOfRangeException">
        /// <paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.
        /// </exception>
        /// <exception cref="T:ArgumentOutOfRangeException">
        /// Record index must be &gt; 0.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        /// Cannot move to a previous record in forward-only mode.
        /// </exception>
        /// <exception cref="T:EndOfStreamException">
        /// Cannot read record at <paramref name="record"/>.
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
        public string this[int record, int field]
        {
            get
            {
                if (!this.MoveTo(record))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, CsvException.CannotReadRecordAtIndex, record));
                }

                return this[field];
            }
        }

        /// <summary>
        /// Gets the field with the specified name. <see cref="M:hasHeaders"/> must be <see langword="true"/>.
        /// </summary>
        /// <param name="field">
        /// The field.
        /// </param>
        /// <value>
        /// The field with the specified name.
        /// </value>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="field"/> is <see langword="null"/> or an empty string.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        /// The CSV does not have headers (<see cref="M:HasHeaders"/> property is
        /// <see langword="false"/>).
        /// </exception>
        /// <exception cref="T:ArgumentException">
        /// <paramref name="field"/> not found.
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
        public string this[string field]
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    throw new ArgumentNullException("field");
                }

                if (!this._hasHeaders)
                {
                    throw new InvalidOperationException(CsvException.NoHeaders);
                }

                var index = this.GetFieldIndex(field);

                if (index < 0)
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, CsvException.FieldHeaderNotFound, field), "field");
                }

                return this[index];
            }
        }

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
        /// <exception cref="T:MalformedCsvException">
        /// The CSV appears to be corrupt at the current position.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public virtual string this[int field]
        {
            get
            {
                return this.ReadField(field, false, false);
            }
        }

        #endregion

        #region Methods

        #region EnsureInitialize

        /// <summary>Ensures that the reader is initialized.</summary>
        private void EnsureInitialize()
        {
            if (!this._initialized)
            {
                this.ReadNextRecord(true, false);
            }

            Debug.Assert(this._fieldHeaders != null);
            Debug.Assert(this._fieldHeaders.Length > 0 || (this._fieldHeaders.Length == 0 && this._fieldHeaderIndexes == null));
        }

        #endregion

        #region GetFieldIndex

        /// <summary>
        /// Gets the field index for the provided header.
        /// </summary>
        /// <param name="header">
        /// The header to look for.
        /// </param>
        /// <returns>
        /// The field index for the provided header. -1 if not found.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        public int GetFieldIndex(string header)
        {
            this.EnsureInitialize();

            int index;

            if (this._fieldHeaderIndexes != null && this._fieldHeaderIndexes.TryGetValue(header, out index))
            {
                return index;
            }

            return -1;
        }

        #endregion

        #region CopyCurrentRecordTo

        /// <summary>
        /// Copies the field array of the current record to a one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:Array"/> that is the destination of the fields of the current record.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of fields in the record is greater than the available space from
        /// <paramref name="index"/> to the end of
        /// <paramref name="array"/>.
        /// </exception>
        public void CopyCurrentRecordTo(string[] array)
        {
            this.CopyCurrentRecordTo(array, 0);
        }

        /// <summary>
        /// Copies the field array of the current record to a one-dimensional array, starting at the beginning of the target array.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:Array"/> that is the destination of the fields of the current record.
        /// </param>
        /// <param name="index">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        /// <paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentOutOfRangeException">
        /// <paramref name="index"/> is les than zero or is equal to or greater than the length <paramref name="array"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No current record.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The number of fields in the record is greater than the available space from
        /// <paramref name="index"/> to the end of
        /// <paramref name="array"/>.
        /// </exception>
        public void CopyCurrentRecordTo(string[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, string.Empty);
            }

            if (this._currentRecordIndex < 0 || !this._initialized)
            {
                throw new InvalidOperationException(CsvException.NoCurrentRecord);
            }

            if (array.Length - index < this._fieldCount)
            {
                throw new ArgumentException(CsvException.NotEnoughSpaceInArray, "array");
            }

            for (var i = 0; i < this._fieldCount; i++)
            {
                if (this._parseErrorFlag)
                {
                    array[index + i] = null;
                }
                else
                {
                    array[index + i] = this[i];
                }
            }
        }

        #endregion

        #region GetCurrentRawData

        /// <summary>Gets the current raw CSV data.</summary>
        /// <remarks>Used for exception handling purpose.</remarks>
        /// <returns>The current raw CSV data.</returns>
        public string GetCurrentRawData()
        {
            if (this._buffer != null && this._bufferLength > 0)
            {
                return new string(this._buffer, 0, this._bufferLength);
            }

            return string.Empty;
        }

        #endregion

        #region IsWhiteSpace

        /// <summary>
        /// Indicates whether the specified Unicode character is categorized as white space.
        /// </summary>
        /// <param name="c">
        /// A Unicode character.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="c"/> is white space; otherwise, <see langword="false"/>.
        /// </returns>
        private bool IsWhiteSpace(char c)
        {
            // Handle cases where the delimiter is a whitespace (e.g. tab)
            if (c == this._delimiter)
            {
                return false;
            }

            // See char.IsLatin1(char c) in Reflector
            if (c <= '\x00ff')
            {
                return c == ' ' || c == '\t';
            }

            return CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
        }

        #endregion

        #region MoveTo

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
        public virtual bool MoveTo(long record)
        {
            if (record < this._currentRecordIndex)
            {
                return false;
            }

            // Get number of record to read
            var offset = record - this._currentRecordIndex;

            while (offset > 0)
            {
                if (!this.ReadNextRecord())
                {
                    return false;
                }

                offset--;
            }

            return true;
        }

        #endregion

        #region ParseNewLine

        /// <summary>
        /// Parses a new line delimiter.
        /// </summary>
        /// <param name="pos">
        /// The starting position of the parsing. Will contain the resulting end position.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a new line delimiter was found; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        private bool ParseNewLine(ref int pos)
        {
            Debug.Assert(pos <= this._bufferLength);

            // Check if already at the end of the buffer
            if (pos == this._bufferLength)
            {
                pos = 0;

                if (!this.ReadBuffer())
                {
                    return false;
                }
            }

            var c = this._buffer[pos];

            // Treat \r as new line only if it's not the delimiter
            if (c == '\r' && this._delimiter != '\r')
            {
                pos++;

                // Skip following \n (if there is one)
                if (pos < this._bufferLength)
                {
                    if (this._buffer[pos] == '\n')
                    {
                        pos++;
                    }
                }
                else
                {
                    if (this.ReadBuffer())
                    {
                        if (this._buffer[0] == '\n')
                        {
                            pos = 1;
                        }
                        else
                        {
                            pos = 0;
                        }
                    }
                }

                if (pos >= this._bufferLength)
                {
                    this.ReadBuffer();
                    pos = 0;
                }

                return true;
            }

            if (c == '\n')
            {
                pos++;

                if (pos >= this._bufferLength)
                {
                    this.ReadBuffer();
                    pos = 0;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the character at the specified position is a new line delimiter.
        /// </summary>
        /// <param name="pos">
        /// The position of the character to verify.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the character at the specified position is a new line delimiter; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        private bool IsNewLine(int pos)
        {
            Debug.Assert(pos < this._bufferLength);

            var c = this._buffer[pos];

            if (c == '\n')
            {
                return true;
            }

            if (c == '\r' && this._delimiter != '\r')
            {
                return true;
            }

            return false;
        }

        #endregion

        #region ReadBuffer

        /// <summary>Fills the buffer with data from the reader.</summary>
        /// <returns><see langword="true" /> if data was successfully read; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        private bool ReadBuffer()
        {
            if (this._eof)
            {
                return false;
            }

            this.CheckDisposed();

            this._bufferLength = this._reader.Read(this._buffer, 0, this._bufferSize);

            if (this._bufferLength > 0)
            {
                return true;
            }

            this._eof = true;
            this._buffer = null;

            return false;
        }

        #endregion

        #region ReadField

        /// <summary>
        /// Reads the field at the specified index. Any unread fields with an inferior index will also be read as part of the required parsing.
        /// </summary>
        /// <param name="field">
        /// The field index.
        /// </param>
        /// <param name="initializing">
        /// Indicates if the reader is currently initializing.
        /// </param>
        /// <param name="discardValue">
        /// Indicates if the value(s) are discarded.
        /// </param>
        /// <returns>
        /// The field at the specified index. A <see langword="null"/> indicates that an error occured or that the last field has been reached during
        /// initialization.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="field"/> is out of range.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// There is no current record.
        /// </exception>
        /// <exception cref="MissingFieldCsvException">
        /// The CSV data appears to be missing a field.
        /// </exception>
        /// <exception cref="CsvException">
        /// The CSV data appears to be malformed.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        private string ReadField(int field, bool initializing, bool discardValue)
        {
            if (!initializing)
            {
                if (field < 0 || field >= this._fieldCount)
                {
                    throw new ArgumentOutOfRangeException(
                        "field", 
                        field, 
                        string.Format(CultureInfo.InvariantCulture, CsvException.FieldIndexOutOfRange, field));
                }

                if (this._currentRecordIndex < 0)
                {
                    throw new InvalidOperationException(CsvException.NoCurrentRecord);
                }

                // Directly return field if cached
                if (this._fields[field] != null)
                {
                    return this._fields[field];
                }

                if (this._missingFieldFlag)
                {
                    return this.HandleMissingField(null, field, ref this._nextFieldStart);
                }
            }

            this.CheckDisposed();

            var index = this._nextFieldIndex;

            while (index < field + 1)
            {
                // Handle case where stated start of field is past buffer
                // This can occur because _nextFieldStart is simply 1 + last char position of previous field
                if (this._nextFieldStart == this._bufferLength)
                {
                    this._nextFieldStart = 0;

                    // Possible EOF will be handled later (see Handle_EOF1)
                    this.ReadBuffer();
                }

                string value = null;

                if (this._missingFieldFlag)
                {
                    value = this.HandleMissingField(value, index, ref this._nextFieldStart);
                }
                else if (this._nextFieldStart == this._bufferLength)
                {
                    // Handle_EOF1: Handle EOF here

                    // If current field is the requested field, then the value of the field is "" as in "f1,f2,f3,(\s*)"
                    // otherwise, the CSV is malformed
                    if (index == field)
                    {
                        if (!discardValue)
                        {
                            value = string.Empty;
                            this._fields[index] = value;
                        }

                        this._missingFieldFlag = true;
                    }
                    else
                    {
                        value = this.HandleMissingField(value, index, ref this._nextFieldStart);
                    }
                }
                else
                {
                    // Trim spaces at start
                    if ((this._trimmingOptions & ValueTrimmingOptions.UnquotedOnly) != 0)
                    {
                        this.SkipWhiteSpaces(ref this._nextFieldStart);
                    }

                    if (this._eof)
                    {
                        value = string.Empty;
                        this._fields[field] = value;

                        if (field < this._fieldCount)
                        {
                            this._missingFieldFlag = true;
                        }
                    }
                    else if (this._buffer[this._nextFieldStart] != this._quote)
                    {
                        // Non-quoted field
                        var start = this._nextFieldStart;
                        var pos = this._nextFieldStart;

                        for (;;)
                        {
                            while (pos < this._bufferLength)
                            {
                                var c = this._buffer[pos];

                                if (c == this._delimiter)
                                {
                                    this._nextFieldStart = pos + 1;

                                    break;
                                }

                                if (c == '\r' || c == '\n')
                                {
                                    this._nextFieldStart = pos;
                                    this._eol = true;

                                    break;
                                }

                                pos++;
                            }

                            if (pos < this._bufferLength)
                            {
                                break;
                            }

                            if (!discardValue)
                            {
                                value += new string(this._buffer, start, pos - start);
                            }

                            start = 0;
                            pos = 0;
                            this._nextFieldStart = 0;

                            if (!this.ReadBuffer())
                            {
                                break;
                            }
                        }

                        if (!discardValue)
                        {
                            if ((this._trimmingOptions & ValueTrimmingOptions.UnquotedOnly) == 0)
                            {
                                if (!this._eof && pos > start)
                                {
                                    value += new string(this._buffer, start, pos - start);
                                }
                            }
                            else
                            {
                                if (!this._eof && pos > start)
                                {
                                    // Do the trimming
                                    pos--;
                                    while (pos > -1 && this.IsWhiteSpace(this._buffer[pos]))
                                    {
                                        pos--;
                                    }

                                    pos++;

                                    if (pos > 0)
                                    {
                                        value += new string(this._buffer, start, pos - start);
                                    }
                                }
                                else
                                {
                                    pos = -1;
                                }

                                // If pos <= 0, that means the trimming went past buffer start,
                                // and the concatenated value needs to be trimmed too.
                                if (pos <= 0)
                                {
                                    pos = value == null ? -1 : value.Length - 1;

                                    // Do the trimming
                                    while (pos > -1 && this.IsWhiteSpace(value[pos]))
                                    {
                                        pos--;
                                    }

                                    pos++;

                                    if (pos > 0 && pos != value.Length)
                                    {
                                        value = value.Substring(0, pos);
                                    }
                                }
                            }

                            if (value == null)
                            {
                                value = string.Empty;
                            }
                        }

                        if (this._eol || this._eof)
                        {
                            this._eol = this.ParseNewLine(ref this._nextFieldStart);

                            // Reaching a new line is ok as long as the parser is initializing or it is the last field
                            if (!initializing && index != this._fieldCount - 1)
                            {
                                if (value != null && value.Length == 0)
                                {
                                    value = null;
                                }

                                value = this.HandleMissingField(value, index, ref this._nextFieldStart);
                            }
                        }

                        if (!discardValue)
                        {
                            this._fields[index] = value;
                        }
                    }
                    else
                    {
                        // Quoted field

                        // Skip quote
                        var start = this._nextFieldStart + 1;
                        var pos = start;

                        var quoted = true;
                        var escaped = false;

                        if ((this._trimmingOptions & ValueTrimmingOptions.QuotedOnly) != 0)
                        {
                            this.SkipWhiteSpaces(ref start);
                            pos = start;
                        }

                        for (;;)
                        {
                            while (pos < this._bufferLength)
                            {
                                var c = this._buffer[pos];

                                if (escaped)
                                {
                                    escaped = false;
                                    start = pos;
                                }

                                // IF current char is escape AND (escape and quote are different OR next char is a quote)
                                else if (c == this._escape
                                         && (this._escape != this._quote || (pos + 1 < this._bufferLength && this._buffer[pos + 1] == this._quote)
                                             || (pos + 1 == this._bufferLength && this._reader.Peek() == this._quote)))
                                {
                                    if (!discardValue)
                                    {
                                        value += new string(this._buffer, start, pos - start);
                                    }

                                    escaped = true;
                                }
                                else if (c == this._quote)
                                {
                                    quoted = false;
                                    break;
                                }

                                pos++;
                            }

                            if (!quoted)
                            {
                                break;
                            }

                            if (!discardValue && !escaped)
                            {
                                value += new string(this._buffer, start, pos - start);
                            }

                            start = 0;
                            pos = 0;
                            this._nextFieldStart = 0;

                            if (!this.ReadBuffer())
                            {
                                this.HandleParseError(
                                    new CsvException(this.GetCurrentRawData(), this._nextFieldStart, Math.Max(0, this._currentRecordIndex), index), 
                                    ref this._nextFieldStart);
                                return null;
                            }
                        }

                        if (!this._eof)
                        {
                            // Append remaining parsed buffer content
                            if (!discardValue && pos > start)
                            {
                                value += new string(this._buffer, start, pos - start);
                            }

                            if (!discardValue && value != null && (this._trimmingOptions & ValueTrimmingOptions.QuotedOnly) != 0)
                            {
                                var newLength = value.Length;
                                while (newLength > 0 && this.IsWhiteSpace(value[newLength - 1]))
                                {
                                    newLength--;
                                }

                                if (newLength < value.Length)
                                {
                                    value = value.Substring(0, newLength);
                                }
                            }

                            // Skip quote
                            this._nextFieldStart = pos + 1;

                            // Skip whitespaces between the quote and the delimiter/eol
                            this.SkipWhiteSpaces(ref this._nextFieldStart);

                            // Skip delimiter
                            bool delimiterSkipped;
                            if (this._nextFieldStart < this._bufferLength && this._buffer[this._nextFieldStart] == this._delimiter)
                            {
                                this._nextFieldStart++;
                                delimiterSkipped = true;
                            }
                            else
                            {
                                delimiterSkipped = false;
                            }

                            // Skip new line delimiter if initializing or last field
                            // (if the next field is missing, it will be caught when parsed)
                            if (!this._eof && !delimiterSkipped && (initializing || index == this._fieldCount - 1))
                            {
                                this._eol = this.ParseNewLine(ref this._nextFieldStart);
                            }

                            // If no delimiter is present after the quoted field and it is not the last field, then it is a parsing error
                            if (!delimiterSkipped && !this._eof && !(this._eol || this.IsNewLine(this._nextFieldStart)))
                            {
                                this.HandleParseError(
                                    new CsvException(this.GetCurrentRawData(), this._nextFieldStart, Math.Max(0, this._currentRecordIndex), index), 
                                    ref this._nextFieldStart);
                            }
                        }

                        if (!discardValue)
                        {
                            if (value == null)
                            {
                                value = string.Empty;
                            }

                            this._fields[index] = value;
                        }
                    }
                }

                this._nextFieldIndex = Math.Max(index + 1, this._nextFieldIndex);

                if (index == field)
                {
                    // If initializing, return null to signify the last field has been reached
                    if (initializing)
                    {
                        if (this._eol || this._eof)
                        {
                            return null;
                        }

                        return string.IsNullOrEmpty(value) ? string.Empty : value;
                    }

                    return value;
                }

                index++;
            }

            // Getting here is bad ...
            this.HandleParseError(
                new CsvException(this.GetCurrentRawData(), this._nextFieldStart, Math.Max(0, this._currentRecordIndex), index), 
                ref this._nextFieldStart);
            return null;
        }

        #endregion

        #region ReadNextRecord

        /// <summary>Reads the next record.</summary>
        /// <returns><see langword="true" /> if a record has been successfully reads; otherwise, <see langword="false" />.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public bool ReadNextRecord()
        {
            return this.ReadNextRecord(false, false);
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
        protected virtual bool ReadNextRecord(bool onlyReadHeaders, bool skipToNextLine)
        {
            if (this._eof)
            {
                if (this._firstRecordInCache)
                {
                    this._firstRecordInCache = false;
                    this._currentRecordIndex++;

                    return true;
                }

                return false;
            }

            this.CheckDisposed();

            if (!this._initialized)
            {
                this._buffer = new char[this._bufferSize];

                // will be replaced if and when headers are read
                this._fieldHeaders = new string[0];

                if (!this.ReadBuffer())
                {
                    return false;
                }

                if (!this.SkipEmptyAndCommentedLines(ref this._nextFieldStart))
                {
                    return false;
                }

                // Keep growing _fields array until the last field has been found
                // and then resize it to its final correct size
                this._fieldCount = 0;
                this._fields = new string[16];

                while (this.ReadField(this._fieldCount, true, false) != null)
                {
                    if (this._parseErrorFlag)
                    {
                        this._fieldCount = 0;
                        Array.Clear(this._fields, 0, this._fields.Length);
                        this._parseErrorFlag = false;
                        this._nextFieldIndex = 0;
                    }
                    else
                    {
                        this._fieldCount++;

                        if (this._fieldCount == this._fields.Length)
                        {
                            Array.Resize(ref this._fields, (this._fieldCount + 1) * 2);
                        }
                    }
                }

                // _fieldCount contains the last field index, but it must contains the field count,
                // so increment by 1
                this._fieldCount++;

                if (this._fields.Length != this._fieldCount)
                {
                    Array.Resize(ref this._fields, this._fieldCount);
                }

                this._initialized = true;

                // If headers are present, call ReadNextRecord again
                if (this._hasHeaders)
                {
                    // Don't count first record as it was the headers
                    this._currentRecordIndex = -1;

                    this._firstRecordInCache = false;

                    this._fieldHeaders = new string[this._fieldCount];
                    this._fieldHeaderIndexes = new Dictionary<string, int>(this._fieldCount, _fieldHeaderComparer);

                    for (var i = 0; i < this._fields.Length; i++)
                    {
                        var headerName = this._fields[i];
                        if (string.IsNullOrEmpty(headerName) || headerName.Trim().Length == 0)
                        {
                            headerName = this.DefaultHeaderName + i;
                        }

                        this._fieldHeaders[i] = headerName;
                        this._fieldHeaderIndexes.Add(headerName, i);
                    }

                    // Proceed to first record
                    if (!onlyReadHeaders)
                    {
                        // Calling again ReadNextRecord() seems to be simpler, 
                        // but in fact would probably cause many subtle bugs because a derived class does not expect a recursive behavior
                        // so simply do what is needed here and no more.
                        if (!this.SkipEmptyAndCommentedLines(ref this._nextFieldStart))
                        {
                            return false;
                        }

                        Array.Clear(this._fields, 0, this._fields.Length);
                        this._nextFieldIndex = 0;
                        this._eol = false;

                        this._currentRecordIndex++;
                        return true;
                    }
                }
                else
                {
                    if (onlyReadHeaders)
                    {
                        this._firstRecordInCache = true;
                        this._currentRecordIndex = -1;
                    }
                    else
                    {
                        this._firstRecordInCache = false;
                        this._currentRecordIndex = 0;
                    }
                }
            }
            else
            {
                if (skipToNextLine)
                {
                    this.SkipToNextLine(ref this._nextFieldStart);
                }
                else if (this._currentRecordIndex > -1 && !this._missingFieldFlag)
                {
                    // If not already at end of record, move there
                    if (!this._eol && !this._eof)
                    {
                        if (!this._supportsMultiline)
                        {
                            this.SkipToNextLine(ref this._nextFieldStart);
                        }
                        else
                        {
                            // a dirty trick to handle the case where extra fields are present
                            while (this.ReadField(this._nextFieldIndex, true, true) != null)
                            {
                            }
                        }
                    }
                }

                if (!this._firstRecordInCache && !this.SkipEmptyAndCommentedLines(ref this._nextFieldStart))
                {
                    return false;
                }

                if (this._hasHeaders || !this._firstRecordInCache)
                {
                    this._eol = false;
                }

                // Check to see if the first record is in cache.
                // This can happen when initializing a reader with no headers
                // because one record must be read to get the field count automatically
                if (this._firstRecordInCache)
                {
                    this._firstRecordInCache = false;
                }
                else
                {
                    Array.Clear(this._fields, 0, this._fields.Length);
                    this._nextFieldIndex = 0;
                }

                this._missingFieldFlag = false;
                this._parseErrorFlag = false;
                this._currentRecordIndex++;
            }

            return true;
        }

        #endregion

        #region SkipEmptyAndCommentedLines

        /// <summary>
        /// Skips empty and commented lines. If the end of the buffer is reached, its content be discarded and filled again from the reader.
        /// </summary>
        /// <param name="pos">
        /// The position in the buffer where to start parsing. Will contains the resulting position after the operation.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        private bool SkipEmptyAndCommentedLines(ref int pos)
        {
            if (pos < this._bufferLength)
            {
                this.DoSkipEmptyAndCommentedLines(ref pos);
            }

            while (pos >= this._bufferLength && !this._eof)
            {
                if (this.ReadBuffer())
                {
                    pos = 0;
                    this.DoSkipEmptyAndCommentedLines(ref pos);
                }
                else
                {
                    return false;
                }
            }

            return !this._eof;
        }

        /// <summary>
        /// <para>
        /// Worker method.
        /// </para>
        /// <para>
        /// Skips empty and commented lines.
        /// </para>
        /// </summary>
        /// <param name="pos">
        /// The position in the buffer where to start parsing. Will contains the resulting position after the operation.
        /// </param>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        private void DoSkipEmptyAndCommentedLines(ref int pos)
        {
            while (pos < this._bufferLength)
            {
                if (this._buffer[pos] == this._comment)
                {
                    pos++;
                    this.SkipToNextLine(ref pos);
                }
                else if (this._skipEmptyLines && this.ParseNewLine(ref pos))
                {
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

        #region SkipWhiteSpaces

        /// <summary>
        /// Skips whitespace characters.
        /// </summary>
        /// <param name="pos">
        /// The starting position of the parsing. Will contain the resulting end position.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        private bool SkipWhiteSpaces(ref int pos)
        {
            for (;;)
            {
                while (pos < this._bufferLength && this.IsWhiteSpace(this._buffer[pos]))
                {
                    pos++;
                }

                if (pos < this._bufferLength)
                {
                    break;
                }

                pos = 0;

                if (!this.ReadBuffer())
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region SkipToNextLine

        /// <summary>
        /// Skips ahead to the next NewLine character. If the end of the buffer is reached, its content be discarded and filled again from the reader.
        /// </summary>
        /// <param name="pos">
        /// The position in the buffer where to start parsing. Will contains the resulting position after the operation.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the end of the reader has not been reached; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// The instance has been disposed of.
        /// </exception>
        private bool SkipToNextLine(ref int pos)
        {
            // ((pos = 0) == 0) is a little trick to reset position inline
            while ((pos < this._bufferLength || (this.ReadBuffer() && ((pos = 0) == 0))) && !this.ParseNewLine(ref pos))
            {
                pos++;
            }

            return !this._eof;
        }

        #endregion

        #region HandleParseError

        /// <summary>
        /// Handles a parsing error.
        /// </summary>
        /// <param name="error">
        /// The parsing error that occured.
        /// </param>
        /// <param name="pos">
        /// The current position in the buffer.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        private void HandleParseError(CsvException error, ref int pos)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this._parseErrorFlag = true;

            switch (this._defaultParseErrorAction)
            {
                case ParseErrorAction.ThrowException:
                    throw error;

                case ParseErrorAction.RaiseEvent:
                    var e = new ParseErrorEventArgs(error, ParseErrorAction.ThrowException);
                    this.OnParseError(e);

                    switch (e.Action)
                    {
                        case ParseErrorAction.ThrowException:
                            throw e.Error;

                        case ParseErrorAction.RaiseEvent:
                            throw new InvalidOperationException(
                                string.Format(CultureInfo.InvariantCulture, CsvException.ParseErrorActionInvalidInsideParseErrorEvent, e.Action), 
                                e.Error);

                        case ParseErrorAction.AdvanceToNextLine:

                            // already at EOL when fields are missing, so don't skip to next line in that case
                            if (!this._missingFieldFlag && pos >= 0)
                            {
                                this.SkipToNextLine(ref pos);
                            }

                            break;

                        default:
                            throw new NotSupportedException(
                                string.Format(CultureInfo.InvariantCulture, CsvException.ParseErrorActionNotSupported, e.Action), 
                                e.Error);
                    }

                    break;

                case ParseErrorAction.AdvanceToNextLine:

                    // already at EOL when fields are missing, so don't skip to next line in that case
                    if (!this._missingFieldFlag && pos >= 0)
                    {
                        this.SkipToNextLine(ref pos);
                    }

                    break;

                default:
                    throw new NotSupportedException(
                        string.Format(CultureInfo.InvariantCulture, CsvException.ParseErrorActionNotSupported, this._defaultParseErrorAction), 
                        error);
            }
        }

        #endregion

        #region HandleMissingField

        /// <summary>
        /// Handles a missing field error.
        /// </summary>
        /// <param name="value">
        /// The partially parsed value, if available.
        /// </param>
        /// <param name="fieldIndex">
        /// The missing field index.
        /// </param>
        /// <param name="currentPosition">
        /// The current position in the raw data.
        /// </param>
        /// <returns>
        /// The resulting value according to <see cref="M:MissingFieldAction"/>. If the action is set to
        /// <see cref="T:MissingFieldAction.TreatAsParseError"/>, then the parse error will be handled according to
        /// <see cref="DefaultParseErrorAction"/>.
        /// </returns>
        private string HandleMissingField(string value, int fieldIndex, ref int currentPosition)
        {
            if (fieldIndex < 0 || fieldIndex >= this._fieldCount)
            {
                throw new ArgumentOutOfRangeException(
                    "fieldIndex", 
                    fieldIndex, 
                    string.Format(CultureInfo.InvariantCulture, CsvException.FieldIndexOutOfRange, fieldIndex));
            }

            this._missingFieldFlag = true;

            for (var i = fieldIndex + 1; i < this._fieldCount; i++)
            {
                this._fields[i] = null;
            }

            if (value != null)
            {
                return value;
            }

            switch (this._missingFieldAction)
            {
                case MissingFieldAction.ParseError:
                    this.HandleParseError(
                        new MissingFieldCsvException(this.GetCurrentRawData(), currentPosition, Math.Max(0, this._currentRecordIndex), fieldIndex), 
                        ref currentPosition);
                    return value;

                case MissingFieldAction.ReplaceByEmpty:
                    return string.Empty;

                case MissingFieldAction.ReplaceByNull:
                    return null;

                default:
                    throw new NotSupportedException(
                        string.Format(CultureInfo.InvariantCulture, CsvException.MissingFieldActionNotSupported, this._missingFieldAction));
            }
        }

        #endregion

        #endregion

        #region IDataReader support methods

        /// <summary>
        /// Validates the RequestState of the data reader.
        /// </summary>
        /// <param name="validations">
        /// The validations to accomplish.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// No current record.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This operation is invalid when the reader is closed.
        /// </exception>
        private void ValidateDataReader(DataReaderValidations validations)
        {
            if ((validations & DataReaderValidations.IsInitialized) != 0 && !this._initialized)
            {
                throw new InvalidOperationException(CsvException.NoCurrentRecord);
            }

            if ((validations & DataReaderValidations.IsNotClosed) != 0 && this._isDisposed)
            {
                throw new InvalidOperationException(CsvException.ReaderClosed);
            }
        }

        /// <summary>
        /// Copy the value of the specified field to an array.
        /// </summary>
        /// <param name="field">
        /// The index of the field.
        /// </param>
        /// <param name="fieldOffset">
        /// The offset in the field value.
        /// </param>
        /// <param name="destinationArray">
        /// The destination array where the field value will be copied.
        /// </param>
        /// <param name="destinationOffset">
        /// The destination array offset.
        /// </param>
        /// <param name="length">
        /// The number of characters to copy from the field value.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private long CopyFieldToArray(int field, long fieldOffset, Array destinationArray, int destinationOffset, int length)
        {
            this.EnsureInitialize();

            if (field < 0 || field >= this._fieldCount)
            {
                throw new ArgumentOutOfRangeException(
                    "field", 
                    field, 
                    string.Format(CultureInfo.InvariantCulture, CsvException.FieldIndexOutOfRange, field));
            }

            if (fieldOffset < 0 || fieldOffset >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("fieldOffset");
            }

            // Array.Copy(...) will do the remaining argument checks
            if (length == 0)
            {
                return 0;
            }

            var value = this[field];

            if (value == null)
            {
                value = string.Empty;
            }

            Debug.Assert(fieldOffset < int.MaxValue);

            Debug.Assert(destinationArray.GetType() == typeof(char[]) || destinationArray.GetType() == typeof(byte[]));

            if (destinationArray.GetType() == typeof(char[]))
            {
                Array.Copy(value.ToCharArray((int)fieldOffset, length), 0, destinationArray, destinationOffset, length);
            }
            else
            {
                var chars = value.ToCharArray((int)fieldOffset, length);
                var source = new byte[chars.Length];

                for (var i = 0; i < chars.Length; i++)
                {
                    source[i] = Convert.ToByte(chars[i]);
                }

                Array.Copy(source, 0, destinationArray, destinationOffset, length);
            }

            return length;
        }

        #endregion

        #region IDataReader Members

        /// <summary>Gets the records affected.</summary>
        int IDataReader.RecordsAffected
        {
            get
            {
                // For SELECT statements, -1 must be returned.
                return -1;
            }
        }

        /// <summary>Gets a value indicating whether is closed.</summary>
        bool IDataReader.IsClosed
        {
            get
            {
                return this._eof;
            }
        }

        /// <summary>The next result.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        bool IDataReader.NextResult()
        {
            this.ValidateDataReader(DataReaderValidations.IsNotClosed);

            return false;
        }

        /// <summary>The close.</summary>
        void IDataReader.Close()
        {
            this.Dispose();
        }

        /// <summary>The read.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        bool IDataReader.Read()
        {
            this.ValidateDataReader(DataReaderValidations.IsNotClosed);

            return this.ReadNextRecord();
        }

        /// <summary>Gets the depth.</summary>
        int IDataReader.Depth
        {
            get
            {
                this.ValidateDataReader(DataReaderValidations.IsNotClosed);

                return 0;
            }
        }

        /// <summary>The get schema table.</summary>
        /// <returns>The <see cref="DataTable" />.</returns>
        DataTable IDataReader.GetSchemaTable()
        {
            this.EnsureInitialize();
            this.ValidateDataReader(DataReaderValidations.IsNotClosed);

            var schema = new DataTable("SchemaTable");
            schema.Locale = CultureInfo.InvariantCulture;
            schema.MinimumCapacity = this._fieldCount;

            schema.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.DataType, typeof(object)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsAliased, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsExpression, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsKey, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsLong, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.NumericPrecision, typeof(short)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.NumericScale, typeof(short)).ReadOnly = true;
            schema.Columns.Add(SchemaTableColumn.ProviderType, typeof(int)).ReadOnly = true;

            schema.Columns.Add(SchemaTableOptionalColumn.BaseCatalogName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.BaseServerName, typeof(string)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsHidden, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsReadOnly, typeof(bool)).ReadOnly = true;
            schema.Columns.Add(SchemaTableOptionalColumn.IsRowVersion, typeof(bool)).ReadOnly = true;

            string[] columnNames;

            if (this._hasHeaders)
            {
                columnNames = this._fieldHeaders;
            }
            else
            {
                columnNames = new string[this._fieldCount];

                for (var i = 0; i < this._fieldCount; i++)
                {
                    columnNames[i] = "Column" + i.ToString(CultureInfo.InvariantCulture);
                }
            }

            // null marks columns that will change for each row
            object[] schemaRow =
                {
                    true, // 00- AllowDBNull
                    null, // 01- BaseColumnName
                    string.Empty, // 02- BaseSchemaName
                    string.Empty, // 03- BaseTableName
                    null, // 04- ColumnName
                    null, // 05- ColumnOrdinal
                    int.MaxValue, // 06- ColumnSize
                    typeof(string), // 07- CsvDataType
                    false, // 08- IsAliased
                    false, // 09- IsExpression
                    false, // 10- IsKey
                    false, // 11- IsLong
                    false, // 12- IsUnique
                    DBNull.Value, // 13- NumericPrecision
                    DBNull.Value, // 14- NumericScale
                    (int)DbType.String, // 15- ProviderType

                    string.Empty, // 16- BaseCatalogName
                    string.Empty, // 17- BaseServerName
                    false, // 18- IsAutoIncrement
                    false, // 19- IsHidden
                    true, // 20- IsReadOnly
                    false // 21- IsRowVersion
                };

            for (var i = 0; i < columnNames.Length; i++)
            {
                schemaRow[1] = columnNames[i]; // Base column name
                schemaRow[4] = columnNames[i]; // Column name
                schemaRow[5] = i; // Column ordinal

                schema.Rows.Add(schemaRow);
            }

            return schema;
        }

        #endregion

        #region IDataRecord Members

        /// <summary>
        /// The get int 32.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int IDataRecord.GetInt32(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            var value = this[i];

            return int.Parse(value == null ? string.Empty : value, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// The i data record.this.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object IDataRecord.this[string name]
        {
            get
            {
                this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return this[name];
            }
        }

        /// <summary>
        /// The i data record.this.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object IDataRecord.this[int i]
        {
            get
            {
                this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
                return this[i];
            }
        }

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        object IDataRecord.GetValue(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            if (((IDataRecord)this).IsDBNull(i))
            {
                return DBNull.Value;
            }

            return this[i];
        }

        /// <summary>
        /// The is db null.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IDataRecord.IsDBNull(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return string.IsNullOrEmpty(this[i]);
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="fieldOffset">
        /// The field offset.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="bufferoffset">
        /// The bufferoffset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            return this.CopyFieldToArray(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// The get byte.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        byte IDataRecord.GetByte(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return byte.Parse(this[i], CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// The get field type.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        Type IDataRecord.GetFieldType(int i)
        {
            this.EnsureInitialize();
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            if (i < 0 || i >= this._fieldCount)
            {
                throw new ArgumentOutOfRangeException("i", i, string.Format(CultureInfo.InvariantCulture, CsvException.FieldIndexOutOfRange, i));
            }

            return typeof(string);
        }

        /// <summary>
        /// The get decimal.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="decimal"/>.
        /// </returns>
        decimal IDataRecord.GetDecimal(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return decimal.Parse(this[i], CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// The get values.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int IDataRecord.GetValues(object[] values)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            IDataRecord record = this;

            for (var i = 0; i < this._fieldCount; i++)
            {
                values[i] = record.GetValue(i);
            }

            return this._fieldCount;
        }

        /// <summary>
        /// The get name.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        string IDataRecord.GetName(int i)
        {
            this.EnsureInitialize();
            this.ValidateDataReader(DataReaderValidations.IsNotClosed);

            if (i < 0 || i >= this._fieldCount)
            {
                throw new ArgumentOutOfRangeException("i", i, string.Format(CultureInfo.InvariantCulture, CsvException.FieldIndexOutOfRange, i));
            }

            if (this._hasHeaders)
            {
                return this._fieldHeaders[i];
            }

            return "Column" + i.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The get int 64.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        long IDataRecord.GetInt64(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return long.Parse(this[i], CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// The get double.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        double IDataRecord.GetDouble(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return double.Parse(this[i], CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// The get boolean.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IDataRecord.GetBoolean(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            var value = this[i];

            int result;

            if (int.TryParse(value, out result))
            {
                return result != 0;
            }

            return bool.Parse(value);
        }

        /// <summary>
        /// The get guid.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        Guid IDataRecord.GetGuid(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return new Guid(this[i]);
        }

        /// <summary>
        /// The get date time.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        DateTime IDataRecord.GetDateTime(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return DateTime.Parse(this[i], CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// The get ordinal.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        int IDataRecord.GetOrdinal(string name)
        {
            this.EnsureInitialize();
            this.ValidateDataReader(DataReaderValidations.IsNotClosed);

            int index;

            if (!this._fieldHeaderIndexes.TryGetValue(name, out index))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, CsvException.FieldHeaderNotFound, name), "name");
            }

            return index;
        }

        /// <summary>
        /// The get data type name.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string IDataRecord.GetDataTypeName(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return typeof(string).FullName;
        }

        /// <summary>
        /// The get float.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        float IDataRecord.GetFloat(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return float.Parse(this[i], CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// The get data.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        IDataReader IDataRecord.GetData(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            if (i == 0)
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// The get chars.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <param name="fieldoffset">
        /// The fieldoffset.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="bufferoffset">
        /// The bufferoffset.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);

            return this.CopyFieldToArray(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// The get string.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string IDataRecord.GetString(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return this[i];
        }

        /// <summary>
        /// The get char.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="char"/>.
        /// </returns>
        char IDataRecord.GetChar(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return char.Parse(this[i]);
        }

        /// <summary>
        /// The get int 16.
        /// </summary>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The <see cref="short"/>.
        /// </returns>
        short IDataRecord.GetInt16(int i)
        {
            this.ValidateDataReader(DataReaderValidations.IsInitialized | DataReaderValidations.IsNotClosed);
            return short.Parse(this[i], CultureInfo.CurrentCulture);
        }

        #endregion

        #region IEnumerable<string[]> Members

        /// <summary>Returns an <see cref="T:RecordEnumerator" />  that can iterate through CSV records.</summary>
        /// <returns>An <see cref="T:RecordEnumerator" />  that can iterate through CSV records.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        public RecordEnumerator GetEnumerator()
        {
            return new RecordEnumerator(this);
        }

        /// <summary>Returns an <see cref="T:System.Collections.Generics.IEnumerator" />  that can iterate through CSV records.</summary>
        /// <returns>An <see cref="T:System.Collections.Generics.IEnumerator" />  that can iterate through CSV records.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        IEnumerator<string[]> IEnumerable<string[]>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IDisposable members

#if DEBUG

        /// <summary>Contains the stack when the object was allocated.</summary>
        private readonly StackTrace _allocStack;
#endif

        /// <summary>Contains the disposed status flag.</summary>
        private bool _isDisposed;

        /// <summary>Contains the locking object for multi-threading purpose.</summary>
        private readonly object _lock = new object();

        /// <summary>Occurs when the instance is disposed of.</summary>
        public event EventHandler Disposed;

        /// <summary>Gets a value indicating whether the instance has been disposed of.</summary>
        /// <value>
        /// <see langword="true" /> if the instance has been disposed of; otherwise, <see langword="false" />.
        /// </value>
        [System.ComponentModel.Browsable(false)]
        public bool IsDisposed
        {
            get
            {
                return this._isDisposed;
            }
        }

        /// <summary>
        /// Raises the <see cref="M:Disposed"/> event.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnDisposed(EventArgs e)
        {
            var handler = this.Disposed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Checks if the instance has been disposed of, and if it has, throws an
        /// <see cref="T:System.ComponentModel.ObjectDisposedException" />; otherwise, does nothing.
        /// </summary>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">The instance has been disposed of.</exception>
        /// <remarks>
        /// Derived classes should call this method at the start of all methods and properties that should not be accessed after a call to
        /// <see cref="M:Dispose()" />.
        /// </remarks>
        protected void CheckDisposed()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>Releases all resources used by the instance.</summary>
        /// <remarks>Calls <see cref="M:Dispose(Boolean)" /> with the disposing parameter set to <see langword="true" /> to free unmanaged and managed resources.</remarks>
        public void Dispose()
        {
            if (!this._isDisposed)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            // Refer to http://www.bluebytesoftware.com/blog/PermaLink,guid,88e62cdf-5919-4ac7-bc33-20c06ae539ae.aspx
            // Refer to http://www.gotdotnet.com/team/libraries/whitepapers/resourcemanagement/resourcemanagement.aspx

            // No exception should ever be thrown except in critical scenarios.
            // Unhandled exceptions during finalization will tear down the process.
            if (!this._isDisposed)
            {
                try
                {
                    // Dispose-time code should call Dispose() on all owned objects that implement the IDisposable interface. 
                    // "owned" means objects whose lifetime is solely controlled by the container. 
                    // In cases where ownership is not as straightforward, techniques such as HandleCollector can be used.  
                    // Large managed object fields should be nulled out.

                    // Dispose-time code should also set references of all owned objects to null, after disposing them. This will allow the referenced objects to be garbage collected even if not all references to the "parent" are released. It may be a significant memory consumption win if the referenced objects are large, such as big arrays, collections, etc. 
                    if (disposing)
                    {
                        // Acquire a lock on the object while disposing.
                        if (this._reader != null)
                        {
                            lock (this._lock)
                            {
                                if (this._reader != null)
                                {
                                    this._reader.Dispose();

                                    this._reader = null;
                                    this._buffer = null;
                                    this._eof = true;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    // Ensure that the flag is set
                    this._isDisposed = true;

                    // Catch any issues about firing an event on an already disposed object.
                    try
                    {
                        this.OnDisposed(EventArgs.Empty);
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="CsvReader" /> class. Releases unmanaged resources and performs other cleanup operations before the
        /// instance is reclaimed by garbage collection.
        /// </summary>
        ~CsvReader()
        {
#if DEBUG
            Debug.WriteLine("FinalizableObject was not disposed" + this._allocStack);
#endif

            this.Dispose(false);
        }

        #endregion
    }
}