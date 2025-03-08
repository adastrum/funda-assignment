using System.Runtime.CompilerServices;
using Assignment.Api.Features.Shared.Clients;
using Microsoft.Extensions.Options;

namespace Assignment.Api.Features.Statistics;

public class ObjectsForSaleDataProvider
{
    private readonly IPartnerApiClient _partnerApiClient;
    private readonly ILogger<ObjectsForSaleDataProvider> _logger;
    private readonly PartnerApiOptions _partnerApiOptions;

    public ObjectsForSaleDataProvider(IPartnerApiClient partnerApiClient, IOptions<PartnerApiOptions> partnerApiOptions,
        ILogger<ObjectsForSaleDataProvider> logger)
    {
        _partnerApiClient = partnerApiClient;
        _logger = logger;
        _partnerApiOptions = partnerApiOptions.Value;
    }

    public async IAsyncEnumerable<IReadOnlyCollection<ObjectForSale>> GetObjectsForSale(string query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var queryParams = new GetObjectsForSaleQueryParams
        {
            Query = query,
            Page = 1,
            PageSize = _partnerApiOptions.PageSize
        };

        while (true)
        {
            var apiResponse = await _partnerApiClient.GetObjectsForSale(_partnerApiOptions.ApiKey, queryParams)
                .WaitAsync(cancellationToken);
            if (!apiResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get objects for sale. Status code: {StatusCode}", apiResponse.StatusCode);
                yield break;
            }

            var response = apiResponse.Content;
            if (response?.Paging is null)
            {
                _logger.LogError("Paging metadata is missing in the response.");
                yield break;
            }

            var hasMorePages = response.Paging.HuidigePagina < response.Paging.AantalPaginas;
            if (!hasMorePages)
            {
                yield break;
            }

            yield return response.Objects;

            queryParams = queryParams with { Page = queryParams.Page + 1 };
        }
    }
}