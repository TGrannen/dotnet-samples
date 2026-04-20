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

    public sealed record Command(Guid Id, string Name, decimal Price) : IRequest<OneOf<Success, NotFound>>;

    public sealed class Handler(ApplicationDbContext db, IFusionCache cache) : IRequestHandler<Command, OneOf<Success, NotFound>>
    {
        public async Task<OneOf<Success, NotFound>> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (product is null)
            {
                return new NotFound();
            }

            product.Name = request.Name;
            product.Price = request.Price;
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveAsync(CacheKeys.ForProductId(request.Id), null, cancellationToken);
            return new Success();
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
