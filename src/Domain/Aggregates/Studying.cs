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
            RegisterTransition<StudyApprovedV1>(Apply);
            RegisterTransition<StudyRejectedV1>(Apply);
        }
        private void Apply(StudyWaitingForApprovalSubmittedV1 @event)
        {
            CorrelationId = @event.Metadata["$correlationId"];
            _study = new Study(@event.StudyId, @event.Title, @event.ShortName, DateTime.Parse(@event.Metadata["$applies"]).ToUniversalTime(), null, @event.ResearcherId, null, StudyRegistrationStatus.WaitingForApproval);
        }
        private void Apply(StudyApprovedV1 @event)
        {
            CorrelationId = @event.Metadata["$correlationId"];
            _study.StudyRegistrationStatus = StudyRegistrationStatus.Approved;
        }
        
        private void Apply(StudyRejectedV1 @event)
        {
            CorrelationId = @event.Metadata["$correlationId"];
            _study.StudyRegistrationStatus = StudyRegistrationStatus.Rejected;
        }
        public static Studying Create()
        {
            return new Studying();
        }

        public void SubmitForApproval(SubmitStudyForApproval cmd)
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

            RaiseEvent(new StudyWaitingForApprovalSubmittedV1(cmd.StudyId, cmd.Title, cmd.ShortName, cmd.ResearcherId, cmd.Metadata));
        }

        public void ApproveStudy(ApproveStudyCommand cmd)
        {
            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.IsPositiveLong(cmd.StudyId, nameof(cmd.StudyId));
            
            RaiseEvent(new StudyApprovedV1(cmd.StudyId, cmd.Metadata));
        }

        public void RejectStudy(RejectStudyCommand cmd)
        {
            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.IsPositiveLong(cmd.StudyId, nameof(cmd.StudyId));
            
            RaiseEvent(new StudyRejectedV1(cmd.StudyId, cmd.Metadata));
        }
    }
}