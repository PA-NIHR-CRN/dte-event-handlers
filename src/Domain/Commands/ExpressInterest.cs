using System.Collections.Generic;
using Evento;

namespace Domain.Commands
{
    public class ExpressInterest : Command
    {
        public ExpressInterest()
        {
            
        }
        
        public ExpressInterest(string correlationId, string studyId, string siteId, string userId, IDictionary<string, string> metadata)
        {
            CorrelationId = correlationId;
            StudyId = studyId;
            SiteId = siteId;
            UserId = userId;
            Metadata = metadata;
        }

        public string CorrelationId { get; set; }
        public string StudyId { get;  set; }
        public string SiteId { get;  set; }
        public string UserId { get;  set; }
        public IDictionary<string, string> Metadata { get;  set; }
    }
}