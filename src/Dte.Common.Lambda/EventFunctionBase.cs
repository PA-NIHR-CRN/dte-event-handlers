using System;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dte.Common.Lambda
{
    public abstract class EventFunctionBase<TInput> : FunctionBase
    {
        protected async Task FunctionHandlerAsync(TInput input)
        {
            using var scope = ServiceProvider.CreateScope();
            var lambdaEventHandler = scope.ServiceProvider.GetService<ILambdaEventHandler<TInput>>();

            if (lambdaEventHandler == null)
            {
                Logger.LogCritical($"No IEventHandler<{typeof(TInput).Name}> could be found.");
                throw new InvalidOperationException($"No IEventHandler<{typeof(TInput).Name}> could be found.");
            }

            Logger.LogInformation($"Invoking: {lambdaEventHandler.GetType().Name}");
            await lambdaEventHandler.HandleLambdaEventAsync(input).ConfigureAwait(false);
        }
    }
}