using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using ScheduledJobs.Contracts;
using ScheduledJobs.JobHandlers;
using ScheduledJobs.Models;
using ScheduledJobs.Services;
using ScheduledJobs.Settings;

namespace ScheduledJobsTests.JobHandlers
{
    [TestFixture]
    public class CpmsImportJobHandlerTests
    {
        private readonly IS3Service _s3Service;
        private readonly ICsvFileReader _csvFileReader;
        private readonly ICpmsStudyDynamoDbRepository _repository;
        private CpmsImportSettings _cpmsImportSettings;

        private CpmsImportJobHandler _handler;

        public CpmsImportJobHandlerTests()
        {
            _s3Service = Substitute.For<IS3Service>();
            _csvFileReader = Substitute.For<ICsvFileReader>();
            _repository = Substitute.For<ICpmsStudyDynamoDbRepository>();
            var logger = Substitute.For<ILogger<CpmsImportJobHandler>>();
            _cpmsImportSettings = new CpmsImportSettings
            {
                S3BucketName = "AnyBucket", ArchiveS3BucketName = "AnyArchiveBucket", GetRecordBatchSize = "1"
            };
            
            _handler = new CpmsImportJobHandler(_s3Service, _csvFileReader, _repository, _cpmsImportSettings, logger);
        }

        [Test]
        public async Task HandleAsync_Inserts_Batch()
        {
            var expectedS3Files = new List<S3FileContent>
            {
                new S3FileContent { Name = "File1", Content = "Content1" }
            };

            var expectedStudies = new List<CpmsStudy>
            {
                new CpmsStudy { Country = "Study1" }
            };
            
            _s3Service.GetFileContentsAsync(_cpmsImportSettings.S3BucketName).Returns(expectedS3Files);
            _csvFileReader.ParseStringCsvContent<CpmsStudyMap, CpmsStudy>(expectedS3Files[0].Content).Returns(expectedStudies);

            var result = await _handler.HandleAsync(new CpmsImport());

            result.Should().BeTrue();

            await _s3Service.Received(1).CreateBucketIfNotExistsAsync(_cpmsImportSettings.ArchiveS3BucketName);
            await _repository.Received(1).BatchInsertCpmsStudyAsync(Arg.Is<IEnumerable<CpmsStudy>>(x => x.Count() == 1 && x.All(y => y.Country == "Study1")));
            await _s3Service.Received(1).MoveObjectAsync(_cpmsImportSettings.S3BucketName, expectedS3Files[0].Name, _cpmsImportSettings.ArchiveS3BucketName, Arg.Is<string>(x => x.EndsWith(expectedS3Files[0].Name)), false);
        }
    }
}