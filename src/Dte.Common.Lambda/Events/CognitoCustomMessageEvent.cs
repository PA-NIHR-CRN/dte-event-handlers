using System;
using System.Text.Json.Serialization;

namespace Dte.Common.Lambda.Events
{
    public class CognitoCustomMessageEvent
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }

        [JsonPropertyName("userPoolId")]
        public string UserPoolId { get; set; }

        [JsonPropertyName("userName")]
        public Guid UserName { get; set; }

        [JsonPropertyName("callerContext")]
        public CallerContext CallerContext { get; set; }

        [JsonPropertyName("triggerSource")]
        public string TriggerSource { get; set; }

        [JsonPropertyName("request")]
        public Request Request { get; set; }

        [JsonPropertyName("response")]
        public Response Response { get; set; }
    }

    public class CallerContext
    {
        [JsonPropertyName("awsSdkVersion")]
        public string AwsSdkVersion { get; set; }

        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }
    }

    public class Request
    {
        [JsonPropertyName("userAttributes")]
        public UserAttributes UserAttributes { get; set; }

        [JsonPropertyName("codeParameter")]
        public string CodeParameter { get; set; }

        [JsonPropertyName("linkParameter")]
        public string LinkParameter { get; set; }

        [JsonPropertyName("usernameParameter")]
        public object UsernameParameter { get; set; }
    }

    public class UserAttributes
    {
        [JsonPropertyName("sub")]
        public Guid Sub { get; set; }

        [JsonPropertyName("cognito:email_alias")]
        public string CognitoEmailAlias { get; set; }

        [JsonPropertyName("email_verified")]
        public string EmailVerified { get; set; }

        [JsonPropertyName("cognito:user_status")]
        public string CognitoUserStatus { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class Response
    {
        [JsonPropertyName("smsMessage")]
        public object SmsMessage { get; set; }

        [JsonPropertyName("emailMessage")]
        public object EmailMessage { get; set; }

        [JsonPropertyName("emailSubject")]
        public object EmailSubject { get; set; }
    }
}