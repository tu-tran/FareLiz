namespace SkyDean.FareLiz.InterFlight
{
    using HtmlAgilityPack;
    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data.Web;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    /// <summary>
    /// The Matrix data parser.
    /// </summary>
    internal class MatrixDataParser
    {
        /// <summary>
        /// The _root domain.
        /// </summary>
        private readonly string _rootDomain;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixDataParser"/> class.
        /// </summary>
        /// <param name="rootDomain">
        /// The root domain.
        /// </param>
        /// <param name="maxFlightsPerAirline">
        /// The max flights per airline.
        /// </param>
        /// <param name="maxAirlines">
        /// The max airlines.
        /// </param>
        /// <param name="minPriceMargin">
        /// The min price margin.
        /// </param>
        internal MatrixDataParser(string rootDomain, int maxFlightsPerAirline, int maxAirlines, int minPriceMargin)
        {
            this._rootDomain = rootDomain;
            this.MaxFlightsPerAirline = maxFlightsPerAirline;
            this.MaxAirlines = maxAirlines;
            this.MinPriceMargin = minPriceMargin;
        }

        /// <summary>
        /// Gets or sets the max flights per airline.
        /// </summary>
        internal int MaxFlightsPerAirline { get; set; }

        /// <summary>
        /// Gets or sets the max airlines.
        /// </summary>
        internal int MaxAirlines { get; set; }

        /// <summary>
        /// Gets or sets the min price margin.
        /// </summary>
        internal int MinPriceMargin { get; set; }

        /// <summary>
        /// Gets or sets the departure.
        /// </summary>
        public Airport Departure { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public Airport Destination { get; set; }

        /// <summary>
        /// Parses the request identifier.
        /// </summary>
        /// <param name="htmlStream">The HTML stream.</param>
        /// <returns></returns>
        internal static string ParseRequestId(Stream htmlStream, Encoding encoding)
        {
            using (var reader = new StreamReader(htmlStream, encoding))
            {
                var regex = new Regex("createFlightQueriesParseResponses\\((?<Id>.+?),");
                var html = reader.ReadToEnd();
                var match = regex.Match(html);
                if (match.Success)
                {
                    return match.Groups["Id"].ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// The parse web archive.
        /// </summary>
        /// <param name="htmlStream">
        /// The html stream.
        /// </param>
        /// <returns>
        /// The <see cref="RouteDataResult"/>.
        /// </returns>
        internal RouteDataResult ParseWebArchive(Stream htmlStream, Encoding encoding)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(htmlStream, encoding);
            return this.ParseWebArchive(htmlDoc);
        }

        /// <summary>
        /// The parse web archive.
        /// </summary>
        /// <param name="webDocument">
        /// The web document.
        /// </param>
        /// <returns>
        /// The <see cref="RouteDataResult"/>.
        /// </returns>
        internal RouteDataResult ParseWebArchive(HtmlDocument webDocument)
        {
            HtmlNode originInput = webDocument.GetElementbyId("flight_places");
            if (originInput == null)
            {
                // Data is not ready yet
                return new RouteDataResult(DataResult.NotReady, null);
            }

            var resultDivs = webDocument.DocumentNode.SelectNodes("//div[@id='results_list']//div[@class='flights_a']");
            if (resultDivs == null || resultDivs.Count < 1)
            {
                // Data is not ready yet
                return new RouteDataResult(DataResult.NotReady, null);
            }

            var route = new TravelRoute(0, this.Departure, this.Destination);

            foreach (var resultSection in resultDivs)
            {
                // Each result set
                DateTime dataDate = DateTime.Now;
                var operatorData = new Dictionary<string, List<Flight>>();
                var newData = new JourneyData(0, "EUR", dataDate);

                FlightLeg outboundLeg = null, inboundLeg = null;
                string flightOperator = null;
                TravelAgency travelAgency = null;
                double price = 0.0;
                List<Fare> fares = new List<Fare>();

                var priceContainers = resultSection.SelectNodes(".//div[@class='f_price_container']//div[@class='row']");
                if (priceContainers == null || priceContainers.Count < 1)
                {
                    continue;
                }

                foreach (var priceContainer in priceContainers)
                {
                    string onClickStr = priceContainer.GetAttributeValue("onclick", string.Empty);
                    travelAgency = this.TryGetTravelAgency(onClickStr, false);

                    var priceNode = priceContainer.SelectSingleNode(".//div[@class='f_price']");
                    if (priceNode == null)
                    {
                        continue;
                    }

                    string priceStr = Regex.Match(priceNode.InnerText.Replace(",", "."), @"\d+(.\d+)?").Value;
                    double.TryParse(priceStr, NumberStyles.Any, NamingRule.NumberCulture, out price);
                    fares.Add(new Fare { TravelAgency = travelAgency, Price = price });
                }

                // Loop through each column in table
                var flightNode = resultSection.SelectSingleNode(".//div[@class='f_normal_info']");
                if (flightNode == null)
                {
                    continue;
                }

                foreach (HtmlNode flightDetailNode in flightNode.ChildNodes)
                {
                    // Fetch the flight detail from the row
                    if (flightDetailNode.Name != "div")
                    {
                        continue;
                    }

                    string className = flightDetailNode.GetAttributeValue("class", string.Empty);
                    switch (className)
                    {
                        case "f_outbound":
                        case "f_outbound_only":
                        case "f_return":
                            var divNodes = flightDetailNode.Descendants("div");
                            if (divNodes != null)
                            {
                                string depDatePartStr = null, depTimePartStr = null, stopStr = null, arrDatePartStr = null, arrTimePartStr = null;
                                TimeSpan duration = TimeSpan.Zero;
                                foreach (var dataNode in divNodes)
                                {
                                    // Each flight
                                    string dataClass = dataNode.GetAttributeValue("class", string.Empty);
                                    switch (dataClass)
                                    {
                                        case "f_dep_date":
                                            depDatePartStr = dataNode.InnerText;
                                            break;
                                        case "f_departure":
                                            depTimePartStr = dataNode.InnerText;
                                            break;
                                        case "f_arr_date":
                                            arrDatePartStr = dataNode.InnerText;
                                            break;
                                        case "f_arrival":
                                            arrTimePartStr = dataNode.InnerText;
                                            break;
                                        case "f_duration":
                                            duration = this.TryGetDuration(dataNode.InnerText);
                                            break;
                                        case "f_stops":
                                            stopStr = TryGetNumberString(dataNode.InnerText);
                                            break;
                                    }
                                }

                                // Validate that we got all required data
                                string depDateStr = string.Format(CultureInfo.InvariantCulture, "{0} {1}", depDatePartStr, depTimePartStr),
                                       arrDateStr = string.Format(CultureInfo.InvariantCulture, "{0} {1}", arrDatePartStr, arrTimePartStr);
                                DateTime deptDate, arrDate;
                                if (DateTime.TryParseExact(
                                    depDateStr,
                                    "dd.MM.yy HH:mm",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out deptDate) && deptDate.IsDefined()
                                    && DateTime.TryParseExact(
                                        arrDateStr,
                                        "dd.MM.yy HH:mm",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None,
                                        out arrDate) && arrDate.IsDefined())
                                {
                                    if (duration > TimeSpan.Zero)
                                    {
                                        int transit;
                                        int.TryParse(stopStr, out transit); // This might fail for straight flight: Just ignore it
                                        var flightLeg = new FlightLeg(deptDate, arrDate, duration, transit);
                                        if (className == "f_return")
                                        {
                                            inboundLeg = flightLeg;
                                        }
                                        else
                                        {
                                            outboundLeg = flightLeg;
                                        }
                                    }
                                }
                            }

                            break;

                        case "f_company":
                            flightOperator = flightDetailNode.InnerText.Replace("mm. ", string.Empty);
                            break;
                    }
                }

                if (outboundLeg != null)
                {
                    bool shouldAdd;
                    if (shouldAdd = !operatorData.ContainsKey(flightOperator))
                    {
                        // Flight will always be added if it is the first from this operator
                        if (operatorData.Keys.Count == this.MaxAirlines)
                        {
                            // If we reached the limit for number of flight operators
                            continue;
                        }

                        operatorData.Add(flightOperator, new List<Flight>(this.MaxFlightsPerAirline));
                    }

                    var opearatorFlights = operatorData[flightOperator];
                    if (!shouldAdd)
                    {
                        // This is not the first fly from this operator
                        TimeSpan totalDuration = (outboundLeg == null ? TimeSpan.Zero : outboundLeg.Duration)
                                                 + (inboundLeg == null ? TimeSpan.Zero : inboundLeg.Duration);
                        var lastFlight = opearatorFlights[opearatorFlights.Count - 1];

                        if ((price - lastFlight.Price) > this.MinPriceMargin)
                        {
                            // If the price differs enough, add new flight if we still have space
                            if (opearatorFlights.Count < this.MaxFlightsPerAirline)
                            {
                                shouldAdd = true;
                            }
                        }
                        else
                        {
                            // The new price does not differ enough from last flight: Add or replace existing flight if the duration is shorter
                            for (int i = opearatorFlights.Count - 1; i >= 0; i--)
                            {
                                var f = opearatorFlights[i];
                                if ((price - f.Price) <= this.MinPriceMargin && totalDuration < f.Duration)
                                {
                                    opearatorFlights.RemoveAt(i);
                                    shouldAdd = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (shouldAdd && fares.Count > 0)
                    {
                        var newFlight = new Flight(newData, flightOperator, outboundLeg, inboundLeg) { Fares = fares };
                        newData.AddFlight(newFlight);
                        opearatorFlights.Add(newFlight);
                    }
                }

                if (newData.Flights.Count > 0)
                {
                    Journey existJourney = null;
                    DateTime deptDate = newData.Flights[0].OutboundLeg.Departure.Date;
                    DateTime retDate = DateTime.MinValue;
                    if (newData.Flights[0].InboundLeg != null)
                    {
                        retDate = newData.Flights[0].InboundLeg.Departure.Date;
                    }

                    foreach (var j in route.Journeys)
                    {
                        if (j.DepartureDate == deptDate && j.ReturnDate == retDate)
                        {
                            existJourney = j;
                            break;
                        }
                    }

                    if (existJourney == null)
                    {
                        existJourney = new Journey();
                        route.AddJourney(existJourney);
                    }

                    existJourney.AddData(newData);
                    existJourney.DepartureDate = deptDate;
                    existJourney.ReturnDate = retDate;
                }
            }

            return new RouteDataResult(DataResult.Ready, route);
        }

        /// <summary>
        /// The parse web archive.
        /// </summary>
        /// <param name="htmlData">
        /// The html data.
        /// </param>
        /// <returns>
        /// The <see cref="RouteDataResult"/>.
        /// </returns>
        internal RouteDataResult ParseWebArchive(string htmlData)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlData);
            return this.ParseWebArchive(htmlDoc);
        }

        /// <summary>
        /// The try get number string.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string TryGetNumberString(string input)
        {
            var match = Regex.Match(input, @"\d+");
            return match.Success ? match.Value : null;
        }

        /// <summary>
        /// The try get duration.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="TimeSpan"/>.
        /// </returns>
        private TimeSpan TryGetDuration(string input)
        {
            var match = Regex.Match(input, @"((?<Hour>\d*)[h|t])? ?((?<Minute>\d*)m)?");
            if (match.Success)
            {
                var hourStr = match.Groups["Hour"].Value;
                var minuteStr = match.Groups["Minute"].Value;
                int totalMinutes = 0;
                int hours = 0;
                if (int.TryParse(hourStr, out hours))
                {
                    totalMinutes += hours * 60;
                }

                int minutes;
                if (int.TryParse(minuteStr, out minutes))
                {
                    totalMinutes += minutes;
                }

                var result = TimeSpan.FromMinutes(totalMinutes);
                return result;
            }

            return TimeSpan.Zero;
        }

        /// <summary>
        /// The try get travel agency.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <param name="forceRetrieveActualUrl">
        /// The force retrieve actual url.
        /// </param>
        /// <returns>
        /// The <see cref="TravelAgency"/>.
        /// </returns>
        private TravelAgency TryGetTravelAgency(string input, bool forceRetrieveActualUrl)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            var decoded = HttpUtility.HtmlDecode(input);
            var match = Regex.Match(decoded, @"click\('(?<OriginUrl>.+?\?snad=(?<TravelAgency>.+?)_flight.+?&url=(?<Url>.+?)?)'\)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var agency = match.Groups["TravelAgency"].Value;
                if (!string.IsNullOrEmpty(agency))
                {
                    var url = HttpUtility.UrlDecode(match.Groups["Url"].Value);
                    TravelAgency result = new TravelAgency(agency);
                    if (url.StartsWith("http"))
                    {
                        result.Url = url;
                    }
                    else
                    {
                        var originUrl = match.Groups["OriginUrl"].Value;
                        var fullUrl = this._rootDomain + "/" + originUrl;
                        if (forceRetrieveActualUrl)
                        {
                            try
                            {
                                var httpRequest = fullUrl.GetRequest("GET");
                                httpRequest.Referer = this._rootDomain;
                                httpRequest.Timeout = 2000;
                                using (var response = (HttpWebResponse)httpRequest.GetResponse())
                                {
                                    var header = response.GetResponseHeader("Refresh");
                                    if (!string.IsNullOrEmpty(header))
                                    {
                                        var urlMatch = Regex.Match(header, @"URL=\""(?<Url>.+?)?\""", RegexOptions.IgnoreCase);
                                        if (urlMatch.Success)
                                        {
                                            url = urlMatch.Groups["Url"].Value;
                                            result.Url = url;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Trace.TraceError("Failed to extract travel agency data: {0}", ex);
                            }
                        }

                        result.Url = fullUrl;
                    }

                    return result;
                }
            }

            return null;
        }
    }
}