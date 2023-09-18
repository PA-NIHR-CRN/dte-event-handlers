using CognitoCustomMessageProcessor.CustomMessages;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Events;
using Microsoft.AspNetCore.Mvc;

namespace Harness.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomMessageControllerController : ControllerBase
{
    private readonly IHandler<CustomMessageSignUp, CognitoCustomMessageEvent> _signUpHandler;
    private readonly IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent> _forgotPasswordHandler;
    private readonly IHandler<CustomMessageResendCode, CognitoCustomMessageEvent> _resendCodeHandler;
    private readonly IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent> _updateAttributeHandler;

    public CustomMessageControllerController(
        IHandler<CustomMessageSignUp, CognitoCustomMessageEvent> signUpHandler,
        IHandler<CustomMessageForgotPassword, CognitoCustomMessageEvent> forgotPasswordHandler,
        IHandler<CustomMessageResendCode, CognitoCustomMessageEvent> resendCodeHandler,
        IHandler<CustomMessageUpdateUserAttribute, CognitoCustomMessageEvent> updateAttributeHandler)
    {
        _signUpHandler = signUpHandler;
        _forgotPasswordHandler = forgotPasswordHandler;
        _resendCodeHandler = resendCodeHandler;
        _updateAttributeHandler = updateAttributeHandler;
    }

    [HttpPost("SendSignUpEmail")]
    public async Task<IActionResult> SendSignUpEmail(CancellationToken cancellationToken)
    {
        var result = await _signUpHandler.HandleAsync(new CustomMessageSignUp(), cancellationToken);

        return Ok(result);
    }

    [HttpPost("SendForgotPasswordEmail")]
    public async Task<IActionResult> SendForgotPasswordEmail(CancellationToken cancellationToken)
    {
        var result = await _forgotPasswordHandler.HandleAsync(new CustomMessageForgotPassword(), cancellationToken);

        return Ok(result);
    }

    [HttpPost("SendResendCodeEmail")]
    public async Task<IActionResult> SendResendCodeEmail(CancellationToken cancellationToken)
    {
        var result = await _resendCodeHandler.HandleAsync(new CustomMessageResendCode(), cancellationToken);

        return Ok(result);
    }

    [HttpPost("SendUpdateUserEmail")]
    public async Task<IActionResult> SendUpdateUserEmail(CancellationToken cancellationToken)
    {
        var result =
            await _updateAttributeHandler.HandleAsync(new CustomMessageUpdateUserAttribute(), cancellationToken);

        return Ok(result);
    }
}