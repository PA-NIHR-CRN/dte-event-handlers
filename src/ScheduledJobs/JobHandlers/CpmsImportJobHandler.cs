using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Contracts;
using Application.Extensions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Jobs;
using ScheduledJobs.Settings;

namespace ScheduledJobs.JobHandlers
{
    public class CpmsImportJobHandler : IHandler<CpmsImport, bool>
    {
        private readonly IAmazonS3 _client;
        private readonly CpmsImportSettings _cpmsImportSettings;
        private readonly ILogger<CpmsImportJobHandler> _logger;

        public CpmsImportJobHandler(IAmazonS3 client, CpmsImportSettings cpmsImportSettings, ILogger<CpmsImportJobHandler> logger)
        {
            _client = client;
            _cpmsImportSettings = cpmsImportSettings;
            _logger = logger;
        }
        
        public async Task<bool> HandleAsync(CpmsImport source)
        {
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} STARTED");
            
            if (!await _client.DoesS3BucketExistAsync(_cpmsImportSettings.ArchiveS3BucketName))
            {
                _logger.LogInformation($"Archive bucket does not exist - Creating S3 bucket {_cpmsImportSettings.ArchiveS3BucketName}");
                await _client.EnsureBucketExistsAsync($"{_cpmsImportSettings.ArchiveS3BucketName}");
            }

            var listObjects = await _client.ListObjectsAsync(new ListObjectsRequest
            {
                BucketName = _cpmsImportSettings.S3BucketName,
                Prefix = "", // Add prefix to filter, case sensitive!
                MaxKeys = 100
            });
	
            var tasks = listObjects.S3Objects.Where(x => x.Size > 0).Select(x => GetFileAsync(_cpmsImportSettings.S3BucketName, x.Key));

            var results = await Task.WhenAll(tasks);

            foreach (var (filename, content) in results)
            {
                using var csv = new CsvReader(new StringReader(content), CultureInfo.InvariantCulture);

                csv.Context.RegisterClassMap<SiteMap>();
                var byBatch = csv.GetRecords<Site>().GetByBatch(_cpmsImportSettings.GetRecordBatchSize);

                var batchNumber = 1;
                foreach (var batch in byBatch)
                {
                    _logger.LogInformation($"Processing - {filename} - Batch ({batchNumber}) - Parsed {batch.Count()} records");
                    
                    // DO SOMETHING WITH THE BATCH
                    
                    batchNumber++;
                }

                await MoveObjectAsync(_cpmsImportSettings.S3BucketName, filename, _cpmsImportSettings.ArchiveS3BucketName, filename, false);
            }
            
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} FINISHED");

            return true;
        }
        
        private async Task<(string, string)> GetFileAsync(string path, string fileName)
        {
            try
            {
                using var response = await _client.GetObjectAsync(path, fileName);
                await using var responseStream = response.ResponseStream;
                using var reader = new StreamReader(responseStream);

                return (fileName, await reader.ReadToEndAsync());
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
    }

    public class Site
    {
        public string Region { get; set; }
        public string Country { get; set; }
        public string ItemType { get; set; }
    }

    public sealed class SiteMap : ClassMap<Site>
    {
        public SiteMap()
        {
            Map(m => m.Region).Name("Region");
            Map(m => m.Country).Name("Country");
            Map(m => m.ItemType).Name("Item Type");
        }
    }
}