using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.SQS;
using Amazon.SQS.Model;
using Application.Contracts;
using Application.Extensions;
using Application.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Application.EventHandlers.Sqs
{
    public class ManualMessageLambdaEventHandler :  ILambdaEventHandler<SQSEvent>
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ISqsMessageHandlerExecutor _handlerExecutor;
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ParallelSqsExecutionOptions _options;

        public ManualMessageLambdaEventHandler(IAmazonSQS sqsClient,
            ISqsMessageHandlerExecutor handlerExecutor,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory,
            IOptions<ParallelSqsExecutionOptions> options)
        {
            _sqsClient = sqsClient;
            _handlerExecutor = handlerExecutor;
            _logger = loggerFactory?.CreateLogger("SqsEventHandler") ?? throw new ArgumentNullException(nameof(loggerFactory));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));;
        }

        public async Task HandleLambdaEventAsync(SQSEvent @event, ILambdaContext context)
        {
            if (!@event.Records.Any())
            {
                throw new Exception("No input, expecting message with property: SqsQueueName");
            }
            
            var message = JsonConvert.DeserializeObject<ManualSqsQueueUrlMessage>(@event.Records[0].Body);
            
            if (message == null)
            {
                throw new Exception("Message is null, expecting ManualSqsQueueUrlMessage with property: SqsQueueName");
            }
            
            var queueUrl = await GetQueueUrlAsync(message.SqsQueueName);
            
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                WaitTimeSeconds = 1,
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10
            };

            _logger.LogInformation($"Listening on {queueUrl}");

            while (true)
            {
                var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);
		
                await receiveMessageResponse.Messages.ForEachAsync(_options.MaxDegreeOfParallelism, async record =>
                {
                    _logger.LogInformation($"Message received: {DateTime.Now}: {record.Body}");
                    _logger.LogInformation("Calling handler");
                    _logger.LogInformation(JsonConvert.SerializeObject(record.Body, Formatting.Indented));

                    var (handlerName, success) = await _handlerExecutor.ExecuteHandlerAsync(record.Body);
                    _logger.LogInformation($"**** Handle {(success ? "SUCCESS" : "FAILURE")} for handler: {handlerName}");

                    //Handle message 
                    _logger.LogInformation($"Deleting message ReceiptHandle: {record.ReceiptHandle}");
                    await _sqsClient.DeleteMessageAsync(queueUrl, record.ReceiptHandle);
                });
            }
        }
        
        public async Task<string> GetQueueUrlAsync(string queueName)
        {
            var request = new GetQueueUrlRequest { QueueName = queueName };
            var response = await _sqsClient.GetQueueUrlAsync(request);

            if (response != null && !string.IsNullOrWhiteSpace(response.QueueUrl))
            {
                return response.QueueUrl;
            }

            throw new ApplicationException($"Can not find the queue named: {queueName} on your account");
        }
    }
}