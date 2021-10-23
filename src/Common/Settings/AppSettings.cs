using System;

namespace Common.Settings
{
    public class AppSettings
    {
        public static string SectionName => "AppSettings";
        public bool RunInParallel { get; set; }
        public string CryptoKey { get; set; }
    }
}