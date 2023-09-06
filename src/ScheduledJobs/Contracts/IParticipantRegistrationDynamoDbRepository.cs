using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface IParticipantRegistrationDynamoDbRepository
    {
        Task<IEnumerable<Participant>> GetAllAsync();
        Task<Participant> GetParticipantAsync(string participantId);
    }
}