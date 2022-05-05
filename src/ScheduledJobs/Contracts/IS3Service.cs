using System.Collections.Generic;
using System.Threading.Tasks;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface IS3Service
    {
        Task CreateBucketIfNotExistsAsync(string bucketName);
        Task<IEnumerable<S3FileContentModel>> GetFileContentsAsync(string bucketName, string prefix = "", int limit = 100);
        Task MoveObjectAsync(string srcBucket, string srcKey, string destBucket, string destKey, bool deleteOriginal);
        Task<S3FileContentModel> GetFileAsync(string bucketName, string key);
        Task<IEnumerable<string>> GetFilesNamesAsync(string bucketName, string prefix);
        Task SaveStringContentAsync(string bucketName, string key, string content);
        Task DeleteFilesAsync(string bucketName, IEnumerable<string> fileNames);
    }
}