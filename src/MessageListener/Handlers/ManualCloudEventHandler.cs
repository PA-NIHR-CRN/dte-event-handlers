using System;
using System.Threading.Tasks;
using Adapter;
using Adapter.Contracts;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using MessageListener.Base;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageListener.Handlers
{
    public class ManualCloudEventHandler : IMessageHandler<ManualSqsQueueUrlMessage>
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly IWorker _worker;
        private readonly ILogger<ManualCloudEventHandler> _logger;
        

        public ManualCloudEventHandler(IAmazonSQS sqsClient, IWorker worker, ILogger<ManualCloudEventHandler> logger)
        {
            _sqsClient = sqsClient;
            _worker = worker;
            _logger = logger;
        }
        
        public async Task HandleAsync(ManualSqsQueueUrlMessage message, ILambdaContext context)
        {
            _logger.LogInformation($"Handle started for: {nameof(ManualCloudEventHandler)}");

            var queueUrl = await GetQueueUrlAsync(message.SqsQueueName);
            
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                WaitTimeSeconds = 1,
                QueueUrl = queueUrl
            };

            _logger.LogInformation($"Listening on {queueUrl}");

            while (true)
            {
                var receiveMessageResponse = await _sqsClient.ReceiveMessageAsync(receiveMessageRequest);
		
                foreach (var m in receiveMessageResponse.Messages)
                {
                    _logger.LogInformation($"Message received: {DateTime.Now}: {m.Body}");
                    _logger.LogInformation("Calling handler");

                    _worker.Process(JsonConvert.DeserializeObject<CloudEvent>(m.Body));
                    
                    //Handle message 
                    _logger.LogInformation($"Deleting message ReceiptHandle: {m.ReceiptHandle}");
                    await _sqsClient.DeleteMessageAsync(queueUrl, m.ReceiptHandle);
                }
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