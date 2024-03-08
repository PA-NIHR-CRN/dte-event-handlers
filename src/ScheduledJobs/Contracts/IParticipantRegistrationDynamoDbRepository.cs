using System.Collections.Generic;
using System.Threading;
using ScheduledJobs.Domain;

namespace ScheduledJobs.Contracts
{
    public interface IParticipantRegistrationDynamoDbRepository
    {
        IAsyncEnumerable<Participant> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
