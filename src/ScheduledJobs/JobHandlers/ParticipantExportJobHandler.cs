using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Settings;

namespace ScheduledJobs.JobHandlers
{
    // Reference class for handler generic type param
    public class ParticipantExport
    {
    }

    public class ParticipantExportJobHandler : IHandler<ParticipantExport, bool>
    {
        private readonly IS3Service _s3Service;
        private readonly ICsvFileReader _csvFileReader;
        private readonly ICpmsStudyDynamoDbRepository _repository;
        private readonly ParticipantExportSettings _participantExportSettings;
        private readonly ILogger<ParticipantExportJobHandler> _logger;

        private const int DefaultBatchSize = 1000;

        public ParticipantExportJobHandler(IS3Service s3Service, ICsvFileReader csvFileReader, ICpmsStudyDynamoDbRepository repository, ParticipantExportSettings participantExportSettings, ILogger<ParticipantExportJobHandler> logger)
        {
            _s3Service = s3Service;
            _csvFileReader = csvFileReader;
            _repository = repository;
            _participantExportSettings = participantExportSettings;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(ParticipantExport source)
        {
            _logger.LogInformation($"***** Bucket: {_participantExportSettings.S3BucketName} *****");
            var sw = Stopwatch.StartNew();

            try
            {
                var fileName = $"participant-{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.csv";

                // await _s3Service.SaveStringContentAsync(_participantExportSettings.S3BucketName, fileName, Guid.NewGuid().ToString());

                var oldExistingFiles = await _s3Service.GetFilesNamesAsync(_participantExportSettings.S3BucketName, "participant");
                // await _s3Service.DeleteFilesAsync(_participantExportSettings.S3BucketName, oldExistingFiles.Except(new[] { fileName }));

                _logger.LogInformation($"{nameof(ParticipantExportJobHandler)} FINISHED in {sw.Elapsed}");

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"ERROR: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR: {ex.GetType().Name}: {ex.Message}: {ex.StackTrace}");
                return false;
            }
        }
    }
}