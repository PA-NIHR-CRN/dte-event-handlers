using System;
using System.Collections.Generic;

namespace Domain.Aggregates.Entities
{
    public class Study : AuditableEntity
    {
        public Study(string id, string title, string shortName, DateTime submittedAt, DateTime? approvedAtUtc, string submissionResearcherId, List<string> researchers, StudyStatus studyStatus)
        {
            Id = id;
            Title = title;
            ShortName = shortName;
            SubmittedAt = submittedAt;
            ApprovedAtUtc = approvedAtUtc;
            SubmissionResearcherId = submissionResearcherId;
            Researchers = researchers;
            StudyStatus = studyStatus;
        }

        public string Id { get; }
        public string Title { get; }
        public string ShortName { get; }
        public DateTime SubmittedAt { get; }
        public DateTime? ApprovedAtUtc { get; }
        public string SubmissionResearcherId { get; }
        public List<string> Researchers { get; }
        public StudyStatus StudyStatus { get; }
    }
}