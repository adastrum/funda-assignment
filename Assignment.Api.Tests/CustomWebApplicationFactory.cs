using Assignment.Api.Features.Statistics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.Server;

namespace Assignment.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string PartnerApiKey = "secret";
    private const int PartnerApiPort = 5002;
    private const int PageSize = 5;

    public WireMockServer PartnerApiStub { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("PartnerApi:ApiKey", PartnerApiKey);
        builder.UseSetting("PartnerApi:BaseUrl", $"http://localhost:{PartnerApiPort}");
        builder.UseSetting("PartnerApi:PageSize", PageSize.ToString());

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(StatisticsBackgroundService));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
        });
    }

    public async Task InitializeAsync()
    {
        PartnerApiStub = WireMockServer.Start(port: PartnerApiPort);
    }

    public async Task DisposeAsync()
    {
        if (PartnerApiStub.IsStarted)
        {
            PartnerApiStub.Stop();
            PartnerApiStub.Dispose();
        }
    }
}