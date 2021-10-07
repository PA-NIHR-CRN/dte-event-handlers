using System;
using Domain.Commands;
using Evento;

namespace Adapter.Handlers
{
    public class CompleteStepHandler : IHandle<CompleteStep>
    {
        public IAggregate Handle(CompleteStep command)
        {
            throw new NotImplementedException("TODO");
        }
    }
}