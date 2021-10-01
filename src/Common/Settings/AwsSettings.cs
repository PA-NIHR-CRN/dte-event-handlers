namespace Common.Settings
{
    public class AwsSettings
    {
        public static string SectionName => "AwsSettings";
        public string StudyDynamoDbTableName { get; set; }
        public string StudyRegistrationDynamoDbTableName { get; set; }
    }
}