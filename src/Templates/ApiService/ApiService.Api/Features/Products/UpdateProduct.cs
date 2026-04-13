using ApiService.Api.Common.Web;
using ApiService.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace ApiService.Api.Features.Products;

public sealed class UpdateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
            "products/{id:guid}",
            async Task<IResult> (Guid id, UpdateProductBody payload, ISender mediator, CancellationToken cancellationToken) =>
            {
                var outcome = await mediator.Send(new Command(id, payload.Name, payload.Price), cancellationToken);
                return outcome.Match<IResult>(
                    _ => TypedResults.NoContent(),
                    _ => TypedResults.NotFound());
            })
            .WithName("UpdateProduct")
            .WithTags("Products");
    }

    public sealed record UpdateProductBody(string Name, decimal Price);

    public readonly record struct ProductUpdated;

    public readonly record struct ProductNotFound;

    public sealed record Command(Guid Id, string Name, decimal Price) : IRequest<OneOf<ProductUpdated, ProductNotFound>>;

    public sealed class Handler(ApplicationDbContext db) : IRequestHandler<Command, OneOf<ProductUpdated, ProductNotFound>>
    {
        public async Task<OneOf<ProductUpdated, ProductNotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await db.Products.FirstOrDefaultAsync(
                p => p.Id == request.Id && !p.IsDeleted,
                cancellationToken);

            if (product is null)
            {
                return new ProductNotFound();
            }

            product.Name = request.Name;
            product.Price = request.Price;
            await db.SaveChangesAsync(cancellationToken);
            return new ProductUpdated();
        }
    }

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(500);
            RuleFor(x => x.Price).GreaterThan(0);
        }
    }
}
