using System.Threading.Tasks;
using Domain.Aggregates.Entities;
using Domain.Services;

namespace Adapter.Fakes
{
    public class FakeStudyService : IStudyService
    {
        private Study _study;
        public Task Set(Study study)
        {
            _study = study;
            return Task.CompletedTask;
        }

        public Study Get(string id)
        {
            return _study;
        }
    }
}