using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ScheduledJobs.Clients
{
    public interface IRtsServiceClient
    {
        Task<HttpResponseMessage> GetOrganisationsAsync(int pageSize, int pageNumber);
        Task<HttpResponseMessage> GetSitesAsync(int pageSize, int pageNumber);
    }
    
    public class RtsServiceClient : IRtsServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RtsServiceClient> _logger;

        public RtsServiceClient(HttpClient httpClient, ILogger<RtsServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async Task<HttpResponseMessage> GetOrganisationsAsync(int pageSize, int pageNumber)
        {
            var requestUri = $"GetOrganisationList?pageSize={pageSize}&pageNumber={pageNumber}";
            
            var response = await _httpClient.GetAsync(requestUri);

            return response;
        }

        public async Task<HttpResponseMessage> GetSitesAsync(int pageSize, int pageNumber)
        {
            var requestUri = $"GetOrganisationSiteList?pageSize={pageSize}&pageNumber={pageNumber}";
            
            var response = await _httpClient.GetAsync(requestUri);

            return response;
        }
    }
    
    public class RtsServiceClientMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}