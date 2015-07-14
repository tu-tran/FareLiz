namespace SkyDean.FareLiz.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Properties;
    using SkyDean.FareLiz.Data.Csv;

    /// <summary>The airport data provider.</summary>
    public static class AirportDataProvider
    {
        /// <summary>Initializes static members of the <see cref="AirportDataProvider" /> class.</summary>
        static AirportDataProvider()
        {
            var data = new List<Airport>();
            using (var txtReader = new StringReader(Resources.Airports))
            using (
                var reader = new CsvReader(txtReader, false) { DefaultParseErrorAction = ParseErrorAction.AdvanceToNextLine, SkipEmptyLines = true })
            {
                while (reader.ReadNextRecord())
                {
                    var iata = reader[4];
                    if (string.IsNullOrEmpty(iata))
                    {
                        continue;
                    }

                    string name = reader[1], city = reader[2], country = reader[3];
                    float latitude = float.Parse(reader[6], NamingRule.NumberCulture), longitude = float.Parse(reader[7], NamingRule.NumberCulture);
                    var airport = new Airport(name, city, country, iata, latitude, longitude);
                    data.Add(airport);
                }
            }

            data.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
            Airports = data.AsReadOnly();
        }

        /// <summary>Gets the airports.</summary>
        public static ReadOnlyCollection<Airport> Airports { get; private set; }

        /// <summary>
        /// Create a new Airport object from the IATA code
        /// </summary>
        /// <param name="iata">
        /// </param>
        /// <returns>
        /// The <see cref="Airport"/>.
        /// </returns>
        public static Airport FromIATA(string iata)
        {
            if (!string.IsNullOrEmpty(iata))
            {
                foreach (var airport in Airports)
                {
                    if (string.Equals(iata, airport.IATA, StringComparison.OrdinalIgnoreCase))
                    {
                        return airport;
                    }
                }
            }

            return null;
        }
    }
}