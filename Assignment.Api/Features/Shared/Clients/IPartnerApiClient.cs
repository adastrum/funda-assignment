using Refit;

namespace Assignment.Api.Features.Shared.Clients;

public interface IPartnerApiClient
{
    [Get("/feeds/Aanbod.svc/json/{apiKey}")]
    Task<ApiResponse<GetObjectsForSaleResponse>> GetObjectsForSale(string apiKey, [Query] GetObjectsForSaleQueryParams queryParams);
}

public record GetObjectsForSaleQueryParams
{
    [AliasAs("type")] public string Type { get; init; } = "koop";
    [AliasAs("zo")] public string Query { get; init; }
    [AliasAs("page")] public int Page { get; init; }
    [AliasAs("pagesize")] public int PageSize { get; init; }
}

public record GetObjectsForSaleResponse(ObjectForSale[] Objects, Paging Paging, int TotaalAantalObjecten);

public record ObjectForSale(string Id, int MakelaarId, string MakelaarNaam);

public record Paging(int AantalPaginas, int HuidigePagina);

public class PartnerApiOptions
{
    public string ApiKey { get; set; }
}