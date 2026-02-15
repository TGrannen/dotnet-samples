using Caching.WebAPI.Data;
using Caching.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Caching.WebAPI.Services;

public class ProductService(AppDbContext db) : IProductService
{
    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
        return entity == null ? null : MapToProduct(entity);
    }

    public async Task<Product?> CreateAsync(ProductCreateDto dto, CancellationToken ct = default)
    {
        var entity = new ProductEntity
        {
            Name = dto.Name,
            Sku = dto.Sku,
            UnitPrice = dto.UnitPrice,
            CreatedAt = DateTime.UtcNow
        };
        db.Products.Add(entity);
        await db.SaveChangesAsync(ct);
        return MapToProduct(entity);
    }

    private static Product MapToProduct(ProductEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Sku = entity.Sku,
        UnitPrice = entity.UnitPrice,
        Created = entity.CreatedAt
    };
}
