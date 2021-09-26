using System.Threading.Tasks;
using Domain.Aggregates.Entities;

namespace Domain.Services
{
    public interface IStudyService
    {
        Task<Study> Get(string id);
        Task Set(Study study);
    }
}