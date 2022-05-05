using System;

namespace ScheduledJobs.Models
{
    public class ParticipantOdpExportModel
    {
        // Details
        public string ParticipantId { get; set; }
        public bool ConsentRegistration { get; set; }
        public DateTime? ConsentRegistrationAtUtc { get; set; }

        // Demographics
        public string MobileNumber { get; set; }
        public string LandlineNumber { get; set; }
        public string Postcode { get; set; }
        public string SexRegisteredAtBirth { get; set; }
        public bool? GenderIsSameAsSexRegisteredAtBirth { get; set; }
        public string EthnicGroup { get; set; }
        public string EthnicBackground { get; set; }
        public bool? Disability { get; set; }
        public string DisabilityDescription { get; set; }
        public string HealthConditionInterests { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}