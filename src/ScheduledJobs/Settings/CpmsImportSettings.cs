using Dte.Common.Lambda.Settings;

namespace ScheduledJobs.Settings
{
    public class CpmsImportSettings : SettingsBase
    {
        public override string SectionName => "CpmsImportSettings";
        public string ArchiveS3BucketName { get; set; }
        public string S3BucketName { get; set; }
        public string GetRecordBatchSize { get; set; }
    }
}