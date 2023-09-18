using ScheduledJobs.Domain;

namespace Harness.Contracts;

public interface IParticipantRepository
{
    Task InsertAllAsync(IEnumerable<Participant> participants, CancellationToken cancellationToken = default);
    Task<int> GetTotalParticipants(CancellationToken cancellationToken = default);
}