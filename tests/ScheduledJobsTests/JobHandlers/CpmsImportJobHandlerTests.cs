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
using ScheduledJobs.Settings;

namespace ScheduledJobsTests.JobHandlers
{
    [TestFixture]
    public class CpmsImportJobHandlerTests
    {
        private IS3Service _s3Service;
        private ICsvFileReader _csvFileReader;
        private ICpmsStudyDynamoDbRepository _repository;
        private CpmsImportSettings _cpmsImportSettings;

        private CpmsImportJobHandler _handler;

        [SetUp]
        public void SetUp()
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
        
        [TestCase(1, 4)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(4, 1)]
        public async Task HandleAsync_Inserts_Batches(int batchSize, int expectedInserts)
        {
            _cpmsImportSettings.GetRecordBatchSize = batchSize.ToString();
            
            var expectedS3Files = new List<S3FileContent>
            {
                new S3FileContent { Name = "File1", Content = "Content1" }
            };

            var expectedStudies = new List<CpmsStudy>
            {
                new CpmsStudy { Country = "Study1" },
                new CpmsStudy { Country = "Study2" },
                new CpmsStudy { Country = "Study3" },
                new CpmsStudy { Country = "Study4" },
            };
            
            _s3Service.GetFileContentsAsync(_cpmsImportSettings.S3BucketName).Returns(expectedS3Files);
            _csvFileReader.ParseStringCsvContent<CpmsStudyMap, CpmsStudy>(expectedS3Files[0].Content).Returns(expectedStudies);

            var result = await _handler.HandleAsync(new CpmsImport());

            result.Should().BeTrue();

            await _s3Service.Received(1).CreateBucketIfNotExistsAsync(_cpmsImportSettings.ArchiveS3BucketName);
            await _repository.Received(expectedInserts).BatchInsertCpmsStudyAsync(Arg.Any<IEnumerable<CpmsStudy>>());
            await _s3Service.Received(1).MoveObjectAsync(_cpmsImportSettings.S3BucketName, expectedS3Files[0].Name, _cpmsImportSettings.ArchiveS3BucketName, Arg.Is<string>(x => x.EndsWith(expectedS3Files[0].Name)), false);
        }
        
        [Test]
        public async Task HandleAsync_Inserts_Batch_For_Multiple_Files()
        {
            var expectedS3Files = new List<S3FileContent>
            {
                new S3FileContent { Name = "File1", Content = "Content1" },
                new S3FileContent { Name = "File2", Content = "Content2" },
                new S3FileContent { Name = "File3", Content = "Content3" }
            };

            var expectedStudies = new List<CpmsStudy>
            {
                new CpmsStudy { Country = "Study1" }
            };
            
            _s3Service.GetFileContentsAsync(_cpmsImportSettings.S3BucketName).Returns(expectedS3Files);
            _csvFileReader.ParseStringCsvContent<CpmsStudyMap, CpmsStudy>(expectedS3Files[0].Content).Returns(expectedStudies);
            _csvFileReader.ParseStringCsvContent<CpmsStudyMap, CpmsStudy>(expectedS3Files[1].Content).Returns(expectedStudies);
            _csvFileReader.ParseStringCsvContent<CpmsStudyMap, CpmsStudy>(expectedS3Files[2].Content).Returns(expectedStudies);

            var result = await _handler.HandleAsync(new CpmsImport());

            result.Should().BeTrue();

            await _s3Service.Received(1).CreateBucketIfNotExistsAsync(_cpmsImportSettings.ArchiveS3BucketName);
            await _repository.Received(expectedS3Files.Count).BatchInsertCpmsStudyAsync(Arg.Is<IEnumerable<CpmsStudy>>(x => x.Count() == 1 && x.All(y => y.Country == "Study1")));
            await _s3Service.Received(1).MoveObjectAsync(_cpmsImportSettings.S3BucketName, expectedS3Files[0].Name, _cpmsImportSettings.ArchiveS3BucketName, Arg.Is<string>(x => x.EndsWith(expectedS3Files[0].Name)), false);
            await _s3Service.Received(1).MoveObjectAsync(_cpmsImportSettings.S3BucketName, expectedS3Files[1].Name, _cpmsImportSettings.ArchiveS3BucketName, Arg.Is<string>(x => x.EndsWith(expectedS3Files[1].Name)), false);
            await _s3Service.Received(1).MoveObjectAsync(_cpmsImportSettings.S3BucketName, expectedS3Files[2].Name, _cpmsImportSettings.ArchiveS3BucketName, Arg.Is<string>(x => x.EndsWith(expectedS3Files[2].Name)), false);
        }

        [TearDown]
        public void TearDown()
        {
            _s3Service.ClearReceivedCalls();
            _repository.ClearReceivedCalls();
        }
    }
}