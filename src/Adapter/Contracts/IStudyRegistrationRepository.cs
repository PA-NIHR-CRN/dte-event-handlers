using System.Threading.Tasks;
using Domain.Persistence.Models;

namespace Adapter.Contracts
{
    public interface IStudyRegistrationRepository
    {
        Task SaveStudyRegistration(StudyRegistration studyRegistration);
    }
}