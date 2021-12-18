using Dte.Common.Lambda.Settings;

namespace MessageListener.Settings
{
    public class AppSettings : SettingsBase
    {
        public override string SectionName => "AppSettings";
        public bool RunInParallel { get; set; }
        public string CryptoKey { get; set; }
    }
}