// HtmlAgilityPack V1.0 - Simon Mourier <simon underscore mourier at hotmail dot com>

namespace SkyDean.FareLiz.Core.Utils.HtmlAgilityPack
{
    using System;
    using System.Diagnostics;

    internal class HtmlConsoleListener : TraceListener
    {
        #region Public Methods

        public override void Write(string Message)
        {
            this.Write(Message, "");
        }

        public override void Write(string Message, string Category)
        {
            Console.Write("T:" + Category + ": " + Message);
        }

        public override void WriteLine(string Message)
        {
            this.Write(Message + "\n");
        }

        public override void WriteLine(string Message, string Category)
        {
            this.Write(Message + "\n", Category);
        }

        #endregion
    }
}