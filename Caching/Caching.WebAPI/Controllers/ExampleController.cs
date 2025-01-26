namespace Caching.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ExampleController(IFusionCache cache, ILogger<ExampleController> logger, TimeProvider timeProvider) : ControllerBase
{
    [HttpGet]
    [Route("Get")]
    public async Task<IActionResult> GetValue(string key)
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
    public async Task<IActionResult> GetOrSet(string key, [FromQuery] string[]? tags = null)
    {
        var response = await cache.GetOrSetAsync<Product>($"product:{key}", _ => GetProductFromDb(key), tags: tags);
        return Ok(response);
    }

    [HttpPost]
    [Route("Create")]
    public async Task<IActionResult> SetCacheValue(Product value, [FromQuery] string[]? tags = null)
    {
        try
        {
            value.Created = timeProvider.GetLocalNow().DateTime;
            await cache.SetAsync($"product:{value.Id}", value, tags: tags);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [HttpDelete]
    [Route("Remove")]
    public async Task<IActionResult> Remove(string key)
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

    private async Task<Product> GetProductFromDb(string id)
    {
        using var activity = Tracing.Source.StartActivity();
        logger.LogInformation("Database call made with {ID}", id);
        await Task.Delay(TimeSpan.FromMilliseconds(600));
        return new Product
        {
            Id = id,
            Created = timeProvider.GetLocalNow().DateTime
        };
    }
}

public class Product
{
    public required string Id { get; set; }
    public DateTime Created { get; set; }
}