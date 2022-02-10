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
using ScheduledJobs.Mappers;
using ScheduledJobs.Models;
using ScheduledJobs.Responses;

namespace ScheduledJobs.JobHandlers
{
    public class RtsOrganisationImport { }

    public class RtsOrganisationImportJobHandler : IHandler<RtsOrganisationImport, bool>
    {
        private readonly IRtsServiceClient _client;
        private readonly IPollyRetryService _retryService;
        private readonly IRtsDataDynamoDbRepository _repository;
        private readonly ILogger<RtsOrganisationImportJobHandler> _logger;

        public RtsOrganisationImportJobHandler(IRtsServiceClient client, IPollyRetryService retryService, IRtsDataDynamoDbRepository repository, ILogger<RtsOrganisationImportJobHandler> logger)
        {
            _client = client;
            _retryService = retryService;
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> HandleAsync(RtsOrganisationImport source)
        {
            var pageNumber = 1;
            const int pageSize = 50;
            const int stopPage = 20;

            try
            {
                do
                {
                    if (pageNumber > stopPage)
                    {
                        _logger.LogInformation($"Stop Page: {stopPage} hit");
                        break;
                    }

                    var response = await _retryService.WaitAndRetryAsync<HttpRequestException, HttpResponseMessage>
                    (
                        3,
                        currentRetryAttempt => TimeSpan.FromSeconds(5),
                        currentRetryAttempt => _logger.LogWarning($"Retry attempt: {currentRetryAttempt}"),
                        () =>
                        {
                            _logger.LogInformation($"Attempting Page: {pageNumber}");
                            return _client.GetOrganisationsAsync(pageSize, pageNumber);
                        }
                    );

                    var result = JsonConvert.DeserializeObject<RtsDataResponse>(await response.Content.ReadAsStringAsync());

                    if (result?.Result?.RtsOrganisations == null)
                    {
                        _logger.LogInformation($"Result RtsOrganisations is null, break on page: {pageNumber}");
                        break;
                    }

                    var results = result.Result.RtsOrganisations.Where(x => x.Status != "Terminated");
                    var list = new List<RtsData>();
                    list.AddRange(results.Select(RtsDataMapper.MapTo));
                    list = list.GroupBy(l => l.Pk).Select(l => l.First()).ToList();
                    await _repository.BatchInsertAsync(list);
                    _logger.LogInformation($"Page {pageNumber} saved {list.Count} items to the DB");

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