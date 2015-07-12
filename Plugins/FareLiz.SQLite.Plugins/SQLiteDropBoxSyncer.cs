using System.ComponentModel;
using SkyDean.FareLiz.DropBox;

namespace SkyDean.FareLiz.SQLite
{
    [DisplayName("SQLite DropBox Sync")]
    [Description("Sync SQLite database with DropBox")]
    public class SQLiteDropBoxSyncer : DropBoxDbSync<SQLiteFareDatabase>
    {
        public override void Initialize()
        {
            base.Initialize();
            OnValidateData += SQLiteDropBoxSyncer_OnValidateData;
        }

        void SQLiteDropBoxSyncer_OnValidateData(SQLiteFareDatabase sender, Core.SyncEventArgs<SQLiteFareDatabase> e)
        {
            sender.TryOpenDatabase();
        }
    }
}
