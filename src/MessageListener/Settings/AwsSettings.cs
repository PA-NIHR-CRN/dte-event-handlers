using Dte.Common.Lambda.Settings;

namespace MessageListener.Settings
{
    public class AwsSettings : SettingsBase
    {
        public override string SectionName => "AwsSettings";
        public string ServiceUrl { get; set; }
    }
}