using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dte.Common.Contracts;
using Dte.Common.Lambda.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ScheduledJobs.Clients;
using ScheduledJobs.Contracts;
using ScheduledJobs.Domain;
using ScheduledJobs.Mappers;
using ScheduledJobs.Responses;

namespace ScheduledJobs.JobHandlers
{
    public class RtsSiteImport
    {
    }

    public class RtsSiteImportJobHandler : IHandler<RtsSiteImport, bool>
    {
        private readonly IRtsServiceClient _client;
        private readonly IIpAddressServiceClient _ipAddressServiceClient;
        private readonly IPollyRetryService _retryService;
        private readonly IRtsDataDynamoDbRepository _repository;
        private readonly ILogger<RtsSiteImportJobHandler> _logger;

        public RtsSiteImportJobHandler(IRtsServiceClient client,
            IIpAddressServiceClient ipAddressServiceClient,
            IPollyRetryService retryService,
            IRtsDataDynamoDbRepository repository,
            ILogger<RtsSiteImportJobHandler> logger)
        {
            _client = client;
            _ipAddressServiceClient = ipAddressServiceClient;
            _retryService = retryService;
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(RtsSiteImport source)
        {
            var pageNumber = 1;
            const int pageSize = 50000;
            const int stopPage = 20;

            _logger.LogInformation("**** Ip: {ExternalIpAddressAsync}",
                await _ipAddressServiceClient.GetExternalIpAddressAsync());

            var types = RtsDataMapper.GetMappedTypeDictionary();

            try
            {
                do
                {
                    if (pageNumber > stopPage)
                    {
                        _logger.LogInformation("Stop Page: {StopPage} hit", stopPage);
                        break;
                    }

                    var response = await _retryService.WaitAndRetryAsync<HttpRequestException, HttpResponseMessage>
                    (
                        3,
                        currentRetryAttempt => TimeSpan.FromSeconds(5),
                        currentRetryAttempt =>
                            _logger.LogWarning("Retry attempt: {CurrentRetryAttempt}", currentRetryAttempt),
                        () =>
                        {
                            _logger.LogInformation("Attempting Page: {PageNumber}", pageNumber);
                            return _client.GetSitesAsync(pageSize, pageNumber);
                        }
                    );

                    var result =
                        JsonConvert.DeserializeObject<RtsDataResponse>(await response.Content.ReadAsStringAsync());

                    if (result?.Result?.RtsOrganisationSites == null)
                    {
                        _logger.LogInformation("Result RtsOrganisationSites is null, break on page: {PageNumber}",
                            pageNumber);
                        break;
                    }

                    var results = result.Result.RtsOrganisationSites.Where(x => types.ContainsKey(x.Type))
                        .Where(x => x.Status != "Terminated");

                    var list = new List<RtsData>();

                    list.AddRange(results.Select(RtsDataMapper.MapTo));

                    list = list.GroupBy(l => l.Pk).Select(l => l.First()).ToList();

                    await _repository.BatchInsertAsync(list);
                    _logger.LogInformation("Page {PageNumber} saved {ListCount} items to the DB", pageNumber,
                        list.Count);

                    pageNumber++;
                    await Task.Delay(50);
                } while (true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred on page: {pageNumber}: {ex.Message}");
            }

            return false;
        }
    }
}