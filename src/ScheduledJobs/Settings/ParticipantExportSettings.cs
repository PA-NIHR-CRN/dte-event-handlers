using Dte.Common.Lambda.Settings;

namespace ScheduledJobs.Settings
{
    public class ParticipantExportSettings : SettingsBase
    {
        public override string SectionName => "ParticipantExportSettings";
        public string S3BucketName { get; set; }
    }
}