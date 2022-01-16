using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Caching.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InMemoryController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        
        public InMemoryController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetValue(string key)
        {
            if (_cache.TryGetValue(key, out CacheEntryModel value))
            {
                return Ok(value);
            }

            return NotFound();
        }


        [HttpPost]
        public IActionResult SetCacheValue(CacheEntryModel value)
        {
            try
            {
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(10),
                    SlidingExpiration = TimeSpan.FromSeconds(5)
                };

                value.Created = DateTime.Now;
                _cache.Set(value.Key, value, cacheExpiryOptions);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        public class CacheEntryModel
        {
            public string Key { get; set; }
            public DateTime Created { get; set; }
        }
    }
}