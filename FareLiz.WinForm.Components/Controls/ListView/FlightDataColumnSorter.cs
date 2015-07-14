namespace SkyDean.FareLiz.WinForm.Components.Controls.ListView
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Forms;

    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    /// <summary>This class is an implementation of the 'IComparer' interface.</summary>
    public class FlightDataColumnSorter : ListViewColumnSorter, IComparer<Flight>
    {
        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int Compare(Flight x, Flight y)
        {
            var compareResult = 0;
            switch (this.SortInfo.SortColumn)
            {
                case 0: // Departure time
                    compareResult = x.OutboundLeg.Departure.TimeOfDay.CompareTo(y.OutboundLeg.Departure.TimeOfDay);
                    break;
                case 1: // 1st Leg duration + transit
                    compareResult = x.OutboundLeg.Duration.CompareTo(y.OutboundLeg.Duration);
                    if (compareResult == 0)
                    {
                        compareResult = x.OutboundLeg.Transit.CompareTo(y.OutboundLeg.Transit);
                    }

                    break;
                case 2: // Return time
                    compareResult = x.InboundLeg.Departure.TimeOfDay.CompareTo(y.InboundLeg.Departure.TimeOfDay);
                    break;
                case 3: // 2nd leg duration + transit
                    compareResult = x.InboundLeg.Duration.CompareTo(y.InboundLeg.Duration);
                    if (compareResult == 0)
                    {
                        compareResult = x.InboundLeg.Transit.CompareTo(y.InboundLeg.Transit);
                    }

                    break;
                case 4: // Operator
                    compareResult = StringLogicalComparer.Compare(x.Operator, y.Operator);
                    break;
                case 5: // Price
                    compareResult = x.Price.CompareTo(y.Price);
                    break;
                case 6: // Travel period
                    compareResult = x.OutboundLeg.Departure.CompareTo(y.OutboundLeg.Departure);
                    if (compareResult == 0)
                    {
                        compareResult = x.InboundLeg.Departure.CompareTo(y.InboundLeg.Departure);
                    }

                    break;
                case 7: // Duration
                    compareResult = (x.InboundLeg.Departure - x.OutboundLeg.Departure).CompareTo(y.InboundLeg.Departure - y.OutboundLeg.Departure);
                    break;
                case 8: // Data Date
                    compareResult = x.JourneyData.DataDate.CompareTo(y.JourneyData.DataDate);
                    break;
            }

            return this.SortInfo.SortAscending ? compareResult : -compareResult;
        }

        /// <summary>
        /// The compare.
        /// </summary>
        /// <param name="textX">
        /// The text x.
        /// </param>
        /// <param name="textY">
        /// The text y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int Compare(string textX, string textY)
        {
            int compareResult;

            // Compare the two items
            switch (this.SortInfo.SortColumn)
            {
                case 5: // Price
                    double priceX = double.Parse(textX.Substring(0, textX.IndexOf(' '))), 
                           priceY = double.Parse(textY.Substring(0, textY.IndexOf(' ')));
                    compareResult = priceX.CompareTo(priceY);
                    break;
                case 6: // Travel Period
                    DatePeriod travelDateX = DatePeriod.Parse(textX), travelDateY = DatePeriod.Parse(textY);
                    compareResult = travelDateX.CompareTo(travelDateY);
                    break;
                case 7: // Duration
                    compareResult = int.Parse(textX, CultureInfo.InvariantCulture).CompareTo(int.Parse(textY, CultureInfo.InvariantCulture));
                    break;
                case 8: // Data Date
                    var xD = DateTime.ParseExact(textX, "ddd, MMM dd, yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    var yD = DateTime.ParseExact(textY, "ddd, MMM dd, yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    compareResult = xD.CompareTo(yD);
                    break;
                default:
                    compareResult = StringLogicalComparer.Compare(textX, textY);
                    break;
            }

            return compareResult;
        }

        /// <summary>
        /// The compare item.
        /// </summary>
        /// <param name="x">
        /// The x.
        /// </param>
        /// <param name="y">
        /// The y.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        protected override int CompareItem(ListViewItem.ListViewSubItem x, ListViewItem.ListViewSubItem y)
        {
            return this.Compare(x.Text, y.Text);
        }
    }
}