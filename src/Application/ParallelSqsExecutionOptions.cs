namespace Application
{
    public class ParallelSqsExecutionOptions
    {
        public int MaxDegreeOfParallelism { get; set; } = System.Environment.ProcessorCount;
    }
}