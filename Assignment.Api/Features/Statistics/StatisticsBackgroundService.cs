using Microsoft.Extensions.Options;

namespace Assignment.Api.Features.Statistics;

public class StatisticsBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly ILogger<StatisticsBackgroundService> _logger;

    private readonly StatisticsOptions _options;
    private readonly PeriodicTimer _updateStatisticsTimer;

    public StatisticsBackgroundService(IOptions<StatisticsOptions> options, IServiceScopeFactory serviceScopeFactory,
        ILogger<StatisticsBackgroundService> logger)
    {
        _options = options.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _updateStatisticsTimer = new PeriodicTimer(TimeSpan.FromSeconds(_options.UpdateIntervalInSeconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();

                var statisticsService = scope.ServiceProvider.GetRequiredService<StatisticsService>();

                await statisticsService.UpdateStatistics(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when fetching statistics");
            }
        } while (await _updateStatisticsTimer.WaitForNextTickAsync(stoppingToken));
    }
}