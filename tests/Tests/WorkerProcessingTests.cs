using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Adapter;
using Adapter.Contracts;
using Adapter.Mappers;
using Common.Settings;
using Domain.CommandHandlers;
using Domain.Commands;
using Domain.Contracts;
using Domain.Events;
using Domain.Factories;
using Evento;
using Infrastructure.Services.Stubs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using InMemoryDomainRepository = Tests.Fakes.InMemoryDomainRepository;

namespace Tests
{
    public class WorkerProcessingTests
    {
        private IList<ICloudEventMapper> _mappers;
        private IServiceProvider _serviceProvider;
        private Worker _sut;
        private InMemoryDomainRepository _domainRepo;

        [SetUp]
        public void SetUp()
        {
            _mappers = new List<ICloudEventMapper>
            {
                new CompleteStepMapper(), 
                new SubmitStudyForApprovalMapper(),
                new ExpressInterestMapper(),
                new ApproveStudyRequestMapper(),
                new RejectStudyRequestMapper()
            };

            _serviceProvider = new ServiceCollection()
                .AddSingleton<IDomainRepository, InMemoryDomainRepository>()
                .AddTransient<IStudyService>(provider => new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())))
                .AddTransient<IHandle<CompleteStep>, CompleteStepHandler>()
                .AddTransient<IHandle<SubmitStudyForApproval>, SubmitStudyForApprovalHandler>()
                .AddTransient<IHandle<ExpressInterest>, ExpressInterestHandler>()
                .AddTransient<IHandle<ApproveStudyCommand>, ApproveStudyCommandHandler>()
                .AddTransient<IHandle<RejectStudyCommand>, RejectStudyCommandHandler>()
                .BuildServiceProvider();
            
            _domainRepo = _serviceProvider.GetService<IDomainRepository>() as InMemoryDomainRepository;
            var commandExecutor = new CommandExecutor(_serviceProvider);
            _sut = new Worker(_domainRepo, new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())), _mappers, new NullLogger<Worker>(), new AppSettings(), commandExecutor);
        }
        
        [Test]
        public void given_valid_submitstudyforapproval_I_expect_to_process_the_cloudevent_and_raise_studysubmitted_event()
        {
            // Assign

            var cloudRequest = ReadCloudEvent("./PayloadSamples/dte-web-submitstudyforapproval.json");

            // Act
            _sut.Process(cloudRequest);

            // Assert
            Assert.IsTrue(_domainRepo.EventStore.Single().Value.Count == 1);
            Assert.IsAssignableFrom<StudyWaitingForApprovalSubmittedV1>(_domainRepo.EventStore.Single().Value[0]);
        }
        
        [Test]
        public void given_valid_expressinterest_command_I_expect_to_process_and_raise_interestexpressed_event()
        {
            // Assign
            var cloudRequest = ReadCloudEvent("./PayloadSamples/dte-web-expressinterest.json");

            // Act
            _sut.Process(cloudRequest);

            // Assert
            Assert.IsTrue(_domainRepo.EventStore.Single().Value.Count == 1);
            Assert.IsAssignableFrom<InterestExpressedV1>(_domainRepo.EventStore.Single().Value[0]);
        }
        
        [Test]
        public void given_valid_approvestudy_command_I_expect_to_process_and_raise_approvingstudy_event()
        {
            // Assign
            var cloudRequests = ReadCloudEvents("./PayloadSamples/Study/submit_study_and_approve.json");

            // Act
            cloudRequests.ToList().ForEach(cloudRequest => _sut.Process(cloudRequest));

            // Assert
            Assert.AreEqual(cloudRequests.Count, _domainRepo.EventStore.Single().Value.Count);
            Assert.IsAssignableFrom<StudyWaitingForApprovalSubmittedV1>(_domainRepo.EventStore.Single().Value[0]);
            Assert.IsAssignableFrom<StudyApprovedV1>(_domainRepo.EventStore.Single().Value[1]);
        }
        
        [Test]
        public void given_valid_rejectstudy_command_I_expect_to_process_and_raise_rejectingstudy_event()
        {
            // Assign
            var cloudRequests = ReadCloudEvents("./PayloadSamples/Study/submit_study_and_reject.json");

            // Act
            cloudRequests.ToList().ForEach(cloudRequest => _sut.Process(cloudRequest));

            // Assert
            Assert.AreEqual(cloudRequests.Count, _domainRepo.EventStore.Single().Value.Count);
            Assert.IsAssignableFrom<StudyWaitingForApprovalSubmittedV1>(_domainRepo.EventStore.Single().Value[0]);
            Assert.IsAssignableFrom<StudyRejectedV1>(_domainRepo.EventStore.Single().Value[1]);
        }

        // [Test]
        // [Category("Integration")]
        public void given_valid_expressinterest_command_I_expect_to_process_and_raise_interestexpressed_event_for_real()
        {
            // Assign
            var cloudRequest = ReadCloudEvent("./PayloadSamples/dte-web-expressinterest.json");

            var appSettings = new AppSettings();
            var eventStoreSettings = new EventStoreSettings();
            var commandExecutor = new CommandExecutor(_serviceProvider);
            
            var sut = new Worker(new DomainRepositoryBuilder(appSettings, eventStoreSettings).Build(), new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())),
                _mappers, new NullLogger<Worker>(), new AppSettings(), commandExecutor);
        
            // Act
            sut.Process(cloudRequest);
        
            // Assert
            Assert.Inconclusive("check in the db...");
        }

        private static CloudEvent ReadCloudEvent(string filePath)
        {
            return JsonSerializer.Deserialize<CloudEvent>(File.ReadAllText(filePath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        
        private static List<CloudEvent> ReadCloudEvents(string filePath)
        {
            return JsonSerializer.Deserialize<List<CloudEvent>>(File.ReadAllText(filePath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}