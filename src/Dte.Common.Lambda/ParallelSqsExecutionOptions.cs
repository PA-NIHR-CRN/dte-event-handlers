namespace Dte.Common.Lambda
{
    public class ParallelSqsExecutionOptions
    {
        public int MaxDegreeOfParallelism { get; set; } = System.Environment.ProcessorCount;
    }
}