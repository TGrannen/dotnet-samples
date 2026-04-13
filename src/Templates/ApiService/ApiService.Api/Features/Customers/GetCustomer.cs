using ApiService.Api.Common.Web;
using ApiService.Api.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Api.Features.Customers;

public sealed class GetCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("customers/{id:guid}", async (Guid id, ISender mediator, CancellationToken cancellationToken) =>
            {
                var dto = await mediator.Send(new Query(id), cancellationToken);
                return dto is null ? Results.NotFound() : Results.Ok(dto);
            })
            .WithName("GetCustomer")
            .WithTags("Customers");
    }

    public sealed record CustomerDto(Guid Id, string Name, string Email);

    public sealed record Query(Guid Id) : IRequest<CustomerDto?>;

    public sealed class Handler(ApplicationDbContext db) : IRequestHandler<Query, CustomerDto?>
    {
        public async Task<CustomerDto?> Handle(Query request, CancellationToken cancellationToken)
        {
            var customer = await db.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            return customer is null ? null : new CustomerDto(customer.Id, customer.Name, customer.Email);
        }
    }
}
