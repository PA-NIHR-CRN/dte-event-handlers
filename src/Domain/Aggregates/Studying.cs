using System;
using Domain.Aggregates.Entities;
using Domain.Commands;
using Domain.Events;
using Evento;
using Researcher = Domain.Aggregates.Entities.Researcher;

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
        
        private void Apply(StudyWaitingForApprovalSubmittedV1 evt)
        {
            CorrelationId = evt.Metadata["$correlationId"];
            _study = new Study(evt.StudyId,
                evt.Title,
                evt.ShortName,
                DateTime.Parse(evt.Metadata["$applies"]).ToUniversalTime(),
                null,
                new Researcher(evt.ResearcherFirstname, evt.ResearcherLastname, evt.ResearcherEmail),
                null,
                evt.RegistrationStatus);
        }
        
        private void Apply(StudyApprovedV1 evt)
        {
            CorrelationId = evt.Metadata["$correlationId"];
            _study.StudyRegistrationStatus = StudyRegistrationStatus.Approved;
        }
        
        private void Apply(StudyRejectedV1 evt)
        {
            CorrelationId = evt.Metadata["$correlationId"];
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
            Ensure.NotNullOrWhiteSpace(cmd.ResearcherFirstname, nameof(cmd.ResearcherFirstname));
            Ensure.NotNullOrWhiteSpace(cmd.ResearcherLastname, nameof(cmd.ResearcherLastname));
            Ensure.NotNullOrWhiteSpace(cmd.ResearcherEmail, nameof(cmd.ResearcherEmail));

            RaiseEvent(new StudyWaitingForApprovalSubmittedV1(cmd.StudyId, cmd.Title, cmd.ShortName, cmd.ResearcherFirstname, cmd.ResearcherLastname, cmd.ResearcherEmail, StudyRegistrationStatus.WaitingForApproval, cmd.Metadata));
        }

        public void ApproveStudy(ApproveStudyCommand cmd)
        {
            Ensure.NotNull(_study, "Study has not yet been created and cannot be approved.");
            if (_study.StudyRegistrationStatus == StudyRegistrationStatus.Approved) return;

            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.IsPositiveLong(cmd.StudyId, nameof(cmd.StudyId));
            
            RaiseEvent(new StudyApprovedV1(cmd.StudyId, cmd.Metadata));
        }

        public void RejectStudy(RejectStudyCommand cmd)
        {
            Ensure.NotNull(_study, "Study has not yet been created and cannot be rejected.");
            if (_study.StudyRegistrationStatus == StudyRegistrationStatus.Rejected) return;

            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.IsPositiveLong(cmd.StudyId, nameof(cmd.StudyId));
            
            RaiseEvent(new StudyRejectedV1(cmd.StudyId, cmd.Metadata));
        }
    }
}