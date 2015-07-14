namespace SkyDean.FareLiz.Core.Data
{
    /// <summary>The data options.</summary>
    public class DataOptions
    {
        /// <summary>The default.</summary>
        public static readonly DataOptions Default = new DataOptions { ShowProgressDialog = true };

        /// <summary>Gets or sets a value indicating whether show progress dialog.</summary>
        public bool ShowProgressDialog { get; set; }

        /// <summary>Gets or sets a value indicating whether archive data files.</summary>
        public bool ArchiveDataFiles { get; set; }
    }
}