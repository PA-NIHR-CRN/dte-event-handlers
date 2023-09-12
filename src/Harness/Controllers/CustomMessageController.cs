using CognitoCustomMessageProcessor.CustomMessages;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using Harness.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Harness.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomMessageControllerController : ControllerBase
{
    private readonly ILogger<CustomMessageControllerController> _logger;
    private readonly IHandler<CustomMessageSignUp, CognitoCustomMessageEvent> _signUpHandler;
    private readonly IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent> _forgotPasswordHandler;
    private readonly IHandler<CustomMessageResendCode, CognitoCustomMessageEvent> _resendCodeHandler;
    private readonly IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent> _updateAttributeHandler;
    private readonly IHandler<CustomMessageSignUp, CognitoCustomMessageEvent> _handler;
    private readonly IParticipantService _participantService;

    public CustomMessageControllerController(ILogger<CustomMessageControllerController> logger,
        IHandler<CustomMessageSignUp, CognitoCustomMessageEvent> signUpHandler,
        IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent> forgotPasswordHandler,
        IHandler<CustomMessageResendCode, CognitoCustomMessageEvent> resendCodeHandler, 
        IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent> updateAttributeHandler,
        IParticipantService participantService)
    {
        _logger = logger;
        _signUpHandler = signUpHandler;
        _forgotPasswordHandler = forgotPasswordHandler;
        _resendCodeHandler = resendCodeHandler;
        _updateAttributeHandler = updateAttributeHandler;
        _participantService = participantService;
    }

    [HttpPost("SendSignUpEmail")]
    public async Task<IActionResult> SendSignUpEmail()
    {
        var result = await _signUpHandler.HandleAsync(new CustomMessageSignUp());

        return Ok(result);
    }

    [HttpPost("SendForgotPasswordEmail")]
    public async Task<IActionResult> SendForgotPasswordEmail()
    {
        var result = await _forgotPasswordHandler.HandleAsync(new CustomMessageForgotPassword());

        return Ok(result);
    }

    [HttpPost("SendResendCodeEmail")]
    public async Task<IActionResult> SendResendCodeEmail()
    {
        var result = await _resendCodeHandler.HandleAsync(new CustomMessageResendCode());

        return Ok(result);
    }

    [HttpPost("SendUpdateUserEmail")]
    public async Task<IActionResult> SendUpdateUserEmail()
    {
        var result = await _updateAttributeHandler.HandleAsync(new CustomMessageUpdateUserAttribute());

        return Ok(result);
    }
}