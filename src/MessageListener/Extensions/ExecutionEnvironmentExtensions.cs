using System;
using MessageListener.Base;

namespace MessageListener.Extensions
{
    public static class ExecutionEnvironmentExtensions
    {
        public static bool IsEnvironment(this IExecutionEnvironment executionEnvironment, string environmentName)
        {
            return string.Equals(executionEnvironment.EnvironmentName, environmentName, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDevelopment(this IExecutionEnvironment executionEnvironment) => executionEnvironment.EnvironmentName == null || IsEnvironment(executionEnvironment, "development");
        public static bool IsProduction(this IExecutionEnvironment executionEnvironment) => IsEnvironment(executionEnvironment, "production");
    }
}