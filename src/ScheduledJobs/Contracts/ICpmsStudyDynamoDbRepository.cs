using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface ICpmsStudyDynamoDbRepository
    {
        Task BatchInsertCpmsStudyAsync(IEnumerable<CpmsStudy> entities);
    }
}