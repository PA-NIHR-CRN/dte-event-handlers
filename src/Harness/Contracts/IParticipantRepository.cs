using ScheduledJobs.Domain;

namespace Harness.Contracts;

public interface IParticipantRepository
{
    Task InsertAllAsync(IEnumerable<Participant> participants);
    Task<int> GetTotalParticipants();
}