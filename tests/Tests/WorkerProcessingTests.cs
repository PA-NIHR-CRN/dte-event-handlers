using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Adapter;
using Adapter.Fakes;
using Adapter.Mappers;
using Domain.Events;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using InMemoryDomainRepository = Tests.Fakes.InMemoryDomainRepository;

namespace Tests
{
    public class WorkerProcessingTests
    {
        [Test]
        public void given_valid_submitstudyforapproval_I_expect_to_process_the_cloudevent_and_raise_studysubmitted_event()
        {
            // Assign
            var domainRepo = new InMemoryDomainRepository();
            var cloudRequest = JsonSerializer.Deserialize<CloudEvent>(File.ReadAllText("./PayloadSamples/submitStudyForApproval.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var mappers = new List<ICloudEventMapper>{new CompleteStepMapper(), new SubmitStudyForApprovalMapper()};
            
            var sut = new Worker(domainRepo, new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())), mappers, new Logger<Worker>(new LoggerFactory()));

            // Act
            sut.Process(cloudRequest);

            // Assert
            Assert.IsTrue(domainRepo.EventStore.Single().Value.Count == 1);
            Assert.IsTrue(domainRepo.EventStore.Single().Value[0] is StudyWaitingForApprovalSubmittedV1);
        }
    }
}