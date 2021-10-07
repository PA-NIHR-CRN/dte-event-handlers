using Domain.Aggregates;
using Domain.Commands;
using Evento;

namespace Adapter.Handlers
{
    public class ApproveStudyCommandHandler : IHandle<ApproveStudyCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public ApproveStudyCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }
        
        public IAggregate Handle(ApproveStudyCommand command)
        {
            var aggregate = _domainRepository.GetById<Studying>(command.Metadata["$correlationId"]);

            aggregate.ApproveStudy(command);

            return aggregate;
        }
    }
}