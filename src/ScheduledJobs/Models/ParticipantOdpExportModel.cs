using System;

namespace ScheduledJobs.Models
{
    public class ParticipantOdpExportModel
    {
        public string Pk { get; set; }
        // Details
        public string ParticipantId { get; set; }
        public bool ConsentRegistration { get; set; }
        public DateTime? ConsentRegistrationAtUtc { get; set; }
        public DateTime? RemovalOfConsentRegistrationAtUtc { get; set; }

        // Demographics
        public DateTime? DateOfBirth { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
        public string SexRegisteredAtBirth { get; set; }
        public string EthnicGroup { get; set; }
        public bool? Disability { get; set; }
        public string HealthConditionInterests { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
