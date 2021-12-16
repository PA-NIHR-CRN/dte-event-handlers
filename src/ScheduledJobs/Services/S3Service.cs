using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Models;

namespace ScheduledJobs.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _client;
        private readonly ILogger<S3Service> _logger;

        public S3Service(IAmazonS3 client, ILogger<S3Service> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task CreateBucketIfNotExistsAsync(string bucketName)
        {
            if (!await _client.DoesS3BucketExistAsync(bucketName))
            {
                _logger.LogInformation($"Archive bucket does not exist - Creating S3 bucket {bucketName}");
                await _client.EnsureBucketExistsAsync(bucketName);
            }
        }

        public async Task<IEnumerable<S3FileContent>> GetFileContentsAsync(string bucketName, string prefix = "", int limit = 100)
        {
            var listObjects = await _client.ListObjectsAsync(new ListObjectsRequest
            {
                BucketName = bucketName, Prefix = prefix, MaxKeys = limit
            });
	
            var tasks = listObjects.S3Objects.Where(x => x.Size > 0).Select(x => GetFileAsync(bucketName, x.Key));

            return await Task.WhenAll(tasks);
        }

        public async Task MoveObjectAsync(string srcBucket, string srcKey, string destBucket, string destKey, bool deleteOriginal)
        {
            var copyResponse = await _client.CopyObjectAsync(new CopyObjectRequest
            {
                SourceBucket = srcBucket,
                SourceKey = srcKey,
                DestinationBucket = destBucket,
                DestinationKey = destKey
            });
            
            if(!(copyResponse.HttpStatusCode >= HttpStatusCode.OK && copyResponse.HttpStatusCode < HttpStatusCode.MultipleChoices))
            {
                _logger.LogError($"Could not copy original file from {srcBucket}/{srcKey} to {destBucket}");
                return;
            }

            if(deleteOriginal)
            {
                var deleteResponse = await _client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = srcBucket,
                    Key = srcKey
                });
                
                if(!(deleteResponse.HttpStatusCode >= HttpStatusCode.OK && deleteResponse.HttpStatusCode < HttpStatusCode.MultipleChoices))
                {
                    _logger.LogError($"Could not delete original file from {srcBucket}/{srcKey}");
                }
            }
        }

        public async Task<S3FileContent> GetFileAsync(string path, string fileName)
        {
            try
            {
                using var response = await _client.GetObjectAsync(path, fileName);
                await using var responseStream = response.ResponseStream;
                using var reader = new StreamReader(responseStream);

                return new S3FileContent { Name = fileName, Content = await reader.ReadToEndAsync()};
            }
            catch (AmazonS3Exception ex)
            {
                // If bucket or object does not exist
                _logger.LogError(ex, $"S3 Error encountered ***. Message:'{ex.Message}' when reading object");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UNKNOWN Error encountered ***. Message:'{ex.Message}' when reading object");
                throw;
            }
        }
    }
}