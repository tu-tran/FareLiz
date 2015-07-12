namespace SkyDean.FareLiz.Core.Data
{
    public class DataOptions
    {
        public bool ShowProgressDialog { get; set; }
        public bool ArchiveDataFiles { get; set; }

        public static readonly DataOptions Default = new DataOptions
        {
            ShowProgressDialog = true,
        };
    }
}
