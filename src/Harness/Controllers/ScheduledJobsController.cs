using Dte.Common.Lambda.Contracts;
using Harness.Contracts;
using Microsoft.AspNetCore.Mvc;
using ScheduledJobs.JobHandlers;

namespace Harness.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduledJobsController : ControllerBase
{
    private readonly ILogger<ScheduledJobsController> _logger;
    private readonly IHandler<ParticipantExport, bool> _handler;
    private readonly IBogusService _bogusService;
    private readonly IParticipantService _participantService;

    public ScheduledJobsController(ILogger<ScheduledJobsController> logger, IHandler<ParticipantExport, bool> handler,
        IBogusService bogusService, IParticipantService participantService)
    {
        _logger = logger;
        _handler = handler;
        _bogusService = bogusService;
        _participantService = participantService;
    }
    
    [HttpPost("ScheduledJobsDailyExport")]
    public async Task<IActionResult> ScheduledJobsDailyExport()
    {
        var result = await _handler.HandleAsync(new ParticipantExport());
        return Ok(result);
    }
    
    [HttpPost("AddFakeUsers")]
    public async Task<IActionResult> AddFakeUsers([FromBody] int count)
    {
        var fakeUsers = _bogusService.GenerateFakeUsers(count);
        await _participantService.InsertAllAsync(fakeUsers);
        _logger.LogInformation("Added {Count} fake users", count);
        return Ok();
    }
    
    [HttpGet("GetTotalParticipants")]
    public async Task<IActionResult> GetTotalParticipants()
    {
        var totalParticipants = await _participantService.GetTotalParticipants();
        _logger.LogInformation("Total participants: {TotalParticipants}", totalParticipants);
        return Ok(totalParticipants);
    }
}