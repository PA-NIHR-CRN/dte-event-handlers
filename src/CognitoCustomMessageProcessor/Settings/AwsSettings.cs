using Dte.Common.Lambda.Settings;

namespace CognitoCustomMessageProcessor.Settings
{
    public class AwsSettings : SettingsBase
    {
        public override string SectionName => "AwsSettings";
        public string ServiceUrl { get; set; }
        public string ParticipantRegistrationDynamoDbTableName { get; set; }
    }
}