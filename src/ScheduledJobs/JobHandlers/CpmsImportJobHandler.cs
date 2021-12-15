using System;
using System.Diagnostics;
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
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Models;
using ScheduledJobs.Settings;

namespace ScheduledJobs.JobHandlers
{
    // Reference class for handler generic type param
    public class CpmsImport { }
    
    public class CpmsImportJobHandler : IHandler<CpmsImport, bool>
    {
        private readonly IAmazonS3 _client;
        private readonly ICpmsStudyDynamoDbRepository _repository;
        private readonly CpmsImportSettings _cpmsImportSettings;
        private readonly ILogger<CpmsImportJobHandler> _logger;
        
        private const int DefaultBatchSize = 1000;
        private readonly string _archiveFolderName;

        public CpmsImportJobHandler(IAmazonS3 client, ICpmsStudyDynamoDbRepository repository, CpmsImportSettings cpmsImportSettings, ILogger<CpmsImportJobHandler> logger)
        {
            _client = client;
            _repository = repository;
            _cpmsImportSettings = cpmsImportSettings;
            _logger = logger;
            
            _archiveFolderName= $"{DateTime.Now:yyyy-MM-dd--HH-mm-ss}";
        }
        
        public async Task<bool> HandleAsync(CpmsImport source)
        {
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} STARTED");
            var sw = Stopwatch.StartNew();
            
            var batchSize = int.TryParse(_cpmsImportSettings.GetRecordBatchSize, out var parsed) ? parsed : DefaultBatchSize;
            _logger.LogInformation($"BatchSize set to: {batchSize}");
            
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
            _logger.LogInformation($"*** Elapsed time taken to get files from S3: {sw.Elapsed}");

            var totalRecords = 0;
            foreach (var (filename, content) in results)
            {
                using var csv = new CsvReader(new StringReader(content), CultureInfo.InvariantCulture);

                csv.Context.RegisterClassMap<CpmsStudyMap>();
                var byBatch = csv.GetRecords<CpmsStudy>().GetByBatch(batchSize);

                var batchNumber = 1;
                foreach (var batch in byBatch)
                {
                    var cpmsStudies = batch.ToList();
                    _logger.LogInformation($"Processing - {filename} - Batch ({batchNumber}) - Parsed {cpmsStudies.Count} records");

                    await _repository.BatchInsertCpmsStudyAsync(cpmsStudies.Select(x => new CpmsStudy
                    {
                        Pk = Guid.NewGuid().ToString(), Sk = "STUDY#",
                        Country = x.Country,
                        Region = x.Region,
                        ItemType = x.ItemType
                    }));
                    
                    batchNumber++;
                    totalRecords += cpmsStudies.Count;
                }

                await MoveObjectAsync(_cpmsImportSettings.S3BucketName, filename, _cpmsImportSettings.ArchiveS3BucketName, $"{_archiveFolderName}/{filename}", false);
            }
            
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} FINISHED in {sw.Elapsed} - {totalRecords} records processed");

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
}