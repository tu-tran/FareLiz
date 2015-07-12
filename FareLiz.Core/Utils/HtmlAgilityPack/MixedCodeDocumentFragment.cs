// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>
namespace SkyDean.FareLiz.Core.Utils.HtmlAgilityPack
{
    /// <summary>
    /// Represents a base class for fragments in a mixed code document.
    /// </summary>
    public abstract class MixedCodeDocumentFragment
    {
        #region Fields

        internal MixedCodeDocument Doc;
        private string _fragmentText;
        internal int Index;
        internal int Length;
        private int _line;
        internal int _lineposition;
        internal MixedCodeDocumentFragmentType _type;

        #endregion

        #region Constructors

        internal MixedCodeDocumentFragment(MixedCodeDocument doc, MixedCodeDocumentFragmentType type)
        {
            this.Doc = doc;
            this._type = type;
            switch (type)
            {
                case MixedCodeDocumentFragmentType.Text:
                    this.Doc._textfragments.Append(this);
                    break;

                case MixedCodeDocumentFragmentType.Code:
                    this.Doc._codefragments.Append(this);
                    break;
            }
            this.Doc._fragments.Append(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fragement text.
        /// </summary>
        public string FragmentText
        {
            get
            {
                if (this._fragmentText == null)
                {
                    this._fragmentText = this.Doc._text.Substring(this.Index, this.Length);
                }
                return this.FragmentText;
            }
            internal set { this._fragmentText = value; }
        }

        /// <summary>
        /// Gets the type of fragment.
        /// </summary>
        public MixedCodeDocumentFragmentType FragmentType
        {
            get { return this._type; }
        }

        /// <summary>
        /// Gets the line number of the fragment.
        /// </summary>
        public int Line
        {
            get { return this._line; }
            internal set { this._line = value; }
        }

        /// <summary>
        /// Gets the line position (column) of the fragment.
        /// </summary>
        public int LinePosition
        {
            get { return this._lineposition; }
        }

        /// <summary>
        /// Gets the fragment position in the document's stream.
        /// </summary>
        public int StreamPosition
        {
            get { return this.Index; }
        }

        #endregion
    }
}