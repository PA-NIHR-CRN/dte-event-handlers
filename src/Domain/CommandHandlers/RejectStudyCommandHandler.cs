using Domain.Aggregates;
using Domain.Commands;
using Domain.Contracts;
using Evento;

namespace Domain.CommandHandlers
{
    public class RejectStudyCommandHandler : IHandle<RejectStudyCommand>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IStudyService _studyService;

        public RejectStudyCommandHandler(IDomainRepository domainRepository, IStudyService studyService)
        {
            _domainRepository = domainRepository;
            _studyService = studyService;
        }
        
        public IAggregate Handle(RejectStudyCommand command)
        {
            var aggregate = _domainRepository.GetById<Studying>(command.Metadata["$correlationId"]);

            aggregate.RejectStudy(command);

            return aggregate;
        }
    }
}