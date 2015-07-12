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
using SkyDean.FareLiz.WinForm.Components.Dialog;
using SkyDean.FareLiz.Data;

namespace SkyDean.FareLiz.SQLite
{
    [DisplayName("SQLite Fare Database")]
    [Description("Data storage for flight data using local SQLite database")]
    public sealed partial class SQLiteFareDatabase : ISyncableDatabase
    {
        private const string DATE_FORMAT = "yyyy-MM-dd";
        private const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        private ProtoBufTransfer _formatter = null;

        SQLiteDatabaseConfig _config;
        public IConfig Configuration { get { return _config; } set { _config = value as SQLiteDatabaseConfig; } }
        public bool IsConfigurable { get { return false; } }
        public IDataSyncer DataSynchronizer { get; set; }
        public IPackageSyncer<TravelRoute> PackageSynchronizer { get; set; }
        public IConfigBuilder CustomConfigBuilder { get { return null; } }
        public IConfig DefaultConfig { get { return new SQLiteDatabaseConfig(); } }
        public ILog Logger { get; set; }

        public string DataFileName { get { return _config.DataFileName; } }

        private string _connectionString;

        public void Initialize()
        {
            if (Configuration == null)
                Configuration = DefaultConfig;

            _formatter = new ProtoBufTransfer(Logger);
            var connBuilder = new SQLiteConnectionStringBuilder()
            {
                DataSource = DataFileName,
                DateTimeKind = DateTimeKind.Utc,
                ForeignKeys = true,
                Pooling = true,
                SyncMode = SynchronizationModes.Full,
                Version = 3
            };

            _connectionString = connBuilder.ToString();

            if (!File.Exists(DataFileName))
            {
                // TODO: Refactor
                Reset(null);
            }
        }

        public IList<TravelRoute> GetRoutes(bool loadJourneys, bool loadJourneyData, bool loadHistory, bool loadFlights, IProgressCallback callback)
        {
            Logger.DebugFormat("Get available routes [{0}{1}{2}{3}]", loadJourneys ? "J" : null, loadJourneyData ? "D" : null, loadHistory ? "H" : null, loadFlights ? "F" : null);
            var result = new List<TravelRoute>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var getPlacesCmd = new SQLiteCommand(
                    "SELECT LID, SDEPARTURE, SDESTINATION FROM ROUTE", connection))
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
                                    LoadData(newRoute, loadJourneyData, loadHistory, loadFlights, callback);
                                result.Add(newRoute);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public void LoadData(TravelRoute route, bool loadJourneyData, bool loadHistory, bool loadFlights, IProgressCallback callback)
        {
            Logger.DebugFormat("Load data for route [{0}-{1}] [{2}{3}{4}]", route.Departure.IATA, route.Destination.IATA, loadJourneyData ? "D" : null, loadHistory ? "H" : null, loadFlights ? "F" : null);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                if (route.Id < 1)
                    throw new ArgumentException("Invalid Route Id");

                route.Journeys.Clear();
                string selectSql =
                    "SELECT J.LID, J.TDEPARTURE, J.TRETURN" + (loadJourneyData ? (", D.LID DATAID, D.SCURRENCY, " + (loadHistory ? "D.TUPDATE" : "MAX(D.TUPDATE) TUPDATE") + (loadFlights ? ", D.BFLIGHT" : "")) : "") +
                    " FROM JOURNEY J" + (loadJourneyData ? ", JOURNEY_DATA D " : "") +
                    " WHERE J.LROUTEID = @lRouteId " + (loadJourneyData ? " AND D.LJOURNEYID = J.LID " +
                    (loadHistory ? "" : " GROUP BY (D.LJOURNEYID) ") : String.Empty);

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
                            int iCurrency = (loadJourneyData ? reader.GetOrdinal("SCURRENCY") : -1);
                            int iDataId = (loadJourneyData ? reader.GetOrdinal("DATAID") : -1);
                            int iFlight = (loadJourneyData ? reader.GetOrdinal("BFLIGHT") : -1);

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
                                    var dataDate = DateTime.ParseExact(reader.GetString(iUpdate), DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
                                    var newData = new JourneyData(reader.GetInt64(iDataId), reader.GetString(iCurrency), dataDate);

                                    if (loadFlights)
                                    {
                                        var dbFlights = _formatter.FromRaw<List<Flight>>((byte[])reader[iFlight]);
                                        if (dbFlights != null && dbFlights.Count > 0)
                                            newData.AddFlights(dbFlights);
                                    }

                                    journey.AddData(newData);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void LoadData(IList<Journey> journeys, bool loadHistory, IProgressCallback callback)
        {
            Logger.DebugFormat("Load data for {0} journeys [{1}]", journeys.Count, loadHistory ? "H" : null);
            foreach (var j in journeys)
                j.Data.Clear();

            string condition = GenerateCondition(journeys);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                string selectSql =
                    "SELECT LID, LJOURNEYID, SCURRENCY, BFLIGHT, " + (loadHistory ? "TUPDATE" : "MAX(TUPDATE) TUPDATE") +
                    " FROM JOURNEY_DATA " +
                    " WHERE " + condition +
                    (loadHistory ? String.Empty : " GROUP BY (LJOURNEYID)");

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
                                    var flights = _formatter.FromRaw<List<Flight>>((byte[])reader[iFlight]);
                                    if (flights != null && flights.Count > 0)
                                    {
                                        long dataId = reader.GetInt64(iId);
                                        long journeyId = reader.GetInt64(iJourneyId);
                                        string currency = reader.GetString(iCurrency);
                                        string dataDateStr = reader.GetString(iUpdate);
                                        DateTime dataDate = DateTime.ParseExact(dataDateStr, DATETIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

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

        private static string GenerateCondition(IList<Journey> data)
        {
            const string idCol = "LJOURNEYID";
            if (data.Count == 1)
                return idCol + "=" + data[0].Id;
            var sb = new StringBuilder();
            int total = data.Count, top = total - 1;
            sb.Append(idCol + " IN (");
            for (int i = 0; i < total; i++)
            {
                sb.Append(data[i].Id);
                if (i != top)
                    sb.Append(",");
            }
            sb.Append(")");
            var result = sb.ToString();
            return result;
        }

        public void LoadData(Journey journey, bool loadHistory, IProgressCallback callback)
        {
            LoadData(new List<Journey> { journey }, loadHistory, callback);
        }

        public ValidateResult ValidateDatabase(IProgressCallback callback)
        {
            Logger.Info("Validate database");
            string error = null;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                error = String.Empty;
                using (var getFlightCmd = new SQLiteCommand(@"SELECT COUNT(*) FROM FLIGHT WHERE LJOURNEYID NOT IN (SELECT LID FROM JOURNEY)", connection))
                {
                    connection.Open();
                    object result = getFlightCmd.ExecuteScalar();

                    Int64 orphanCount;
                    if (result == null || result is DBNull)
                        orphanCount = 0;
                    else
                        orphanCount = (Int64)result;
                    if (orphanCount > 0)
                    {
                        error = String.Format("There are {0} orphan flights without any assosiated journey detail",
                                              orphanCount);
                    }
                }
            }

            var valResult = new ValidateResult(error == null, error);
            return valResult;
        }

        public void ResetData(IList<TravelRoute> data, IProgressCallback callback)
        {
            if (data == null || data.Count < 1)
                return;

            ProgressDialog.ExecuteTask(null, "Reset Database", "Please wait...", "SQLiteDbReset", ProgressBarStyle.Marquee, Logger, cb =>
                    {
                        cb.Begin();
                        Logger.Info("Clear database");
                        Reset(cb);
                        AddData(data, cb);
                    });
        }

        public void RepairDatabase(IProgressCallback callback)
        {
            try
            {
                TryOpenDatabase();
                Optimize();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to optimize database: " + ex);
                Reset(callback);
            }
        }

        private void Optimize()
        {
            Logger.Info("Optimize database");
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = String.Format("DELETE FROM JOURNEY WHERE TDEPARTURE = '{0}' OR TRETURN = '{0}'", DateTime.MinValue.ToString(DATE_FORMAT));
                    connection.Open();
                    command.ExecuteNonQuery();

                    command.CommandText = "VACUUM";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Reset(IProgressCallback callback)
        {
            ProgressDialog.ExecuteTask(null, "Initialize new database", "Please wait...", "SQLiteDbReset", ProgressBarStyle.Marquee, Logger, cb =>
                    {
                        cb.Begin();
                        DoResetDatabase(cb);
                    });
        }

        private void DoResetDatabase(IProgressCallback callback)
        {
            Logger.Info("Reset database");
            string sql;
            using (Stream dataStream = GetType().Assembly.GetManifestResourceStream(GetType().Namespace + "." + "DbSchema.sqlite"))
            {
                if (dataStream == null)
                    throw new ApplicationException("There is no embedded backup database script!");

                using (StreamReader sr = new StreamReader(dataStream))
                    sql = sr.ReadToEnd();
            }

            SQLiteConnection.ClearAllPools();
            SQLiteConnection.CreateFile(DataFileName);
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddData(IList<TravelRoute> data, IProgressCallback callback)
        {
            AddData(data, null, DateTime.MinValue, callback);
        }

        public void AddData(IList<TravelRoute> data, string packageId, DateTime packageCreatedAt, IProgressCallback callback)
        {
            ProgressDialog.ExecuteTask(null, "Data Import", "Please wait...", "SQLiteDbAddData", ProgressBarStyle.Continuous, Logger, cb =>
                    {
                        cb.Begin();
                        DoAddData(data, packageId, packageCreatedAt, cb as ProgressDialog);
                    });
        }

        public void AddData(IList<DataPackage<TravelRoute>> data, IProgressCallback callback)
        {
            if (data != null && data.Count > 0)
            {
                ProgressDialog.ExecuteTask(null, "Data Package Import", "Please wait...", "SQLiteDbAddDataPkg", ProgressBarStyle.Continuous, Logger, cb =>
                {
                    cb.Begin();
                    cb.SetRange(0, data.Count);
                    foreach (var pkg in data)
                    {
                        string log = "Saving package " + pkg.Id;
                        Logger.Info(log);
                        cb.Text = log;
                        AddData(pkg.Data, pkg.Id, pkg.CreatedDate, cb);
                        cb.Increment(1);
                    }
                });
            }
        }

        private void DoAddData(IList<TravelRoute> data, string packageId, DateTime packageCreatedAt, ProgressDialog callback)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                if (data != null && data.Count > 0)
                {
                    long totalData = 0;
                    Logger.InfoFormat("Add new {0} travel routes into database", data.Count);

                    var getIdRouteCmd =
                        new SQLiteCommand(
                            "SELECT LID FROM ROUTE WHERE SDEPARTURE=@sDept AND SDESTINATION=@sDest", connection);
                    getIdRouteCmd.Parameters.Add("@sDept", DbType.String);
                    getIdRouteCmd.Parameters.Add("@sDest", DbType.String);

                    var insertRouteCmd =
                        new SQLiteCommand(
                            "INSERT INTO ROUTE(LID, SDEPARTURE, SDESTINATION) " +
                            "VALUES(@lId, @sDept, @sDest)", connection);
                    insertRouteCmd.Parameters.Add("@lId", DbType.Int64);
                    insertRouteCmd.Parameters.Add("@sDept", DbType.String);
                    insertRouteCmd.Parameters.Add("@sDest", DbType.String);

                    var getIdJourneyCmd =
                        new SQLiteCommand(
                            "SELECT LID FROM JOURNEY WHERE LROUTEID=@lRouteId AND TDEPARTURE=@tDept AND TRETURN=@tRet", connection);
                    getIdJourneyCmd.Parameters.Add("@lRouteId", DbType.Int64);
                    getIdJourneyCmd.Parameters.Add("@tDept", DbType.String);
                    getIdJourneyCmd.Parameters.Add("@tRet", DbType.String);

                    var insertJourneyCmd =
                        new SQLiteCommand(
                            "INSERT INTO JOURNEY(LID, LROUTEID, TDEPARTURE, TRETURN) " +
                            "VALUES(@lId, @lRouteId, @tDept, @tRet)", connection);
                    insertJourneyCmd.Parameters.Add("@lId", DbType.Int64);
                    insertJourneyCmd.Parameters.Add("@lRouteId", DbType.Int64);
                    insertJourneyCmd.Parameters.Add("@tDept", DbType.String);
                    insertJourneyCmd.Parameters.Add("@tRet", DbType.String);

                    var getIdJourneyDataCmd =
                        new SQLiteCommand(
                            "SELECT LID FROM JOURNEY_DATA WHERE LJOURNEYID=@lJourneyId AND TUPDATE=@tUpdate", connection);
                    getIdJourneyDataCmd.Parameters.Add("@lJourneyId", DbType.Int64);
                    getIdJourneyDataCmd.Parameters.Add("@tUpdate", DbType.String);

                    var insertJourneyDataCmd =
                        new SQLiteCommand(
                            "INSERT INTO JOURNEY_DATA(LID, LJOURNEYID, SCURRENCY, TUPDATE, BFLIGHT) " +
                            "VALUES(@lId, @lJourneyId, @sCurrency, @tUpdate, @bFlight)", connection);
                    insertJourneyDataCmd.Parameters.Add("@lId", DbType.Int64);
                    insertJourneyDataCmd.Parameters.Add("@lJourneyId", DbType.Int64);
                    insertJourneyDataCmd.Parameters.Add("@sCurrency", DbType.String);
                    insertJourneyDataCmd.Parameters.Add("@tUpdate", DbType.String);
                    insertJourneyDataCmd.Parameters.Add("@bFlight", DbType.Binary);

                    var updateJourneyDataCmd =
                        new SQLiteCommand(
                            "UPDATE JOURNEY_DATA " +
                            "SET SCURRENCY = @sCurrency, BFLIGHT = @bFlight " +
                            "WHERE LID = @lId", connection);
                    updateJourneyDataCmd.Parameters.Add("@lId", DbType.Int64);
                    updateJourneyDataCmd.Parameters.Add("@sCurrency", DbType.String);
                    updateJourneyDataCmd.Parameters.Add("@bFlight", DbType.Binary);

                    connection.Open();
                    using (var startCmd = new SQLiteCommand("BEGIN TRANSACTION", connection))
                    {
                        startCmd.ExecuteNonQuery(); // Begin transaction
                    }

                    Int64 nextRouteId = GetMaxRowId(connection, "ROUTE"),
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
                                continue;

                            var deptAirport = AirportDataProvider.FromIATA(r.Departure.IATA);
                            if (deptAirport == null)
                                continue;

                            var destAirport = AirportDataProvider.FromIATA(r.Destination.IATA);
                            if (destAirport == null)
                                continue;

                            getIdRouteCmd.Parameters["@sDept"].Value = deptAirport.IATA;
                            getIdRouteCmd.Parameters["@sDest"].Value = destAirport.IATA;
                            var dbRouteId = getIdRouteCmd.ExecuteScalar();

                            bool isNewRoute = dbRouteId == null || dbRouteId is DBNull;
                            if (isNewRoute) // Create new route if it does not exist
                            {
                                insertRouteCmd.Parameters["@lId"].Value = ++nextRouteId;
                                insertRouteCmd.Parameters["@sDept"].Value = deptAirport.IATA;
                                insertRouteCmd.Parameters["@sDest"].Value = destAirport.IATA;
                                insertRouteCmd.ExecuteNonQuery();
                            }
                            else
                                nextRouteId = (long)dbRouteId; // Reuse existing route

                            callback.Text = "[" + deptAirport + "] - [" + destAirport + "]";

                            foreach (Journey j in r.Journeys)
                            {
                                if (callback.IsAborting)
                                    return;

                                if (j.Data.Count < 1)
                                    continue;

                                bool isNewJourney = true;
                                if (!isNewRoute)
                                {
                                    getIdJourneyCmd.Parameters["@lRouteId"].Value = nextRouteId;
                                    getIdJourneyCmd.Parameters["@tDept"].Value = j.DepartureDate.ToString(DATE_FORMAT);
                                    getIdJourneyCmd.Parameters["@tRet"].Value = j.ReturnDate.ToString(DATE_FORMAT);

                                    var dbJourneyId = getIdJourneyCmd.ExecuteScalar();
                                    isNewJourney = dbJourneyId == null || dbJourneyId is DBNull;
                                    if (!isNewJourney)
                                        j.Id = (long)dbJourneyId; // Reuse existing journey
                                }

                                if (isNewJourney) // Create new journey if it does not exist
                                {
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
                                        getIdJourneyDataCmd.Parameters["@tUpdate"].Value = journeyData.DataDate.ToUniversalTime().ToString(DATETIME_FORMAT, null);
                                        var dbDataId = getIdJourneyDataCmd.ExecuteScalar();
                                        isNewJourneyData = dbDataId == null || dbDataId is DBNull;
                                        if (!isNewJourneyData)
                                            journeyData.Id = (long)dbDataId;
                                    }

                                    if (isNewJourneyData) // Create new journey data history if it does not exist
                                    {
                                        journeyData.Id = ++nextDataId;
                                        insertJourneyDataCmd.Parameters["@lId"].Value = journeyData.Id;
                                        insertJourneyDataCmd.Parameters["@lJourneyId"].Value = j.Id;
                                        insertJourneyDataCmd.Parameters["@sCurrency"].Value = journeyData.Currency;
                                        insertJourneyDataCmd.Parameters["@tUpdate"].Value = journeyData.DataDate.ToUniversalTime().ToString(DATETIME_FORMAT, null);
                                        insertJourneyDataCmd.Parameters["@bFlight"].Value = _formatter.ToRaw(journeyData.Flights);
                                        insertJourneyDataCmd.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        // Update the journey data if it already exists
                                        updateJourneyDataCmd.Parameters["@lId"].Value = journeyData.Id;
                                        updateJourneyDataCmd.Parameters["@sCurrency"].Value = journeyData.Currency;
                                        updateJourneyDataCmd.Parameters["@bFlight"].Value = _formatter.ToRaw(journeyData.Flights);
                                        updateJourneyDataCmd.ExecuteNonQuery(); // Update existing journey data
                                    }
                                }

                                callback.Increment(1);
                                totalData += j.Data.Count;
                            }
                        }
                    }

                    // Process the data package id
                    if (!String.IsNullOrEmpty(packageId) && !IsPackageImported(packageId, connection, callback))
                    {
                        Logger.Info("Register package ID " + packageId);
                        // TODO: Check this
                        // callback.Style = ProgressStyleMarquee;
                        using (var insertPkgCmd = new SQLiteCommand("INSERT INTO DATA_PACKAGE (SID, TCREATED, TINSERTED) " +
                                                                    " VALUES (@pkgId, @pkgCreatedAt, @pkgInsertedAt)", connection))
                        {
                            insertPkgCmd.Parameters.Add(new SQLiteParameter("@pkgId", packageId));
                            insertPkgCmd.Parameters.Add(new SQLiteParameter("@pkgCreatedAt", packageCreatedAt.ToUniversalTime().ToString(DATE_FORMAT)));
                            insertPkgCmd.Parameters.Add(new SQLiteParameter("@pkgInsertedAt", DateTime.UtcNow.ToString(DATE_FORMAT)));

                            if (connection.State != ConnectionState.Open)
                                connection.Open();

                            insertPkgCmd.ExecuteNonQuery();
                        }
                    }

                    // Finally, commit the transaction
                    Logger.InfoFormat("Completed adding {0} journey data. Commiting", totalData);
                    // Completed inserting all journeys or inserted nothing
                    using (var endCmd = new SQLiteCommand("COMMIT TRANSACTION", connection))
                    {
                        endCmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void AddPackage(string packageId, IList<TravelRoute> data, IProgressCallback callback)
        {
            if (!IsPackageImported(packageId, callback))
                AddData(data, packageId, DateTime.Now, callback);
        }

        public bool IsPackageImported(string packageId, IProgressCallback callback)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                return IsPackageImported(packageId, connection, callback);
            }
        }

        private bool IsPackageImported(string packageId, SQLiteConnection connection, IProgressCallback callback)
        {
            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (var getPackageCountCmd = new SQLiteCommand(@"SELECT COUNT(*) FROM DATA_PACKAGE WHERE SID = '" + packageId + "'", connection))
            {
                object result = getPackageCountCmd.ExecuteScalar();
                if (!(result == null || result is DBNull))
                {
                    return ((Int64)result > 0);
                }
                return false;
            }
        }

        public IList<string> GetImportedPackages(IProgressCallback callback)
        {
            var result = new List<string>();
            using (var connection = new SQLiteConnection(_connectionString))
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

        public void TryOpenDatabase()
        {
            Logger.Info("Try opening database");
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var testCmd = new SQLiteCommand(@"SELECT COUNT(*) FROM sqlite_master WHERE type='table'", connection))
                {
                    connection.Open();
                    object result = testCmd.ExecuteScalar();
                }
            }
        }

        public string GetStatistics(IProgressCallback callback)
        {
            Logger.Info("Get database statistics");
            Initialize();
            Int64 route, journey, package;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var command = new SQLiteCommand(connection))
                {
                    connection.Open();
                    command.CommandText = "SELECT COUNT(*) FROM ROUTE";
                    route = (Int64)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM JOURNEY";
                    journey = (Int64)command.ExecuteScalar();
                    command.CommandText = "SELECT COUNT(*) FROM DATA_PACKAGE";
                    package = (Int64)command.ExecuteScalar();
                }
            }

            return String.Format(@"==============================
     SQLite FareDatabase
==============================
Data File: {0}
Size: {1}
Routes: {2}
Journeys: {3}
Packages: {4}", DataFileName, StringUtil.FormatSize(new FileInfo(DataFileName).Length), route, journey, package);
        }

        private static long GetPlaceId(SQLiteConnection connection, string placeName)
        {
            using (var getPlaceCmd = new SQLiteCommand("SELECT LID FROM PLACE WHERE SNAME=@sname", connection))
            {
                getPlaceCmd.Parameters.AddWithValue("@sname", placeName);

                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var result = getPlaceCmd.ExecuteScalar();
                if (result == null || result is DBNull)
                    return -1;

                return (long)result;
            }
        }

        private static Int64 GetMaxRowId(SQLiteConnection connection, string tableName)
        {
            using (var getRowIdCmd = new SQLiteCommand("SELECT MAX(_ROWID_) FROM " + tableName, connection))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                var result = getRowIdCmd.ExecuteScalar();
                if (result == null || result is DBNull)
                    return 0;

                return (Int64)result;
            }
        }
    }
}