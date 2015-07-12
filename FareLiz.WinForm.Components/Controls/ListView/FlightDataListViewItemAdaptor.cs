namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    internal class FlightDataListViewItemAdaptor
    {
        internal string GetFlightPrice(Flight flight, string targetCurrency)
        {
            double price = flight.Price;
            string flightCurrency = flight.JourneyData.Currency;
            string currencyCode = flightCurrency;

            var currencyProvider = AppContext.MonitorEnvironment.CurrencyProvider;
            if (currencyProvider != null)
            {
                if (String.IsNullOrEmpty(targetCurrency) || String.Equals(flightCurrency, targetCurrency, StringComparison.OrdinalIgnoreCase))
                    price = flight.Price;
                else
                {
                    if (currencyProvider.Convert(flight.Price, flightCurrency, targetCurrency, out price))
                        flightCurrency = targetCurrency;
                }

                currencyCode = currencyProvider.GetCurrencyInfo(flightCurrency).Symbol;
            }

            string result = price.ToString("#,0.0 ") + currencyCode;
            return result;
        }

        internal List<string> GetPresentationStrings(Flight flight, string targetCurrency)
        {
            string deptTime = flight.OutboundLeg.Departure.ToShortTimeString(),
                   outboundInfo = String.Format("({0}) - {1}", flight.OutboundLeg.Transit, flight.OutboundLeg.Duration.ToHourMinuteString()),
                   returnTime = flight.InboundLeg == null ? null : flight.InboundLeg.Departure.ToShortTimeString(),
                   inboundInfo = flight.InboundLeg == null ? null : String.Format("({0}) - {1}", flight.InboundLeg.Transit, flight.InboundLeg.Duration.ToHourMinuteString()),
                   flightCompany = flight.Operator,
                   price = this.GetFlightPrice(flight, targetCurrency),
                   travelPeriod = flight.OutboundLeg.Departure.ToString("ddd dd/MM/yyyy") + " - " + (flight.InboundLeg == null ? "" : flight.InboundLeg.Departure.ToString("ddd dd/MM/yyyy")),
                   stayDuration = ((int)(flight.InboundLeg == null ? 0 : (flight.InboundLeg.Departure.Date - flight.OutboundLeg.Departure.Date).TotalDays)).ToString(CultureInfo.InvariantCulture),
                   dataDate = flight.JourneyData.DataDate.ToString("ddd, MMM dd, yyyy h:mm:ss tt"),
                   agency = flight.TravelAgency == null ? null : flight.TravelAgency.Name;

            var result = new List<string>()
                {
                    deptTime, outboundInfo, returnTime, inboundInfo, flightCompany, price, travelPeriod, stayDuration, dataDate, agency
                };

            return result;
        }

        internal ListViewItem GetListViewItem(Flight flight, string targetCurrency)
        {
            List<string> data = this.GetPresentationStrings(flight, targetCurrency);

            var item = new ListViewItem()
            {
                UseItemStyleForSubItems = false,
                Tag = flight,
                Text = data[0]
            };

            var subItems = new ListViewItem.ListViewSubItem[data.Count - 1];
            for (int i = 1; i < data.Count; i++)
            {
                var sub = new ListViewItem.ListViewSubItem(item, data[i]);
                subItems[i - 1] = sub;
                if (i == 5)
                    sub.Font = new Font(sub.Font, FontStyle.Bold);
            }

            item.SubItems.AddRange(subItems);
            return item;
        }
    }
}
