using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ScheduledJobs.Domain;

namespace ScheduledJobs.Contracts
{
    public interface IParticipantRegistrationDynamoDbRepository
    {
        IAsyncEnumerable<Participant> GetAllAsync(
            [EnumeratorCancellation] CancellationToken cancellationToken = default);

        Task<Participant> GetParticipantAsync(string participantId);
    }
}