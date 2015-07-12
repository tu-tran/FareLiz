using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using SkyDean.FareLiz.Core.Utils;

namespace SkyDean.FareLiz.Core.Data
{
    public enum FlightStatus { New, PriceIncreased, PriceDecreased }

    public class FlightMonitorItem
    {
        public Flight FlightData { get; private set; }
        public FlightStatus FlightStatus { get; private set; }
        public double OldPrice { get; private set; }

        public FlightMonitorItem(Flight data, FlightStatus status, double oldPrice)
        {
            this.FlightData = data;
            this.FlightStatus = status;
            this.OldPrice = oldPrice;
        }
    }

    public class FlightDisplayItemsCollection : List<FlightMonitorItem>
    {
        public void Add(Flight flight, FlightStatus status, double previousPrice)
        {
            this.Add(new FlightMonitorItem(flight, status, previousPrice));
        }

        public void AddRange(IEnumerable<Flight> data, FlightStatus status)
        {
            foreach (var f in data)
                this.Add(new FlightMonitorItem(f, status, 0));
        }

        public string GenerateRtfReport(string fontName, int fontSize)
        {
            if (this.Count < 1)
                return null;

            int maxOperatorLength = 0, maxNewPriceLength = 0, maxOpLength = 0, maxOldPriceLength = 0;
            var dataLines = new List<string[]>();
            var sb = new StringBuilder();

            for (int i = 0; i < this.Count; i++)
            {
                var item = this[i];

                string flightOperator = item.FlightData.Operator;
                if (flightOperator.Length > maxOperatorLength)
                    maxOperatorLength = flightOperator.Length;

                string operatorUrl = (item.FlightData.TravelAgency == null ? null : item.FlightData.TravelAgency.Url);

                string newPrice = item.FlightData.Price.ToString(CultureInfo.InvariantCulture);
                if (newPrice.Length > maxNewPriceLength)
                    maxNewPriceLength = newPrice.Length;

                if (item.FlightStatus == FlightStatus.New)
                    dataLines.Add(new string[] { "\\cf0 ", flightOperator, operatorUrl, newPrice });
                else
                {
                    string oldPrice = item.OldPrice.ToString(CultureInfo.InvariantCulture);
                    if (oldPrice.Length > maxOldPriceLength)
                        maxOldPriceLength = oldPrice.Length;

                    string colorCode = (item.FlightStatus == FlightStatus.PriceDecreased ? "\\cf2 " : "\\cf1 ");
                    string status = (item.FlightStatus == FlightStatus.PriceDecreased ? "▼" : "▲");
                    if (maxOpLength == 0)
                        maxOpLength = status.Length + 2;

                    dataLines.Add(new string[] { colorCode, flightOperator, operatorUrl, oldPrice, status, newPrice });
                }
            }

            int maxLineLength = maxOperatorLength + 2 + maxNewPriceLength + maxOpLength + maxOldPriceLength;
            for (int i = 0; i < dataLines.Count; i++)
            {
                var strArray = dataLines[i];
                string newLine = "";

                string operatorLink = String.IsNullOrEmpty(strArray[2])
                    ? strArray[1]
                    : String.Format(CultureInfo.InvariantCulture, @"{{\field{{\*\fldinst HYPERLINK ""{0}""}}{{\fldrslt {1}}}}}",
                    strArray[2], strArray[1]);

                if (strArray[1].Length < maxOperatorLength)
                    operatorLink = new string(' ', maxOperatorLength - strArray[1].Length) + operatorLink;

                if (strArray.Length == 4)
                    newLine = String.Format("{0}: {1}", operatorLink, strArray[3]);
                else if (strArray.Length == 6)
                    newLine = String.Format("{0}: {1} {2} {3}", operatorLink, strArray[3].PadRight(maxOldPriceLength), strArray[4], strArray[5]);

                newLine = strArray[0] + newLine.Replace("▼", "\\u9660?").Replace("▲", "\\u9650?");    //  RTF for "▼" : "▲"
                sb.AppendLine(newLine + (i == dataLines.Count - 1 ? String.Empty : "\\line\r\n"));
            }

            var journeyData = this[0].FlightData.JourneyData;
            var journeyInfo = journeyData.JourneyInfo;
            string header = String.Format("{0} (Currency: {1})", StringUtil.GetPeriodString(journeyInfo.DepartureDate, journeyInfo.ReturnDate), journeyData.Currency);
            if (header.Length > maxLineLength)
                maxLineLength = header.Length;

            string report = sb.ToString();
            string content = String.Format(
@"{{\rtf1\ansi\ansicpg1252\deff0\deflang1033{{\fonttbl{{\f0\fnil\fcharset0 {0};}}}}{{\colortbl ;\red181\green20\blue20;\red55\green115\blue0;}}\pard\f0\fs{1}
{{\b {2}}}\b0\line
{3}\line
{4}\par}}", fontName, fontSize * 2, header.PadLeft(maxLineLength), new string('-', maxLineLength), report);

            return content;
        }
    }


}
