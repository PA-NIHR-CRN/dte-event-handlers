using Domain.Aggregates;
using Domain.Commands;
using Domain.Contracts;
using Evento;

namespace Domain.CommandHandlers
{
    public class SubmitStudyForApprovalHandler : IHandle<SubmitStudyForApproval>
    {
        private readonly IDomainRepository _domainRepository;

        public SubmitStudyForApprovalHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }
        
        public IAggregate Handle(SubmitStudyForApproval command)
        {
            Studying aggregate;

            try
            {
                aggregate = _domainRepository.GetById<Studying>(command.Metadata["$correlationId"]);
            }
            catch (AggregateNotFoundException)
            {
                aggregate = Studying.Create();
            }

            aggregate.SubmitForApproval(command);

            return aggregate;
        }
    }
}