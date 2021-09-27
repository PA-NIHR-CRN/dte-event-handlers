namespace Adapter
{
    public interface IWorker
    {
        void Process(CloudEvent cloudRequest);
    }
}