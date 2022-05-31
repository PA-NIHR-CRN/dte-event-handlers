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

        // Details
        [DynamoDBProperty] public string ParticipantId { get; set; }
        [DynamoDBProperty] public string Email { get; set; }
        [DynamoDBProperty] public string Firstname { get; set; }
        [DynamoDBProperty] public string Lastname { get; set; }
        [DynamoDBProperty] public bool ConsentRegistration { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))] public DateTime? ConsentRegistrationAtUtc { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))] public DateTime? RemovalOfConsentRegistrationAtUtc { get; set; }

        // Demographics
        [DynamoDBProperty] public string MobileNumber { get; set; }
        [DynamoDBProperty] public string LandlineNumber { get; set; }
        [DynamoDBProperty] public ParticipantAddressModel Address { get; set; }
        [DynamoDBProperty] public DateTime? DateOfBirth { get; set; }
        [DynamoDBProperty] public string SexRegisteredAtBirth { get; set; }
        [DynamoDBProperty] public bool? GenderIsSameAsSexRegisteredAtBirth { get; set; }
        [DynamoDBProperty] public string EthnicGroup { get; set; }
        [DynamoDBProperty] public string EthnicBackground { get; set; }
        [DynamoDBProperty] public bool? Disability { get; set; }
        [DynamoDBProperty] public string DisabilityDescription { get; set; }
        [DynamoDBProperty] public List<string> HealthConditionInterests { get; set; }

        [DynamoDBProperty(typeof(DateTimeUtcConverter))] public DateTime CreatedAtUtc { get; set; }
        [DynamoDBProperty(typeof(DateTimeUtcConverter))] public DateTime? UpdatedAtUtc { get; set; }
    }
}