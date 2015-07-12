// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>
namespace SkyDean.FareLiz.Core.Utils.HtmlAgilityPack
{
    /// <summary>
    /// Represents a fragment of code in a mixed code document.
    /// </summary>
    public class MixedCodeDocumentCodeFragment : MixedCodeDocumentFragment
    {
        #region Fields

        private string _code;

        #endregion

        #region Constructors

        internal MixedCodeDocumentCodeFragment(MixedCodeDocument doc)
            :
                base(doc, MixedCodeDocumentFragmentType.Code)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fragment code text.
        /// </summary>
        public string Code
        {
            get
            {
                if (this._code == null)
                {
                    this._code = this.FragmentText.Substring(this.Doc.TokenCodeStart.Length,
                                                   this.FragmentText.Length - this.Doc.TokenCodeEnd.Length -
                                                   this.Doc.TokenCodeStart.Length - 1).Trim();
                    if (this._code.StartsWith("="))
                    {
                        this._code = this.Doc.TokenResponseWrite + this._code.Substring(1, this._code.Length - 1);
                    }
                }
                return this._code;
            }
            set { this._code = value; }
        }

        #endregion
    }
}