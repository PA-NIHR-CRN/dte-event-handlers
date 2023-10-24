using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace ScheduledJobs.Contracts
{
    public interface ICsvUtilities
    {
        string WriteCsvString<T>(IEnumerable<T> records);
        IEnumerable<TResult> ParseStringCsvContent<TMapping, TResult>(string content) where TMapping : ClassMap;
        Task WriteCsvToStreamAsync<T>(IAsyncEnumerable<T> participants, Stream ms,
            CancellationToken cancellationToken = default);
    }
}
