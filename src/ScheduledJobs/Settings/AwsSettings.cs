using Application.Settings;

namespace ScheduledJobs.Settings
{
    public class AwsSettings : SettingsBase
    {
        public override string SectionName => "AwsSettings";
        public string ServiceUrl { get; set; }
    }
}