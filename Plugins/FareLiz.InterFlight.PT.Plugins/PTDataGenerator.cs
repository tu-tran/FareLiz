namespace SkyDean.FareLiz.InterFlight
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web;

    using SkyDean.FareLiz.Core;

    /// <summary>
    /// The pt data generator.
    /// </summary>
    public class PTDataGenerator
    {
        /// <summary>
        /// The f i_ culture.
        /// </summary>
        public static readonly CultureInfo FI_CULTURE = new CultureInfo("fi-FI");

        /// <summary>
        /// The generate post data.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public byte[] GeneratePOSTData(FlightFareRequest request)
        {
            DateTime retDate = request.IsRoundTrip ? request.ReturnDate : request.DepartureDate.AddDays(7);
            string dataStr = "fly_radio_route_selection=" + (request.IsRoundTrip ? "both" : "oneway") + "&text_fly_from="
                             + this.Encode(request.Departure.IATA) + "&text_fly_to=" + this.Encode(request.Destination.IATA) + "&cal_pickdate1="
                             + request.DepartureDate.ToString("ddd+d.M.yyyy", FI_CULTURE) + "&cal_pickdate2="
                             + retDate.ToString("ddd+d.M.yyyy", FI_CULTURE) + "&fly_select_passengers_adults=1" + "&fly_select_passengers_children=0"
                             + "&fly_age_select_1=&fly_age_select_2=&fly_age_select_3=&fly_age_select_4=&fly_age_select_5=" + "&from_IATA="
                             + "&to_IATA=" + "&search_type=01_flights&submit=Hae+lennot";

            return Encoding.UTF8.GetBytes(dataStr);
        }

        /// <summary>
        /// The encode.
        /// </summary>
        /// <param name="postData">
        /// The post data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string Encode(string postData)
        {
            string result = HttpUtility.UrlEncode(postData);
            return result;
        }
    }
}