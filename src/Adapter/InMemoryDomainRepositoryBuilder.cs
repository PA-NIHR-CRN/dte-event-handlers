using Adapter.Contracts;
using Evento;

namespace Adapter
{
    public class InMemoryDomainRepositoryBuilder : IDomainRepositoryBuilder
    {
        public IDomainRepository Build()
        {
            return new InMemoryDomainRepository();
        }
    }
}