using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Dte.Common.Converters;
using ScheduledJobs.Models;

namespace ScheduledJobs.Domain
{
    public class Participant
    {
        [DynamoDBHashKey("PK")] public string Pk { get; set; } // PARTICIPANT#1
        [DynamoDBRangeKey("SK")] public string Sk { get; set; } //PARTICIPANT#

        [DynamoDBProperty] public string ParticipantId { get; set; }
        [DynamoDBProperty] public string Email { get; set; }
        [DynamoDBProperty] public string Firstname { get; set; }
        [DynamoDBProperty] public string Lastname { get; set; }
        [DynamoDBProperty] public string NhsId { get; set; }
        [DynamoDBProperty] public string SessionId { get; set; }
        [DynamoDBProperty] public string NhsNumber { get; set; }
        [DynamoDBProperty] public bool ConsentRegistration { get; set; }
        [DynamoDBProperty] public DateTime? DateOfBirth { get; set; }
        [DynamoDBProperty] public string MfaChangePhoneCode { get; set; }
        [DynamoDBProperty] public DateTime? MfaChangePhoneCodeExpiry { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? ConsentRegistrationAtUtc { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? RemovalOfConsentRegistrationAtUtc { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? UpdatedAtUtc { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime? Stage2CompleteUtc { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))]
        public DateTime CreatedAtUtc { get; set; }

        [DynamoDBProperty] public string MobileNumber { get; set; }
        [DynamoDBProperty] public string LandlineNumber { get; set; }
        [DynamoDBProperty] public ParticipantAddressModel Address { get; set; }
        [DynamoDBProperty] public string SexRegisteredAtBirth { get; set; }
        [DynamoDBProperty] public bool? GenderIsSameAsSexRegisteredAtBirth { get; set; }
        [DynamoDBProperty] public string EthnicGroup { get; set; }
        [DynamoDBProperty] public string EthnicBackground { get; set; }
        [DynamoDBProperty] public bool? Disability { get; set; }
        [DynamoDBProperty] public string DisabilityDescription { get; set; }
        [DynamoDBProperty] public string SelectedLocale { get; set; }
        [DynamoDBProperty] public List<string> HealthConditionInterests { get; set; }

        public bool HasDemographics => SexRegisteredAtBirth != null;
    }
}
