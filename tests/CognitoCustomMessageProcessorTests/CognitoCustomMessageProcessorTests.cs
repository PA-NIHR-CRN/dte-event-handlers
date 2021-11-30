using Amazon.Lambda.TestUtilities;
using Application.Events;
using CognitoCustomMessageProcessor;
using NUnit.Framework;

namespace CognitoCustomMessageProcessorTests
{
    [TestFixture]
    public class CognitoCustomMessageProcessorTests
    {
        [Test]
        public void Function_Executes_Correct_Handler_From_Incoming_Event()
        {
            var cognitoCustomMessageEvent = new CognitoCustomMessageEvent();
            var function = new CognitoCustomMessageFunction();

            cognitoCustomMessageEvent.Request = new Request { CodeParameter = "", UserAttributes = new UserAttributes{Email = ""} };
            cognitoCustomMessageEvent.Response = new Response();
            
            cognitoCustomMessageEvent.TriggerSource = "CustomMessageSignUp";
            Assert.DoesNotThrowAsync(() => function.FunctionHandler(cognitoCustomMessageEvent, new TestLambdaContext()), $"No handler for event type: {cognitoCustomMessageEvent.TriggerSource}");
            
            cognitoCustomMessageEvent.TriggerSource = "CustomMessageForgotPassword";
            Assert.DoesNotThrowAsync(() => function.FunctionHandler(cognitoCustomMessageEvent, new TestLambdaContext()), $"No handler for event type: {cognitoCustomMessageEvent.TriggerSource}");

            cognitoCustomMessageEvent.TriggerSource = "CustomMessageResendCode";
            Assert.DoesNotThrowAsync(() => function.FunctionHandler(cognitoCustomMessageEvent, new TestLambdaContext()), $"No handler for event type: {cognitoCustomMessageEvent.TriggerSource}");
        }
    }
}
