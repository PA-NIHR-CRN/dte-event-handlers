using Dte.Common.Lambda.Settings;

namespace ScheduledJobs.Settings
{
    public class AwsSettings : SettingsBase
    {
        public override string SectionName => "AwsSettings";
        public string ParticipantRegistrationDynamoDbTableName { get; set; }
        public string ServiceUrl { get; set; }
    }
}
