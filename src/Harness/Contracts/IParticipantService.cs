using ScheduledJobs.Domain;

namespace Harness.Contracts;

public interface IParticipantService
{
    Task InsertAllAsync(IEnumerable<Participant> participants);
    Task<int> GetTotalParticipants();
}