using TUnitTesting.WebApi.Clients;

namespace TUnitTesting.WebApi.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(IDownstreamCatalogApi downstreamCatalog) : ControllerBase
{
    [HttpGet("items/{id:int}")]
    public async Task<CatalogItemResponse> GetItem(int id, CancellationToken cancellationToken)
    {
        var catalogItemResponse = await downstreamCatalog.GetItemAsync(id, cancellationToken);
        return catalogItemResponse;
    }
}
