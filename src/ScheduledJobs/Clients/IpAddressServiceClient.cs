using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ScheduledJobs.Clients
{
    public interface IIpAddressServiceClient
    {
        Task<string> GetExternalIpAddressAsync();
    }

    public class IpAddressServiceClient : IIpAddressServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<IpAddressServiceClient> _logger;

        public IpAddressServiceClient(HttpClient httpClient, ILogger<IpAddressServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        
        public async Task<string> GetExternalIpAddressAsync()
        {
            var response = await _httpClient.GetAsync("ip");

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
    
    public class IpAddressServiceClientMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}