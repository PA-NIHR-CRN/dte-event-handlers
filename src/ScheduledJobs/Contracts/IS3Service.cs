using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface IS3Service
    {
        Task CreateBucketIfNotExistsAsync(string bucketName);

        Task<IEnumerable<S3FileContentModel>> GetFileContentsAsync(string bucketName, string prefix = "",
            int limit = 100, CancellationToken cancellationToken = default);

        Task MoveObjectAsync(string srcBucket, string srcKey, string destBucket, string destKey, bool deleteOriginal,
            CancellationToken cancellationToken = default);

        Task<S3FileContentModel> GetFileAsync(string bucketName, string key,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetFilesNamesAsync(string bucketName, string prefix,
            CancellationToken cancellationToken = default);

        Task SaveStringContentAsync(string bucketName, string key, string content,
            CancellationToken cancellationToken = default);

        Task DeleteFilesAsync(string bucketName, IEnumerable<string> fileNames,
            CancellationToken cancellationToken = default);

        Task SaveStreamContentAsync(string s3BucketName, string fileName, Stream ms,
            CancellationToken cancellationToken = default);
    }
}