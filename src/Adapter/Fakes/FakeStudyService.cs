using System.Threading.Tasks;
using Domain.Aggregates.Entities;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Adapter.Fakes
{
    public class FakeStudyService : IStudyService
    {
        private readonly ILogger<FakeStudyService> _logger;
        private Study _study;

        public FakeStudyService(ILogger<FakeStudyService> logger)
        {
            _logger = logger;
        }

        public async Task<Study> Get(string id)
        {
            return await Task.FromResult(_study);
        }

        public Task Set(Study study)
        {
            _logger.LogInformation($"Saving study: {study.Id}");
            _study = study;
            return Task.CompletedTask;
        }
    }
}