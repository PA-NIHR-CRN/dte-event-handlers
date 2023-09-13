using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using ScheduledJobs.Domain;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface ICsvUtilities
    {
        string WriteCsvString<T>(IEnumerable<T> records);
        IEnumerable<TResult> ParseStringCsvContent<TMapping, TResult>(string content) where TMapping : ClassMap;
        Task WriteCsvToStreamAsync<T>(IAsyncEnumerable<T> participants, MemoryStream ms);
    }
}