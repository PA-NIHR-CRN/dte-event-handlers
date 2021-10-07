using System;
using Domain.Commands;
using Evento;
using Microsoft.Extensions.DependencyInjection;

namespace Adapter.Handlers
{
    public interface ICommandExecutor
    {
        IAggregate Execute(Command command);
    }
    
    public class CommandExecutor : ICommandExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IAggregate Execute(Command command)
        {
            return command switch
            {
                SubmitStudyForApproval cmd => _serviceProvider.GetService<IHandle<SubmitStudyForApproval>>()?.Handle(cmd),
                CompleteStep cmd => _serviceProvider.GetService<IHandle<CompleteStep>>()?.Handle(cmd),
                ExpressInterest cmd => _serviceProvider.GetService<IHandle<ExpressInterest>>()?.Handle(cmd),
                ApproveStudyCommand cmd => _serviceProvider.GetService<IHandle<ApproveStudyCommand>>()?.Handle(cmd),
                RejectStudyCommand cmd => _serviceProvider.GetService<IHandle<RejectStudyCommand>>()?.Handle(cmd),
                _ => throw new Exception($"I can't find an available handler for command: {command.GetType()}")
            };
        }
    }
}