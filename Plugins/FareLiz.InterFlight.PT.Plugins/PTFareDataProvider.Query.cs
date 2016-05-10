namespace SkyDean.FareLiz.InterFlight
{
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data.Monitoring;

    /// <summary>
    /// The pt fare data provider.
    /// </summary>
    public partial class PTFareDataProvider
    {
        /// <summary>
        /// The seed.
        /// </summary>
        private static readonly byte[] Seed = { 0x95, 0x64, 0x01, 0x38, 0x46, 0x64, 0x78, 0x77 };

        /// <summary>
        /// The root base.
        /// </summary>
        private static readonly byte[] RootBase =
            {
                0x37, 0x62, 0x38, 0x32, 0x64, 0x36, 0x63, 0x64, 0x30, 0x30, 0x31, 0x37, 0x38, 0x38, 0x36, 0x63,
                0x33, 0x32, 0x66, 0x33, 0x33, 0x39, 0x37, 0x37, 0x35, 0x31, 0x31, 0x61, 0x64, 0x38, 0x63, 0x32,
                0x32, 0x64, 0x66, 0x30, 0x36, 0x33, 0x37, 0x37, 0x30, 0x34, 0x34, 0x35, 0x38, 0x33, 0x63, 0x37,
                0x33, 0x62, 0x66, 0x31, 0x32, 0x38, 0x36, 0x62, 0x34, 0x35, 0x35, 0x33, 0x39, 0x65, 0x39, 0x61
            };

        /// <summary>
        /// The request base.
        /// </summary>
        private static readonly byte[] RequestBase =
            {
                0x37, 0x62, 0x38, 0x32, 0x64, 0x36, 0x63, 0x64, 0x30, 0x30, 0x31, 0x37, 0x38, 0x38, 0x36, 0x63,
                0x33, 0x32, 0x66, 0x33, 0x33, 0x39, 0x37, 0x37, 0x35, 0x31, 0x31, 0x61, 0x64, 0x38, 0x63, 0x32,
                0x32, 0x64, 0x66, 0x30, 0x36, 0x33, 0x37, 0x37, 0x30, 0x34, 0x34, 0x35, 0x38, 0x33, 0x63, 0x37,
                0x33, 0x62, 0x66, 0x31, 0x32, 0x38, 0x36, 0x62, 0x34, 0x35, 0x35, 0x33, 0x39, 0x65, 0x39, 0x61,
                0x33, 0x36, 0x65, 0x32, 0x32, 0x33, 0x37, 0x33, 0x30, 0x34, 0x35, 0x64, 0x39, 0x36, 0x64, 0x65,
                0x32, 0x66, 0x66, 0x33, 0x33, 0x38, 0x36, 0x62, 0x30, 0x34, 0x35, 0x65, 0x38, 0x34, 0x64, 0x30,
                0x32, 0x65, 0x61, 0x39, 0x33, 0x64, 0x36, 0x66, 0x31, 0x62, 0x30, 0x61, 0x38, 0x34, 0x64, 0x36,
                0x36, 0x37, 0x62, 0x36, 0x34, 0x64, 0x30, 0x37, 0x36, 0x62, 0x33, 0x35, 0x66, 0x37, 0x62, 0x35
            };

        /// <summary>
        /// The domain base.
        /// </summary>
        private static readonly byte[] DomainBase =
            {
                0x37, 0x62, 0x38, 0x32, 0x64, 0x36, 0x63, 0x64, 0x30, 0x30, 0x31, 0x37, 0x38, 0x38, 0x36, 0x63, 
                0x32, 0x61, 0x65, 0x38, 0x33, 0x64, 0x37, 0x33, 0x31, 0x39, 0x35, 0x34, 0x38, 0x31, 0x64, 0x30, 
                0x33, 0x36, 0x61, 0x39, 0x32, 0x62, 0x36, 0x65, 0x36, 0x62, 0x33, 0x35, 0x66, 0x37, 0x62, 0x35
            };

        /// <summary>
        /// The cookie base.
        /// </summary>
        private static readonly byte[] CookieBase =
            {
                0x37, 0x62, 0x38, 0x32, 0x64, 0x36, 0x63, 0x64, 0x30, 0x30, 0x31, 0x37, 0x38, 0x38, 0x36, 0x63, 
                0x30, 0x61, 0x65, 0x38, 0x33, 0x64, 0x37, 0x33, 0x31, 0x39, 0x35, 0x34, 0x38, 0x31, 0x64, 0x30, 
                0x33, 0x36, 0x38, 0x37, 0x34, 0x64, 0x30, 0x37, 0x36, 0x62, 0x33, 0x35, 0x66, 0x37, 0x62, 0x35
            };

        /// <summary>
        /// The intermediate result base.
        /// </summary>
        private static readonly byte[] IntermediateResultBase =
            {
                0x31, 0x31, 0x37, 0x33, 0x62, 0x31, 0x39, 0x30, 0x63, 0x31, 0x36, 0x66, 0x38, 0x31,
                0x39, 0x65, 0x34, 0x35, 0x61, 0x62, 0x37, 0x62, 0x38, 0x33, 0x32, 0x33, 0x66, 0x32,
                0x33, 0x65, 0x34, 0x38, 0x35, 0x61, 0x61, 0x38, 0x32, 0x31, 0x38, 0x33, 0x37, 0x36,
                0x61, 0x64, 0x36, 0x35, 0x34, 0x64, 0x34, 0x63, 0x61, 0x39, 0x36, 0x61, 0x39, 0x66,
                0x33, 0x37, 0x62, 0x62, 0x37, 0x38, 0x31, 0x30, 0x35, 0x66, 0x62, 0x61, 0x37, 0x63,
                0x38, 0x36, 0x37, 0x35, 0x61, 0x39, 0x36, 0x32, 0x31, 0x30, 0x31, 0x64, 0x65, 0x65,
                0x35, 0x30, 0x39, 0x35, 0x37, 0x64, 0x61, 0x66, 0x33, 0x66, 0x34, 0x66, 0x34, 0x35,
                0x61, 0x66, 0x33, 0x30, 0x38, 0x30, 0x37, 0x30, 0x62, 0x39, 0x32, 0x63, 0x34, 0x34,
                0x31, 0x64, 0x61, 0x32, 0x32, 0x39, 0x38, 0x33, 0x37, 0x30, 0x62, 0x39, 0x32, 0x63,
                0x34, 0x34, 0x31, 0x63, 0x61, 0x32, 0x30, 0x66, 0x66, 0x33, 0x31, 0x39, 0x64, 0x64,
                0x31, 0x31, 0x33, 0x66
            };

        /// <summary>
        /// The result base.
        /// </summary>
        private static readonly byte[] ResultBase =
            {
                0x31, 0x31, 0x37, 0x33, 0x62, 0x31, 0x39, 0x30, 0x63, 0x31, 0x36, 0x66, 0x38, 0x31, 0x39, 0x65,
                0x34, 0x35, 0x61, 0x62, 0x37, 0x62, 0x38, 0x33, 0x32, 0x33, 0x66, 0x32, 0x33, 0x65, 0x34, 0x38,
                0x35, 0x61, 0x61, 0x38, 0x32, 0x31, 0x38, 0x33, 0x37, 0x36, 0x61, 0x64, 0x36, 0x35, 0x34, 0x64,
                0x34, 0x63, 0x61, 0x39, 0x36, 0x61, 0x39, 0x66, 0x33, 0x37, 0x62, 0x62, 0x37, 0x38, 0x31, 0x30,
                0x35, 0x66, 0x62, 0x61, 0x37, 0x63, 0x38, 0x36, 0x37, 0x35, 0x61, 0x39, 0x36, 0x32, 0x36, 0x30,
                0x31, 0x64, 0x65, 0x65, 0x35, 0x30, 0x39, 0x62, 0x36, 0x64, 0x62, 0x30, 0x37, 0x64, 0x36, 0x30,
                0x34, 0x65, 0x62, 0x30, 0x36, 0x31, 0x38, 0x37, 0x37, 0x63, 0x62, 0x33, 0x36, 0x35, 0x31, 0x31,
                0x35, 0x64, 0x62, 0x37, 0x37, 0x66, 0x63, 0x63, 0x36, 0x61, 0x62, 0x34, 0x37, 0x35, 0x30, 0x32,
                0x35, 0x36, 0x65, 0x66, 0x37, 0x32, 0x66, 0x33, 0x31, 0x39, 0x64, 0x64, 0x31, 0x31, 0x33, 0x66
            };

        /// <summary>
        /// The result referal base.
        /// </summary>
        private static readonly byte[] ResultReferalBase =
            {
                0x31, 0x31, 0x37, 0x33, 0x62, 0x31, 0x39, 0x30, 0x63, 0x31, 0x36, 0x66, 0x38, 0x31, 0x39,
                0x65, 0x34, 0x35, 0x61, 0x62, 0x37, 0x62, 0x38, 0x33, 0x32, 0x33, 0x66, 0x32, 0x33, 0x65,
                0x34, 0x38, 0x35, 0x61, 0x61, 0x38, 0x32, 0x31, 0x38, 0x33, 0x37, 0x36, 0x61, 0x64, 0x36,
                0x35, 0x34, 0x64, 0x34, 0x63, 0x61, 0x39, 0x36, 0x61, 0x39, 0x66, 0x33, 0x37, 0x62, 0x62,
                0x37, 0x38, 0x31, 0x30, 0x34, 0x31, 0x62, 0x61, 0x36, 0x31, 0x38, 0x37, 0x37, 0x36, 0x62,
                0x35, 0x37, 0x30, 0x35, 0x34, 0x35, 0x38, 0x61, 0x62, 0x37, 0x61, 0x39, 0x66, 0x37, 0x36,
                0x62, 0x36, 0x36, 0x32, 0x35, 0x61, 0x35, 0x39, 0x66, 0x31, 0x37, 0x66, 0x39, 0x62, 0x36,
                0x39, 0x65, 0x32, 0x36, 0x32, 0x35, 0x63, 0x31, 0x30, 0x65, 0x65, 0x30, 0x66, 0x66, 0x33,
                0x31, 0x39, 0x64, 0x64, 0x31, 0x31, 0x33, 0x66
            };

        /// <summary>
        /// The _data grep.
        /// </summary>
        private readonly DataGrep _dataGrep;

        /// <summary>
        /// The _generator.
        /// </summary>
        private readonly PTDataGenerator _generator = new PTDataGenerator();

        /// <summary>
        /// The _root_.
        /// </summary>
        private readonly string _root_;

        /// <summary>
        /// The _request_.
        /// </summary>
        private readonly string _request_;

        /// <summary>
        /// The _domain_.
        /// </summary>
        private readonly string _domain_;

        /// <summary>
        /// The _cookie_.
        /// </summary>
        private readonly string _cookie_;

        /// <summary>
        /// The _intermediate result_.
        /// </summary>
        private string _intermediateResult_;

        /// <summary>
        /// The _result_.
        /// </summary>
        private readonly string _result_;

        /// <summary>
        /// The _result referal_.
        /// </summary>
        private readonly string _resultReferal_;

        /// <summary>
        /// Initializes a new instance of the <see cref="PTFareDataProvider"/> class.
        /// </summary>
        public PTFareDataProvider()
        {
            this._dataGrep = new DataGrep(Seed);
            this._root_ = this._dataGrep.Convert(RootBase);
            this._request_ = this._dataGrep.Convert(RequestBase);
            this._domain_ = this._dataGrep.Convert(DomainBase);
            this._cookie_ = this._dataGrep.Convert(CookieBase);
            this._result_ = this._dataGrep.Convert(ResultBase);
            this._intermediateResult_ = this._dataGrep.Convert(IntermediateResultBase);
            this._resultReferal_ = this._dataGrep.Convert(ResultReferalBase);
        }

        /// <summary>
        /// The query data.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="progressChangedHandler">
        /// The progress changed handler.
        /// </param>
        /// <returns>
        /// The <see cref="DataRequestResult"/>.
        /// </returns>
        public DataRequestResult QueryData(FlightFareRequest request, JourneyProgressChangedEventHandler progressChangedHandler)
        {
            byte[] postData = this._generator.GeneratePOSTData(request);

            var httpRequest = (HttpWebRequest)WebRequest.Create(this._request_ + "1");
            httpRequest.Method = "POST";
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.Accept = PTDataGenerator.ACCEPT;
            httpRequest.Referer = this._root_;
            httpRequest.UserAgent = PTDataGenerator.USER_AGENT;
            httpRequest.ContentType = "application/x-www-form-urlencoded";
            httpRequest.ContentLength = postData.Length;

            Stream stream = httpRequest.GetRequestStream();
            stream.Write(postData, 0, postData.Length);
            stream.Close();

            string tokenId = null;
            string requestId = null;

            using (var response = (HttpWebResponse)httpRequest.GetResponse())
            {
                string waitUri = response.ResponseUri.ToString();
                requestId = this.GetTicketId(response);
                response.Close();

                if (string.IsNullOrEmpty(requestId))
                {
                    return new DataRequestResult(DataRequestState.Failed, null);
                }

                var match = Regex.Match(waitUri, @"\?" + this._cookie_ + @"\=(?<id>.+?)\&", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    tokenId = match.Groups["id"].Value;
                }
                else
                {
                    var cookies = response.Cookies;
                    foreach (Cookie c in cookies)
                    {
                        if (c.Name == this._cookie_)
                        {
                            tokenId = c.Value;
                            break;
                        }
                    }
                }
            }

            DataRequestResult requestResult;
            do
            {
                requestResult = this.GetResult(requestId, tokenId, request);
            }
            while (requestResult.RequestState == DataRequestState.Pending || requestResult.RequestState == DataRequestState.Requested);

            return requestResult;
        }

        /// <summary>
        /// Gets the ticket identifier.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns>The ticket identifier.</returns>
        private string GetTicketId(HttpWebResponse response)
        {
            using (var dataStream = response.GetResponseStream())
            {
                if (dataStream != null)
                {
                    return PTDataParser.ParseRequestId(dataStream, response.GetEncoding());
                }
            }

            return null;
        }

        /// <summary>
        /// The get result.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <param name="requestToken">The request token.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The <see cref="DataRequestResult" />.
        /// </returns>
        private DataRequestResult GetResult(string requestId, string requestToken, FlightFareRequest request)
        {
            TravelRoute result = null;

            var cookies = new CookieContainer();
            var cookie = new Cookie(this._cookie_, requestToken) { Domain = this._domain_ };
            cookies.Add(cookie);

            for (var i = 0; i < 4; i++)
            {
                var requestUrl = string.Format(this._intermediateResult_, requestId, i);
                using(requestUrl.GetResponse("GET", cookies, this._resultReferal_)){}
            }

            var resultUrl = string.Format(this._result_, requestId);
            using (var response = resultUrl.GetResponse("GET", cookies, this._resultReferal_))
            {
                using (var dataStream = response.GetResponseStream())
                {
                    if (dataStream != null)
                    {
                        var parser = this.GetParser(request);
                        var dataResult = parser.ParseWebArchive(dataStream, response.GetEncoding());
                        if (dataResult.ResultState == DataResult.NotReady)
                        {
                            return new DataRequestResult(DataRequestState.Pending, dataResult.ResultRoute);

                                // Data is not yet ready, return the current RequestState
                        }

                        result = dataResult.ResultRoute;
                        dataStream.Close();
                    }
                }
            }

            var newState = result == null
                                ? DataRequestState.Failed
                                : (result.Journeys.Count < 1 || result.Journeys[0].Data.Count < 1 ? DataRequestState.NoData : DataRequestState.Ok);
            return new DataRequestResult(newState, result);
        }
    }
}