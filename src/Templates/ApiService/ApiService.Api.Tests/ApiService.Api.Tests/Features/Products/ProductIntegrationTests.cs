using ApiService.Api.Domain.Entities;

namespace ApiService.Api.Tests.Features.Products;

public class ProductIntegrationTests : IntegrationTestsBase
{
    [Test]
    public async Task GetProduct_WhenSeeded_ReturnsProduct()
    {
        var productId = Guid.NewGuid();
        await SeedAsync(db =>
        {
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 99.99m,
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

    [Test]
    public async Task ListProducts_WhenSeeded_ReturnsAvailableProducts()
    {
        await SeedAsync(db =>
        {
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "List A",
                    Price = 10m,
                    StockCount = 1,
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "List B",
                    Price = 20m,
                    StockCount = 1,
                });
            return Task.CompletedTask;
        });

        var api = RestService.For<IProductsApi>(Factory.CreateClient());
        var list = await api.ListAsync();

        await Assert.That(list.Any(p => p.Name == "List A")).IsTrue();
        await Assert.That(list.Any(p => p.Name == "List B")).IsTrue();
    }

    [Test]
    public async Task UpdateProduct_ThenGet_ReturnsUpdatedValues()
    {
        var api = RestService.For<IProductsApi>(Factory.CreateClient());
        var id = await api.CreateAsync<Guid>(new CreateProductRequest("Before", 50m));

        await api.UpdateAsync(id, new UpdateProductRequest("After", 75m));
        var product = await api.GetAsync<ProductResponse>(id);

        await Assert.That(product.Name).IsEqualTo("After");
        await Assert.That(product.Price).IsEqualTo(75m);
    }

    [Test]
    public async Task UpdateProduct_WithInvalidName_ReturnsBadRequest()
    {
        var api = RestService.For<IProductsApi>(Factory.CreateClient());
        var id = await api.CreateAsync<Guid>(new CreateProductRequest("Valid", 50m));

        var exception = await Assert.That(async () =>
                await api.UpdateAsync(id, new UpdateProductRequest(string.Empty, 10m)))
            .Throws<ApiException>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.StatusCode).IsEqualTo(System.Net.HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteProduct_ThenGet_ReturnsNotFound()
    {
        var api = RestService.For<IProductsApi>(Factory.CreateClient());
        var id = await api.CreateAsync<Guid>(new CreateProductRequest("To Remove", 10m));

        await api.DeleteAsync(id);

        var exception = await Assert.That(async () => await api.GetAsync<ProductResponse>(id))
            .Throws<ApiException>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteProduct_WhenIdUnknown_ReturnsNotFound()
    {
        var api = RestService.For<IProductsApi>(Factory.CreateClient());

        var exception = await Assert.That(async () => await api.DeleteAsync(Guid.NewGuid()))
            .Throws<ApiException>();

        await Assert.That(exception).IsNotNull();
        await Assert.That(exception!.StatusCode).IsEqualTo(System.Net.HttpStatusCode.NotFound);
    }
}
