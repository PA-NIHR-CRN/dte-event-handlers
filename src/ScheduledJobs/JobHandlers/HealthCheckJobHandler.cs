using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Clients;

namespace ScheduledJobs.JobHandlers
{
    public class HealthCheck { }

    public class HealthCheckJobHandler : IHandler<HealthCheck, bool>
    {
        private readonly IStudyServiceClient _studyServiceClient;
        private readonly ILogger<HealthCheckJobHandler> _logger;

        public HealthCheckJobHandler(IStudyServiceClient studyServiceClient, ILogger<HealthCheckJobHandler> logger)
        {
            _studyServiceClient = studyServiceClient;
            _logger = logger;
        }
        
        public async Task<bool> HandleAsync(HealthCheck source)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var response = await _studyServiceClient.GetHealthReadyAsync();
                sw.Stop();
                
                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;
                    _logger.LogError($"STUDY SERVICE HealthCheck failed - took {sw.Elapsed} - response StatusCode: {response.StatusCode}: {(!string.IsNullOrWhiteSpace(responseContent) ? responseContent.Replace("\r\n", "") : "")}");
                    return false;
                }

                _logger.LogInformation($"STUDY SERVICE HealthCheck succeeded - took {sw.Elapsed}");
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"ERROR: {ex.GetType().Name} - took {sw.Elapsed}: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ERROR: {ex.GetType().Name} - took {sw.Elapsed}: {ex.Message}");
                return false;
            }
        }
    }
}