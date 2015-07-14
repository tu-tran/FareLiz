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

    /// <summary>The flight data list view item adaptor.</summary>
    internal class FlightDataListViewItemAdaptor
    {
        /// <summary>
        /// The get flight price.
        /// </summary>
        /// <param name="flight">
        /// The flight.
        /// </param>
        /// <param name="targetCurrency">
        /// The target currency.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GetFlightPrice(Flight flight, string targetCurrency)
        {
            double price = flight.Price;
            var flightCurrency = flight.JourneyData.Currency;
            var currencyCode = flightCurrency;

            var currencyProvider = AppContext.MonitorEnvironment.CurrencyProvider;
            if (currencyProvider != null)
            {
                if (string.IsNullOrEmpty(targetCurrency) || string.Equals(flightCurrency, targetCurrency, StringComparison.OrdinalIgnoreCase))
                {
                    price = flight.Price;
                }
                else
                {
                    if (currencyProvider.Convert(flight.Price, flightCurrency, targetCurrency, out price))
                    {
                        flightCurrency = targetCurrency;
                    }
                }

                currencyCode = currencyProvider.GetCurrencyInfo(flightCurrency).Symbol;
            }

            var result = price.ToString("#,0.0 ") + currencyCode;
            return result;
        }

        /// <summary>
        /// The get presentation strings.
        /// </summary>
        /// <param name="flight">
        /// The flight.
        /// </param>
        /// <param name="targetCurrency">
        /// The target currency.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal List<string> GetPresentationStrings(Flight flight, string targetCurrency)
        {
            string deptTime = flight.OutboundLeg.Departure.ToShortTimeString(), 
                   outboundInfo = string.Format("({0}) - {1}", flight.OutboundLeg.Transit, flight.OutboundLeg.Duration.ToHourMinuteString()), 
                   returnTime = flight.InboundLeg == null ? null : flight.InboundLeg.Departure.ToShortTimeString(), 
                   inboundInfo = flight.InboundLeg == null
                                     ? null
                                     : string.Format("({0}) - {1}", flight.InboundLeg.Transit, flight.InboundLeg.Duration.ToHourMinuteString()), 
                   flightCompany = flight.Operator, 
                   price = this.GetFlightPrice(flight, targetCurrency), 
                   travelPeriod = flight.OutboundLeg.Departure.ToString("ddd dd/MM/yyyy") + " - "
                                  + (flight.InboundLeg == null ? string.Empty : flight.InboundLeg.Departure.ToString("ddd dd/MM/yyyy")), 
                   stayDuration =
                       ((int)(flight.InboundLeg == null ? 0 : (flight.InboundLeg.Departure.Date - flight.OutboundLeg.Departure.Date).TotalDays))
                           .ToString(CultureInfo.InvariantCulture), 
                   dataDate = flight.JourneyData.DataDate.ToString("ddd, MMM dd, yyyy h:mm:ss tt"), 
                   agency = flight.TravelAgency == null ? null : flight.TravelAgency.Name;

            var result = new List<string>
                             {
                                 deptTime, 
                                 outboundInfo, 
                                 returnTime, 
                                 inboundInfo, 
                                 flightCompany, 
                                 price, 
                                 travelPeriod, 
                                 stayDuration, 
                                 dataDate, 
                                 agency
                             };

            return result;
        }

        /// <summary>
        /// The get list view item.
        /// </summary>
        /// <param name="flight">
        /// The flight.
        /// </param>
        /// <param name="targetCurrency">
        /// The target currency.
        /// </param>
        /// <returns>
        /// The <see cref="ListViewItem"/>.
        /// </returns>
        internal ListViewItem GetListViewItem(Flight flight, string targetCurrency)
        {
            var data = this.GetPresentationStrings(flight, targetCurrency);

            var item = new ListViewItem { UseItemStyleForSubItems = false, Tag = flight, Text = data[0] };

            var subItems = new ListViewItem.ListViewSubItem[data.Count - 1];
            for (var i = 1; i < data.Count; i++)
            {
                var sub = new ListViewItem.ListViewSubItem(item, data[i]);
                subItems[i - 1] = sub;
                if (i == 5)
                {
                    sub.Font = new Font(sub.Font, FontStyle.Bold);
                }
            }

            item.SubItems.AddRange(subItems);
            return item;
        }
    }
}