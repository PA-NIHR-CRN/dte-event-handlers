using System.Collections.Generic;
using Evento;

namespace Domain.Events
{
    public class StudyApprovedV1 : Event
    {
        public StudyApprovedV1(string studyId, IDictionary<string, string> metadata)
        {
            StudyId = studyId;
            Metadata = metadata;
        }
        
        public string StudyId { get; }
        public IDictionary<string, string> Metadata { get; }
    }
}