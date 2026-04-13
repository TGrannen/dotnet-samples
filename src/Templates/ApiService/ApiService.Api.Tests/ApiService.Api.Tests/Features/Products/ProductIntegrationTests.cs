using ApiService.Api.Domain.Entities;

namespace ApiService.Api.Tests.Features.Products;

[Repeat(100)]
public class ProductIntegrationTests
{
    [ClassDataSource<WebApplicationFactory>(Shared = SharedType.PerTestSession)]
    public required WebApplicationFactory Factory { get; init; }

    [Test]
    public async Task GetProduct_WhenSeeded_ReturnsProduct()
    {
        var productId = Guid.NewGuid();
        await Factory.SeedAsync(db =>
        {
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 99.99m,
                IsDeleted = false,
                StockCount = 1,
            });
            return Task.CompletedTask;
        });

        var api = RestService.For<IProductsApi>(Factory.CreateClient());
        var result = await api.GetAsync<ProductResponse>(productId);

        await Assert.That(result.Name).IsEqualTo("Test Product");
        await Assert.That(result.Price).IsEqualTo(99.99m);
    }

    [Test]
    public async Task CreateProduct_ThenGet_ReturnsCreatedProduct()
    {
        var api = RestService.For<IProductsApi>(Factory.CreateClient());

        var id = await api.CreateAsync<Guid>(new CreateProductRequest("High-End GPU", 1200.00m));
        var product = await api.GetAsync<ProductResponse>(id);

        await Assert.That(id).IsNotEqualTo(Guid.Empty);
        await Assert.That(product.Name).IsEqualTo("High-End GPU");
        await Assert.That(product.Price).IsEqualTo(1200.00m);
    }

    [Test]
    public async Task CreateProduct_WithInvalidName_ReturnsBadRequest()
    {
        var api = RestService.For<IProductsApi>(Factory.CreateClient());

        var exception = await Assert.That(async () => await api.CreateAsync<Guid>(new CreateProductRequest(string.Empty, 10m)))
            .Throws<ApiException>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
    }
}
