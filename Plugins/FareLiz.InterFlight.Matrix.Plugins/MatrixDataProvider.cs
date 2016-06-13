namespace SkyDean.FareLiz.InterFlight
{
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Data.Web;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;

    /// <summary>Data handler for International Flights (PT)</summary>
    [DisplayName("International Flight Data Handler (ITA Matrix)")]
    [Description("Best covered for international flight routes")]
    public sealed partial class MatrixDataProvider : WebDataProviderBase
    {
        /// <summary>
        /// Gets the service name.
        /// </summary>
        public override string ServiceName { get { return "Matrix"; } }

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
        public override DataRequestResult QueryData(FlightFareRequest request, JourneyProgressChangedEventHandler progressChangedHandler)
        {
            byte[] postData = this._generator.GeneratePOSTData(request);

            var httpRequest = (this._request_ + "1").GetRequest("POST");
            httpRequest.CookieContainer = new CookieContainer();
            httpRequest.Referer = this._root_;
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
        /// The export data.
        /// </summary>
        /// <param name="targetStream">
        /// The target stream.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        public override void ExportData(Stream targetStream, TravelRoute route)
        {
            if (route == null || route.Journeys.Count < 1)
            {
                return;
            }

            var exporter = new MatrixDataExporter();
            exporter.ExportData(targetStream, route);
        }

        /// <summary>
        /// The read data.
        /// </summary>
        /// <param name="routeStringData">
        /// The route string data.
        /// </param>
        /// <returns>
        /// The <see cref="TravelRoute"/>.
        /// </returns>
        public override TravelRoute ReadData(string routeStringData)
        {
            var parser = this.GetParser(null);
            var result = parser.ParseWebArchive(routeStringData);
            return result == null ? null : result.ResultRoute;
        }

        /// <summary>
        /// The get parser.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The <see cref="MatrixDataParser" />.
        /// </returns>
        private MatrixDataParser GetParser(FlightFareRequest request)
        {
            var parser = new MatrixDataParser(this._root_, this.Config.MaxFlightsPerAirline, this.Config.MaxAirlineCount, 1)
            {
                Departure = request.Departure,
                Destination = request.Destination
            };
            return parser;
        }
    }
}