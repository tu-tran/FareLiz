// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>

namespace SkyDean.FareLiz.Core.Utils.HtmlAgilityPack
{
    using System.Xml;

    internal class HtmlNameTable : XmlNameTable
    {
        #region Fields

        private NameTable _nametable = new NameTable();

        #endregion

        #region Public Methods

        public override string Add(string array)
        {
            return this._nametable.Add(array);
        }

        public override string Add(char[] array, int offset, int length)
        {
            return this._nametable.Add(array, offset, length);
        }

        public override string Get(string array)
        {
            return this._nametable.Get(array);
        }

        public override string Get(char[] array, int offset, int length)
        {
            return this._nametable.Get(array, offset, length);
        }

        #endregion

        #region Internal Methods

        internal string GetOrAdd(string array)
        {
            string s = this.Get(array);
            if (s == null)
            {
                return this.Add(array);
            }
            return s;
        }

        #endregion
    }
}