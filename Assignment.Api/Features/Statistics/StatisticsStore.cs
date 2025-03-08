using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Assignment.Api.Features.Statistics;

public class StatisticsStore
{
    private readonly IMemoryCache _cache;
    private readonly StatisticsOptions _options;

    public StatisticsStore(IOptions<StatisticsOptions> options, IMemoryCache cache)
    {
        _options = options.Value;
        _cache = cache;
    }

    public async Task UpdateStatistics(string key, ObjectsPerAgent[] topAgents)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(_options.CacheExpirationInSeconds));

        _cache.Set(key, topAgents, cacheEntryOptions);
    }

    public async Task<TopAgentsResponse> GetStatistics(string key, int top)
    {
        var topAgents = _cache.TryGetValue<ObjectsPerAgent[]>(key, out var objectsPerAgent)
            ? objectsPerAgent.OrderByDescending(x => x.ObjectCount)
                .Take(top)
                .ToArray()
            : [];

        return new TopAgentsResponse(topAgents);
    }
}

public record ObjectsPerAgent(int AgentId, string AgentName, int ObjectCount);

public record TopAgentsResponse(ObjectsPerAgent[] TopAgents);

public static class CacheKeys
{
    public static string TopAmsterdam => "top:amsterdam";

    public static string TopAmsterdamWithGarden => "top:amsterdam:garden";
}