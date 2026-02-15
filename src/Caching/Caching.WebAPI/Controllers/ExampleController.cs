using Caching.WebAPI.Data;
using Caching.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Caching.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ExampleController(
    [FromKeyedServices("product-cache")] IFusionCache cache,
    AppDbContext db,
    ILogger<ExampleController> logger) : ControllerBase
{
    [HttpGet]
    [Route("Get")]
    public async Task<IActionResult> GetValue(string key = "1")
    {
        var cachedResponse = await cache.GetOrDefaultAsync<Product>($"product:{key}");
        if (cachedResponse != null)
        {
            return Ok(cachedResponse);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("GetOrSet")]
    public async Task<IActionResult> GetOrSet(int id = 1, [FromQuery] string[]? tags = null)
    {
        try
        {
            var response = await cache.GetOrSetAsync<Product>(
                $"product:{id}",
                async token =>
                {
                    var product = await GetProductFromDb(id, token);
                    return product ?? throw new InvalidOperationException($"Product {id} not found.");
                },
                tags: tags);
            return Ok(response);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> SetCacheValue(ProductCreateDto value, [FromQuery] string[]? tags = null)
    {
        try
        {
            var entity = new ProductEntity
            {
                Name = value.Name,
                Sku = value.Sku,
                UnitPrice = value.UnitPrice,
                CreatedAt = DateTime.UtcNow
            };
            db.Products.Add(entity);
            await db.SaveChangesAsync();
            var product = MapToProduct(entity)!;
            await cache.SetAsync($"product:{product.Id}", product, tags: tags);
            return CreatedAtAction(nameof(GetValue), new { key = product.Id.ToString() }, product);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    [Route("Remove")]
    public async Task<IActionResult> Remove(string key = "1")
    {
        await cache.RemoveAsync($"product:{key}");
        return Ok();
    }

    [HttpDelete]
    [Route("Remove/ByTag")]
    public async Task<IActionResult> RemoveByTag(string tag)
    {
        await cache.RemoveByTagAsync(tag);
        return Ok();
    }

    private async Task<Product?> GetProductFromDb(int id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Database call made with {ID}", id);
        var entity = await db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return MapToProduct(entity);
    }

    private static Product? MapToProduct(ProductEntity? entity) => entity == null
        ? null
        : new Product
        {
            Id = entity.Id,
            Name = entity.Name,
            Sku = entity.Sku,
            UnitPrice = entity.UnitPrice,
            Created = entity.CreatedAt
        };
}