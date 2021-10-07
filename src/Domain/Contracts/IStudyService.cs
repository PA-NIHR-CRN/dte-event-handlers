using System.Threading.Tasks;
using Domain.Aggregates.Entities;

namespace Domain.Contracts
{
    public interface IStudyService
    {
        Task SaveWaitingForApprovalStudy(Study study);
    }
}