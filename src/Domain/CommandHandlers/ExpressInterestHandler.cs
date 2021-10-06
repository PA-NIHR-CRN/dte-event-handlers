using Domain.Aggregates;
using Domain.Commands;
using Evento;

namespace Domain.CommandHandlers
{
    public class ExpressInterestHandler : IHandle<ExpressInterest>
    {
        private readonly IDomainRepository _domainRepository;

        public ExpressInterestHandler(IDomainRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }
        
        public IAggregate Handle(ExpressInterest command)
        {
            ParticipatingToStudy aggregate;

            try
            {
                aggregate = _domainRepository.GetById<ParticipatingToStudy>(command.Metadata["$correlationId"]);
            }
            catch (AggregateNotFoundException)
            {
                aggregate = ParticipatingToStudy.Create();
            }

            aggregate.ExpressInterest(command);

            return aggregate;
        }
    }
}