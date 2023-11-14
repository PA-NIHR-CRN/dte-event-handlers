using System.Collections.Generic;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace ScheduledJobs.Mappers
{
    public static class ParticipantMapper
    {
        private static string GetOutcodeFromPostcode(string postcode, string sk)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                return "";
            }

            var postcodeWithoutSpace = postcode.Replace(" ", "");

            return sk == "DELETED#" || postcodeWithoutSpace.Length < 3
                ? postcodeWithoutSpace
                : postcodeWithoutSpace[..^3];
        }

        public static ParticipantExportModel MapToParticipantExportModel(Participant source)
        {
            return new ParticipantExportModel
            {
                Pk = source.Pk,
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
                Pk = source.Pk,
                ParticipantId = source.ParticipantId,
                ConsentRegistration = source.ConsentRegistration,
                ConsentRegistrationAtUtc = source.ConsentRegistrationAtUtc,
                RemovalOfConsentRegistrationAtUtc = source.RemovalOfConsentRegistrationAtUtc,
                DateOfBirth = source.DateOfBirth,
                Postcode = GetOutcodeFromPostcode(source.Address?.Postcode, source.Sk),
                Town = source.Address?.Town,
                SexRegisteredAtBirth = source.SexRegisteredAtBirth,
                EthnicGroup = source.EthnicGroup,
                Disability = source.Disability,
                HealthConditionInterests = string.Join(", ", source.HealthConditionInterests ?? new List<string>()),
                CreatedAtUtc = source.CreatedAtUtc,
                UpdatedAtUtc = source.UpdatedAtUtc
            };
        }
    }
}
