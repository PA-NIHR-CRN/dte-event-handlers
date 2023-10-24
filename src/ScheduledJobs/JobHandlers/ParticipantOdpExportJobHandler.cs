using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Mappers;
using ScheduledJobs.Settings;

namespace ScheduledJobs.JobHandlers
{
    // Reference class for handler generic type param
    public class ParticipantOdpExport
    {
    }

    public class ParticipantOdpExportJobHandler : IHandler<ParticipantOdpExport, bool>
    {
        private readonly IS3Service _s3Service;
        private readonly ICsvUtilities _csvUtilities;
        private readonly IParticipantRegistrationDynamoDbRepository _repository;
        private readonly AwsSettings _awsSettings;
        private readonly ParticipantOdpExportSettings _participantOdpExportSettings;
        private readonly ILogger<ParticipantOdpExportJobHandler> _logger;

        public ParticipantOdpExportJobHandler(IS3Service s3Service, ICsvUtilities csvUtilities,
            IParticipantRegistrationDynamoDbRepository repository, AwsSettings awsSettings,
            ParticipantOdpExportSettings participantOdpExportSettings, ILogger<ParticipantOdpExportJobHandler> logger)
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
            _logger.LogInformation(
                "**** Getting files names from bucket: {S3BucketName} with DynamoDB table name: {AwsSettingsParticipantRegistrationDynamoDbTableName}",
                _participantOdpExportSettings.S3BucketName, _awsSettings.ParticipantRegistrationDynamoDbTableName);

            return await HandleExport(
                "participant-odp-export",
                _participantOdpExportSettings.S3BucketName
            );
        }

        private async Task<bool> HandleExport(string exportType, string bucketName,
            CancellationToken cancellationToken = default)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var participants = _repository.GetAllAsync(cancellationToken);
                using var ms = new MemoryStream();

                await _csvUtilities.WriteCsvToStreamAsync(
                    participants.Select(ParticipantMapper.MapToParticipantOdpExportModel), ms, cancellationToken);

                ms.Position = 0;

                var fileName = $"{exportType}-{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.csv";

                await _s3Service.SaveStreamContentAsync(bucketName, fileName, ms, cancellationToken);

                _logger.LogInformation("Export {ExportType} FINISHED in {SwElapsed}", exportType, sw.Elapsed);

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "ERROR {HandleExportName}: {Name}: {ExMessage}", nameof(HandleExport),
                    ex.GetType().Name, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EXPORT ERROR {HandleExportName}: {Name}: {ExMessage}: {ExStackTrace}",
                    nameof(HandleExport), ex.GetType().Name, ex.Message, ex.StackTrace);
                return false;
            }
        }
    }
}
