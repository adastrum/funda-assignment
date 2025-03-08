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

        return app;
    }
}