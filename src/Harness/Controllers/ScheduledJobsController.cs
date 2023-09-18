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
    private readonly IHandler<ParticipantExport, bool> _participantExportHandler;
    private readonly IHandler<ParticipantOdpExport, bool> _participantOdpExportHandler;
    private readonly IBogusService _bogusService;
    private readonly IParticipantRepository _participantRepository;

    public ScheduledJobsController(ILogger<ScheduledJobsController> logger,
        IHandler<ParticipantExport, bool> participantExportHandler,
        IHandler<ParticipantOdpExport, bool> participantOdpExportHandler,
        IBogusService bogusService, IParticipantRepository participantRepository)
    {
        _logger = logger;
        _participantExportHandler = participantExportHandler;
        _participantOdpExportHandler = participantOdpExportHandler;
        _bogusService = bogusService;
        _participantRepository = participantRepository;
    }

    [HttpPost("ScheduledJobsDailyExport")]
    public async Task<IActionResult> ScheduledJobsDailyExport(CancellationToken cancellationToken)
    {
        var result = await _participantExportHandler.HandleAsync(new ParticipantExport(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("ScheduledJobsOdpDailyExport")]
    public async Task<IActionResult> ScheduledJobsOdpDailyExport(CancellationToken cancellationToken)
    {
        var result = await _participantOdpExportHandler.HandleAsync(new ParticipantOdpExport(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("AddFakeUsers")]
    public async Task<IActionResult> AddFakeUsers([FromBody] int count, CancellationToken cancellationToken)
    {
        var fakeUsers = _bogusService.GenerateFakeUsers(count);
        await _participantRepository.InsertAllAsync(fakeUsers, cancellationToken);
        _logger.LogInformation("Added {Count} fake users", count);
        return Ok();
    }

    [HttpGet("GetTotalParticipants")]
    public async Task<IActionResult> GetTotalParticipants(CancellationToken cancellationToken)
    {
        var totalParticipants = await _participantRepository.GetTotalParticipants(cancellationToken);
        _logger.LogInformation("Total participants: {TotalParticipants}", totalParticipants);
        return Ok(totalParticipants);
    }
}