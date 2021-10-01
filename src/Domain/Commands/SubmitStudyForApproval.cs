using System.Collections.Generic;
using Evento;

namespace Domain.Commands
{
    public class SubmitStudyForApproval : Command
    {
        public SubmitStudyForApproval()
        {
            
        }
        
        public SubmitStudyForApproval(string studyId, string title, string shortName, string researcherId, IDictionary<string, string> metadata)
        {
            StudyId = studyId;
            Title = title;
            ShortName = shortName;
            ResearcherId = researcherId;
            Metadata = metadata;
        }
        public string StudyId { get; set; }
        public string Title { get;  set;}
        public string ShortName { get;  set;}
        public string ResearcherId { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}