using Domain.Aggregates;
using Domain.Commands;
using Domain.Contracts;
using Evento;

namespace Domain.CommandHandlers
{
    public class SubmitStudyForApprovalHandler : IHandle<SubmitStudyForApproval>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IStudyService _studyService;

        public SubmitStudyForApprovalHandler(IDomainRepository domainRepository, IStudyService studyService)
        {
            _domainRepository = domainRepository;
            _studyService = studyService;
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

            aggregate.SubmitForApproval(command, _studyService).Wait();

            return aggregate;
        }
    }
}