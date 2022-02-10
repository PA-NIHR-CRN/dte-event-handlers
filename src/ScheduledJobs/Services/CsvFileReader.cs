using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using ScheduledJobs.Contracts;

namespace ScheduledJobs.Services
{
    public class CsvFileReader : ICsvFileReader
    {
        public IEnumerable<TResult> ParseStringCsvContent<TMapping, TResult>(string content) where TMapping : ClassMap
        {
            var csv = new CsvReader(new StringReader(content), CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TMapping>();
            return csv.GetRecords<TResult>();
        }
    }
}