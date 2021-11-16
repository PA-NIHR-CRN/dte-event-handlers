using System.Threading.Tasks;
using Application.Contracts;
using Application.Events;
using CognitoCustomMessageProcessor.Content;
using CognitoCustomMessageProcessor.CustomMessages;
using Microsoft.Extensions.Logging;

namespace CognitoCustomMessageProcessor.CustomMessageHandlers
{
    public class ForgotPasswordHandler : IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent>
    {
        private readonly ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(ILogger<ForgotPasswordHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task<CognitoCustomMessageEvent> HandleAsync(CustomMessageForgotPassword source)
        {
            _logger.LogInformation($"************** {nameof(ForgotPasswordHandler)} STARTED");
            
            var link = $"http://localhost:3000/verify?code={source.Request.CodeParameter}&clientId={source.CallerContext.ClientId}&region={source.Region}&email={source.Request.UserAttributes.Email}";
            source.Response.EmailSubject = $"hi, from ForgotPassword";
            source.Response.EmailMessage = CustomMessageEmail.GetCustomMessageHtml()
                .Replace("###TEXT_REPLACE1###", "You have asked us to reset your password.")
                .Replace("###TEXT_REPLACE2###", "Please click the link below to reset your password")
                .Replace("###LINK_REPLACE###", @$"<a href=""{link}"">Reset Password</a>");
            
            _logger.LogInformation($"************** {nameof(ForgotPasswordHandler)} FINISHED");
            
            return await Task.FromResult(source);
        }
    }
}