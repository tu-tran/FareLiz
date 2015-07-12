using System;
using System.Globalization;
using System.Text;
using System.Web;
using SkyDean.FareLiz.Core;

namespace SkyDean.FareLiz.InterFlight
{
    public class PTDataGenerator
    {
        public static readonly CultureInfo FI_CULTURE = new CultureInfo("fi-FI");
        public static readonly string ACCEPT = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8",
                                      USER_AGENT = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.64 Safari/537.31";

        public byte[] GeneratePOSTData(FlightFareRequest request)
        {
            DateTime retDate = request.IsRoundTrip ? request.ReturnDate : request.DepartureDate.AddDays(7);
            string dataStr = "fly_radio_route_selection=" + (request.IsRoundTrip ? "both" : "oneway") +
                "&text_fly_from=" + Encode(request.Departure.IATA) +
                "&text_fly_to=" + Encode(request.Destination.IATA) +
                "&cal_pickdate1=" + request.DepartureDate.ToString("ddd+d.M.yyyy", FI_CULTURE) +
                "&cal_pickdate2=" + retDate.ToString("ddd+d.M.yyyy", FI_CULTURE) +
                "&fly_select_passengers_adults=1" +
                "&fly_select_passengers_children=0" +
                "&fly_age_select_1=&fly_age_select_2=&fly_age_select_3=&fly_age_select_4=&fly_age_select_5=" +
                "&from_IATA=" +
                "&to_IATA=" +
                "&search_type=01_flights&submit=Hae+lennot";

            return Encoding.UTF8.GetBytes(dataStr);
        }

        public string Encode(string postData)
        {
            string result = HttpUtility.UrlEncode(postData);
            return result;
        }
    }
}
