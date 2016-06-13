namespace SkyDean.FareLiz.InterFlight
{
    using SkyDean.FareLiz.Core.Data;
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// The Matrix data exporter.
    /// </summary>
    internal class MatrixDataExporter
    {
        /// <summary>
        /// The export data.
        /// </summary>
        /// <param name="targetStream">
        /// The target stream.
        /// </param>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void ExportData(Stream targetStream, TravelRoute route)
        {
            if (targetStream == null)
            {
                throw new ArgumentException("targetStream cannot be null");
            }

            if (route == null)
            {
                throw new ArgumentException("route cannot be null");
            }

            var setting = new XmlWriterSettings();
            setting.OmitXmlDeclaration = true;
            setting.Indent = true;
            setting.Encoding = Encoding.Default;

            /* TODO :FIX XML stream writer
            using (XmlWriter writer = XmlWriter.Create(targetStream, setting))
            {
                writer.WriteStartElement("body");

                writer.WriteStartElement("input");
                writer.WriteAttributeString("id", "text_fly_from");
                writer.WriteAttributeString("value", route.Departure.IATA);
                writer.WriteEndElement();
                writer.WriteStartElement("input");
                writer.WriteAttributeString("id", "text_fly_to");
                writer.WriteAttributeString("value", route.Destination.IATA);
                writer.WriteEndElement();

                foreach (var journey in route.Journeys)
                {
                    foreach (var data in journey.Data)
                    {
                        writer.WriteStartElement("div");
                        writer.WriteAttributeString("id", "results_list");
                        writer.WriteAttributeString("dataDate", XmlConvert.ToString(data.DataDate, XmlDateTimeSerializationMode.Utc));

                        foreach (Flight f in data.Flights)
                        {
                            var outboundLeg = f.OutboundLeg;
                            if (outboundLeg == null)
                            {
                                continue;
                            }

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "flights_b");
                            writer.WriteAttributeString("id", "flight_result_0");
                            if (f.TravelAgency != null && !string.IsNullOrEmpty(f.TravelAgency.Url))
                            {
                                writer.WriteAttributeString("onclick", "click('" + f.TravelAgency.Url + "')");
                            }

                            // Outbound
                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_outbound");

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_dep_date");
                            writer.WriteString(outboundLeg.Departure.ToString("dd.MM.yy"));
                            writer.WriteEndElement();

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_departure");
                            writer.WriteString(outboundLeg.Departure.ToString("HH:mm"));
                            writer.WriteEndElement();

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_arr_date");
                            writer.WriteString(outboundLeg.Arrival.ToString("dd.MM.yy"));
                            writer.WriteEndElement();

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_arrival");
                            writer.WriteString(outboundLeg.Arrival.ToString("HH:mm"));
                            writer.WriteEndElement();

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_stops");
                            writer.WriteString(outboundLeg.Transit.ToString(CultureInfo.InvariantCulture));
                            writer.WriteEndElement();

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_duration");
                            writer.WriteString(outboundLeg.Duration + "h");
                            writer.WriteEndElement();
                            writer.WriteEndElement();

                            // Return trip
                            var inboundLeg = f.InboundLeg;
                            if (inboundLeg != null)
                            {
                                writer.WriteStartElement("div");
                                writer.WriteAttributeString("class", "f_return");

                                writer.WriteStartElement("div");
                                writer.WriteAttributeString("class", "f_dep_date");
                                writer.WriteString(inboundLeg.Departure.ToString("dd.MM.yy"));
                                writer.WriteEndElement();

                                writer.WriteStartElement("div");
                                writer.WriteAttributeString("class", "f_departure");
                                writer.WriteString(inboundLeg.Departure.ToString("HH:mm"));
                                writer.WriteEndElement();

                                writer.WriteStartElement("div");
                                writer.WriteAttributeString("class", "f_arr_date");
                                writer.WriteString(inboundLeg.Arrival.ToString("dd.MM.yy"));
                                writer.WriteEndElement();

                                writer.WriteStartElement("div");
                                writer.WriteAttributeString("class", "f_arrival");
                                writer.WriteString(inboundLeg.Arrival.ToString("HH:mm"));
                                writer.WriteEndElement();

                                writer.WriteStartElement("div");
                                writer.WriteAttributeString("class", "f_stops");
                                writer.WriteString(inboundLeg.Transit.ToString(CultureInfo.InvariantCulture));
                                writer.WriteEndElement();

                                writer.WriteStartElement("div");
                                writer.WriteAttributeString("class", "f_duration");
                                writer.WriteString(inboundLeg.Duration + "h");
                                writer.WriteEndElement();

                                writer.WriteEndElement();
                            }

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_company");
                            writer.WriteString(f.Operator);
                            writer.WriteEndElement();

                            writer.WriteStartElement("div");
                            writer.WriteAttributeString("class", "f_price");
                            writer.WriteString(f.Price.ToString(CultureInfo.InvariantCulture) + " €");
                            writer.WriteEndElement();

                            writer.WriteEndElement();
                        }

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
             */
        }
    }
}