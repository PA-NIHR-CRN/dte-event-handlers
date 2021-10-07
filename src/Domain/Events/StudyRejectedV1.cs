using System.Collections.Generic;
using Evento;

namespace Domain.Events
{
    public class StudyRejectedV1 : Event
    {
        public StudyRejectedV1(string studyId, IDictionary<string, string> metadata)
        {
            StudyId = studyId;
            Metadata = metadata;
        }
        
        public string StudyId { get; }
        public IDictionary<string, string> Metadata { get; }
    }
}