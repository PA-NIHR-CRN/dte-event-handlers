using System.Collections.Generic;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace ScheduledJobs.Mappers
{
    public static class ParticipantMapper
    {
        public static ParticipantExportModel MapToParticipantExportModel(Participant source)
        {
            return new ParticipantExportModel
            {
                ParticipantId = source.ParticipantId,
                Email = source.Email,
                Firstname = source.Firstname,
                Lastname = source.Lastname,
                ConsentRegistration = source.ConsentRegistration,
                ConsentRegistrationAtUtc = source.ConsentRegistrationAtUtc,
                RemovalOfConsentRegistrationAtUtc = source.RemovalOfConsentRegistrationAtUtc,
                MobileNumber = source.MobileNumber,
                LandlineNumber = source.LandlineNumber,
                Address = source.Address,
                DateOfBirth = source.DateOfBirth,
                SexRegisteredAtBirth = source.SexRegisteredAtBirth,
                GenderIsSameAsSexRegisteredAtBirth = source.GenderIsSameAsSexRegisteredAtBirth,
                EthnicGroup = source.EthnicGroup,
                EthnicBackground = source.EthnicBackground,
                Disability = source.Disability,
                DisabilityDescription = source.DisabilityDescription,
                HealthConditionInterests = string.Join(", ", source.HealthConditionInterests ?? new List<string>()),
                CreatedAtUtc = source.CreatedAtUtc,
                UpdatedAtUtc = source.UpdatedAtUtc
            };
        }
        
        public static ParticipantOdpExportModel MapToParticipantOdpExportModel(Participant source)
        {
            return new ParticipantOdpExportModel
            {
                ParticipantId = source.ParticipantId,
                ConsentRegistration = source.ConsentRegistration,
                ConsentRegistrationAtUtc = source.ConsentRegistrationAtUtc,
                RemovalOfConsentRegistrationAtUtc = source.RemovalOfConsentRegistrationAtUtc,
                MobileNumber = source.MobileNumber,
                LandlineNumber = source.LandlineNumber,
                Postcode = source.Address?.Postcode,
                SexRegisteredAtBirth = source.SexRegisteredAtBirth,
                GenderIsSameAsSexRegisteredAtBirth = source.GenderIsSameAsSexRegisteredAtBirth,
                EthnicGroup = source.EthnicGroup,
                EthnicBackground = source.EthnicBackground,
                Disability = source.Disability,
                DisabilityDescription = source.DisabilityDescription,
                HealthConditionInterests = string.Join(", ", source.HealthConditionInterests ?? new List<string>()),
                CreatedAtUtc = source.CreatedAtUtc,
                UpdatedAtUtc = source.UpdatedAtUtc
            };
        }
    }
}