using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace FeatureFlags.WebAPI.Feature.Filters
{
    [FilterAlias("CustomEndpointFilter")]
    public class CustomEndpointFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _accessor;

        public CustomEndpointFilter(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var settings = context.Parameters.Get<CustomEndpointFilterSettings>();
            var endpoint = _accessor.HttpContext?.Request.Path;

            return Task.FromResult(settings.AllowedEndpoints.Any(x => x == endpoint));
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class CustomEndpointFilterSettings
        {
            public string[] AllowedEndpoints { get; set; }
        }
    }
}