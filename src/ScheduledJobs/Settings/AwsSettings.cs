using Dte.Common.Lambda.Settings;

namespace ScheduledJobs.Settings
{
    public class AwsSettings : SettingsBase
    {
        public override string SectionName => "AwsSettings";
        public string CpmsStudyDynamoDbTableName { get; set; }
        public string RtsDataDynamoDbTableName { get; set; }
        public string ServiceUrl { get; set; }
    }
}