using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Data;
using SkyDean.FareLiz.Core.Properties;
using SkyDean.FareLiz.Data.Csv;

namespace SkyDean.FareLiz.Data
{
    public static class AirportDataProvider
    {
        public static ReadOnlyCollection<Airport> Airports { get; private set; }

        static AirportDataProvider()
        {
            var data = new List<Airport>();
            using (var txtReader = new StringReader(Resources.Airports))
            using (var reader = new CsvReader(txtReader, false) { DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine, SkipEmptyLines = true })
            {
                while (reader.ReadNextRecord())
                {
                    string iata = reader[4];
                    if (String.IsNullOrEmpty(iata))
                        continue;

                    string name = reader[1],
                           city = reader[2],
                           country = reader[3];
                    float latitude = float.Parse(reader[6], NamingRule.NumberCulture),
                          longitude = float.Parse(reader[7], NamingRule.NumberCulture);
                    var airport = new Airport(name, city, country, iata, latitude, longitude);
                    data.Add(airport);
                }
            }

            data.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
            Airports = data.AsReadOnly();
        }

        /// <summary>
        /// Create a new Airport object from the IATA code
        /// </summary>
        /// <param name="iata"></param>
        /// <returns></returns>
        public static Airport FromIATA(string iata)
        {
            if (!String.IsNullOrEmpty(iata))
            {
                foreach (var airport in Airports)
                {
                    if (String.Equals(iata, airport.IATA, StringComparison.OrdinalIgnoreCase))
                        return airport;
                }
            }

            return null;
        }
    }
}
