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

    /// <summary>
    /// The live fare file storage.
    /// </summary>
    public class LiveFareFileStorage : IFareStorage
    {
        /// <summary>
        /// The dat e_ format.
        /// </summary>
        private const string DATE_FORMAT = "yyyy.MM.dd";

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveFareFileStorage"/> class.
        /// </summary>
        /// <param name="baseDataPath">
        /// The base data path.
        /// </param>
        public LiveFareFileStorage(string baseDataPath)
        {
            this.BaseDataPath = baseDataPath;
            Directory.CreateDirectory(baseDataPath);
            this.DaysPerFile = 6;
        }

        /// <summary>
        /// Gets the base data path.
        /// </summary>
        public string BaseDataPath { get; private set; }

        /// <summary>
        /// Gets or sets the days per file.
        /// </summary>
        public int DaysPerFile { get; set; }

        /// <summary>
        /// Gets the catalog file.
        /// </summary>
        public string CatalogFile
        {
            get
            {
                return Path.Combine(this.BaseDataPath, "Catalog.bin");
            }
        }

        /// <summary>
        /// The save live fare.
        /// </summary>
        /// <param name="flights">
        /// The flights.
        /// </param>
        public void SaveLiveFare(IEnumerable<Flight> flights)
        {
            if (flights == null)
            {
                return;
            }

            var routeGroups = flights.GroupBy(f => new { f.JourneyData.JourneyInfo.DepartureCode, f.JourneyData.JourneyInfo.DestinationCode });
            if (routeGroups == null)
            {
                return;
            }

            foreach (var routeFlights in routeGroups)
            {
                if (routeFlights == null)
                {
                    continue;
                }

                var routeInfo = this.SaveRoute(new StorageRoute(routeFlights.Key.DepartureCode, routeFlights.Key.DestinationCode));

                var dateGroups = routeFlights.GroupBy(f => new { f.JourneyData.JourneyInfo.DepartureDate, f.JourneyData.JourneyInfo.ReturnDate });
                if (dateGroups == null)
                {
                    continue;
                }

                foreach (var dateFlights in dateGroups)
                {
                    if (dateFlights != null)
                    {
                        var targetPath = this.GetPath(routeInfo, new DatePeriod(dateFlights.Key.DepartureDate, dateFlights.Key.ReturnDate));
                        Directory.CreateDirectory(targetPath);

                        foreach (var flight in dateFlights)
                        {
                            DateTime dataDate = flight.JourneyData.DataDate;
                            string fileName = string.Format(
                                "{0}-{1}.csv", 
                                dataDate.Date.StartOfWeek(DayOfWeek.Monday).ToString(DATE_FORMAT), 
                                dataDate.Date.StartOfWeek(DayOfWeek.Monday).AddDays(this.DaysPerFile).ToString(DATE_FORMAT));

                            string targetFile = Path.Combine(targetPath, fileName);
                            if (!File.Exists(targetFile))
                            {
                                // Write the header first if the file does not exist
                                File.AppendAllText(targetFile, "Date,Price,Operator,Outbound,Inbound" + Environment.NewLine);
                            }

                            File.AppendAllText(
                                targetFile, 
                                string.Format(
                                    "\"{0}\",\"{1} {2}\",\"{3}\",\"{4} - {5}h ({6})\",\"{7} - {8}h ({9})\"{10}", 
                                    dataDate, 
                                    flight.Price, 
                                    flight.JourneyData.Currency, 
                                    flight.Operator, 
                                    flight.OutboundLeg.Departure.ToShortTimeString(), 
                                    flight.OutboundLeg.Duration, 
                                    flight.OutboundLeg.Transit, 
                                    flight.InboundLeg.Departure.ToShortTimeString(), 
                                    flight.InboundLeg.Duration, 
                                    flight.InboundLeg.Transit, 
                                    Environment.NewLine));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The get routes.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<StorageRoute> GetRoutes()
        {
            var infos = this.GetRoutesInfo();
            return infos.Select(i => i.Route).ToList();
        }

        /// <summary>
        /// The get travel dates.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<DatePeriod> GetTravelDates(StorageRoute route)
        {
            var routeInfo = this.SaveRoute(route);
            return this.GetTravelDates(routeInfo);
        }

        /// <summary>
        /// The get data dates.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="travelDate">
        /// The travel date.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<DatePeriod> GetDataDates(StorageRoute route, DatePeriod travelDate)
        {
            var routeInfo = this.SaveRoute(route);
            return this.GetDataDates(routeInfo, travelDate);
        }

        /// <summary>
        /// The get routes info.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
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
                            {
                                result.Add(existRoute);
                            }
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

        /// <summary>
        /// The get travel dates.
        /// </summary>
        /// <param name="routeInfo">
        /// The route info.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
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
                    {
                        result.Add(travelDate);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get data dates.
        /// </summary>
        /// <param name="routeInfo">
        /// The route info.
        /// </param>
        /// <param name="travelDate">
        /// The travel date.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
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
                    {
                        result.Add(dataDate);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get data files.
        /// </summary>
        /// <param name="routeInfo">
        /// The route info.
        /// </param>
        /// <param name="travelDate">
        /// The travel date.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
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
                    {
                        result.Add(f);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get data file.
        /// </summary>
        /// <param name="routeInfo">
        /// The route info.
        /// </param>
        /// <param name="travelDate">
        /// The travel date.
        /// </param>
        /// <param name="dataPeriod">
        /// The data period.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetDataFile(RouteInfo routeInfo, DatePeriod travelDate, DatePeriod dataPeriod)
        {
            var dataFiles = this.GetDataFiles(routeInfo, travelDate);
            if (dataFiles != null)
            {
                foreach (var f in dataFiles)
                {
                    var dataDate = this.GetDatePeriod(Path.GetFileNameWithoutExtension(f));
                    if (dataDate != null && dataPeriod.IsEquals(dataDate))
                    {
                        return f;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Save the route to the catalogue
        /// </summary>
        /// <param name="route">
        /// Travel route
        /// </param>
        /// <returns>
        /// The saved route's information
        /// </returns>
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
                            {
                                max = existRoute.Id;
                            }

                            if (existRoute.Route.Equals(route))
                            {
                                return existRoute;
                            }
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

        /// <summary>
        /// The get path.
        /// </summary>
        /// <param name="routeInfo">
        /// The route info.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetPath(RouteInfo routeInfo)
        {
            return Path.Combine(this.BaseDataPath, routeInfo.Id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The get route.
        /// </summary>
        /// <param name="dataPath">
        /// The data path.
        /// </param>
        /// <returns>
        /// The <see cref="StorageRoute"/>.
        /// </returns>
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

        /// <summary>
        /// The get date period folder name.
        /// </summary>
        /// <param name="travelDate">
        /// The travel date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDatePeriodFolderName(DatePeriod travelDate)
        {
            return travelDate.StartDate.ToString(DATE_FORMAT) + " - " + travelDate.EndDate.ToString(DATE_FORMAT);
        }

        /// <summary>
        /// The get date period.
        /// </summary>
        /// <param name="dataPath">
        /// The data path.
        /// </param>
        /// <returns>
        /// The <see cref="DatePeriod"/>.
        /// </returns>
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

        /// <summary>
        /// The get path.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="travelDate">
        /// The travel date.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetPath(RouteInfo route, DatePeriod travelDate)
        {
            return Path.Combine(this.GetPath(route), this.GetDatePeriodFolderName(travelDate));
        }
    }
}