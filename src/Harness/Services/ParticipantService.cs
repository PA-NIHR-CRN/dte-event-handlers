using Harness.Contracts;
using ScheduledJobs.Domain;

namespace Harness.Services;

public class ParticipantService: IParticipantService
{
    private readonly ILogger<ParticipantService> _logger;
    private readonly IParticipantRepository _repository;

    public ParticipantService(ILogger<ParticipantService> logger, IParticipantRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public Task InsertAllAsync(IEnumerable<Participant> participants)
    {
        try
        {
            return _repository.InsertAllAsync(participants);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error inserting participants");
            throw;
        }
    }

    public Task<int> GetTotalParticipants()
    {
        try
        {
            return _repository.GetTotalParticipants();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting total participants");
            throw;
        }
    }
}