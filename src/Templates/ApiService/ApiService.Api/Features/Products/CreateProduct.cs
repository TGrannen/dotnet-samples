using ApiService.Api.Common.Web;
using ApiService.Api.Persistence;
using ApiService.Api.Persistence.Entities;

namespace ApiService.Api.Features.Products;

public sealed class CreateProduct : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("products", async (Command command, ISender mediator, CancellationToken cancellationToken) =>
            {
                var id = await mediator.Send(command, cancellationToken);
                return TypedResults.CreatedAtRoute(id, "GetProduct", new { id });
            })
            .WithName("CreateProduct")
            .WithTags("Products");
    }

    public sealed record Command(string Name, decimal Price) : IRequest<Guid>;

    public sealed class Handler(ApplicationDbContext db) : IRequestHandler<Command, Guid>
    {
        public async Task<Guid> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Price = request.Price,
                StockCount = 1,
            };
            db.Products.Add(product);
            await db.SaveChangesAsync(cancellationToken);
            return product.Id;
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
