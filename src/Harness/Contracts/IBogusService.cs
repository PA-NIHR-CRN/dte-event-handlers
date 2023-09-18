using ScheduledJobs.Domain;

namespace Harness.Contracts;

public interface IBogusService
{
    IEnumerable<Participant> GenerateFakeUsers(int count);
}