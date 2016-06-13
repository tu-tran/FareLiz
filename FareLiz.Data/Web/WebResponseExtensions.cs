namespace SkyDean.FareLiz.Data.Web
{
    using System.Net;
    using System.Text;

    /// <summary>
    /// The web response extensions.
    /// </summary>
    public static class WebResponseExtensions
    {
        /// <summary>
        /// The accept.
        /// </summary>
        public static readonly string AcceptString = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

        /// <summary>
        /// The content type.
        /// </summary>
        public const string ContentType = "application/x-www-form-urlencoded";

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
        /// Gets the request.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="requuestUrl">The requuest URL.</param>
        /// <returns>The web request.</returns>
        public static HttpWebRequest GetRequest(this string requuestUrl, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(requuestUrl);
            request.Method = method;
            request.Accept = AcceptString;
            request.UserAgent = UserAgent.Get();
            return request;
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
            var request = requuestUrl.GetRequest(method);
            request.CookieContainer = cookies;
            return (HttpWebResponse)request.GetResponse();
        }
    }
}
