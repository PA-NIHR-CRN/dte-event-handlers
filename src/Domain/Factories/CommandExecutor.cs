using System;
using Domain.Commands;
using Evento;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Domain.Factories
{
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CommandExecutor> _logger;

        public CommandExecutor(IServiceProvider serviceProvider, ILogger<CommandExecutor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        
        public IAggregate Execute(Command command)
        {
            return command switch
            {
                SubmitStudyForApproval submitStudyForApproval => _serviceProvider.GetService<IHandle<SubmitStudyForApproval>>()?.Handle(submitStudyForApproval),
                CompleteStep completeStep => _serviceProvider.GetService<IHandle<CompleteStep>>()?.Handle(completeStep),
                ExpressInterest expressInterest => _serviceProvider.GetService<IHandle<ExpressInterest>>()?.Handle(expressInterest),
                _ => throw new Exception($"I can't find an available handler for command: {command.GetType()}")
            };
        }
    }
}