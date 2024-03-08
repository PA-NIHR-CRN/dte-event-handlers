using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using ScheduledJobs.Services;

namespace ScheduledJobsTests.Services
{
    [TestFixture]
    public class S3ServiceTests
    {
        private IAmazonS3 _client;
        
        private S3Service _s3Service;
        
        [SetUp]
        public void Setup()
        {
            _client = Substitute.For<IAmazonS3>();
            
            _s3Service = new S3Service(_client, new NullLogger<S3Service>());
        }

        [Test]
        public async Task CreateBucketIfNotExistsAsync_Creates_Bucket()
        {
            const string bucketName = "SomeBucket";
            _client.DoesS3BucketExistAsync(bucketName).Returns(false);
            
            await _s3Service.CreateBucketIfNotExistsAsync(bucketName);

            await _client.Received(1).EnsureBucketExistsAsync(bucketName);
        }
        
        [Test]
        public async Task CreateBucketIfNotExistsAsync_Does_Not_Create_Bucket()
        {
            const string bucketName = "SomeBucket";
            _client.DoesS3BucketExistAsync(bucketName).Returns(true);
            
            await _s3Service.CreateBucketIfNotExistsAsync(bucketName);

            await _client.DidNotReceive().EnsureBucketExistsAsync(bucketName);
        }

        [Test]
        public async Task GetFileContentsAsync_Returns_S3Files()
        {
            const string bucketName = "SomeBucket";

            var listObjectsResponse = new ListObjectsResponse
            {
                S3Objects = new List<S3Object>
                {
                    new S3Object{ Size = 1, Key = "SomeKey1"}, 
                    new S3Object{ Size = 1, Key = "SomeKey2"}
                }
            };
            
            _client.ListObjectsAsync(Arg.Any<ListObjectsRequest>()).Returns(listObjectsResponse);

            _client.GetObjectAsync(bucketName, "SomeKey1").Returns(new GetObjectResponse{ ResponseStream = new MemoryStream(Encoding.Default.GetBytes("some content1")) });
            _client.GetObjectAsync(bucketName, "SomeKey2").Returns(new GetObjectResponse{ ResponseStream = new MemoryStream(Encoding.Default.GetBytes("some content2")) });

            var results = (await _s3Service.GetFileContentsAsync(bucketName)).ToList();

            results.Should().NotBeNull();
            results.Should().HaveCount(2);
            
            results.ElementAt(0).Name.Should().Be("SomeKey1");
            results.ElementAt(0).Content.Should().Be("some content1");
            
            results.ElementAt(1).Name.Should().Be("SomeKey2");
            results.ElementAt(1).Content.Should().Be("some content2");
        }

        [Test]
        public async Task MoveObjectAsync_Fails_Copy_Does_Not_Delete()
        {
            _client.CopyObjectAsync(Arg.Any<CopyObjectRequest>()).Returns(new CopyObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.InternalServerError
            });

            await _s3Service.MoveObjectAsync("", "", "", "", true);
            
            await _client.DidNotReceive().DeleteObjectAsync(Arg.Any<DeleteObjectRequest>());
        }
        
        [Test]
        public async Task MoveObjectAsync_Does_Not_Delete()
        {
            _client.CopyObjectAsync(Arg.Any<CopyObjectRequest>()).Returns(new CopyObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            });

            await _s3Service.MoveObjectAsync("", "", "", "", false);
            
            await _client.DidNotReceive().DeleteObjectAsync(Arg.Any<DeleteObjectRequest>());
        }
        
        [Test]
        public async Task MoveObjectAsync_Does_Delete()
        {
            _client.CopyObjectAsync(Arg.Any<CopyObjectRequest>()).Returns(new CopyObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            });

            _client.DeleteObjectAsync(Arg.Any<DeleteObjectRequest>()).Returns(new DeleteObjectResponse
            {
                HttpStatusCode = System.Net.HttpStatusCode.OK
            });

            await _s3Service.MoveObjectAsync("", "", "", "", true);
            
            await _client.Received(1).DeleteObjectAsync(Arg.Any<DeleteObjectRequest>());
        }

        [Test]
        public async Task GetFileAsync_Returns_S3File()
        {
            const string bucketName = "SomeBucket";
            const string filename = "file1.csv";
            
            var stringInMemoryStream = new MemoryStream(Encoding.Default.GetBytes("some content"));
            var getObjectResponse = new GetObjectResponse{ ResponseStream = stringInMemoryStream };
            _client.GetObjectAsync(bucketName, filename).Returns(getObjectResponse);

            var result = await _s3Service.GetFileAsync(bucketName, filename);

            result.Should().NotBeNull();
            result.Content.Should().Be("some content");
        }
        
        [Test]
        public void GetFileAsync_Throws_AmazonS3Exception()
        {
            const string bucketName = "SomeBucket";
            const string filename = "file1.csv";
            _client.GetObjectAsync(bucketName, filename).Throws(new AmazonS3Exception("Message"));

            Assert.ThrowsAsync<AmazonS3Exception>(() => _s3Service.GetFileAsync(bucketName, filename));
        }
        
        [Test]
        public void GetFileAsync_Throws_Exception()
        {
            const string bucketName = "SomeBucket";
            const string filename = "file1.csv";
            _client.GetObjectAsync(bucketName, filename).Throws(new Exception("Message"));

            Assert.ThrowsAsync<Exception>(() => _s3Service.GetFileAsync(bucketName, filename));
        }
    }
}
