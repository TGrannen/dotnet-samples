using Caching.WebAPI.Models;

namespace Caching.WebAPI.Services;

public interface IProductService
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Product?> CreateAsync(ProductCreateDto dto, CancellationToken ct = default);
}
