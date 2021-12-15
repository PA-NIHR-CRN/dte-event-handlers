using Application.Settings;

namespace ScheduledJobs.Settings
{
    public class AwsSettings : SettingsBase
    {
        public override string SectionName => "AwsSettings";
        public string CpmsStudyDynamoDbTableName { get; set; }
        public string ServiceUrl { get; set; }
    }
}