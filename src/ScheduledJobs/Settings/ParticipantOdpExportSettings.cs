using Dte.Common.Lambda.Settings;

namespace ScheduledJobs.Settings
{
    public class ParticipantOdpExportSettings : SettingsBase
    {
        public override string SectionName => "ParticipantOdpExportSettings";
        public string S3BucketName { get; set; }
    }
}