using System;
using System.Threading.Tasks;
using Adapter.Contracts;
using Common.Interfaces;
using Domain.Aggregates.Entities;
using Domain.Contracts;
using Domain.Persistence.Models;

namespace Infrastructure.Services
{
    public class StudyService : IStudyService
    {
        private readonly IStudyRegistrationRepository _studyRegistrationRepository;
        private readonly IClock _clock;

        public StudyService(IStudyRegistrationRepository studyRegistrationRepository, IClock clock)
        {
            _studyRegistrationRepository = studyRegistrationRepository;
            _clock = clock;
        }

        public async Task SaveWaitingForApprovalStudy(Study study)
        {
            var model = new StudyRegistration
            {
                StudyId = long.Parse(study.Id),
                Title = study.Title,
                ShortName = study.ShortName,
                ApprovedAtUtc = study.StudyRegistrationStatus == StudyRegistrationStatus.Approved ? _clock.UtcNow() : (DateTime?)null,
                StudyRegistrationStatus = study.StudyRegistrationStatus,
                SubmissionResearcherId = study.SubmissionResearcherId,
                SubmittedAt = study.SubmittedAt
            };

            await _studyRegistrationRepository.SaveStudyRegistration(model);
        }
    }
}