namespace MessageListener.Base
{
    public class ParallelSqsExecutionOptions
    {
        public int MaxDegreeOfParallelism { get; set; } = System.Environment.ProcessorCount;
    }
}