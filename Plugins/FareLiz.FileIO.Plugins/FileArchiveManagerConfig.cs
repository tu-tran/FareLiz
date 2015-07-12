using System;
using System.ComponentModel;
using SkyDean.FareLiz.Core;
using SkyDean.FareLiz.Core.Config;

namespace SkyDean.FareLiz.IO
{
    /// <summary>
    /// Configuration object for FileArchiveManager
    /// </summary>
    [Serializable]
    public class FileArchiveManagerConfig : IConfig
    {
        [DisplayName("Archive Path")]
        [Description("The location for storing archived data")]
        public string ArchivePath { get; set; }

        [DisplayName("Processing Batch Size")]
        [Description("The size of each processing batch (e.g. number of entries to be processed at once)")]
        public int ProcessBatchSize { get; set; }

        public ValidateResult Validate()
        {
            if (this.ProcessBatchSize < 1)
                this.ProcessBatchSize = 500;
            return ValidateResult.Success;
        }
    }
}
