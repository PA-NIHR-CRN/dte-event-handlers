namespace Common.Settings
{
    public class EventStoreSettings 
    {
        public static string SectionName => "EventStoreSettings";
        public string ProcessorLink { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}