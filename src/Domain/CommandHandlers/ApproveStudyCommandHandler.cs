using Domain.Aggregates;
using Domain.Commands;
using Domain.Contracts;
using Evento;

namespace Domain.CommandHandlers
{
    public class ApproveStudyCommandHandler : IHandle<ApproveStudyCommand>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IStudyService _studyService;

        public ApproveStudyCommandHandler(IDomainRepository domainRepository, IStudyService studyService)
        {
            _domainRepository = domainRepository;
            _studyService = studyService;
        }
        
        public IAggregate Handle(ApproveStudyCommand command)
        {
            var aggregate = _domainRepository.GetById<Studying>(command.Metadata["$correlationId"]);

            aggregate.ApproveStudy(command);

            return aggregate;
        }
    }
}