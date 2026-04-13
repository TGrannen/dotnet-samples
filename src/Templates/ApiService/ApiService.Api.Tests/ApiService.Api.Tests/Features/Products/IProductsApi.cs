using ApiService.Api.Common.Web;
using Refit;

namespace ApiService.Api.Tests.Features.Products;

public record CreateProductRequest(string Name, decimal Price);

public record ProductResponse(Guid Id, string Name, decimal Price);

[Headers(SupportedApiVersions.V1RefitHeaderLine)]
public interface IProductsApi
{
    [Post("/api/products")]
    Task<TResponse> CreateAsync<TResponse>([Body] object request);

    [Get("/api/products/{id}")]
    Task<TResponse> GetAsync<TResponse>(Guid id);
}