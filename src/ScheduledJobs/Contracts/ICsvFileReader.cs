using System.Collections.Generic;
using CsvHelper.Configuration;

namespace ScheduledJobs.Contracts
{
    public interface ICsvFileReader
    {
        IEnumerable<TResult> ParseStringCsvContent<TMapping, TResult>(string content) where TMapping : ClassMap;
    }
}