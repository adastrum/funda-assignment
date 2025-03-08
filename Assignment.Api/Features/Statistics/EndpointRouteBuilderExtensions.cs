namespace Assignment.Api.Features.Statistics;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapStatisticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/statistics");

        group.MapPost("/update",
            async (StatisticsService statisticsService, CancellationToken cancellationToken) =>
            {
                await statisticsService.UpdateStatistics(cancellationToken);

                return Results.Ok();
            });

        group.MapGet("/amsterdam", async (StatisticsStore statisticsStore, int top = 10) =>
        {
            var result = await statisticsStore.GetStatistics(CacheKeys.TopAmsterdam, top);

            return result.TopAgents.Length != 0
                ? Results.Ok(result)
                : Results.NoContent();
        });

        group.MapGet("/amsterdam/garden", async (StatisticsStore statisticsStore, int top = 10) =>
        {
            var result = await statisticsStore.GetStatistics(CacheKeys.TopAmsterdamWithGarden, top);

            return result.TopAgents.Length != 0
                ? Results.Ok(result)
                : Results.NoContent();
        });

        return app;
    }
}