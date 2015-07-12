namespace SkyDean.FareLiz.Data.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Utils;

    public class LiveFareFileStorage : IFareStorage
    {
        const string DATE_FORMAT = "yyyy.MM.dd";
        public string BaseDataPath { get; private set; }
        public int DaysPerFile { get; set; }

        public LiveFareFileStorage(string baseDataPath)
        {
            this.BaseDataPath = baseDataPath;
            Directory.CreateDirectory(baseDataPath);
            this.DaysPerFile = 6;
        }

        public string CatalogFile { get { return Path.Combine(this.BaseDataPath, "Catalog.bin"); } }

        public void SaveLiveFare(IEnumerable<Flight> flights)
        {
            if (flights == null)
                return;

            var routeGroups = flights.GroupBy(f => new { f.JourneyData.JourneyInfo.DepartureCode, f.JourneyData.JourneyInfo.DestinationCode });
            if (routeGroups == null)
                return;

            foreach (var routeFlights in routeGroups)
            {
                if (routeFlights == null)
                    continue;

                var routeInfo = this.SaveRoute(new StorageRoute(routeFlights.Key.DepartureCode, routeFlights.Key.DestinationCode));

                var dateGroups = routeFlights.GroupBy(f => new { f.JourneyData.JourneyInfo.DepartureDate, f.JourneyData.JourneyInfo.ReturnDate });
                if (dateGroups == null)
                    continue;

                foreach (var dateFlights in dateGroups)
                {
                    if (dateFlights != null)
                    {
                        var targetPath = this.GetPath(routeInfo, new DatePeriod(dateFlights.Key.DepartureDate, dateFlights.Key.ReturnDate));
                        Directory.CreateDirectory(targetPath);

                        foreach (var flight in dateFlights)
                        {
                            DateTime dataDate = flight.JourneyData.DataDate;
                            string fileName = String.Format("{0}-{1}.csv", dataDate.Date.StartOfWeek(DayOfWeek.Monday).ToString(DATE_FORMAT),
                                dataDate.Date.StartOfWeek(DayOfWeek.Monday).AddDays(this.DaysPerFile).ToString(DATE_FORMAT));

                            string targetFile = Path.Combine(targetPath, fileName);
                            if (!File.Exists(targetFile))   // Write the header first if the file does not exist
                                File.AppendAllText(targetFile, "Date,Price,Operator,Outbound,Inbound" + Environment.NewLine);

                            File.AppendAllText(targetFile,
                                               String.Format(
                                                   "\"{0}\",\"{1} {2}\",\"{3}\",\"{4} - {5}h ({6})\",\"{7} - {8}h ({9})\"{10}",
                                                   dataDate, flight.Price, flight.JourneyData.Currency, flight.Operator,
                                                   flight.OutboundLeg.Departure.ToShortTimeString(),
                                                   flight.OutboundLeg.Duration, flight.OutboundLeg.Transit,
                                                   flight.InboundLeg.Departure.ToShortTimeString(),
                                                   flight.InboundLeg.Duration, flight.InboundLeg.Transit,
                                                   Environment.NewLine));
                        }
                    }
                }
            }
        }

        public List<StorageRoute> GetRoutes()
        {
            var infos = this.GetRoutesInfo();
            return infos.Select(i => i.Route).ToList();
        }

        public List<RouteInfo> GetRoutesInfo()
        {
            var result = new List<RouteInfo>();
            var logger = AppContext.Logger;

            if (File.Exists(this.CatalogFile))
            {
                using (var fs = File.Open(this.CatalogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    try
                    {
                        var formatter = new TolerantBinaryFormatter(logger);
                        while (fs.Position != fs.Length)
                        {
                            var existRoute = formatter.Deserialize(fs) as RouteInfo;
                            if (existRoute != null)
                                result.Add(existRoute);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Failed to read route information: " + ex.Message);
                        fs.SetLength(0);
                        fs.Position = 0;
                    }
                }
            }

            return result;
        }

        public List<DatePeriod> GetTravelDates(StorageRoute route)
        {
            var routeInfo = this.SaveRoute(route);
            return this.GetTravelDates(routeInfo);
        }

        public List<DatePeriod> GetTravelDates(RouteInfo routeInfo)
        {
            string routePath = this.GetPath(routeInfo);
            var result = new List<DatePeriod>();

            if (Directory.Exists(routePath))
            {
                var dirs = Directory.GetDirectories(routePath);
                foreach (var d in dirs)
                {
                    var travelDate = this.GetDatePeriod(d);
                    if (travelDate != null)
                        result.Add(travelDate);
                }
            }

            return result;
        }

        public List<DatePeriod> GetDataDates(StorageRoute route, DatePeriod travelDate)
        {
            var routeInfo = this.SaveRoute(route);
            return this.GetDataDates(routeInfo, travelDate);
        }

        public List<DatePeriod> GetDataDates(RouteInfo routeInfo, DatePeriod travelDate)
        {
            var dataFiles = this.GetDataFiles(routeInfo, travelDate);
            var result = new List<DatePeriod>();
            if (dataFiles != null)
            {
                foreach (var f in dataFiles)
                {
                    var dataDate = this.GetDatePeriod(Path.GetFileNameWithoutExtension(f));
                    if (dataDate != null)
                        result.Add(dataDate);
                }
            }

            return result;
        }

        public List<string> GetDataFiles(RouteInfo routeInfo, DatePeriod travelDate)
        {
            var targetPath = this.GetPath(routeInfo, travelDate);
            var result = new List<string>();
            if (Directory.Exists(targetPath))
            {
                var files = Directory.GetFiles(targetPath, "*.csv");
                foreach (var f in files)
                {
                    var dataDate = this.GetDatePeriod(Path.GetFileNameWithoutExtension(f));
                    if (dataDate != null)
                        result.Add(f);
                }
            }

            return result;
        }

        public string GetDataFile(RouteInfo routeInfo, DatePeriod travelDate, DatePeriod dataPeriod)
        {
            var dataFiles = this.GetDataFiles(routeInfo, travelDate);
            if (dataFiles != null)
            {
                foreach (var f in dataFiles)
                {
                    var dataDate = this.GetDatePeriod(Path.GetFileNameWithoutExtension(f));
                    if (dataDate != null && dataPeriod.IsEquals(dataDate))
                        return f;
                }
            }

            return null;
        }

        /// <summary>
        /// Save the route to the catalogue
        /// </summary>
        /// <param name="route">Travel route</param>
        /// <returns>The saved route's information</returns>
        private RouteInfo SaveRoute(StorageRoute route)
        {
            var logger = AppContext.Logger;
            int max = 0;

            using (var fs = File.Open(this.CatalogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var formatter = new TolerantBinaryFormatter(logger);
                try
                {
                    while (fs.Position != fs.Length)
                    {
                        var existRoute = formatter.Deserialize(fs) as RouteInfo;
                        if (existRoute != null)
                        {
                            if (existRoute.Id > max)
                                max = existRoute.Id;

                            if (existRoute.Route.Equals(route))
                                return existRoute;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to read route information: " + ex.Message);
                    fs.SetLength(0);
                    fs.Position = 0;
                }

                var newItem = new RouteInfo(++max, route);
                formatter.Serialize(fs, newItem);

                return new RouteInfo(max, route);
            }
        }

        private string GetPath(RouteInfo routeInfo)
        {
            return Path.Combine(this.BaseDataPath, routeInfo.Id.ToString(CultureInfo.InvariantCulture));
        }

        private StorageRoute GetRoute(string dataPath)
        {
            var d = Path.GetFileName(dataPath);
            var parts = d.Split('|');
            if (parts.Length == 2)
            {
                var newRoute = new StorageRoute(parts[0].Trim(), parts[1].Trim());
                return newRoute;
            }

            return null;
        }

        private string GetDatePeriodFolderName(DatePeriod travelDate)
        {
            return travelDate.StartDate.ToString(DATE_FORMAT) + " - " + travelDate.EndDate.ToString(DATE_FORMAT);
        }

        private DatePeriod GetDatePeriod(string dataPath)
        {
            var d = Path.GetFileName(dataPath);
            var parts = d.Split('-');
            if (parts.Length == 2)
            {
                DateTime dept, ret;
                if (DateTime.TryParseExact(parts[0].Trim(), DATE_FORMAT, null, DateTimeStyles.AssumeLocal, out dept)
                    && DateTime.TryParseExact(parts[1].Trim(), DATE_FORMAT, null, DateTimeStyles.AssumeLocal, out ret))
                {
                    var newDate = new DatePeriod(dept, ret);
                    return newDate;
                }
            }

            return null;
        }

        private string GetPath(RouteInfo route, DatePeriod travelDate)
        {
            return Path.Combine(this.GetPath(route), this.GetDatePeriodFolderName(travelDate));
        }
    }
}