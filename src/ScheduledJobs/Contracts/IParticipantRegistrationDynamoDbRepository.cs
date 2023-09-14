using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduledJobs.Domain;

namespace ScheduledJobs.Contracts
{
    public interface IParticipantRegistrationDynamoDbRepository
    {
        Task<IAsyncEnumerable<Participant>> GetAllAsync();
        Task<Participant> GetParticipantAsync(string participantId);
    }
}