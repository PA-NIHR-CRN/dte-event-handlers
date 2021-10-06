using Domain.Commands;
using Domain.Events;
using Evento;

namespace Domain.Aggregates
{
    public class ParticipatingToStudy : AggregateBase
    {
        public override string AggregateId => _correlationId;
        private string _correlationId;
        private bool _interestExpressed;
        private string _studyId;
        private string _siteId;
        private string _userId;

        public ParticipatingToStudy()
        {
            RegisterTransition<InterestExpressedV1>(Apply);
        }

        private void Apply(InterestExpressedV1 obj)
        {
            _correlationId = obj.Metadata[$"$correlationId"];
            _interestExpressed = true;
            _studyId = obj.StudyId;
            _siteId = obj.SiteId;
            _userId = obj.UserId;
        }

        public static ParticipatingToStudy Create()
        {
            return new ParticipatingToStudy();
        }

        public void ExpressInterest(ExpressInterest cmd)
        {
            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.NotNullOrWhiteSpace(cmd.CorrelationId, cmd.CorrelationId);
            Ensure.NotNullOrWhiteSpace(cmd.StudyId, cmd.StudyId);
            Ensure.NotNullOrWhiteSpace(cmd.SiteId, cmd.SiteId);
            Ensure.NotNullOrWhiteSpace(cmd.UserId, cmd.UserId);

            // Idempotency
            if (_interestExpressed)
                return;
            
            // TODO do you want to keep up-to-date another system? here you can do it 

            RaiseEvent(new InterestExpressedV1(cmd.StudyId, cmd.SiteId, cmd.UserId, cmd.Metadata));
        }
    }
}