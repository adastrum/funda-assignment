using Assignment.Api.Features.Shared.Clients;
using Refit;

namespace Assignment.Api.Features.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddRefitClient<IPartnerApiClient>()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(configuration["PartnerApi:BaseUrl"]));

        services.Configure<PartnerApiOptions>(configuration.GetSection("PartnerApi"));

        return services;
    }
}