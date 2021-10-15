using System.Collections.Generic;
using Evento;

namespace Domain.Commands
{
    public class SubmitStudyForApproval : Command
    {
        public string CorrelationId { get; set; }
        public string StudyId { get; set; }
        public string Title { get; set;}
        public string ShortName { get; set;}
        public string ResearcherFirstname { get; set; }
        public string ResearcherLastname { get; set; }
        public string ResearcherEmail { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}