using System.Collections.Generic;
using CsvHelper.Configuration;

namespace ScheduledJobs.Contracts
{
    public interface ICsvUtilities
    {
        string WriteCsvString<T>(IEnumerable<T> records);
        IEnumerable<TResult> ParseStringCsvContent<TMapping, TResult>(string content) where TMapping : ClassMap;
    }
}