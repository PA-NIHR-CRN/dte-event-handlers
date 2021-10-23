using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageListener.Base
{
    public abstract class EventFunctionBase<TInput> : FunctionBase
    {
        protected async Task FunctionHandlerAsync(TInput input, ILambdaContext context)
        {
            using var scope = ServiceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetService<IEventHandler<TInput>>();

            if (handler == null)
            {
                Logger.LogCritical($"No IEventHandler<{typeof(TInput).Name}> could be found.");
                throw new InvalidOperationException($"No IEventHandler<{typeof(TInput).Name}> could be found.");
            }

            Logger.LogInformation("Invoking handler");
            await handler.HandleAsync(input, context).ConfigureAwait(false);
        }
    }
}