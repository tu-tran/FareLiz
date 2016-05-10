namespace SkyDean.FareLiz.InterFlight
{
    using System.Net;
    using System.Text;

    /// <summary>
    /// The web response extensions.
    /// </summary>
    public static class WebResponseExtensions
    {
        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The encoding.</returns>
        public static Encoding GetEncoding(this HttpWebResponse response)
        {
            return response.CharacterSet == null ? Encoding.UTF8 : Encoding.GetEncoding(response.CharacterSet);
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <param name="requuestUrl">The requuest URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="cookies">The cookies.</param>
        /// <param name="referral">The referral.</param>
        /// <returns>The web response.</returns>
        public static HttpWebResponse GetResponse(this string requuestUrl, string method, CookieContainer cookies, string referral)
        {
            var request = (HttpWebRequest)WebRequest.Create(requuestUrl);
            request.Method = method;
            request.Accept = PTDataGenerator.ACCEPT;
            request.CookieContainer = cookies;
            request.UserAgent = PTDataGenerator.USER_AGENT;
            return (HttpWebResponse)request.GetResponse();
        }
    }
}
