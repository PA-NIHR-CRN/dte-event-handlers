using Dte.Common.Lambda.Settings;

namespace CognitoCustomMessageProcessor.Settings
{
    public class ContentfulSettings : SettingsBase
    {
        public static readonly string ConfigSectionName = "ContentfulSettings";
        public override string SectionName => ConfigSectionName;
        public string DeliveryApiKey { get; set; }
        public string PreviewApiKey { get; set; }
        public string SpaceId { get; set; }
        public bool UsePreviewApi { get; set; }
        public string BaseUrl { get; set; }
        public EmailTemplates EmailTemplates { get; set; }
    }
    public class EmailTemplates
    {
        public string ForgotPassword { get; set; }
        public string ResendCode { get; set; }
        public string SignUp { get; set; }
        public string UpdateUserAttribute { get; set; }
    }
}