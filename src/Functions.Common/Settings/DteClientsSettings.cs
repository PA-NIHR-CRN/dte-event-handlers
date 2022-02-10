using Dte.Common.Authentication;
using Dte.Common.Lambda.Settings;

namespace Functions.Common.Settings
{
    public class DteClientsSettings : SettingsBase
    {
        public override string SectionName => "Clients";
        public ClientSettings StudyService { get; set; }
        public ClientSettings StudyManagementService { get; set; }
        public ClientSettings ParticipantService { get; set; }
        public ClientSettings LocationService { get; set; }
        public ClientSettings ReferenceDataService { get; set; }
        public ClientSettings RtsService { get; set; }
    }
}
