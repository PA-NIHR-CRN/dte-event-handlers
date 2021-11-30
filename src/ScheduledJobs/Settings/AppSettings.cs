using Application.Settings;

namespace ScheduledJobs.Settings
{
    public class AppSettings : SettingsBase
    {
        public override string SectionName => "AppSettings";
        public string CryptoKey { get; set; }
    }
}