namespace Application.Contracts
{
    public interface IHandlerResolver
    {
        (object handlerImpl, object method) ResolveHandler(string handlerType, string source);
    }
}