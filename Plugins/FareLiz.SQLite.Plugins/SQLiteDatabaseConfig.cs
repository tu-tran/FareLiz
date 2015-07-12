using System;
using System.ComponentModel;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;

namespace SkyDean.FareLiz.SQLite
{
    /// <summary>
    /// Configuration object for SQLite database
    /// </summary>
    [Serializable]
    public class SQLiteDatabaseConfig : IConfig
    {
        [DisplayName("SQLite Data File")]
        [Description("Path to the main file used for storing SQLite data")]
        public string DataFileName { get; set; }

        public SQLiteDatabaseConfig()
        {
            DataFileName = "Data.tudb";
        }

        public ValidateResult Validate()
        {
            string error = (String.IsNullOrEmpty(DataFileName) ? "Data file name cannot be empty" : null);
            var result = new ValidateResult(error == null, error);
            return result;
        }
    }
}
