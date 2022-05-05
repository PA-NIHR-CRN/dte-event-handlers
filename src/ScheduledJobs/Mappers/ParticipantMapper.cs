using System.Collections.Generic;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace ScheduledJobs.Mappers
{
    public static class ParticipantMapper
    {
        public static ParticipantExportModel MapTo(Participant source)
        {
            return new ParticipantExportModel
            {
                ParticipantId = source.ParticipantId,
                Email = source.Email,
                Firstname = source.Firstname,
                Lastname = source.Lastname,
                ConsentRegistration = source.ConsentRegistration,
                ConsentRegistrationAtUtc = source.ConsentRegistrationAtUtc,
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
    }
}