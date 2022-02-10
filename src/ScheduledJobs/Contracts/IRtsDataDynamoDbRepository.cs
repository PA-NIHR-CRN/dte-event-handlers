using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface IRtsDataDynamoDbRepository
    {
        Task BatchInsertAsync(IEnumerable<RtsData> entities);
    }
}