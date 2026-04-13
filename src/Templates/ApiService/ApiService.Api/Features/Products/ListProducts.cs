using ApiService.Api.Common.Web;
using ApiService.Api.Features.Products.Extensions;
using ApiService.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Api.Features.Products;

public sealed class ListProducts : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products", async (ISender mediator, CancellationToken cancellationToken) =>
            {
                var items = await mediator.Send(new Query(), cancellationToken);
                return Results.Ok(items);
            })
            .WithName("ListProducts")
            .WithTags("Products");
    }

    public sealed record Query : IRequest<IReadOnlyList<GetProduct.ProductDto>>;

    public sealed class Handler(ApplicationDbContext db) : IRequestHandler<Query, IReadOnlyList<GetProduct.ProductDto>>
    {
        public async Task<IReadOnlyList<GetProduct.ProductDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var items = await db.Products
                .AvailableForSale()
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .Select(p => new GetProduct.ProductDto(p.Id, p.Name, p.Price))
                .ToListAsync(cancellationToken);

            return items;
        }
    }
}
