using System;
using System.Threading.Tasks;
using Domain.Aggregates.Entities;
using Domain.Commands;
using Domain.Contracts;
using Domain.Events;
using Evento;

namespace Domain.Aggregates
{
    public class Studying : AggregateBase
    {
        public override string AggregateId => CorrelationId;
        private string CorrelationId { get; set; }
        private Study _study;

        public Studying()
        {
            RegisterTransition<StudyWaitingForApprovalSubmittedV1>(Apply);
        }

        private void Apply(StudyWaitingForApprovalSubmittedV1 obj)
        {
            CorrelationId = obj.Metadata["$correlationId"];
            _study = new Study(obj.StudyId, obj.Title, obj.ShortName, DateTime.Parse(obj.Metadata["$applies"]).ToUniversalTime(), null, obj.ResearcherId, null, StudyRegistrationStatus.WaitingForApproval);
        }

        public static Studying Create()
        {
            return new Studying();
        }

        public async Task SubmitForApproval(SubmitStudyForApproval cmd, IStudyService studyService)
        {
            // Idempotency
            if (_study != null)
            {
                return;
            }
            
            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.IsPositiveLong(cmd.StudyId, nameof(cmd.StudyId));
            Ensure.NotNullOrWhiteSpace(cmd.Title, nameof(cmd.Title));
            Ensure.NotNullOrWhiteSpace(cmd.ShortName, nameof(cmd.ShortName));
            Ensure.NotNullOrWhiteSpace(cmd.ResearcherId, nameof(cmd.ResearcherId));

            var study = new Study(cmd.StudyId, cmd.Title, cmd.ShortName, DateTime.Parse(cmd.Metadata["$applies"]), null, cmd.ResearcherId, null, StudyRegistrationStatus.WaitingForApproval);
            
            await studyService.SaveStudyRegistration(study);
            
            RaiseEvent(new StudyWaitingForApprovalSubmittedV1(study.Id, study.Title, study.ShortName, study.SubmissionResearcherId, cmd.Metadata));
        }
    }
}