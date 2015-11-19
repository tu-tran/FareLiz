namespace SkyDean.FareLiz.SQLite
{
    using System.ComponentModel;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.DropBox;

    /// <summary>
    /// The sq lite drop box syncer.
    /// </summary>
    [DisplayName("SQLite DropBox Sync")]
    [Description("Sync SQLite database with DropBox")]
    public class SQLiteDropBoxSyncer : DropBoxDbSync<SQLiteFareDatabase>
    {
        /// <summary>
        /// The initialize.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            this.OnValidateData += this.SQLiteDropBoxSyncer_OnValidateData;
        }

        /// <summary>
        /// The sq lite drop box syncer_ on validate data.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void SQLiteDropBoxSyncer_OnValidateData(SQLiteFareDatabase sender, SyncEventArgs<SQLiteFareDatabase> e)
        {
            sender.TryOpenDatabase();
        }
    }
}