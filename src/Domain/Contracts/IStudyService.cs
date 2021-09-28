using System.Threading.Tasks;
using Domain.Aggregates.Entities;

namespace Domain.Contracts
{
    public interface IStudyService
    {
        Task<Study> GetStudyRegistration(string studyId);
        Task SaveStudyRegistration(Study study);
    }
}