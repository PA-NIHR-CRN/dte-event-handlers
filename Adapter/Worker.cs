using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adapter.Mappers;
using Domain.Aggregates;
using Domain.Commands;
using Domain.Services;
using Evento;
using Microsoft.Extensions.Logging;

namespace Adapter
{
    public class Worker : 
        IHandle<SubmitStudyForApproval>, 
        IHandle<CompleteStep>
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IStudyService _studyService;
        private readonly ILogger<Worker> _logger;
        private int _maxLengthForLogs = 255;
        private readonly Dictionary<string, Func<CloudEvent, Command>> _deserializers = CreateDeserializersMapping();

        public Worker(IDomainRepository domainRepository, IStudyService studyService, ILogger<Worker> logger)
        {
            _domainRepository = domainRepository;
            _studyService = studyService;
            _logger = logger;
        }

        public void Process(CloudEvent cloudRequest)
        {
            if (!_deserializers.ContainsKey(cloudRequest.DataSchema.ToString()) &&
                !_deserializers.ContainsKey($"{cloudRequest.DataSchema}{cloudRequest.Source}"))
                throw new Exception(
                    $"I can't find a mapper for schema:'{cloudRequest.DataSchema}' source:''{cloudRequest.Source}''");

            var command = _deserializers.ContainsKey(cloudRequest.DataSchema.ToString())
                ? _deserializers[cloudRequest.DataSchema.ToString()](cloudRequest)
                : _deserializers[$"{cloudRequest.DataSchema}{cloudRequest.Source}"](cloudRequest);

            if (command == null)
                throw new Exception(
                    $"I received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' but I was unable to deserialize a Command out of it");

            IAggregate aggregate = null;
            try
            {
                switch (command)
                {
                    case SubmitStudyForApproval submitStudyForApproval:
                        aggregate = Handle(submitStudyForApproval);
                        break;
                    case CompleteStep completeStep:
                        aggregate = Handle(completeStep);
                        break;
                    // Add here any further command matches
                }

                if (aggregate == null)
                    throw new Exception(
                        $"Received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' but I can't find an available handler for it");
            }
            finally
            {
                if (aggregate != null && aggregate.UncommitedEvents().Any())
                {
                    var uncommittedEventsList = aggregate.UncommitedEvents().ToList();
                    _domainRepository.Save(aggregate);

                    var error = new StringBuilder();
                    foreach (var uncommittedEvent in uncommittedEventsList)
                    {
                        _logger.LogInformation($"Handled '{cloudRequest.Type}' AggregateId:'{aggregate.AggregateId}' [0]Resulted event:'{uncommittedEvent.GetType()}'");

                        if (uncommittedEvent.GetType().ToString().EndsWith("FailedV1"))
                        {
                            error.Append(HandleFailedEvent(uncommittedEvent, command));
                        }
                    }

                    if (error.Length > 0)
                    {
                        throw new Exception(error.ToString());
                    }
                }
                else
                    _logger.LogInformation(
                        $"Handled CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequest.Source}' Schema:'{cloudRequest.DataSchema}' with no events to save");
            }
        }

        public IAggregate Handle(SubmitStudyForApproval command)
        {
            Studying aggregate;

            try
            {
                aggregate = _domainRepository.GetById<Studying>(command.Metadata["$correlationId"]);
            }
            catch (AggregateNotFoundException)
            {
                aggregate = Studying.Create();
            }
            
            aggregate.SubmitForApproval(command, _studyService);
            return aggregate;
        }

        public IAggregate Handle(CompleteStep command)
        {
            throw new NotImplementedException("TODO");
        }
        
        private static Dictionary<string, Func<CloudEvent, Command>> CreateDeserializersMapping()
        {
            // TODO make this automatic loading all the available mappers using reflection
            var submitStudyForApprovalMapper = new SubmitStudyForApprovalMapper();
            var completeStepMapper = new CompleteStepMapper();
            var deserialisers = new Dictionary<string, Func<CloudEvent, Command>>
            {
                {submitStudyForApprovalMapper.Schema.ToString(), submitStudyForApprovalMapper.Map},
                {completeStepMapper.Schema.ToString(), completeStepMapper.Map}
            };
            return deserialisers;
        }
        
        private string HandleFailedEvent(Event uncommittedEvent, Command command)
        {
            var errMessage = string.Empty;
            var errForLogging = string.Empty;
            
            // TODO add specific condition for specific events
            
            var errStack = !uncommittedEvent.Metadata.ContainsKey("error-stack")
                ? string.Empty
                : $"StackTrace: {uncommittedEvent.Metadata["error-stack"]}";
            var err = $"{errMessage} - {errStack}";
            var id = uncommittedEvent.Metadata.ContainsKey("id")
                ? uncommittedEvent.Metadata["id"]
                : "undefined";
            var correlationId = uncommittedEvent.Metadata.ContainsKey("$correlationId")
                ? uncommittedEvent.Metadata["$correlationId"]
                : "undefined";
             
            var msgToLog = $"id:'{id}';CorrelationId:'{correlationId}';{errForLogging}";
            _logger.LogError(TruncateFieldIfNecessary(msgToLog));
            return err;
        }
        
        private string TruncateFieldIfNecessary(string field)
        {
            return field.Length > _maxLengthForLogs ? field.Substring(0, _maxLengthForLogs) : field;
        }
    }
}