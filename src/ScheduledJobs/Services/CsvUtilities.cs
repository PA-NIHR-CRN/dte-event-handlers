using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ScheduledJobs.Contracts;

namespace ScheduledJobs.Services
{
    public class CsvUtilities : ICsvUtilities
    {
        public string WriteCsvString<T>(IEnumerable<T> records)
        {
            using var sw = new StringWriter();
            using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            
            var options = new TypeConverterOptions { Formats = new[] { "s" } };
            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
            csv.Context.TypeConverterOptionsCache.AddOptions<DateTime?>(options);
            
            csv.WriteHeader<T>();
            csv.NextRecord();
            foreach (var record in records)
            {
                csv.WriteRecord(record);
                csv.NextRecord();
            }

            return sw.ToString();
        }

        public IEnumerable<TResult> ParseStringCsvContent<TMapping, TResult>(string content) where TMapping : ClassMap
        {
            var csv = new CsvReader(new StringReader(content), CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TMapping>();
            return csv.GetRecords<TResult>();
        }
    }
}