using Application.Settings;

namespace ScheduledJobs.Settings
{
    public class CpmsImportSettings : SettingsBase
    {
        public override string SectionName => "CpmsImportSettings";
        public string ExportS3BucketName { get; set; }
        public string ExportS3FileName { get; set; }
    }
}