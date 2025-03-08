using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Assignment.Api.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string PartnerApiKey = "secret";
    private const int PartnerApiPort = 5002;
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("PartnerApi:ApiKey", PartnerApiKey);
        builder.UseSetting("PartnerApi:BaseUrl", $"http://localhost:{PartnerApiPort}");
    }
}