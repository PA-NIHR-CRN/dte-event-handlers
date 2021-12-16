using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface IS3Service
    {
        Task CreateBucketIfNotExistsAsync(string bucketName);
        Task<IEnumerable<S3FileContent>> GetFileContentsAsync(string bucketName, string prefix = "", int limit = 100);
        Task MoveObjectAsync(string srcBucket, string srcKey, string destBucket, string destKey, bool deleteOriginal);
    }
}