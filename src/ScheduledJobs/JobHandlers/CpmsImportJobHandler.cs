using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Application.Contracts;
using Application.Extensions;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Jobs;
using ScheduledJobs.Settings;

namespace ScheduledJobs.JobHandlers
{
    public class CpmsImportJobHandler : IHandler<CpmsImport, bool>
    {
        private readonly IAmazonS3 _s3Client;
        private readonly CpmsImportSettings _cpmsImportSettings;
        private readonly ILogger<CpmsImportJobHandler> _logger;

        public CpmsImportJobHandler(IAmazonS3 s3Client, CpmsImportSettings cpmsImportSettings, ILogger<CpmsImportJobHandler> logger)
        {
            _s3Client = s3Client;
            _cpmsImportSettings = cpmsImportSettings;
            _logger = logger;
        }
        
        public async Task<bool> HandleAsync(CpmsImport source)
        {
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} STARTED");

            var responseBody = await GetFileAsync(_cpmsImportSettings.ExportS3BucketName, _cpmsImportSettings.ExportS3FileName);
	
            var responseString = responseBody;
            using var csv = new CsvReader(new StringReader(responseString), CultureInfo.InvariantCulture);

            csv.Context.RegisterClassMap<SiteMap>();
            var records = csv.GetRecords<Site>().GetByBatch(6);
            
            records.ToList().ForEach(x => x.ToList().ForEach(site => _logger.LogInformation($"{site.SiteName} {site.SiteCode}")));
            
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} FINISHED");

            return true;
        }
        
        private async Task<string> GetFileAsync(string path, string fileName)
        {
            try
            {
                using var response = await _s3Client.GetObjectAsync(path, fileName);
                await using var responseStream = response.ResponseStream;
                using var reader = new StreamReader(responseStream);

                return await reader.ReadToEndAsync();
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, $"Error getting object from S3: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting object from S3: {ex.Message}");
                throw;
            }
        }
    }
    
    public class Site
    {
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public string Postcode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
    }

    public sealed class SiteMap : ClassMap<Site>
    {
        public SiteMap()
        {
            Map(m => m.SiteName).Name("Site Name");
            Map(m => m.SiteCode).Name("Site Code");
            Map(m => m.Postcode);
            Map(m => m.AddressLine1).Name("Address Line 1");
            Map(m => m.AddressLine2).Name("Address Line 2");
            Map(m => m.AddressLine3).Name("Address Line 3");
            Map(m => m.AddressLine4).Name("Address Line 4");
        }
    }
}