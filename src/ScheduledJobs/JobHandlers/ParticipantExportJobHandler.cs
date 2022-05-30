using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Mappers;
using ScheduledJobs.Settings;

namespace ScheduledJobs.JobHandlers
{
    // Reference class for handler generic type param
    public class ParticipantExport { }

    public class ParticipantExportJobHandler : IHandler<ParticipantExport, bool>
    {
        private readonly IS3Service _s3Service;
        private readonly ICsvUtilities _csvUtilities;
        private readonly IParticipantRegistrationDynamoDbRepository _repository;
        private readonly AwsSettings _awsSettings;
        private readonly ParticipantExportSettings _participantExportSettings;
        private readonly ILogger<ParticipantExportJobHandler> _logger;

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

                var csv = _csvUtilities.WriteCsvString(participants.Select(ParticipantMapper.MapToParticipantExportModel));

                var fileName = $"participant-export-{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.csv";

                await _s3Service.SaveStringContentAsync(_participantExportSettings.S3BucketName, fileName, csv);

                _logger.LogInformation($"{nameof(ParticipantExportJobHandler)} FINISHED in {sw.Elapsed}");

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"ERROR {nameof(ParticipantExportJobHandler)}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR {nameof(ParticipantExportJobHandler)}: {ex.GetType().Name}: {ex.Message}: {ex.StackTrace}");
                return false;
            }
        }
    }
}