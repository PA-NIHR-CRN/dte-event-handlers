using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Mappers;
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

        public ParticipantExportJobHandler(IS3Service s3Service, ICsvUtilities csvUtilities,
            IParticipantRegistrationDynamoDbRepository repository, AwsSettings awsSettings,
            ParticipantExportSettings participantExportSettings, ILogger<ParticipantExportJobHandler> logger)
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
            _logger.LogInformation(
                $"**** Getting files names from bucket: {_participantExportSettings.S3BucketName} with DynamoDB table name: {_awsSettings.ParticipantRegistrationDynamoDbTableName}");

            return await HandleExport(
                ParticipantMapper.MapToParticipantExportModel,
                "participant-export",
                _participantExportSettings.S3BucketName
            );
        }

        private async Task<bool> HandleExport<TModel>(Func<Participant, TModel> mapper, string exportType,
            string bucketName)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var mappedParticipants = _repository.GetAllMappedAsync(mapper);
                using var ms = new MemoryStream();

                await _csvUtilities.WriteCsvToStreamAsync(mappedParticipants, ms);

                ms.Position = 0;

                var fileName = $"{exportType}-{DateTime.Now:yyyy-MM-dd--HH-mm-ss}.csv";

                await _s3Service.SaveStreamContentAsync(bucketName, fileName, ms);

                _logger.LogInformation($"Export {exportType} FINISHED in {sw.Elapsed}");

                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"ERROR {nameof(HandleExport)}: {ex.GetType().Name}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"EXPORT ERROR {nameof(HandleExport)}: {ex.GetType().Name}: {ex.Message}: {ex.StackTrace}");
                return false;
            }
        }
    }
}