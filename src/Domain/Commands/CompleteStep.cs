using System.Collections.Generic;
using Evento;

namespace Domain.Commands
{
    public class CompleteStep : Command
    {
        public CompleteStep(string studyId, string stepId, string question, string answer)
        {
            StudyId = studyId;
            StepId = stepId;
            Question = question;
            Answer = answer;
        }

        public string StudyId { get; }
        public string StepId { get; }
        public string Question { get; }
        public string Answer { get; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}