using System.Collections.Generic;
using Evento;

namespace Domain.Events
{
    public class InterestExpressedV1 : Event
    {
        public InterestExpressedV1(string studyId, string siteId, string userId, IDictionary<string, string> metadata)
        {
            StudyId = studyId;
            SiteId = siteId;
            UserId = userId;
            Metadata = metadata;
        }
        public string StudyId { get;  set; }
        public string SiteId { get;  set; }
        public string UserId { get;  set; }
        public IDictionary<string, string> Metadata { get; }
    }
}