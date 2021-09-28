namespace Adapter.Contracts
{
    public interface IWorker
    {
        void Process(CloudEvent cloudRequest);
    }
}