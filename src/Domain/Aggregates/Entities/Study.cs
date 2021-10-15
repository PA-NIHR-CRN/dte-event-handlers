using System;
using System.Collections.Generic;

namespace Domain.Aggregates.Entities
{
    public class Study
    {
        public Study(string id,
            string title,
            string shortName,
            DateTime submittedAt,
            DateTime? approvedAtUtc,
            Researcher submissionResearcher,
            List<string> researchers,
            StudyRegistrationStatus studyRegistrationStatus)
        {
            Id = id;
            Title = title;
            ShortName = shortName;
            SubmittedAt = submittedAt;
            ApprovedAtUtc = approvedAtUtc;
            SubmissionResearcher = submissionResearcher;
            Researchers = researchers;
            StudyRegistrationStatus = studyRegistrationStatus;
        }

        public string Id { get; }
        public string Title { get; }
        public string ShortName { get; }
        public DateTime SubmittedAt { get; }
        public DateTime? ApprovedAtUtc { get; }
        public Researcher SubmissionResearcher { get; }
        public List<string> Researchers { get; }
        public StudyRegistrationStatus StudyRegistrationStatus { get; set; }
    }
}