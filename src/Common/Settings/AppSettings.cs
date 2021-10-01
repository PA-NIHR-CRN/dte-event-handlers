namespace Common.Settings
{
    public class AppSettings
    {
        public static string SectionName => "AppSettings";
        public string CertificateFqdn { get; set; }
        public string CryptoKey { get; set; }
    }
}