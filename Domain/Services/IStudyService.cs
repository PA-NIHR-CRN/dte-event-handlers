using System.Threading.Tasks;
using Domain.Aggregates.Entities;

namespace Domain.Services
{
    public interface IStudyService
    {
        Task Set(Study study);
        Study Get(string id);
    }
}