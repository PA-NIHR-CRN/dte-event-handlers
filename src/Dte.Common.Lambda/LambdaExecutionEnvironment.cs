namespace Dte.Common.Lambda
{
    public interface IExecutionEnvironment
    {
        string EnvironmentName { get; }
        bool IsLambda { get; }
        bool RunAsQueueListener { get; }
    }

    public class LambdaExecutionEnvironment : IExecutionEnvironment
    {
        public string EnvironmentName { get; set; }
        public bool IsLambda { get; set; }
        public bool RunAsQueueListener { get; set; }
    }
}