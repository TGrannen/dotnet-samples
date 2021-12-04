using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;

namespace FeatureFlags.WebAPI.Feature.Filters
{
    [FilterAlias("CustomContextualFilter")]
    public class CustomContextualFilter : IContextualFeatureFilter<CustomContextualFilter.FilterContext>
    {
        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context, FilterContext appContext)
        {
            var settings = context.Parameters.Get<CustomContextualFilterSettings>();

            return Task.FromResult(appContext.RandomNumber > settings.MinimumValue);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class CustomContextualFilterSettings
        {
            public int MinimumValue { get; set; }
        }

        public class FilterContext
        {
            public int RandomNumber { get; set; }
        }
    }
}