using System.Threading.Tasks;
using Application.Contracts;
using Application.Events;
using CognitoCustomMessageProcessor.Content;
using CognitoCustomMessageProcessor.CustomMessages;
using Microsoft.Extensions.Logging;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class SignUpHandler : IHandler<CustomMessageSignUp, CognitoCustomMessageEvent>
    {
        private readonly ILogger<SignUpHandler> _logger;

        public SignUpHandler(ILogger<SignUpHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageSignUp source)
        {
            _logger.LogInformation($"************** {nameof(SignUpHandler)} STARTED");
            
            var link = $"http://localhost:3000/verify?code={source.Request.CodeParameter}&clientId={source.CallerContext.ClientId}&region={source.Region}&email={source.Request.UserAttributes.Email}";
            source.Response.EmailSubject = $"hi, from SignUp";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TEXT_REPLACE1###", "Welcome to Be Part of Research.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to verify your account")
                .Replace("###LINK_REPLACE###", @$"<a href=""{link}"">Verify</a>");
            
            _logger.LogInformation($"************** {nameof(SignUpHandler)} FINISHED");
            
            return await Task.FromResult(source);
        }
    }
}