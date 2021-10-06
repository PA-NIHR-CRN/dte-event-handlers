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

        [SetUp]
        public void SetUp()
        {
            _mappers = new List<ICloudEventMapper>
            {
                new CompleteStepMapper(), 
                new SubmitStudyForApprovalMapper(),
                new ExpressInterestMapper()
            };

            _serviceProvider = new ServiceCollection()
                .AddTransient<IDomainRepository, InMemoryDomainRepository>()
                .AddTransient<IStudyService>(provider => new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())))
                .AddTransient<IHandle<CompleteStep>, CompleteStepHandler>()
                .AddTransient<IHandle<SubmitStudyForApproval>, SubmitStudyForApprovalHandler>()
                .AddTransient<IHandle<ExpressInterest>, ExpressInterestHandler>()
                .BuildServiceProvider();
        }
        
        [Test]
        public void given_valid_submitstudyforapproval_I_expect_to_process_the_cloudevent_and_raise_studysubmitted_event()
        {
            // Assign
            var domainRepo = new InMemoryDomainRepository();
            var cloudRequest = JsonSerializer.Deserialize<CloudEvent>(File.ReadAllText("./PayloadSamples/submitStudyForApproval.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var commandExecutor = new CommandExecutor(_serviceProvider, new Logger<CommandExecutor>(new LoggerFactory()));
            
            var sut = new Worker(domainRepo, new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())),
                _mappers, new NullLogger<Worker>(), new AppSettings(), commandExecutor);

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
            var commandExecutor = new CommandExecutor(_serviceProvider, new Logger<CommandExecutor>(new LoggerFactory()));
            
            var sut = new Worker(domainRepo, new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())),
                _mappers, new NullLogger<Worker>(), new AppSettings(), commandExecutor);

            // Act
            sut.Process(cloudRequest);

            // Assert
            Assert.IsTrue(domainRepo.EventStore.Single().Value.Count == 1);
            Assert.IsTrue(domainRepo.EventStore.Single().Value[0] is InterestExpressedV1);
        }
        
        // [Test]
        // [Category("Integration")]
        public void given_valid_expressinterest_command_I_expect_to_process_and_raise_interestexpressed_event_for_real()
        {
            // Assign
            var cloudRequest = JsonSerializer.Deserialize<CloudEvent>(File.ReadAllText("./PayloadSamples/dte-web-expressinterest.json"),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var appSettings = new AppSettings();
            var eventStoreSettings = new EventStoreSettings();
            var commandExecutor = new CommandExecutor(_serviceProvider, new Logger<CommandExecutor>(new LoggerFactory()));
            
            var sut = new Worker(new DomainRepositoryBuilder(appSettings, eventStoreSettings).Build(), new FakeStudyService(new Logger<FakeStudyService>(new LoggerFactory())),
                _mappers, new NullLogger<Worker>(), new AppSettings(), commandExecutor);
        
            // Act
            sut.Process(cloudRequest);
        
            // Assert
            Assert.Inconclusive("check in the db...");
        }
    }
}