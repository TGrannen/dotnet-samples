using System.Threading.Tasks;

namespace FeatureFlags.WebAPI.Feature
{
    /// <summary>
    /// Abstraction over IFeatureManager
    /// </summary>
    public interface IFeatureService
    {
        Task<bool> IsEnabledAsync(Features feature);
        Task<bool> IsNotEnabledAsync(Features feature);
    }
}