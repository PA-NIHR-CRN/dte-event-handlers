using Evento;

namespace Domain.Factories
{
    public interface ICommandExecutor
    {
        IAggregate Execute(Command command);
    }
}