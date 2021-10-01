using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Adapter;
using Adapter.Contracts;
using Adapter.Mappers;
using Domain.Events;
using Infrastructure.Services.Fakes;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using InMemoryDomainRepository = Tests.Fakes.InMemoryDomainRepository;

namespace Tests
{
    public class WorkerProcessingTests
    {
        private IList<ICloudEventMapper> _mappers;

        [SetUp]
        public void SetUp()
        {
            _mappers = new List<ICloudEventMapper>
            {
                new CompleteStepMapper(), 
                new SubmitStudyForApprovalMapper(),
                new ExpressInterestMapper()
            };
        }
        
        [Test]
        public void given_valid_submitstudyforapproval_I_expect_to_process_the_cloudevent_and_raise_studysubmitted_event()
        {
            // Assign
            var domainRepo = new InMemoryDomainRepository();
            var cloudRequest = JsonSerializer.Deserialize<CloudEvent>(File.ReadAllText("./PayloadSamples/submitStudyForApproval.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var sut = new Worker(domainRepo, new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())),
                _mappers, new Logger<Worker>(new LoggerFactory()));

            // Act
            sut.Process(cloudRequest);

            // Assert
            Assert.IsTrue(domainRepo.EventStore.Single().Value.Count == 1);
            Assert.IsTrue(domainRepo.EventStore.Single().Value[0] is StudyWaitingForApprovalSubmittedV1);
        }
        
        [Test]
        public void given_valid_expressinterest_command_I_expect_to_process_and_raise_interestexpressed_event()
        {
            // Assign
            var domainRepo = new InMemoryDomainRepository();
            var cloudRequest = JsonSerializer.Deserialize<CloudEvent>(File.ReadAllText("./PayloadSamples/dte-web-expressinterest.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var sut = new Worker(domainRepo, new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())),
                _mappers, new Logger<Worker>(new LoggerFactory()));

            // Act
            sut.Process(cloudRequest);

            // Assert
            Assert.IsTrue(domainRepo.EventStore.Single().Value.Count == 1);
            Assert.IsTrue(domainRepo.EventStore.Single().Value[0] is InterestExpressedV1);
        }
    }
}