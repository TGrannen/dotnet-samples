using ApiService.Api.Persistence.Entities;

namespace ApiService.Api.Features.Products.Extensions;

public static class ProductQueryableExtensions
{
    /// <summary>Shared definition of products that can be sold (see vertical-slice shared query pattern).</summary>
    public static IQueryable<Product> AvailableForSale(this IQueryable<Product> query) =>
        query.Where(p => p.StockCount > 0 && p.Price > 0);
}
