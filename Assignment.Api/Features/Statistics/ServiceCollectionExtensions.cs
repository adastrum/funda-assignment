namespace Assignment.Api.Features.Statistics;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStatisticsServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<StatisticsService>();
        services.AddScoped<ObjectsForSaleDataProvider>();
        services.AddScoped<StatisticsStore>();

        services.Configure<StatisticsOptions>(configuration.GetSection("Statistics"));

        services.AddHostedService<StatisticsBackgroundService>();

        return services;
    }
}