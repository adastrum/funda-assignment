namespace Assignment.Api.Features.Statistics;

public class StatisticsService
{
    private readonly ObjectsForSaleDataProvider _objectsForSaleDataProvider;
    private readonly StatisticsStore _statisticsStore;
    private readonly ILogger<ObjectsForSaleDataProvider> _logger;

    public StatisticsService(ObjectsForSaleDataProvider objectsForSaleDataProvider,
        StatisticsStore statisticsStore, ILogger<ObjectsForSaleDataProvider> logger)
    {
        _objectsForSaleDataProvider = objectsForSaleDataProvider;
        _statisticsStore = statisticsStore;
        _logger = logger;
    }

    public async Task UpdateStatistics(CancellationToken cancellationToken)
    {
        try
        {
            await UpdateStatistics("/amsterdam/", CacheKeys.TopAmsterdam, cancellationToken);
            
            await UpdateStatistics("/amsterdam/tuin/", CacheKeys.TopAmsterdamWithGarden, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update statistics");
        }
    }

    private async Task UpdateStatistics(string query, string key, CancellationToken cancellationToken)
    {
        var counter = 0;

        var objectsPerAgent = new List<ObjectsPerAgent>();

        await foreach (var page in _objectsForSaleDataProvider.GetObjectsForSale(query, cancellationToken))
        {
            counter += page.Count;

            var aggregatedPage = page
                .GroupBy(x => new { x.MakelaarId, x.MakelaarNaam })
                .Select(x => new ObjectsPerAgent(x.Key.MakelaarId, x.Key.MakelaarNaam, x.Count()));

            objectsPerAgent.AddRange(aggregatedPage);
        }

        var result = objectsPerAgent
            .GroupBy(x => x.AgentId)
            .Select(x => new ObjectsPerAgent(x.Key, x.First().AgentName, x.Sum(x => x.ObjectCount)))
            .ToArray();

        await _statisticsStore.UpdateStatistics(key, result);

        _logger.LogInformation("ObjectsForSaleDataProvider retrieved {Count} objects for query {Query}",
            counter, query);
    }
}