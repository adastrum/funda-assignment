namespace Assignment.Api.Features.Statistics;

public class StatisticsService
{
    private readonly ObjectsForSaleDataProvider _objectsForSaleDataProvider;
    private readonly ILogger<ObjectsForSaleDataProvider> _logger;

    public StatisticsService(ObjectsForSaleDataProvider objectsForSaleDataProvider,
        ILogger<ObjectsForSaleDataProvider> logger)
    {
        _objectsForSaleDataProvider = objectsForSaleDataProvider;
        _logger = logger;
    }

    public async Task UpdateStatistics(CancellationToken cancellationToken)
    {
        const string query = "/amsterdam/";

        var counter = 0;

        await foreach (var page in _objectsForSaleDataProvider.GetObjectsForSale(query, cancellationToken))
        {
            counter += page.Count;
        }

        _logger.LogInformation("ObjectsForSaleDataProvider retrieved {Count} objects", counter);
    }
}