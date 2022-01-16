using System;
using System.Text.Json;
using System.Threading.Tasks;
using Caching.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Caching.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DistributedController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public DistributedController(IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> GetValue(string key)
        {
            var cachedResponse = await GetFromCache<CacheEntryModel>(key);
            if (cachedResponse != null)
            {
                return Ok(cachedResponse);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> SetCacheValue(CacheEntryModel value)
        {
            try
            {
                var cacheExpiryOptions = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };

                value.Created = DateTime.Now;
                await SetCache(value.Key, value, cacheExpiryOptions);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }


        private async Task<T?> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }
        
        private async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            var response = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, response, options);
        }
    }
}