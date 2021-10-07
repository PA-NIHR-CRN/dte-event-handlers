﻿using System.Threading.Tasks;
using Domain.Aggregates.Entities;
using Domain.Contracts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Stubs
{
    public class FakeStudyService : IStudyService
    {
        private readonly ILogger<FakeStudyService> _logger;
        private Study _study;

        public FakeStudyService(ILogger<FakeStudyService> logger)
        {
            _logger = logger;
        }

        public Task SaveWaitingForApprovalStudy(Study study)
        {
            _logger.LogInformation($"Saving study: {study.Id}");
            _study = study;
            return Task.CompletedTask;
        }
    }
}