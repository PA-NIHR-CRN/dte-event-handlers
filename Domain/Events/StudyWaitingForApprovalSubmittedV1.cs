using System.Collections.Generic;
using Evento;

namespace Domain.Events
{
    public class StudyWaitingForApprovalSubmittedV1 : Event
    {
        public StudyWaitingForApprovalSubmittedV1(string studyId, string title, string researcherId, IDictionary<string, string> metadata)
        {
            StudyId = studyId;
            Title = title;
            ResearcherId = researcherId;
            Metadata = metadata;
        }

        public string StudyId { get; }
        public string Title { get; }
        public string ResearcherId { get; }
        public IDictionary<string, string> Metadata { get; }
    }
}