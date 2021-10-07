using System.Collections.Generic;
using Evento;

namespace Domain.Commands
{
    public class ApproveStudyCommand : Command
    {
        public string CorrelationId { get; set; }
        public string StudyId { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}