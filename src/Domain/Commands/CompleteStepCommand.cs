using System.Collections.Generic;
using Evento;

namespace Domain.Commands
{
    public class CompleteStepCommand : Command
    {
        public CompleteStepCommand(string studyId, string stepId, string question, string answer)
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