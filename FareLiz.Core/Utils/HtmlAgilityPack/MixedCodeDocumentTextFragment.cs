// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>
namespace SkyDean.FareLiz.Core.Utils.HtmlAgilityPack
{
    /// <summary>
    /// Represents a fragment of text in a mixed code document.
    /// </summary>
    public class MixedCodeDocumentTextFragment : MixedCodeDocumentFragment
    {
        #region Constructors

        internal MixedCodeDocumentTextFragment(MixedCodeDocument doc)
            :
                base(doc, MixedCodeDocumentFragmentType.Text)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the fragment text.
        /// </summary>
        public string Text
        {
            get { return this.FragmentText; }
            set { this.FragmentText = value; }
        }

        #endregion
    }
}