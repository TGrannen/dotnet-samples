using ApiService.Api.Common.Web.Versioning;

namespace ApiService.Api.Tests.Features.Products;

public record CreateProductRequest(string Name, decimal Price);

public record UpdateProductRequest(string Name, decimal Price);

public record ProductResponse(Guid Id, string Name, decimal Price);

[Headers(SupportedApiVersions.V1RefitHeaderLine)]
public interface IProductsApi
{
    [Post("/api/products")]
    Task<TResponse> CreateAsync<TResponse>([Body] object request);

    [Get("/api/products")]
    Task<List<ProductResponse>> ListAsync();

    [Get("/api/products/{id}")]
    Task<TResponse> GetAsync<TResponse>(Guid id);

    [Put("/api/products/{id}")]
    Task UpdateAsync(Guid id, [Body] UpdateProductRequest body);

    [Delete("/api/products/{id}")]
    Task DeleteAsync(Guid id);
}