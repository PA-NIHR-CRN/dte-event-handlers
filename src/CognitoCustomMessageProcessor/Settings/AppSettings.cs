using Dte.Common.Lambda.Settings;

namespace CognitoCustomMessageProcessor.Settings
{
    public class AppSettings : SettingsBase
    {
        public override string SectionName => "AppSettings";
        public string DteWebBaseUrl { get; set; }
    }
}