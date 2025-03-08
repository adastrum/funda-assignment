namespace Assignment.Api.Features.Statistics;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStatisticsServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddScoped<StatisticsService>();
        services.AddScoped<ObjectsForSaleDataProvider>();

        return services;
    }
}