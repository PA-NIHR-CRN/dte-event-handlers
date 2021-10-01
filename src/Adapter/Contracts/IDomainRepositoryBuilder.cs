using Evento;

namespace Adapter.Contracts
{
    public interface IDomainRepositoryBuilder
    {
        IDomainRepository Build();
    }
}