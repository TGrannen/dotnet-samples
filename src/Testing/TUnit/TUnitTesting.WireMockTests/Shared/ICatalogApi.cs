using Refit;
using TUnitTesting.WebApi.Clients;

namespace TUnitTesting.WireMockTests.Shared;

public interface ICatalogApi
{
    [Get("/api/catalog/items/{id}")]
    Task<CatalogItemResponse> GetItemAsync(int id, CancellationToken cancellationToken = default);
}
