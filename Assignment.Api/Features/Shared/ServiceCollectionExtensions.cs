using System.Net;
using Assignment.Api.Features.Shared.Clients;
using Polly;
using Polly.Extensions.Http;
using Refit;

namespace Assignment.Api.Features.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddRefitClient<IPartnerApiClient>()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(configuration["PartnerApi:BaseUrl"]))
            .AddPolicyHandler(GetRetryTransientErrorsPolicy());

        services.Configure<PartnerApiOptions>(configuration.GetSection("PartnerApi"));

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryTransientErrorsPolicy()
    {
        const int retryCount = 5;
        const int baseDelayInSeconds = 2;
        
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            // while it should be HTTP 429 Too Many Requests, it's not implemented in the Partner API
            .OrResult(response => response.StatusCode == HttpStatusCode.Unauthorized)
            .WaitAndRetryAsync(retryCount, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(baseDelayInSeconds, retryAttempt)));
    }
}