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
        private readonly ICsvUtilities _csvUtilities;
        private readonly IParticipantRegistrationDynamoDbRepository _repository;
        private readonly AwsSettings _awsSettings;
        private readonly ParticipantExportSettings _participantExportSettings;
        private readonly ILogger<ParticipantExportJobHandler> _logger;

        private const int DefaultBatchSize = 1000;

        public ParticipantExportJobHandler(IS3Service s3Service, ICsvUtilities csvUtilities, IParticipantRegistrationDynamoDbRepository repository, AwsSettings awsSettings, ParticipantExportSettings participantExportSettings, ILogger<ParticipantExportJobHandler> logger)
        {
            _s3Service = s3Service;
            _csvUtilities = csvUtilities;
            _repository = repository;
            _awsSettings = awsSettings;
            _participantExportSettings = participantExportSettings;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(ParticipantExport source)
        {
            _logger.LogInformation($"**** Getting files names from bucket: {_participantExportSettings.S3BucketName} with DynamoDB table name: {_awsSettings.ParticipantRegistrationDynamoDbTableName}");

            var sw = Stopwatch.StartNew();

            try
            {
                var participants = await _repository.GetAllAsync();
                
                var csv = _csvUtilities.WriteCsvString(participants);
                
                _logger.LogInformation(csv);
                
                var fileName = $"participant-export-{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.csv";

                await _s3Service.SaveStringContentAsync(_participantExportSettings.S3BucketName, fileName, csv);

                var oldExistingFiles = await _s3Service.GetFilesNamesAsync(_participantExportSettings.S3BucketName, "participant-export");
                await _s3Service.DeleteFilesAsync(_participantExportSettings.S3BucketName, oldExistingFiles.Except(new[] { fileName }));

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