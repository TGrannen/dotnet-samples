using Caching.WebAPI.Models;
using Caching.WebAPI.Services;

namespace Caching.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ExampleController(
    [FromKeyedServices("product-cache")] IFusionCache cache,
    IProductService productService,
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
                async _ =>
                {
                    var product = await GetProductFromDb(id);
                    if (product == null)
                        throw new InvalidOperationException($"Product {id} not found.");
                    return product;
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
            var product = await productService.CreateAsync(value);
            if (product == null)
                return BadRequest("Failed to create product.");
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

    private async Task<Product?> GetProductFromDb(int id)
    {
        using var activity = Tracing.Source.StartActivity();
        logger.LogInformation("Database call made with {ID}", id);
        return await productService.GetByIdAsync(id);
    }
}
