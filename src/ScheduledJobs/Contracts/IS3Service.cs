using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduledJobs.Contracts
{
    public interface IS3Service
    {
        Task SaveStreamContentAsync(string s3BucketName, string fileName, Stream ms,
            CancellationToken cancellationToken = default);
    }
}
