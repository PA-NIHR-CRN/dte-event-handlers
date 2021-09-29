using System.Threading.Tasks;
using Domain.Persistence.Models;

namespace Adapter.Contracts
{
    public interface IStudyRepository
    {
        Task SaveStudyRegistration(StudyRegistration studyRegistration);
    }
}