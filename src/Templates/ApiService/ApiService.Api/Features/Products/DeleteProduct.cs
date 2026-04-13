using ApiService.Api.Common.Web;
using ApiService.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace ApiService.Api.Features.Products;

public sealed class DeleteProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
            "products/{id:guid}",
            async Task<IResult> (Guid id, ISender mediator, CancellationToken cancellationToken) =>
            {
                var outcome = await mediator.Send(new Command(id), cancellationToken);
                return outcome.Match<IResult>(
                    _ => TypedResults.NotFound(),
                    _ => TypedResults.NoContent());
            })
            .WithName("DeleteProduct")
            .WithTags("Products");
    }

    public readonly record struct ProductMissing;

    public readonly record struct ProductDeleted;

    public sealed record Command(Guid Id) : IRequest<OneOf<ProductMissing, ProductDeleted>>;

    public sealed class Handler(ApplicationDbContext db) : IRequestHandler<Command, OneOf<ProductMissing, ProductDeleted>>
    {
        public async Task<OneOf<ProductMissing, ProductDeleted>> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (product is null)
            {
                return new ProductMissing();
            }

            if (product.IsDeleted)
            {
                return new ProductDeleted();
            }

            product.IsDeleted = true;
            await db.SaveChangesAsync(cancellationToken);
            return new ProductDeleted();
        }
    }
}
