using Application.Settings;

namespace ScheduledJobs.Settings
{
    public class CpmsImportSettings : SettingsBase
    {
        public override string SectionName => "CpmsImportSettings";
        public string ArchiveS3BucketName { get; set; }
        public string S3BucketName { get; set; }
        public int GetRecordBatchSize { get; set; }
    }
}