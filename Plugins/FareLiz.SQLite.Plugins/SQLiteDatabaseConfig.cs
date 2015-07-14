namespace SkyDean.FareLiz.SQLite
{
    using System;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;

    /// <summary>Configuration object for SQLite database</summary>
    [Serializable]
    public class SQLiteDatabaseConfig : IConfig
    {
        /// <summary>Initializes a new instance of the <see cref="SQLiteDatabaseConfig" /> class.</summary>
        public SQLiteDatabaseConfig()
        {
            this.DataFileName = "Data.tudb";
        }

        /// <summary>Gets or sets the data file name.</summary>
        [DisplayName("SQLite Data File")]
        [Description("Path to the main file used for storing SQLite data")]
        public string DataFileName { get; set; }

        /// <summary>The validate.</summary>
        /// <returns>The <see cref="ValidateResult" />.</returns>
        public ValidateResult Validate()
        {
            var error = string.IsNullOrEmpty(this.DataFileName) ? "Data file name cannot be empty" : null;
            var result = new ValidateResult(error == null, error);
            return result;
        }
    }
}