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
    public class ParticipantOdpExport { }

    public class ParticipantOdpExportJobHandler : IHandler<ParticipantOdpExport, bool>
    {
        private readonly IS3Service _s3Service;
        private readonly ICsvUtilities _csvUtilities;
        private readonly IParticipantRegistrationDynamoDbRepository _repository;
        private readonly AwsSettings _awsSettings;
        private readonly ParticipantOdpExportSettings _participantOdpExportSettings;
        private readonly ILogger<ParticipantOdpExportJobHandler> _logger;

        public ParticipantOdpExportJobHandler(IS3Service s3Service, ICsvUtilities csvUtilities, IParticipantRegistrationDynamoDbRepository repository, AwsSettings awsSettings, ParticipantOdpExportSettings participantOdpExportSettings, ILogger<ParticipantOdpExportJobHandler> logger)
        {
            _s3Service = s3Service;
            _csvUtilities = csvUtilities;
            _repository = repository;
            _awsSettings = awsSettings;
            _participantOdpExportSettings = participantOdpExportSettings;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(ParticipantOdpExport source)
        {
            _logger.LogInformation($"**** Getting files names from bucket: {_participantOdpExportSettings.S3BucketName} with DynamoDB table name: {_awsSettings.ParticipantRegistrationDynamoDbTableName}");

            var sw = Stopwatch.StartNew();

            try
            {
                var participants = await _repository.GetAllAsync();

                var csv = _csvUtilities.WriteCsvString(participants.Select(ParticipantMapper.MapToParticipantOdpExportModel));

                var fileName = $"participant-odp-export-{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.csv";

                await _s3Service.SaveStringContentAsync(_participantOdpExportSettings.S3BucketName, fileName, csv);

                _logger.LogInformation($"{nameof(ParticipantOdpExportJobHandler)} FINISHED in {sw.Elapsed}");

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"ERROR {nameof(ParticipantOdpExportJobHandler)}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR {nameof(ParticipantOdpExportJobHandler)}: {ex.GetType().Name}: {ex.Message}: {ex.StackTrace}");
                return false;
            }
        }
    }
}