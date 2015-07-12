using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkyDean.FareLiz.Service.Utils
{
    /// <summary>
    /// The delegate to be used in conjuction with the FtpConnectionEventArgs class
    /// </summary>
    public delegate void FtpConnectionEventHandler(object sender, FtpConnectionEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="FtpConnection.MessageReceived">MessageReceived event</see>
    /// </summary>
    public class FtpConnectionEventArgs : EventArgs
    {
        private string mMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpConnectionEventArgs"/>
        /// </summary>
        /// <param name="message">The status message to be sendt</param>
        internal FtpConnectionEventArgs(string message)
        {
            mMessage = message;
        }

        /// <summary>
        /// Contains status messages from the <see cref="FtpConnection"/> class during operations
        /// </summary>
        public string Message
        {
            get { return mMessage; }
        }

    }
}
