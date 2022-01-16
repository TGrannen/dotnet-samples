using System;

namespace Caching.WebAPI.Models
{
    public class CacheEntryModel
    {
        public string Key { get; set; }
        public DateTime Created { get; set; }
    }
}