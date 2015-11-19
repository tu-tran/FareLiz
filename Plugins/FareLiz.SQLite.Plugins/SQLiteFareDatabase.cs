namespace SkyDean.FareLiz.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.SQLite;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using log4net;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;
    using SkyDean.FareLiz.Core.Data;
    using SkyDean.FareLiz.Core.Presentation;
    using SkyDean.FareLiz.Core.Utils;
    using SkyDean.FareLiz.Data;
    using SkyDean.FareLiz.WinForm.Components.Dialog;

    /// <summary>
    /// The sq lite fare database.
    /// </summary>
    [DisplayName("SQLite Fare Database")]
    [Description("Data storage for flight data using local SQLite database")]
    public sealed partial class SQLiteFareDatabase : ISyncableDatabase
    {
        /// <summary>
        /// The dat e_ format.
        /// </summary>
        private const string DATE_FORMAT = "yyyy-MM-dd";

        /// <summary>
        /// The datetim e_ format.
        /// </summary>
        private const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// The _config.
        /// </summary>
        private SQLiteDatabaseConfig _config;

        /// <summary>
        /// The _connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// The _formatter.
        /// </summary>
        private ProtoBufTransfer _formatter;

        /// <summary>
        /// Gets a value indicating whether is configurable.
        /// </summary>
        public bool IsConfigurable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the data file name.
        /// </summary>
        public string DataFileName
        {
            get
            {
                return this._config.DataFileName;
            }
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public IConfig Configuration
        {
            get
            {
                return this._config;
            }

            set
            {
                this._config = value as SQLiteDatabaseConfig;
            }
        }

        /// <summary>
        /// Gets or sets the data synchronizer.
        /// </summary>
        public IDataSyncer DataSynchronizer { get; set; }

        /// <summary>
        /// Gets or sets the package synchronizer.
        /// </summary>
        public IPackageSyncer<TravelRoute> PackageSynchronizer { get; set; }

        /// <summary>
        /// Gets the custom config builder.
        /// </summary>
        public IConfigBuilder CustomConfigBuilder
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the default config.
        /// </summary>
        public IConfig DefaultConfig
        {
            get
            {
                return new SQLiteDatabaseConfig();
            }
        }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILog Logger { get; set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        public void Initialize()
        {
            if (this.Configuration == null)
            {
                this.Configuration = this.DefaultConfig;
            }

            this._formatter = new ProtoBufTransfer(this.Logger);
            var connBuilder = new SQLiteConnectionStringBuilder
                                  {
                                      DataSource = this.DataFileName, 
                                      DateTimeKind = DateTimeKind.Utc, 
                                      ForeignKeys = true, 
                                      Pooling = true, 
                                      SyncMode = SynchronizationModes.Full, 
                                      Version = 3
                                  };

            this._connectionString = connBuilder.ToString();

            if (!File.Exists(this.DataFileName))
            {
                // TODO: Refactor
                this.Reset(null);
            }
        }

        /// <summary>
        /// The get routes.
        /// </summary>
        /// <param name="loadJourneys">
        /// The load journeys.
        /// </param>
        /// <param name="loadJourneyData">
        /// The load journey data.
        /// </param>
        /// <param name="loadHistory">
        /// The load history.
        /// </param>
        /// <param name="loadFlights">
        /// The load flights.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<TravelRoute> GetRoutes(bool loadJourneys, bool loadJourneyData, bool loadHistory, bool loadFlights, IProgressCallback callback)
        {
            this.Logger.DebugFormat(
                "Get available routes [{0}{1}{2}{3}]", 
                loadJourneys ? "J" : null, 
                loadJourneyData ? "D" : null, 
                loadHistory ? "H" : null, 
                loadFlights ? "F" : null);
            var result = new List<TravelRoute>();
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                using (var getPlacesCmd = new SQLiteCommand("SELECT LID, SDEPARTURE, SDESTINATION FROM ROUTE", connection))
                {
                    connection.Open();
                    using (var reader = getPlacesCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int iLid = reader.GetOrdinal("LID");
                            int iDeparture = reader.GetOrdinal("SDEPARTURE");
                            int iDestination = reader.GetOrdinal("SDESTINATION");

                            while (reader.Read())
                            {
                                long id = reader.GetInt64(iLid);
                                string origin = reader.GetString(iDeparture);
                                string destination = reader.GetString(iDestination);
                                var newRoute = new TravelRoute(id, AirportDataProvider.FromIATA(origin), AirportDataProvider.FromIATA(destination));

                                if (loadJourneys)
                                {
                                    this.LoadData(newRoute, loadJourneyData, loadHistory, loadFlights, callback);
                                }

                                result.Add(newRoute);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The load data.
        /// </summary>
        /// <param name="route">
        /// The route.
        /// </param>
        /// <param name="loadJourneyData">
        /// The load journey data.
        /// </param>
        /// <param name="loadHistory">
        /// The load history.
        /// </param>
        /// <param name="loadFlights">
        /// The load flights.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        public void LoadData(TravelRoute route, bool loadJourneyData, bool loadHistory, bool loadFlights, IProgressCallback callback)
        {
            this.Logger.DebugFormat(
                "Load data for route [{0}-{1}] [{2}{3}{4}]", 
                route.Departure.IATA, 
                route.Destination.IATA, 
                loadJourneyData ? "D" : null, 
                loadHistory ? "H" : null, 
                loadFlights ? "F" : null);
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                if (route.Id < 1)
                {
                    throw new ArgumentException("Invalid Route Id");
                }

                route.Journeys.Clear();
                string selectSql = "SELECT J.LID, J.TDEPARTURE, J.TRETURN"
                                   + (loadJourneyData
                                          ? (", D.LID DATAID, D.SCURRENCY, " + (loadHistory ? "D.TUPDATE" : "MAX(D.TUPDATE) TUPDATE")
                                             + (loadFlights ? ", D.BFLIGHT" : string.Empty))
                                          : string.Empty) + " FROM JOURNEY J" + (loadJourneyData ? ", JOURNEY_DATA D " : string.Empty) + " WHERE J.LROUTEID = @lRouteId "
                                   + (loadJourneyData ? " AND D.LJOURNEYID = J.LID " + (loadHistory ? string.Empty : " GROUP BY (D.LJOURNEYID) ") : string.Empty);

                using (var getJourneyCmd = new SQLiteCommand(selectSql, connection))
                {
                    getJourneyCmd.Parameters.AddWithValue("@lRouteId", route.Id);

                    connection.Open();
                    using (var reader = getJourneyCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int iId = reader.GetOrdinal("LID");
                            int iDeparture = reader.GetOrdinal("TDEPARTURE");
                            int iReturn = reader.GetOrdinal("TRETURN");
                            int iUpdate = reader.GetOrdinal("TUPDATE");
                            int iCurrency = loadJourneyData ? reader.GetOrdinal("SCURRENCY") : -1;
                            int iDataId = loadJourneyData ? reader.GetOrdinal("DATAID") : -1;
                            int iFlight = loadJourneyData ? reader.GetOrdinal("BFLIGHT") : -1;

                            while (reader.Read())
                            {
                                long journeyId = reader.GetInt64(iId);
                                Journey journey = null;
                                var allJourneys = route.Journeys;
                                foreach (var j in allJourneys)
                                {
                                    if (j.Id == journeyId)
                                    {
                                        journey = j;
                                        break;
                                    }
                                }

                                if (journey == null)
                                {
                                    var deptDate = DateTime.ParseExact(reader.GetString(iDeparture), DATE_FORMAT, CultureInfo.InvariantCulture);
                                    var retDate = DateTime.ParseExact(reader.GetString(iReturn), DATE_FORMAT, CultureInfo.InvariantCulture);
                                    journey = new Journey(journeyId, route, deptDate, retDate);
                                    route.AddJourney(journey);
                                }

                                if (loadJourneyData)
                                {
                                    var dataDate = DateTime.ParseExact(
                                        reader.GetString(iUpdate), 
                                        DATETIME_FORMAT, 
                                        CultureInfo.InvariantCulture, 
                                        DateTimeStyles.AssumeUniversal);
                                    var newData = new JourneyData(reader.GetInt64(iDataId), reader.GetString(iCurrency), dataDate);

                                    if (loadFlights)
                                    {
                                        var dbFlights = this._formatter.FromRaw<List<Flight>>((byte[])reader[iFlight]);
                                        if (dbFlights != null && dbFlights.Count > 0)
                                        {
                                            newData.AddFlights(dbFlights);
                                        }
                                    }

                                    journey.AddData(newData);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The load data.
        /// </summary>
        /// <param name="journeys">
        /// The journeys.
        /// </param>
        /// <param name="loadHistory">
        /// The load history.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void LoadData(IList<Journey> journeys, bool loadHistory, IProgressCallback callback)
        {
            this.Logger.DebugFormat("Load data for {0} journeys [{1}]", journeys.Count, loadHistory ? "H" : null);
            foreach (var j in journeys)
            {
                j.Data.Clear();
            }

            string condition = GenerateCondition(journeys);
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                string selectSql = "SELECT LID, LJOURNEYID, SCURRENCY, BFLIGHT, " + (loadHistory ? "TUPDATE" : "MAX(TUPDATE) TUPDATE")
                                   + " FROM JOURNEY_DATA " + " WHERE " + condition + (loadHistory ? string.Empty : " GROUP BY (LJOURNEYID)");

                using (var getFlightsCmd = new SQLiteCommand(selectSql, connection))
                {
                    connection.Open();

                    using (var reader = getFlightsCmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            if (reader.HasRows)
                            {
                                int iId = reader.GetOrdinal("LID");
                                int iJourneyId = reader.GetOrdinal("LJOURNEYID");
                                int iCurrency = reader.GetOrdinal("SCURRENCY");
                                int iUpdate = reader.GetOrdinal("TUPDATE");
                                int iFlight = reader.GetOrdinal("BFLIGHT");

                                while (reader.Read())
                                {
                                    var flights = this._formatter.FromRaw<List<Flight>>((byte[])reader[iFlight]);
                                    if (flights != null && flights.Count > 0)
                                    {
                                        long dataId = reader.GetInt64(iId);
                                        long journeyId = reader.GetInt64(iJourneyId);
                                        string currency = reader.GetString(iCurrency);
                                        string dataDateStr = reader.GetString(iUpdate);
                                        DateTime dataDate = DateTime.ParseExact(
                                            dataDateStr, 
                                            DATETIME_FORMAT, 
                                            CultureInfo.InvariantCulture, 
                                            DateTimeStyles.AssumeUniversal);

                                        var newData = new JourneyData(dataId, currency, dataDate);
                                        newData.AddFlights(flights);

                                        foreach (var j in journeys)
                                        {
                                            if (j.Id == journeyId)
                                            {
                                                j.AddData(newData);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The load data.
        /// </summary>
        /// <param name="journey">
        /// The journey.
        /// </param>
        /// <param name="loadHistory">
        /// The load history.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void LoadData(Journey journey, bool loadHistory, IProgressCallback callback)
        {
            this.LoadData(new List<Journey> { journey }, loadHistory, callback);
        }

        /// <summary>
        /// The validate database.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="ValidateResult"/>.
        /// </returns>
        public ValidateResult ValidateDatabase(IProgressCallback callback)
        {
            this.Logger.Info("Validate database");
            string error = null;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                error = string.Empty;
                using (
                    var getFlightCmd = new SQLiteCommand(@"SELECT COUNT(*) FROM FLIGHT WHERE LJOURNEYID NOT IN (SELECT LID FROM JOURNEY)", connection)
                    )
                {
                    connection.Open();
                    object result = getFlightCmd.ExecuteScalar();

                    long orphanCount;
                    if (result == null || result is DBNull)
                    {
                        orphanCount = 0;
                    }
                    else
                    {
                        orphanCount = (long)result;
                    }

                    if (orphanCount > 0)
                    {
                        error = string.Format("There are {0} orphan flights without any assosiated journey detail", orphanCount);
                    }
                }
            }

            var valResult = new ValidateResult(error == null, error);
            return valResult;
        }

        /// <summary>
        /// The reset data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void ResetData(IList<TravelRoute> data, IProgressCallback callback)
        {
            if (data == null || data.Count < 1)
            {
                return;
            }

            ProgressDialog.ExecuteTask(
                null, 
                "Reset Database", 
                "Please wait...", 
                "SQLiteDbReset", 
                ProgressBarStyle.Marquee, 
                this.Logger, 
                cb =>
                    {
                        cb.Begin();
                        this.Logger.Info("Clear database");
                        this.Reset(cb);
                        this.AddData(data, cb);
                    });
        }

        /// <summary>
        /// The repair database.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void RepairDatabase(IProgressCallback callback)
        {
            try
            {
                this.TryOpenDatabase();
                this.Optimize();
            }
            catch (Exception ex)
            {
                this.Logger.Error("Failed to optimize database: " + ex);
                this.Reset(callback);
            }
        }

        /// <summary>
        /// The reset.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void Reset(IProgressCallback callback)
        {
            ProgressDialog.ExecuteTask(
                null, 
                "Initialize new database", 
                "Please wait...", 
                "SQLiteDbReset", 
                ProgressBarStyle.Marquee, 
                this.Logger, 
                cb =>
                    {
                        cb.Begin();
                        this.DoResetDatabase(cb);
                    });
        }

        /// <summary>
        /// The add data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void AddData(IList<TravelRoute> data, IProgressCallback callback)
        {
            this.AddData(data, null, DateTime.MinValue, callback);
        }

        /// <summary>
        /// The add package.
        /// </summary>
        /// <param name="packageId">
        /// The package id.
        /// </param>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void AddPackage(string packageId, IList<TravelRoute> data, IProgressCallback callback)
        {
            if (!this.IsPackageImported(packageId, callback))
            {
                this.AddData(data, packageId, DateTime.Now, callback);
            }
        }

        /// <summary>
        /// The is package imported.
        /// </summary>
        /// <param name="packageId">
        /// The package id.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsPackageImported(string packageId, IProgressCallback callback)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                return this.IsPackageImported(packageId, connection, callback);
            }
        }

        /// <summary>
        /// The get imported packages.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="IList"/>.
        /// </returns>
        public IList<string> GetImportedPackages(IProgressCallback callback)
        {
            var result = new List<string>();
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                connection.Open();
                using (var getPackageCmd = new SQLiteCommand(@"SELECT SID FROM DATA_PACKAGE", connection))
                {
                    using (var reader = getPackageCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader["SID"].ToString());
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get statistics.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetStatistics(IProgressCallback callback)
        {
            this.Logger.Info("Get database statistics");
            this.Initialize();
            long route, journey, package;

            using (var connection = new SQLiteConnection(this._connectionString))
            {
                using (var command = new SQLiteCommand(connection))
                {
                    connection.Open();
                    command.CommandText = "SELECT COUNT(*) FROM ROUTE";
                    route = (long)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM JOURNEY";
                    journey = (long)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM DATA_PACKAGE";
                    package = (long)command.ExecuteScalar();
                }
            }

            return string.Format(@"==============================
     SQLite FareDatabase
==============================
Data File: {0}
Size: {1}
Routes: {2}
Journeys: {3}
Packages: {4}", this.DataFileName, StringUtil.FormatSize(new FileInfo(this.DataFileName).Length), route, journey, package);
        }

        /// <summary>
        /// The generate condition.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GenerateCondition(IList<Journey> data)
        {
            const string idCol = "LJOURNEYID";
            if (data.Count == 1)
            {
                return idCol + "=" + data[0].Id;
            }

            var sb = new StringBuilder();
            int total = data.Count, top = total - 1;
            sb.Append(idCol + " IN (");
            for (int i = 0; i < total; i++)
            {
                sb.Append(data[i].Id);
                if (i != top)
                {
                    sb.Append(",");
                }
            }

            sb.Append(")");
            var result = sb.ToString();
            return result;
        }

        /// <summary>
        /// The optimize.
        /// </summary>
        private void Optimize()
        {
            this.Logger.Info("Optimize database");
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = string.Format(
                        "DELETE FROM JOURNEY WHERE TDEPARTURE = '{0}' OR TRETURN = '{0}'", 
                        DateTime.MinValue.ToString(DATE_FORMAT));
                    connection.Open();
                    command.ExecuteNonQuery();

                    command.CommandText = "VACUUM";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The do reset database.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <exception cref="ApplicationException">
        /// </exception>
        private void DoResetDatabase(IProgressCallback callback)
        {
            this.Logger.Info("Reset database");
            string sql;
            using (Stream dataStream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Namespace + "." + "DbSchema.sqlite"))
            {
                if (dataStream == null)
                {
                    throw new ApplicationException("There is no embedded backup database script!");
                }

                using (StreamReader sr = new StreamReader(dataStream)) sql = sr.ReadToEnd();
            }

            SQLiteConnection.ClearAllPools();
            SQLiteConnection.CreateFile(this.DataFileName);
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// The add data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="packageId">
        /// The package id.
        /// </param>
        /// <param name="packageCreatedAt">
        /// The package created at.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void AddData(IList<TravelRoute> data, string packageId, DateTime packageCreatedAt, IProgressCallback callback)
        {
            ProgressDialog.ExecuteTask(
                null, 
                "Data Import", 
                "Please wait...", 
                "SQLiteDbAddData", 
                ProgressBarStyle.Continuous, 
                this.Logger, 
                cb =>
                    {
                        cb.Begin();
                        this.DoAddData(data, packageId, packageCreatedAt, cb as ProgressDialog);
                    });
        }

        /// <summary>
        /// The add data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public void AddData(IList<DataPackage<TravelRoute>> data, IProgressCallback callback)
        {
            if (data != null && data.Count > 0)
            {
                ProgressDialog.ExecuteTask(
                    null, 
                    "Data Package Import", 
                    "Please wait...", 
                    "SQLiteDbAddDataPkg", 
                    ProgressBarStyle.Continuous, 
                    this.Logger, 
                    cb =>
                        {
                            cb.Begin();
                            cb.SetRange(0, data.Count);
                            foreach (var pkg in data)
                            {
                                string log = "Saving package " + pkg.Id;
                                this.Logger.Info(log);
                                cb.Text = log;
                                this.AddData(pkg.Data, pkg.Id, pkg.CreatedDate, cb);
                                cb.Increment(1);
                            }
                        });
            }
        }

        /// <summary>
        /// The do add data.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="packageId">
        /// The package id.
        /// </param>
        /// <param name="packageCreatedAt">
        /// The package created at.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        private void DoAddData(IList<TravelRoute> data, string packageId, DateTime packageCreatedAt, ProgressDialog callback)
        {
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                if (data != null && data.Count > 0)
                {
                    long totalData = 0;
                    this.Logger.InfoFormat("Add new {0} travel routes into database", data.Count);

                    var getIdRouteCmd = new SQLiteCommand("SELECT LID FROM ROUTE WHERE SDEPARTURE=@sDept AND SDESTINATION=@sDest", connection);
                    getIdRouteCmd.Parameters.Add("@sDept", DbType.String);
                    getIdRouteCmd.Parameters.Add("@sDest", DbType.String);

                    var insertRouteCmd = new SQLiteCommand(
                        "INSERT INTO ROUTE(LID, SDEPARTURE, SDESTINATION) " + "VALUES(@lId, @sDept, @sDest)", 
                        connection);
                    insertRouteCmd.Parameters.Add("@lId", DbType.Int64);
                    insertRouteCmd.Parameters.Add("@sDept", DbType.String);
                    insertRouteCmd.Parameters.Add("@sDest", DbType.String);

                    var getIdJourneyCmd = new SQLiteCommand(
                        "SELECT LID FROM JOURNEY WHERE LROUTEID=@lRouteId AND TDEPARTURE=@tDept AND TRETURN=@tRet", 
                        connection);
                    getIdJourneyCmd.Parameters.Add("@lRouteId", DbType.Int64);
                    getIdJourneyCmd.Parameters.Add("@tDept", DbType.String);
                    getIdJourneyCmd.Parameters.Add("@tRet", DbType.String);

                    var insertJourneyCmd =
                        new SQLiteCommand(
                            "INSERT INTO JOURNEY(LID, LROUTEID, TDEPARTURE, TRETURN) " + "VALUES(@lId, @lRouteId, @tDept, @tRet)", 
                            connection);
                    insertJourneyCmd.Parameters.Add("@lId", DbType.Int64);
                    insertJourneyCmd.Parameters.Add("@lRouteId", DbType.Int64);
                    insertJourneyCmd.Parameters.Add("@tDept", DbType.String);
                    insertJourneyCmd.Parameters.Add("@tRet", DbType.String);

                    var getIdJourneyDataCmd = new SQLiteCommand(
                        "SELECT LID FROM JOURNEY_DATA WHERE LJOURNEYID=@lJourneyId AND TUPDATE=@tUpdate", 
                        connection);
                    getIdJourneyDataCmd.Parameters.Add("@lJourneyId", DbType.Int64);
                    getIdJourneyDataCmd.Parameters.Add("@tUpdate", DbType.String);

                    var insertJourneyDataCmd =
                        new SQLiteCommand(
                            "INSERT INTO JOURNEY_DATA(LID, LJOURNEYID, SCURRENCY, TUPDATE, BFLIGHT) "
                            + "VALUES(@lId, @lJourneyId, @sCurrency, @tUpdate, @bFlight)", 
                            connection);
                    insertJourneyDataCmd.Parameters.Add("@lId", DbType.Int64);
                    insertJourneyDataCmd.Parameters.Add("@lJourneyId", DbType.Int64);
                    insertJourneyDataCmd.Parameters.Add("@sCurrency", DbType.String);
                    insertJourneyDataCmd.Parameters.Add("@tUpdate", DbType.String);
                    insertJourneyDataCmd.Parameters.Add("@bFlight", DbType.Binary);

                    var updateJourneyDataCmd =
                        new SQLiteCommand("UPDATE JOURNEY_DATA " + "SET SCURRENCY = @sCurrency, BFLIGHT = @bFlight " + "WHERE LID = @lId", connection);
                    updateJourneyDataCmd.Parameters.Add("@lId", DbType.Int64);
                    updateJourneyDataCmd.Parameters.Add("@sCurrency", DbType.String);
                    updateJourneyDataCmd.Parameters.Add("@bFlight", DbType.Binary);

                    connection.Open();
                    using (var startCmd = new SQLiteCommand("BEGIN TRANSACTION", connection))
                    {
                        startCmd.ExecuteNonQuery(); // Begin transaction
                    }

                    long nextRouteId = GetMaxRowId(connection, "ROUTE"), 
                         nextJourneyId = GetMaxRowId(connection, "JOURNEY"), 
                         nextDataId = GetMaxRowId(connection, "JOURNEY_DATA");

                    int totalJourneys = data.Sum(r => r.Journeys.Count);
                    callback.SetRange(0, totalJourneys);

                    using (getIdJourneyCmd)
                    using (insertJourneyCmd)
                    using (getIdJourneyDataCmd)
                    using (insertJourneyDataCmd)
                    {
                        foreach (var r in data)
                        {
                            if (r.Departure == null || r.Destination == null)
                            {
                                continue;
                            }

                            var deptAirport = AirportDataProvider.FromIATA(r.Departure.IATA);
                            if (deptAirport == null)
                            {
                                continue;
                            }

                            var destAirport = AirportDataProvider.FromIATA(r.Destination.IATA);
                            if (destAirport == null)
                            {
                                continue;
                            }

                            getIdRouteCmd.Parameters["@sDept"].Value = deptAirport.IATA;
                            getIdRouteCmd.Parameters["@sDest"].Value = destAirport.IATA;
                            var dbRouteId = getIdRouteCmd.ExecuteScalar();

                            bool isNewRoute = dbRouteId == null || dbRouteId is DBNull;
                            if (isNewRoute)
                            {
                                // Create new route if it does not exist
                                insertRouteCmd.Parameters["@lId"].Value = ++nextRouteId;
                                insertRouteCmd.Parameters["@sDept"].Value = deptAirport.IATA;
                                insertRouteCmd.Parameters["@sDest"].Value = destAirport.IATA;
                                insertRouteCmd.ExecuteNonQuery();
                            }
                            else
                            {
                                nextRouteId = (long)dbRouteId; // Reuse existing route
                            }

                            callback.Text = "[" + deptAirport + "] - [" + destAirport + "]";

                            foreach (Journey j in r.Journeys)
                            {
                                if (callback.IsAborting)
                                {
                                    return;
                                }

                                if (j.Data.Count < 1)
                                {
                                    continue;
                                }

                                bool isNewJourney = true;
                                if (!isNewRoute)
                                {
                                    getIdJourneyCmd.Parameters["@lRouteId"].Value = nextRouteId;
                                    getIdJourneyCmd.Parameters["@tDept"].Value = j.DepartureDate.ToString(DATE_FORMAT);
                                    getIdJourneyCmd.Parameters["@tRet"].Value = j.ReturnDate.ToString(DATE_FORMAT);

                                    var dbJourneyId = getIdJourneyCmd.ExecuteScalar();
                                    isNewJourney = dbJourneyId == null || dbJourneyId is DBNull;
                                    if (!isNewJourney)
                                    {
                                        j.Id = (long)dbJourneyId; // Reuse existing journey
                                    }
                                }

                                if (isNewJourney)
                                {
                                    // Create new journey if it does not exist
                                    j.Id = ++nextJourneyId;
                                    insertJourneyCmd.Parameters["@lId"].Value = j.Id;
                                    insertJourneyCmd.Parameters["@lRouteId"].Value = nextRouteId;
                                    insertJourneyCmd.Parameters["@tDept"].Value = j.DepartureDate.ToString(DATE_FORMAT);
                                    insertJourneyCmd.Parameters["@tRet"].Value = j.ReturnDate.ToString(DATE_FORMAT);
                                    insertJourneyCmd.ExecuteNonQuery();
                                }

                                foreach (JourneyData journeyData in j.Data)
                                {
                                    bool isNewJourneyData = true;
                                    if (!isNewJourney)
                                    {
                                        getIdJourneyDataCmd.Parameters["@lJourneyId"].Value = j.Id;
                                        getIdJourneyDataCmd.Parameters["@tUpdate"].Value =
                                            journeyData.DataDate.ToUniversalTime().ToString(DATETIME_FORMAT, null);
                                        var dbDataId = getIdJourneyDataCmd.ExecuteScalar();
                                        isNewJourneyData = dbDataId == null || dbDataId is DBNull;
                                        if (!isNewJourneyData)
                                        {
                                            journeyData.Id = (long)dbDataId;
                                        }
                                    }

                                    if (isNewJourneyData)
                                    {
                                        // Create new journey data history if it does not exist
                                        journeyData.Id = ++nextDataId;
                                        insertJourneyDataCmd.Parameters["@lId"].Value = journeyData.Id;
                                        insertJourneyDataCmd.Parameters["@lJourneyId"].Value = j.Id;
                                        insertJourneyDataCmd.Parameters["@sCurrency"].Value = journeyData.Currency;
                                        insertJourneyDataCmd.Parameters["@tUpdate"].Value =
                                            journeyData.DataDate.ToUniversalTime().ToString(DATETIME_FORMAT, null);
                                        insertJourneyDataCmd.Parameters["@bFlight"].Value = this._formatter.ToRaw(journeyData.Flights);
                                        insertJourneyDataCmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        // Update the journey data if it already exists
                                        updateJourneyDataCmd.Parameters["@lId"].Value = journeyData.Id;
                                        updateJourneyDataCmd.Parameters["@sCurrency"].Value = journeyData.Currency;
                                        updateJourneyDataCmd.Parameters["@bFlight"].Value = this._formatter.ToRaw(journeyData.Flights);
                                        updateJourneyDataCmd.ExecuteNonQuery(); // Update existing journey data
                                    }
                                }

                                callback.Increment(1);
                                totalData += j.Data.Count;
                            }
                        }
                    }

                    // Process the data package id
                    if (!string.IsNullOrEmpty(packageId) && !this.IsPackageImported(packageId, connection, callback))
                    {
                        this.Logger.Info("Register package ID " + packageId);

                        // TODO: Check this
                        // callback.Style = ProgressStyleMarquee;
                        using (
                            var insertPkgCmd =
                                new SQLiteCommand(
                                    "INSERT INTO DATA_PACKAGE (SID, TCREATED, TINSERTED) " + " VALUES (@pkgId, @pkgCreatedAt, @pkgInsertedAt)", 
                                    connection))
                        {
                            insertPkgCmd.Parameters.Add(new SQLiteParameter("@pkgId", packageId));
                            insertPkgCmd.Parameters.Add(
                                new SQLiteParameter("@pkgCreatedAt", packageCreatedAt.ToUniversalTime().ToString(DATE_FORMAT)));
                            insertPkgCmd.Parameters.Add(new SQLiteParameter("@pkgInsertedAt", DateTime.UtcNow.ToString(DATE_FORMAT)));

                            if (connection.State != ConnectionState.Open)
                            {
                                connection.Open();
                            }

                            insertPkgCmd.ExecuteNonQuery();
                        }
                    }

                    // Finally, commit the transaction
                    this.Logger.InfoFormat("Completed adding {0} journey data. Commiting", totalData);

                    // Completed inserting all journeys or inserted nothing
                    using (var endCmd = new SQLiteCommand("COMMIT TRANSACTION", connection))
                    {
                        endCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        /// <summary>
        /// The is package imported.
        /// </summary>
        /// <param name="packageId">
        /// The package id.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="callback">
        /// The callback.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsPackageImported(string packageId, SQLiteConnection connection, IProgressCallback callback)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            using (var getPackageCountCmd = new SQLiteCommand(@"SELECT COUNT(*) FROM DATA_PACKAGE WHERE SID = '" + packageId + "'", connection))
            {
                object result = getPackageCountCmd.ExecuteScalar();
                if (!(result == null || result is DBNull))
                {
                    return (long)result > 0;
                }

                return false;
            }
        }

        /// <summary>
        /// The try open database.
        /// </summary>
        public void TryOpenDatabase()
        {
            this.Logger.Info("Try opening database");
            using (var connection = new SQLiteConnection(this._connectionString))
            {
                using (var testCmd = new SQLiteCommand(@"SELECT COUNT(*) FROM sqlite_master WHERE type='table'", connection))
                {
                    connection.Open();
                    object result = testCmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// The get place id.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="placeName">
        /// The place name.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private static long GetPlaceId(SQLiteConnection connection, string placeName)
        {
            using (var getPlaceCmd = new SQLiteCommand("SELECT LID FROM PLACE WHERE SNAME=@sname", connection))
            {
                getPlaceCmd.Parameters.AddWithValue("@sname", placeName);

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var result = getPlaceCmd.ExecuteScalar();
                if (result == null || result is DBNull)
                {
                    return -1;
                }

                return (long)result;
            }
        }

        /// <summary>
        /// The get max row id.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="long"/>.
        /// </returns>
        private static long GetMaxRowId(SQLiteConnection connection, string tableName)
        {
            using (var getRowIdCmd = new SQLiteCommand("SELECT MAX(_ROWID_) FROM " + tableName, connection))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                var result = getRowIdCmd.ExecuteScalar();
                if (result == null || result is DBNull)
                {
                    return 0;
                }

                return (long)result;
            }
        }
    }
}