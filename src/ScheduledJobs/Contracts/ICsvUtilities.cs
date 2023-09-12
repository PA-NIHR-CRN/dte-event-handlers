using System.Collections.Generic;
using System.IO;
using CsvHelper.Configuration;
using ScheduledJobs.Models;

namespace ScheduledJobs.Contracts
{
    public interface ICsvUtilities
    {
        string WriteCsvString<T>(IEnumerable<T> records);
        IEnumerable<TResult> ParseStringCsvContent<TMapping, TResult>(string content) where TMapping : ClassMap;
        void WriteCsvToStream<T>(IEnumerable<T> participants, MemoryStream ms);
    }
}