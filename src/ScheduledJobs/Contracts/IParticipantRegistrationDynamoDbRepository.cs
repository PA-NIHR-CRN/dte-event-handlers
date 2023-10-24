using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using ScheduledJobs.Domain;

namespace ScheduledJobs.Contracts
{
    public interface IParticipantRegistrationDynamoDbRepository
    {
        IAsyncEnumerable<Participant> GetAllAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
