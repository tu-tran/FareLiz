namespace SkyDean.FareLiz.Data.Csv
{
    using System;

    using SkyDean.FareLiz.Core.Data;

    /// <summary>Provides data for the <see cref="M:CsvReader.ParseError" /> event.</summary>
    public class ParseErrorEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ParseErrorEventArgs class.
        /// </summary>
        /// <param name="error">
        /// The error that occured.
        /// </param>
        /// <param name="defaultAction">
        /// The default action to take.
        /// </param>
        public ParseErrorEventArgs(CsvException error, ParseErrorAction defaultAction)
        {
            this._error = error;
            this._action = defaultAction;
        }

        #endregion

        #region Fields

        /// <summary>Contains the error that occured.</summary>
        private readonly CsvException _error;

        /// <summary>Contains the action to take.</summary>
        private ParseErrorAction _action;

        #endregion

        #region Properties

        /// <summary>Gets the error that occured.</summary>
        /// <value>The error that occured.</value>
        public CsvException Error
        {
            get
            {
                return this._error;
            }
        }

        /// <summary>Gets or sets the action to take.</summary>
        /// <value>The action to take.</value>
        public ParseErrorAction Action
        {
            get
            {
                return this._action;
            }

            set
            {
                this._action = value;
            }
        }

        #endregion
    }
}