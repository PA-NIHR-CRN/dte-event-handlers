using Dte.Common.Lambda.Contracts;
using Harness.Contracts;
using Harness.Requests;
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
    public async Task<IActionResult> ScheduledJobsDailyExport()
    {
        var result = await _participantExportHandler.HandleAsync(new ParticipantExport());
        return Ok(result);
    }

    [HttpPost("ScheduledJobsOdpDailyExport")]
    public async Task<IActionResult> ScheduledJobsOdpDailyExport()
    {
        var result = await _participantOdpExportHandler.HandleAsync(new ParticipantOdpExport());
        return Ok(result);
    }

    [HttpPost("AddFakeUsers")]
    public async Task<IActionResult> AddFakeUsers([FromBody] AddFakeUsersRequest request,
        CancellationToken cancellationToken)
    {
        var fakeUsers = _bogusService.GenerateFakeUsers(request.ParticipantRecords, request.DeletedRecords);
        await _participantRepository.InsertAllAsync(fakeUsers, cancellationToken);
        _logger.LogInformation("Added {Count} fake users", request.ParticipantRecords + request.DeletedRecords);
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
