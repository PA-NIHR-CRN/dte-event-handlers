using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Dte.Common.Lambda.Extensions;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Models;
using ScheduledJobs.Services;
using ScheduledJobs.Settings;

namespace ScheduledJobs.JobHandlers
{
    // Reference class for handler generic type param
    public class CpmsImport { }
    
    public class CpmsImportJobHandler : IHandler<CpmsImport, bool>
    {
        private readonly IS3Service _s3Service;
        private readonly ICsvFileReader _csvFileReader;
        private readonly ICpmsStudyDynamoDbRepository _repository;
        private readonly CpmsImportSettings _cpmsImportSettings;
        private readonly ILogger<CpmsImportJobHandler> _logger;
        
        private const int DefaultBatchSize = 1000;
        private readonly string _archiveFolderName;

        public CpmsImportJobHandler(IS3Service s3Service, ICsvFileReader csvFileReader, ICpmsStudyDynamoDbRepository repository, CpmsImportSettings cpmsImportSettings, ILogger<CpmsImportJobHandler> logger)
        {
            _s3Service = s3Service;
            _csvFileReader = csvFileReader;
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
            
            await _s3Service.CreateBucketIfNotExistsAsync(_cpmsImportSettings.ArchiveS3BucketName);
            
            var files = await _s3Service.GetFileContentsAsync(_cpmsImportSettings.S3BucketName);
            _logger.LogInformation($"*** Elapsed time taken to get files from S3: {sw.Elapsed}");

            var totalRecords = 0;
            foreach (var file in files)
            {
                var batchedStudies = _csvFileReader.ParseStringCsvContent<CpmsStudyMap, CpmsStudy>(file.Content).GetByBatch(batchSize);

                var batchNumber = 1;
                foreach (var studies in batchedStudies)
                {
                    var cpmsStudies = studies.ToList();
                    _logger.LogInformation($"Processing - {file.Name} - Batch ({batchNumber}) - Parsed {cpmsStudies.Count} records");

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

                await _s3Service.MoveObjectAsync(_cpmsImportSettings.S3BucketName, file.Name, _cpmsImportSettings.ArchiveS3BucketName, $"{_archiveFolderName}/{file.Name}", false);
            }
            
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} FINISHED in {sw.Elapsed} - {totalRecords} records processed");

            return true;
        }
    }
}