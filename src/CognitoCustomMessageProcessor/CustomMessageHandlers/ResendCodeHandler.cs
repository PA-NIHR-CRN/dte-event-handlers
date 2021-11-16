using System.Threading.Tasks;
using Application.Contracts;
using Application.Events;
using CognitoCustomMessageProcessor.Content;
using CognitoCustomMessageProcessor.CustomMessages;
using Microsoft.Extensions.Logging;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class ResendCodeHandler : IHandler<CustomMessageResendCode, CognitoCustomMessageEvent>
    {
        private readonly ILogger<ResendCodeHandler> _logger;

        public ResendCodeHandler(ILogger<ResendCodeHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageResendCode source)
        {
            _logger.LogInformation($"************** {nameof(ResendCodeHandler)} STARTED");
            
            var link = $"http://localhost:3000/verify?code={source.Request.CodeParameter}&clientId={source.CallerContext.ClientId}&region={source.Region}&email={source.Request.UserAttributes.Email}";
            source.Response.EmailSubject = $"hi, from ResendCode";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TEXT_REPLACE1###", "You have asked us to resend a verification email.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to verify your account")
                .Replace("###LINK_REPLACE###", @$"<a href=""{link}"">Verify</a>");
            
            _logger.LogInformation($"************** {nameof(ResendCodeHandler)} FINISHED");
            
            return await Task.FromResult(source);
        }
    }
}