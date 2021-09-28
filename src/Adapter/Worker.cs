using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adapter.Contracts;
using Adapter.Mappers;
using Domain.Aggregates;
using Domain.Commands;
using Domain.Contracts;
using Evento;
using Microsoft.Extensions.Logging;

namespace Adapter
{
    public class Worker : IHandle<SubmitStudyForApprovalCommand>, IHandle<CompleteStepCommand>, IWorker
    {
        private const int MaxLengthForLogs = 255;
        
        private readonly IDomainRepository _domainRepository;
        private readonly IStudyService _studyService;
        private readonly ILogger<Worker> _logger;
        private readonly Dictionary<string, Func<CloudEvent, Command>> _deserializers;

        public Worker(IDomainRepository domainRepository, IStudyService studyService, IEnumerable<ICloudEventMapper> mappers, ILogger<Worker> logger)
        {
            _domainRepository = domainRepository;
            _studyService = studyService;
            _deserializers = mappers.ToDictionary<ICloudEventMapper, string, Func<CloudEvent, Command>>(x => x.Schema.ToString().ToLower(), x => x.Map);
            _logger = logger;
        }

        public void Process(CloudEvent cloudRequest)
        {
            var requestDataScheme = cloudRequest.DataSchema.ToString().ToLower();
            var cloudRequestSource = cloudRequest.Source.ToString().ToLower();
            
            if (!_deserializers.ContainsKey(requestDataScheme) &&
                !_deserializers.ContainsKey($"{requestDataScheme}{cloudRequestSource}"))
            {
                throw new Exception($"I can't find a mapper for schema:'{requestDataScheme}' source:''{cloudRequestSource}''");
            }

            var command = _deserializers.ContainsKey(requestDataScheme)
                ? _deserializers[requestDataScheme](cloudRequest)
                : _deserializers[$"{requestDataScheme}{cloudRequestSource}"](cloudRequest);

            if (command == null)
            {
                throw new Exception($"I received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequestSource}' Schema:'{requestDataScheme}' but I was unable to deserialize a Command out of it");
            }

            IAggregate aggregate = null;
            try
            {
                aggregate = command switch
                {
                    SubmitStudyForApprovalCommand submitStudyForApproval => Handle(submitStudyForApproval),
                    CompleteStepCommand completeStep => Handle(completeStep),
                    _ => throw new Exception($"Received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequestSource}' Schema:'{requestDataScheme}' but I can't find an available handler for it")
                };
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
                        $"Handled CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequestSource}' Schema:'{requestDataScheme}' with no events to save");
            }
        }

        public IAggregate Handle(SubmitStudyForApprovalCommand command)
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
            
            aggregate.SubmitForApproval(command, _studyService).Wait();
            
            return aggregate;
        }

        public IAggregate Handle(CompleteStepCommand command)
        {
            throw new NotImplementedException("TODO");
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
        
        private static string TruncateFieldIfNecessary(string field)
        {
            return field.Length > MaxLengthForLogs ? field[..MaxLengthForLogs] : field;
        }
    }
}