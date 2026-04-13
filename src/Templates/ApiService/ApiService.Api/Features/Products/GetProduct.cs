using ApiService.Api.Common.Web;
using ApiService.Api.Features.Products.Extensions;
using ApiService.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Api.Features.Products;

public sealed class GetProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products/{id:guid}", async (Guid id, ISender mediator, CancellationToken cancellationToken) =>
            {
                var dto = await mediator.Send(new Query(id), cancellationToken);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("GetProduct")
            .WithTags("Products");
    }

    public sealed record ProductDto(Guid Id, string Name, decimal Price);

    public sealed record Query(Guid Id) : IRequest<ProductDto?>;

    public sealed class Handler(ApplicationDbContext db, IFusionCache cache) : IRequestHandler<Query, ProductDto?>
    {
        public async Task<ProductDto?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await cache.GetOrSetAsync<ProductDto?>(
                CacheKeys.ForProductId(request.Id),
                async ct =>
                {
                    var product = await db.Products
                        .AvailableForSale()
                        .AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

                    return product is null ? null : new ProductDto(product.Id, product.Name, product.Price);
                },
                options: null,
                cancellationToken);
        }
    }
}
