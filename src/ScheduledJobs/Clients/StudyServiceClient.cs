using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ScheduledJobs.Clients
{
    public interface IStudyServiceClient
    {
        Task<HttpResponseMessage> GetHealthReadyAsync();     
    }

    public class StudyServiceClient : IStudyServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StudyServiceClient> _logger;

        public StudyServiceClient(HttpClient httpClient, ILogger<StudyServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async Task<HttpResponseMessage> GetHealthReadyAsync()
        {
            var requestUri = $"api/health/ready";
            
            var response = await _httpClient.GetAsync(requestUri);

            return response;
        }
    }

    public class StudyServiceClientMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}