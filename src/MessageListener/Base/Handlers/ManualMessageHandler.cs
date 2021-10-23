using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.SQS;
using Amazon.SQS.Model;
using MessageListener.Base.Messages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MessageListener.Base.Handlers
{
    public class ManualMessageHandler : IMessageHandler
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger<ManualMessageHandler> _logger;
        

        public ManualMessageHandler(IAmazonSQS sqsClient, ILogger<ManualMessageHandler> logger)
        {
            _sqsClient = sqsClient;
            _logger = logger;
        }

        public string MessageType => "ManualMessage";

        public async Task HandleAsync(string messageBody, ILambdaContext context)
        {
            _logger.LogInformation($"Handle started for: {nameof(ManualMessageHandler)}");

            var message = JsonConvert.DeserializeObject<ManualSqsQueueUrlMessage>(messageBody);
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
                    _logger.LogInformation(JsonConvert.SerializeObject(m.Body, Formatting.Indented));

                    // JsonConvert.DeserializeObject<CloudEvent>(m.Body)
                    //_worker.Process();
                    
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