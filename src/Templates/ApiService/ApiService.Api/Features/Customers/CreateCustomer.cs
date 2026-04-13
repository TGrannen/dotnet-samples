using ApiService.Api.Common.Web;
using ApiService.Api.Persistence;
using ApiService.Api.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace ApiService.Api.Features.Customers;

public sealed class CreateCustomer : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "customers",
            async Task<IResult> (CreateCustomerBody request, ISender mediator, CancellationToken cancellationToken) =>
            {
                var outcome = await mediator.Send(new CreateCustomerCommand(request.Name, request.Email), cancellationToken);
                return outcome.Match<IResult>(
                    id => TypedResults.CreatedAtRoute(id, "GetCustomer", new { id }),
                    _ => TypedResults.Conflict());
            })
            .WithName("CreateCustomer")
            .WithTags("Customers");
    }

    public sealed record CreateCustomerBody(string Name, string Email);

    public readonly record struct EmailAlreadyRegistered;

    public sealed record CreateCustomerCommand(string Name, string Email) : IRequest<OneOf<Guid, EmailAlreadyRegistered>>;

    public sealed class Handler(ApplicationDbContext db) : IRequestHandler<CreateCustomerCommand, OneOf<Guid, EmailAlreadyRegistered>>
    {
        public async Task<OneOf<Guid, EmailAlreadyRegistered>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var normalizedEmail = request.Email.Trim();
            var exists = await db.Customers.AsNoTracking().AnyAsync(c => c.Email == normalizedEmail, cancellationToken);
            if (exists)
            {
                return new EmailAlreadyRegistered();
            }

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Email = normalizedEmail,
            };
            db.Customers.Add(customer);
            await db.SaveChangesAsync(cancellationToken);
            return customer.Id;
        }
    }

    public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().MaximumLength(320).EmailAddress();
        }
    }
}
