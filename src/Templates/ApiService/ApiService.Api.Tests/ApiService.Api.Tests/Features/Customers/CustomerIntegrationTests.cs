using ApiService.Api.Domain.Entities;

namespace ApiService.Api.Tests.Features.Customers;

[Repeat(100)]
public class CustomerIntegrationTests
{
    [ClassDataSource<WebApplicationFactory>(Shared = SharedType.PerTestSession)]
    public required WebApplicationFactory Factory { get; init; }

    [Test]
    public async Task CreateCustomer_ThenGet_ReturnsCustomer()
    {
        var api = RestService.For<ICustomersApi>(Factory.CreateClient());
        var email = $"acme-{Guid.NewGuid():N}@example.com";

        var id = await api.CreateAsync<Guid>(new CreateCustomerRequest("Acme Corp", email));
        var customer = await api.GetAsync<CustomerResponse>(id);

        await Assert.That(id).IsNotEqualTo(Guid.Empty);
        await Assert.That(customer.Name).IsEqualTo("Acme Corp");
        await Assert.That(customer.Email).IsEqualTo(email);
    }

    [Test]
    public async Task ListCustomers_WhenSeeded_ReturnsCustomers()
    {
        var alphaEmail = $"alpha-{Guid.NewGuid():N}@example.com";
        var zetaEmail = $"zeta-{Guid.NewGuid():N}@example.com";
        var alphaId = Guid.Empty;
        var zetaId = Guid.Empty;

        var sortKey = Guid.NewGuid().ToString("N");
        await Factory.SeedAsync(db =>
        {
            alphaId = Guid.NewGuid();
            zetaId = Guid.NewGuid();
            db.Customers.AddRange(
                new Customer { Id = zetaId, Name = $"\u0001{sortKey}-zeta", Email = zetaEmail },
                new Customer { Id = alphaId, Name = $"\u0001{sortKey}-alpha", Email = alphaEmail });
            return Task.CompletedTask;
        });

        var api = RestService.For<ICustomersApi>(Factory.CreateClient());

        var alpha = await api.GetAsync<CustomerResponse>(alphaId);
        var zeta = await api.GetAsync<CustomerResponse>(zetaId);
        await Assert.That(alpha.Email).IsEqualTo(alphaEmail);
        await Assert.That(zeta.Email).IsEqualTo(zetaEmail);

        var list = await api.ListAsync(new ListCustomersQuery(0, 500));
        await Assert.That(list.Any(c => c.Id == alphaId)).IsTrue();
        await Assert.That(list.Any(c => c.Id == zetaId)).IsTrue();
    }

    [Test]
    public async Task CreateCustomer_WithEmptyEmail_ReturnsBadRequest()
    {
        var api = RestService.For<ICustomersApi>(Factory.CreateClient());

        var exception = await Assert.That(async () =>
                await api.CreateAsync<Guid>(new CreateCustomerRequest("Name", string.Empty)))
            .Throws<ApiException>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateCustomer_WithDuplicateEmail_ReturnsConflict()
    {
        var api = RestService.For<ICustomersApi>(Factory.CreateClient());
        var email = $"dup-{Guid.NewGuid():N}@example.com";

        _ = await api.CreateAsync<Guid>(new CreateCustomerRequest("First", email));

        var exception = await Assert.That(async () =>
                await api.CreateAsync<Guid>(new CreateCustomerRequest("Second", email)))
            .Throws<ApiException>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Conflict);
    }
}
