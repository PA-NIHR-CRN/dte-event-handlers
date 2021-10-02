using System;
using Amazon.DynamoDBv2.DataModel;
using Domain.Aggregates.Entities;

namespace Domain.Persistence.Models
{
    public class StudyRegistration
    {
        [DynamoDBHashKey("PK")] public string Pk { get; set; }

        [DynamoDBProperty] public long StudyId { get; set; }
        [DynamoDBProperty] public string Title { get; set; }
        [DynamoDBProperty] public string ShortName{ get; set; }
        [DynamoDBProperty] public string SubmissionResearcherId { get; set;}
        [DynamoDBProperty] public StudyRegistrationStatus StudyRegistrationStatus { get; set; }
        [DynamoDBProperty] public DateTime? ApprovedAtUtc { get; set; }
        [DynamoDBProperty] public DateTime SubmittedAt { get; set; }
    }
}