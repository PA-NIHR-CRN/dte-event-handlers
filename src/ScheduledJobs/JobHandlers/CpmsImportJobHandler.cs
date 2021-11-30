using System.Threading.Tasks;
using Application.Contracts;
using Microsoft.Extensions.Logging;
using ScheduledJobs.Jobs;

namespace ScheduledJobs.JobHandlers
{
    public class CpmsImportJobHandler : IHandler<CpmsImport, bool>
    {
        private readonly ILogger<CpmsImportJobHandler> _logger;

        public CpmsImportJobHandler(ILogger<CpmsImportJobHandler> logger)
        {
            _logger = logger;
        }
        
        public async Task<bool> HandleAsync(CpmsImport source)
        {
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} STARTED");
            await Task.Delay(500);
            _logger.LogInformation($"************** {nameof(CpmsImportJobHandler)} FINISHED");

            return true;
        }   
    }
}