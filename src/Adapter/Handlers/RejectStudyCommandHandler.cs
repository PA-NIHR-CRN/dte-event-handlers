using Domain.Aggregates;
using Domain.Commands;
using Evento;

namespace Adapter.Handlers
{
    public class RejectStudyCommandHandler : IHandle<RejectStudyCommand>
    {
        private readonly IDomainRepository _domainRepository;

        public RejectStudyCommandHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }
        
        public IAggregate Handle(RejectStudyCommand command)
        {
            var aggregate = _domainRepository.GetById<Studying>(command.Metadata["$correlationId"]);

            aggregate.RejectStudy(command);

            return aggregate;
        }
    }
}