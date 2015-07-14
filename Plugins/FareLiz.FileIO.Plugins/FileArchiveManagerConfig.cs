namespace SkyDean.FareLiz.IO
{
    using System;
    using System.ComponentModel;

    using SkyDean.FareLiz.Core;
    using SkyDean.FareLiz.Core.Config;

    /// <summary>Configuration object for FileArchiveManager</summary>
    [Serializable]
    public class FileArchiveManagerConfig : IConfig
    {
        /// <summary>Gets or sets the archive path.</summary>
        [DisplayName("Archive Path")]
        [Description("The location for storing archived data")]
        public string ArchivePath { get; set; }

        /// <summary>Gets or sets the process batch size.</summary>
        [DisplayName("Processing Batch Size")]
        [Description("The size of each processing batch (e.g. number of entries to be processed at once)")]
        public int ProcessBatchSize { get; set; }

        /// <summary>The validate.</summary>
        /// <returns>The <see cref="ValidateResult" />.</returns>
        public ValidateResult Validate()
        {
            if (this.ProcessBatchSize < 1)
            {
                this.ProcessBatchSize = 500;
            }

            return ValidateResult.Success;
        }
    }
}