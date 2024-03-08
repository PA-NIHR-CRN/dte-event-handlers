using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduledJobs.Contracts
{
    public interface ICsvUtilities
    {
        Task WriteCsvToStreamAsync<T>(IAsyncEnumerable<T> participants, Stream ms,
            CancellationToken cancellationToken = default);
    }
}
