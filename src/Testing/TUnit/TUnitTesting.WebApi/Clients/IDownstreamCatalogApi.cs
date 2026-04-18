using Refit;

namespace TUnitTesting.WebApi.Clients;

public interface IDownstreamCatalogApi
{
    [Get("/items/{id}")]
    Task<CatalogItemResponse> GetItemAsync(int id, CancellationToken cancellationToken = default);
}

public sealed class CatalogItemResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
