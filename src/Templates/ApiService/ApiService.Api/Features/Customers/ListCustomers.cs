using ApiService.Api.Common.Web;
using ApiService.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Api.Features.Customers;

public sealed class ListCustomers : IEndpoint
{
    private const int DefaultTake = 20;
    private const int MaxTake = 500;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("customers", async ([AsParameters] Query query, ISender mediator, CancellationToken cancellationToken) =>
            {
                var items = await mediator.Send(query, cancellationToken);
                return Results.Ok(items);
            })
            .WithName("ListCustomers")
            .WithTags("Customers");
    }

    public sealed record Query(int Skip = 0, int Take = DefaultTake) : IRequest<IReadOnlyList<GetCustomer.CustomerDto>>;

    public sealed class Handler(ApplicationDbContext db) : IRequestHandler<Query, IReadOnlyList<GetCustomer.CustomerDto>>
    {
        public async Task<IReadOnlyList<GetCustomer.CustomerDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var items = await db.Customers
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Id)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(c => new GetCustomer.CustomerDto(c.Id, c.Name, c.Email))
                .ToListAsync(cancellationToken);

            return items;
        }
    }

    public sealed class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(x => x.Skip).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Take).InclusiveBetween(1, MaxTake);
        }
    }
}
