using System.Collections.Generic;
using Domain.Aggregates.Entities;
using Evento;

namespace Domain.Events
{
    public class StudyWaitingForApprovalSubmittedV1 : Event
    {
        public StudyWaitingForApprovalSubmittedV1(string studyId,
            string title,
            string shortName,
            string researcherFirstname,
            string researcherLastname,
            string researcherEmail,
            StudyRegistrationStatus registrationStatus,
            IDictionary<string, string> metadata)
        {
            StudyId = studyId;
            Title = title;
            ShortName = shortName;
            ResearcherFirstname = researcherFirstname;
            ResearcherLastname = researcherLastname;
            ResearcherEmail = researcherEmail;
            RegistrationStatus = registrationStatus;
            Metadata = metadata;
        }

        public string StudyId { get; }
        public string Title { get; }
        public string ShortName { get; }
        public string ResearcherFirstname { get; set; }
        public string ResearcherLastname { get; set; }
        public string ResearcherEmail { get; set; }
        public StudyRegistrationStatus RegistrationStatus { get; set; }
        public IDictionary<string, string> Metadata { get; }
    }
}