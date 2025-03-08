using System.Net;
using Assignment.Api.Features.Shared.Clients;
using Assignment.Api.Features.Statistics;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Assignment.Api.Tests;

public class ApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WireMockServer _partnerApiStub;
    private readonly HttpClient _client;

    public ApiTests(CustomWebApplicationFactory factory)
    {
        _serviceProvider = factory.Services;
        _partnerApiStub = factory.PartnerApiStub;
        _partnerApiStub.Reset();
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_check()
    {
        var response = await _client.GetAsync("/api/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/amsterdam/")]
    [InlineData("/amsterdam/tuin/")]
    public async Task Get_top_makelaars(string query)
    {
        // Arrange
        const int pageSize = 5;
        const int top = 10;
        const int updateStatisticsDelayInSeconds = 1;

        SetupStub(_partnerApiStub, query, 1, pageSize, null, "page 2 401",
            MakeGetObjectsForSaleResponse(2, 1, 6,
                ("1", 1000),
                ("2", 1000),
                ("3", 1000),
                ("4", 1001),
                ("5", 1001)));

        SetupStub(_partnerApiStub, query, 2, pageSize, "page 2 401", "page 2 200",
            (object)null, HttpStatusCode.Unauthorized);

        SetupStub(_partnerApiStub, query, 2, pageSize, "page 2 200", null,
            MakeGetObjectsForSaleResponse(2, 2, 6,
                ("6", 1002)));

        await using var scope = _serviceProvider.CreateAsyncScope();

        var sut = scope.ServiceProvider.GetRequiredService<StatisticsService>();

        // Act
        await sut.UpdateStatistics(CancellationToken.None);

        await Task.Delay(TimeSpan.FromSeconds(updateStatisticsDelayInSeconds));

        // Assert
        var getStatisticsResponse = await _client.GetAsync($"/api/statistics{query}?top={top}");
        getStatisticsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var getStatisticsResponseAsString = await getStatisticsResponse.Content.ReadAsStringAsync();
        var topAgentsResponse = JsonConvert.DeserializeObject<TopAgentsResponse>(getStatisticsResponseAsString);
        topAgentsResponse.Should().NotBeNull();
        topAgentsResponse.TopAgents.Should().BeEquivalentTo(
            new[]
            {
                new ObjectsPerAgent(1000, "Makelaar 1000", 3),
                new ObjectsPerAgent(1001, "Makelaar 1001", 2),
                new ObjectsPerAgent(1002, "Makelaar 1002", 1)
            });
    }

    private static IRespondWithAProvider SetupStub<TResponse>(WireMockServer stub, string query, int page, int pagesize,
        string state, string newState, TResponse response, HttpStatusCode responseStatusCode = HttpStatusCode.OK)
    {
        var result = stub.Given(
                Request.Create()
                    .WithPath("/feeds/Aanbod.svc/json/*")
                    .WithParam("type", "koop")
                    .WithParam("zo", query)
                    .WithParam("page", page.ToString())
                    .WithParam("pagesize", pagesize.ToString())
                    .UsingGet())
            .InScenario("scenario");

        if (!string.IsNullOrWhiteSpace(state))
        {
            result.WhenStateIs(state);
        }

        result
            .WillSetStateTo(newState)
            .RespondWith(
                Response.Create()
                    .WithStatusCode(responseStatusCode)
                    .WithBody(JsonConvert.SerializeObject(response)));

        return result;
    }

    private static GetObjectsForSaleResponse MakeGetObjectsForSaleResponse(int totalPages, int currentPage,
        int totalObjects, params (string ObjectId, int AgentId)[] objects)
    {
        return new GetObjectsForSaleResponse(
            objects
                .Select(x => new ObjectForSale(x.ObjectId, x.AgentId, $"Makelaar {x.AgentId}"))
                .ToArray(),
            new Paging(totalPages, currentPage),
            totalObjects);
    }
}