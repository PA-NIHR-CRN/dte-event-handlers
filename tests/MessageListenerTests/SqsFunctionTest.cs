using System.Collections.Generic;
using Amazon.Lambda.SQSEvents;
using Amazon.Lambda.TestUtilities;
using Application.Messages;
using MessageListener;
using Newtonsoft.Json;
using NUnit.Framework;

namespace MessageListenerTests
{
    [TestFixture]
    public class SqsFunctionTest
    {
        [Test]
        public void Function_Executes_Correct_Handler_From_Incoming_Event()
        {
            var sqsEvent = new SQSEvent();
            var function = new SqsFunction();
            
            sqsEvent.Records = new List<SQSEvent.SQSMessage>{new SQSEvent.SQSMessage{Body = JsonConvert.SerializeObject(new MessageBase{Type = "DoSomethingV1"})}};
            Assert.DoesNotThrowAsync(() => function.FunctionHandler(sqsEvent, new TestLambdaContext()), $"No handler for message type: {sqsEvent.Records[0].Body}");
            
            sqsEvent.Records = new List<SQSEvent.SQSMessage>{new SQSEvent.SQSMessage{Body = JsonConvert.SerializeObject(new MessageBase{Type = "SendNotificationV1"})}};
            Assert.DoesNotThrowAsync(() => function.FunctionHandler(sqsEvent, new TestLambdaContext()), $"No handler for message type: {sqsEvent.Records[0].Body}");
        }
    }
}
