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

    public sealed record Command(Guid Id) : IRequest<OneOf<NotFound, Success>>;

    public sealed class Handler(ApplicationDbContext db, IFusionCache cache) : IRequestHandler<Command, OneOf<NotFound, Success>>
    {
        public async Task<OneOf<NotFound, Success>> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await db.Products
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (product is null)
            {
                return new NotFound();
            }

            if (product.IsDeleted)
            {
                return new Success();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync(cancellationToken);
            await cache.RemoveAsync(CacheKeys.ForProductId(request.Id), null, cancellationToken);
            return new Success();
        }
    }
}
