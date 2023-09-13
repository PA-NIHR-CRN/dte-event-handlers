using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface IParticipantRegistrationDynamoDbRepository
    {
        Task<IAsyncEnumerable<Participant>> GetAllAsync();
        Task<Participant> GetParticipantAsync(string participantId);
        IAsyncEnumerable<TModel> GetAllMappedAsync<TModel>(Func<Participant, TModel> mapper);
    }
}