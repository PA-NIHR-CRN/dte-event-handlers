using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adapter.Contracts;
using Adapter.Handlers;
using Common;
using Common.Settings;
using Evento;
using Microsoft.Extensions.Logging;

namespace Adapter
{
    public class Worker : IWorker
    {
        private const int MaxLengthForLogs = 255;
        private readonly IDomainRepository _domainRepository;
        private readonly ILogger<Worker> _logger;
        private readonly AppSettings _appSettings;
        private readonly ICommandExecutor _commandExecutor;
        private readonly Dictionary<string, Func<CloudEvent, Command>> _deserializers;

        public Worker(IDomainRepository domainRepository,
            IEnumerable<ICloudEventMapper> mappers,
            ILogger<Worker> logger,
            AppSettings appSettings,
            ICommandExecutor commandExecutor)
        {
            _domainRepository = domainRepository;
            _deserializers = mappers.ToDictionary<ICloudEventMapper, string, Func<CloudEvent, Command>>(x => x.Schema.ToString().ToLower(), x => x.Map);
            _logger = logger;
            _appSettings = appSettings;
            _commandExecutor = commandExecutor;
        }

        public void Process(CloudEvent cloudRequest)
        {
            DecryptMessageIfNeeded(cloudRequest, _appSettings.CryptoKey);
            var requestDataScheme = cloudRequest.DataSchema.ToString().ToLower();
            var cloudRequestSource = cloudRequest.Source.ToString().ToLower();

            if (!_deserializers.ContainsKey(requestDataScheme) &&
                !_deserializers.ContainsKey($"{requestDataScheme}{cloudRequestSource}"))
            {
                throw new Exception(
                    $"I can't find a mapper for schema:'{requestDataScheme}' source:''{cloudRequestSource}''");
            }

            var command = _deserializers.ContainsKey(requestDataScheme)
                ? _deserializers[requestDataScheme](cloudRequest)
                : _deserializers[$"{requestDataScheme}{cloudRequestSource}"](cloudRequest);

            if (command == null)
            {
                throw new Exception(
                    $"I received CloudRequest Type:'{cloudRequest.Type}' Source:'{cloudRequestSource}' Schema:'{requestDataScheme}' but I was unable to deserialize a Command out of it");
            }

            IAggregate aggregate = null;
            try
            {
                aggregate = _commandExecutor.Execute(command);
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
                        _logger.LogInformation(
                            $"Handled '{cloudRequest.Type}' AggregateId:'{aggregate.AggregateId}' [0]Resulted event:'{uncommittedEvent.GetType()}'");

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
        
        private static void DecryptMessageIfNeeded(CloudEvent request, string cryptoKey)
        {
            if (!string.IsNullOrWhiteSpace(cryptoKey))
            {
                var cryptoService = new AesCryptoService(Convert.FromBase64String(cryptoKey));
                request.Data = cryptoService.Decrypt(Convert.FromBase64String(request.Data));
            }
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