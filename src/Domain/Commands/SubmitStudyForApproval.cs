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
        public string ResearcherId { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}