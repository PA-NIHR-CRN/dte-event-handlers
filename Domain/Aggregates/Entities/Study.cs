using System;
using System.Collections.Generic;

namespace Domain.Aggregates.Entities
{
    public class Study
    {
        public Study(string id, string title, DateTime submittedAt, DateTime? approvedAt, string submissionResearcherId, List<string> researchers, StudyStatus studyStatus)
        {
            Id = id;
            Title = title;
            SubmittedAt = submittedAt;
            ApprovedAt = approvedAt;
            SubmissionResearcherId = submissionResearcherId;
            Researchers = researchers;
            StudyStatus = studyStatus;
        }

        public string Id { get; }
        public string Title { get; }
        public DateTime SubmittedAt { get; }
        public DateTime? ApprovedAt { get; }
        public string SubmissionResearcherId { get; }
        public List<string> Researchers { get; }
        public StudyStatus StudyStatus { get; }
    }

    public enum StudyStatus
    {
        Open,
        Closed,
        WaitingForApproval
    }
}